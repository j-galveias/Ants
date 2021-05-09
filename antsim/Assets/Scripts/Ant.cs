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
    private float viewAngle = 90;

    public Transform targetFood;

    public LayerMask foodLayer;
    int takenFoodLayer = 9;

    Vector2 position;
    Vector2 velocity;
    Vector2 desiredDirection;

    public Transform head;

    // Update is called once per frame
    void Update()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle));
        HandleFood();
    }

    void HandleFood() {
        if (targetFood == null)
        {
            Debug.DrawLine(head.position, new Vector2(head.position.x + viewRadius, head.position.y + viewRadius), Color.blue);
            Debug.DrawRay(head.position, -head.transform.up);
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
            }
        }
    }

    /*void HandlePheromoneSteering() {
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
    }*/

    /*void UpdateSensor(Sensor sensor) {
        sensor.UpdatePosition(position, head.transform.right);
        sensor.value = 0;

        PheromoneMap map = (searchingForFood) ? foodMarkers : homeMarkers;
        Pheromone[] pheromones = map.GetAllinCircle(sensor.position, sensor.radius);

        foreach(Pheromone pheromone in pheromones)
        {
            float lifetime = Time.time - pheromone.creationTime;
            float evaporateAmount = Mathf.Max(1, lifetime / pheromoneEvaporateTime);
            sensor.value += 1 - evaporateAmount;
        }
    }*/

    float Distance(Vector3 food, Vector3 head)
    {
        return Mathf.Sqrt(Mathf.Pow(food.x - head.x, 2) + Mathf.Pow(food.y - head.y, 2));
    }
}
