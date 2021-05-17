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

    public bool searchingForFood = true;
    private Pheromone pheromone = new Pheromone();

    public Transform targetFood;
    public Transform targetNest;

    public LayerMask wallLayer;
    public LayerMask foodLayer;
    public LayerMask nestLayer;
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

    public Renderer body;

    public GameManager gameManager;

    public int count = 0;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //TODO: Inicializar sensores
        map = FindObjectOfType<PheromoneMap>();
        lastMarkerPosition = transform.position;
        //desiredDirection = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        //RaycastHit2D resultLeft = Physics2D.Raycast(head.position, Quaternion.AngleAxis(30, Vector3.forward) * transform.right, 0.5f, wallLayer);
        //Debug.DrawRay(head.position, Quaternion.AngleAxis(30, Vector3.forward) * transform.right, Color.red);
        //Debug.DrawRay(head.position, Quaternion.AngleAxis(-30, Vector3.forward) * transform.right, Color.red);
        RaycastHit2D resultLeft = Physics2D.Raycast(head.position, Quaternion.AngleAxis(30, Vector2.right) * transform.right, 0.5f, wallLayer);
        //Debug.DrawRay(head.position, Quaternion.AngleAxis(30, Vector2.right) * transform.right, Color.red);
        //Debug.DrawRay(head.position, Quaternion.AngleAxis(-30, Vector2.right) * transform.right, Color.red);
        if (resultLeft.collider != null)
        {
            desiredDirection = resultLeft.point + resultLeft.normal * avoidDistance;
        }
        else
        {
            //RaycastHit2D resultRight = Physics2D.Raycast(head.position, Quaternion.AngleAxis(-30, Vector3.forward) * transform.right, 0.5f, wallLayer);
            RaycastHit2D resultRight = Physics2D.Raycast(head.position, Quaternion.AngleAxis(-30, Vector2.right) * transform.right, 0.5f, wallLayer);

            if (resultRight.collider != null)
            {
                desiredDirection = resultRight.point + resultLeft.normal * avoidDistance;
            }
        }

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        
        position = new Vector2(transform.position.x, transform.position.y) + velocity * Time.deltaTime;

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
        if (targetFood == null && gameManager.mode >= 2 )
        {
            if (Random.value < 0.01f && gameManager.mode == 3)
            {
                if (Random.value < 0.5f)
                {
                    desiredDirection = head.transform.up;
                }
                else
                {
                    desiredDirection = -head.transform.up;
                }
            }
            else
            {
                HandlePheromoneSteering();
            }
        }
        if (!searchingForFood)
        {
            HandleNest();
        }
        DropPheromone();
    }

    void DropPheromone()
    {
        if ((new Vector2(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y)) - new Vector2(lastMarkerPosition.x, lastMarkerPosition.y)).magnitude >= 0.2)
        {
            Pheromone marker;
            count++;
            if (searchingForFood)
            {
                marker = Instantiate(homeMarker).GetComponent<Pheromone>();
                marker.createPheromone(count);
                /*marker = map.homeMap[Mathf.CeilToInt(transform.position.x), Mathf.CeilToInt(transform.position.y)];
                if (marker != null)
                {
                    marker.createPheromone(count);
                }*/
            }
            else
            {
                marker = Instantiate(foodMarker).GetComponent<Pheromone>();
                marker.createPheromone(count);

                /*marker = map.foodMap[Mathf.CeilToInt(transform.position.x), Mathf.CeilToInt(transform.position.y)];
                if (marker != null)
                {
                    marker.createPheromone(count);
                }*/
            }

            lastMarkerPosition = marker.transform.position = new Vector3(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y), 0);
        }
    }

    float RoundDecimal(float x) {
        return (Mathf.Round(x * 10)) / 10;
    }

    /*void DropPheromone()
    {
        if ((new Vector2(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y)) - new Vector2(lastMarkerPosition.x, lastMarkerPosition.y)).magnitude >= 0.2)
        {
            Pheromone marker;
            count++;
            if (searchingForFood)
            {
                Pair<float, float> coords = new Pair<float, float>(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y));
                if (map.homeDic.ContainsKey(coords))
                {
                    marker = map.homeDic[coords];
                    marker.createPheromone(count);
                }
                else
                {
                    marker = Instantiate(homeMarker).GetComponent<Pheromone>();
                    marker.createPheromone(count);
                    map.homeDic.Add(coords, marker);
                }

            }
            else
            {
                Pair<float, float> coords = new Pair<float, float>(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y));
                if (map.foodDic.ContainsKey(coords))
                {
                    marker = map.foodDic[coords];
                    marker.createPheromone(count);
                }
                else
                {
                    marker = Instantiate(foodMarker).GetComponent<Pheromone>();
                    marker.createPheromone(count);
                    map.foodDic.Add(coords, marker);
                }
            }

            lastMarkerPosition = marker.transform.position = new Vector3(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y), 0);
        }
    }*/

    /*void DropPheromone()
    {
        if ((new Vector2(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y)) - new Vector2(lastMarkerPosition.x, lastMarkerPosition.y)).magnitude >= 0.2)
        {
            Pheromone marker;
            count++;
            if (searchingForFood)
            {
                //Hard coded 0 = homepheromone
                //if (gameManager.objectPool[0].Count < gameManager.numberstospawn)
                //{
                    marker = gameManager.objectPool[0].Spawn(this.transform.position).GetComponent<Pheromone>();
                    marker.createPheromone(count);
                //}
            }
            else
            {
                //Hard coded 1 = homepheromone
                //if (gameManager.objectPool[1].Count < gameManager.numberstospawn)
                //{
                    marker = gameManager.objectPool[1].Spawn(this.transform.position).GetComponent<Pheromone>();
                    marker.createPheromone(count);
                //}
            }

            lastMarkerPosition = marker.transform.position = new Vector3(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y), 0);

            //EasyObjectPool.instance.ReturnObjectToPool(marker.gameObject);
        }
    }*/

    void HandleFood() {
        if (targetFood == null)
        {
            Collider2D[] allFood = Physics2D.OverlapCircleAll(position, viewRadius, foodLayer);

            if (allFood.Length > 0)
            {
                Transform food = allFood[Random.Range(0, allFood.Length)].transform;
                Vector2 dirToFood = (food.position - head.position).normalized;
                //Debug.Log(Vector2.Angle(head.transform.right, dirToFood));
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

            const float foodPickupRadius = 0.3f;

            if (Distance(targetFood.position, head.position) < foodPickupRadius)
            {
                targetFood.position = head.position;
                targetFood.parent = head;
                targetFood = null;
                searchingForFood = false;
                body.material.color = Color.yellow;
                desiredDirection = -head.transform.right;
                count = 0;
            }
        }
    }

    void HandleNest()
    {
        if (targetNest == null)
        {
            Collider2D[] nest = Physics2D.OverlapCircleAll(position, 1, nestLayer);

            if (nest.Length > 0)
            {
                Transform nestTransform = nest[0].transform;
                Vector2 dirToNest = (nestTransform.position - head.position).normalized;

                if (Vector2.Angle(head.transform.right, dirToNest) < viewAngle)
                {
                    targetNest = nestTransform;
                }
            }
        }
        else
        {
            desiredDirection = (targetNest.position - head.position).normalized;
        }
    }

    void HandlePheromoneSteering() {
        leftSensor.UpdateSensor(searchingForFood);
        centerSensor.UpdateSensor(searchingForFood);
        rightSensor.UpdateSensor(searchingForFood);

        if (centerSensor.value > Mathf.Max(leftSensor.value, rightSensor.value))
        {
            desiredDirection = head.transform.right;
        }
        else if ((centerSensor.value < leftSensor.value) && (centerSensor.value < rightSensor.value))
        {
            if (Random.value < 0.5f)
            {
                desiredDirection = head.transform.up;
            }
            else
            {
                desiredDirection = -head.transform.up;
            }
        }
        else if (leftSensor.value > rightSensor.value) {
            desiredDirection = head.transform.up;
        }
        else if (rightSensor.value > leftSensor.value) {
            desiredDirection = -head.transform.up;
        }
    }

    float Distance(Vector3 food, Vector3 head)
    {
        return Mathf.Sqrt(Mathf.Pow(food.x - head.x, 2) + Mathf.Pow(food.y - head.y, 2));
    }

    public void ReverseDirection()
    {
        //transform.Rotate(Vector2.right * 180);
        desiredDirection = -head.transform.right;
    }

    bool CanMove(Vector2 velocity) {
        var temp = this.position;
        temp += velocity * Time.timeScale;

        return temp.x >= -5f && temp.x <= 5f && temp.y >= -5f && temp.y <= 5f;
    }
}
