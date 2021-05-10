using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.utils;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public float maxSpeed = 2;
    public float steerStrength = 2;
    public float wanderStrength = 1;
    public float viewRadius = 1;
    public float avoidDistance;
    private float viewAngle = 90;

    private bool searchingForFood = true;
    private Pheromone pheromone = new Pheromone();

    public Transform targetFood;

    public LayerMask wallLayer;
    public LayerMask foodLayer;
    int takenFoodLayer = 9;

    Vector2 position;
    Vector2 velocity;
    Vector2 desiredDirection;

    public Sensor centerSensor;
    public Sensor rightSensor;
    public Sensor leftSensor;

    public Transform head;

    private PheromoneMap map;

    private float pheromoneEvaporateTime;

    public GameObject foodMarker;
    public GameObject homeMarker;
    private Vector2 lastMarkerPosition;

    private void Start()
    {
        //TODO: Inicializar sensores
        map = FindObjectOfType<PheromoneMap>();
        lastMarkerPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        RaycastHit2D resultLeft = Physics2D.Raycast(head.position, Quaternion.AngleAxis(30, Vector3.forward) * transform.right, 0.5f, wallLayer);
        Debug.DrawRay(head.position, Quaternion.AngleAxis(30, Vector3.forward) * transform.right, Color.red);
        Debug.DrawRay(head.position, Quaternion.AngleAxis(-30, Vector3.forward) * transform.right, Color.red);
        if (resultLeft.collider != null)
        {
            desiredDirection = resultLeft.point + resultLeft.normal * avoidDistance;
        }

        RaycastHit2D resultRight = Physics2D.Raycast(head.position, Quaternion.AngleAxis(-30, Vector3.forward) * transform.right, 0.5f, wallLayer);

        if (resultRight.collider != null)
        {
            desiredDirection = resultRight.point + resultLeft.normal * avoidDistance;
        }

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle));

        if (searchingForFood)
        {
            pheromone.searchingForFoodMarker = true;
            HandleFood();
        }
        else
        {
            pheromone.searchingForFoodMarker = false;

        }

        DropPheromone();
    }

    void DropPheromone()
    {
        if ((transform.position - new Vector3(lastMarkerPosition.x, lastMarkerPosition.y)).magnitude >= 0.2)
        {
            GameObject marker;
            if (searchingForFood)
            {
                marker = Instantiate(homeMarker);
            }
            else
            {
                marker = Instantiate(foodMarker);
            }

            lastMarkerPosition = marker.transform.position = transform.position;
        }
    }

    void HandleFood() {
        if (targetFood == null)
        {
            Collider2D[] allFood = Physics2D.OverlapCircleAll(position, viewRadius, foodLayer);

            if (allFood.Length > 0)
            {
                Transform food = allFood[Random.Range(0, allFood.Length)].transform;
                Vector2 dirToFood = (food.position - head.position).normalized;
                Debug.Log(Vector2.Angle(head.transform.right, dirToFood));
                if (Vector2.Angle(head.transform.right, dirToFood) < viewAngle)
                {
                    food.gameObject.layer = takenFoodLayer;
                    targetFood = food;
                }
            }
        }
        else
        {
            desiredDirection = (targetFood.position - head.position).normalized;

            const float foodPickupRadius = 0.05f;

            if (Distance(targetFood.position, head.position) < foodPickupRadius)
            {
                targetFood.position = head.position;
                targetFood.parent = head;
                targetFood = null;
                searchingForFood = false;
            }
        }
    }

    void HandlePheromoneSteering() {
        UpdateSensor(leftSensor);
        UpdateSensor(centerSensor);
        UpdateSensor(rightSensor);

        if (centerSensor.value > Mathf.Max(leftSensor.value, rightSensor.value))
        {
            desiredDirection = head.transform.right;
        }
        else if (leftSensor.value > rightSensor.value) {
            desiredDirection = head.transform.up;
        }
        else if (rightSensor.value > leftSensor.value) {
            desiredDirection = -head.transform.up;
        }
    }

    void UpdateSensor(Sensor sensor) {
        sensor.UpdatePosition(position, head.transform.right);
        sensor.value = 0;

        Pheromone[] pheromones = map.GetAllInCircle(sensor.position, sensor.radius, searchingForFood);

        foreach(Pheromone pheromone in pheromones)
        {
            float lifetime = Time.time - pheromone.creationTime;
            float evaporateAmount = Mathf.Max(1, lifetime / pheromoneEvaporateTime);
            sensor.value += 1 - evaporateAmount;
        }
    }

    float Distance(Vector3 food, Vector3 head)
    {
        return Mathf.Sqrt(Mathf.Pow(food.x - head.x, 2) + Mathf.Pow(food.y - head.y, 2));
    }
}
