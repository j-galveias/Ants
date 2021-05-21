using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.utils;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public float maxSpeed;
    public float steerStrength;
    public float wanderStrength;
    public float viewRadius;
    public float avoidDistance;
    private float viewAngle = 90;

    public bool searchingForFood;

    public Food targetFood;
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

    private PheromonePooler pheromonePooler;

    private float pheromoneEvaporateTime;

    public string foodMarker;
    public string homeMarker;
    private Vector2 lastMarkerPosition;

    public Renderer body;

    public GameManager gameManager;

    public float count;
    public float pheromonePeriod;

    Anthill nest;

    MapGenerator mapGenerator;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mapGenerator = FindObjectOfType<MapGenerator>();
        nest = FindObjectOfType<Anthill>();
        pheromonePooler = PheromonePooler.Instance;
        lastMarkerPosition = transform.position;
        searchingForFood = true;
        count = 0;
        pheromonePeriod = 0.125f;
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        //RaycastHit2D resultLeft = Physics2D.Raycast(head.position, Quaternion.AngleAxis(30, Vector3.forward) * transform.right, 0.5f, wallLayer);
        //Debug.DrawRay(head.position, Quaternion.AngleAxis(30, Vector3.forward) * transform.right, Color.red);
        //Debug.DrawRay(head.position, Quaternion.AngleAxis(-30, Vector3.forward) * transform.right, Color.red);

        if (targetFood == null && gameManager.mode >= 2 )
        {
            if (Random.value < 0.001f && gameManager.mode >= 3 && !searchingForFood)
            {
                if (Random.value < 0.5f)
                {
                    desiredDirection = MathHelper.Rotate2D(desiredDirection, gameManager.mode == 4 ? Random.Range(0, MathConstants.MATH_2PI) : Random.Range(0, MathConstants.MATH_PI_4));
                }
                else
                {
                    desiredDirection = MathHelper.Rotate2D(desiredDirection, -(gameManager.mode == 4 ? Random.Range(0, MathConstants.MATH_2PI) : Random.Range(0, MathConstants.MATH_PI_4)));
                }
            }
            else
            {
                HandlePheromoneSteering();
            }
        }
        if (searchingForFood)
        {
            HandleFood();
        }
        else
        {
            HandleNest();
        }

        RaycastHit2D resultFront = Physics2D.Raycast(head.position, desiredDirection/*Quaternion.AngleAxis(30, Vector2.right) * head.transform.right*/, 1f, wallLayer);
        if (resultFront.collider != null)
        {
            desiredDirection = resultFront.point + resultFront.normal * avoidDistance;
            //desiredDirection = -transform.right;
        }
        else
        {
            RaycastHit2D resultLeft = Physics2D.Raycast(head.position, MathHelper.Rotate2D(desiredDirection, MathConstants.MATH_PI / 6)/*Quaternion.AngleAxis(30, Vector2.right) * head.transform.right*/, 0.8f, wallLayer);
            Debug.DrawRay(head.position, MathHelper.Rotate2D(desiredDirection, MathConstants.MATH_PI / 6), Color.red);
            Debug.DrawRay(head.position, MathHelper.Rotate2D(desiredDirection, -MathConstants.MATH_PI / 6), Color.red);
            if (resultLeft.collider != null)
            {
                desiredDirection = resultLeft.point + resultLeft.normal * avoidDistance;
                //desiredDirection = -transform.right;
            }
            else
            {
                //RaycastHit2D resultRight = Physics2D.Raycast(head.position, Quaternion.AngleAxis(-30, Vector3.forward) * transform.right, 0.5f, wallLayer);
                RaycastHit2D resultRight = Physics2D.Raycast(head.position, MathHelper.Rotate2D(desiredDirection, -MathConstants.MATH_PI / 6)/*Quaternion.AngleAxis(-30, Vector2.right) * head.transform.right*/, 0.8f, wallLayer);

                if (resultRight.collider != null)
                {
                    desiredDirection = resultRight.point + resultLeft.normal * avoidDistance;
                    //desiredDirection = -transform.right;
                }
            }
        }

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);

        position = new Vector2(transform.position.x, transform.position.y) + velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle));
        DropPheromone();
        CheckOutsideMap();
    }

    void DropPheromone()
    {
        if ((new Vector2(RoundDecimal(transform.position.x), RoundDecimal(transform.position.y)) - new Vector2(lastMarkerPosition.x, lastMarkerPosition.y)).magnitude >= 0.5)
        {
            Pheromone marker;
            count += pheromonePeriod;
            if (searchingForFood)
            {
                marker = pheromonePooler.SpawnFromPool(homeMarker, transform.position).GetComponent<Pheromone>();
                marker.createPheromone(count);
                /*marker = map.homeMap[Mathf.CeilToInt(transform.position.x), Mathf.CeilToInt(transform.position.y)];
                if (marker != null)
                {
                    marker.createPheromone(count);
                }*/
            }
            else
            {
                marker = pheromonePooler.SpawnFromPool(foodMarker, transform.position).GetComponent<Pheromone>();
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

            
        }
    }*/

    void HandleFood() {
        if (targetFood == null)
        {
            /*Collider2D[] allFood = Physics2D.OverlapCircleAll(position, viewRadius, foodLayer);

            if (allFood.Length > 0)
            {
                Transform food = allFood[Random.Range(0, allFood.Length)].transform;
                Vector2 dirToFood = (food.position - new Vector3(head.position.x, head.position.y)).normalized;
                //Debug.Log(Vector2.Angle(head.transform.right, dirToFood));
                if (Vector2.Angle(head.transform.right, dirToFood) < viewAngle)
                {
                    food.gameObject.layer = takenFoodLayer;
                    targetFood = food;
                }
            }*/
            foreach(Food food in mapGenerator.listFoods){
                if (Distance(this.position, food.transform.position) <= viewRadius)
                {
                    if (food.count > 0)
                    {
                        Vector2 dirToFood = (food.transform.position - new Vector3(head.position.x, head.position.y)).normalized;
                        //Debug.Log(Vector2.Angle(head.transform.right, dirToFood));
                        if (Vector2.Angle(head.transform.right, dirToFood) < viewAngle)
                        {
                            food.gameObject.layer = takenFoodLayer;
                            targetFood = food;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            desiredDirection = (targetFood.transform.position - new Vector3(head.position.x, head.position.y)).normalized;

            const float foodPickupRadius = 0.3f;

            if (Distance(targetFood.transform.position, head.position) < foodPickupRadius)
            {
                if (targetFood.count <= 0)
                {
                    targetFood = null;
                }
                else
                {
                    targetFood.count--;
                    targetFood = null;
                    searchingForFood = false;
                    body.material.color = Color.yellow;
                    count = 0;
                    ReverseDirection();
                }
            }
        }
    }

    void HandleNest()
    {
        if (targetNest == null)
        {
            if (Distance(this.position, nest.transform.position) <= 3)
            {
                Transform nestTransform = nest.transform;
                Vector2 dirToNest = (nestTransform.position - new Vector3(head.position.x, head.position.y)).normalized;

                if (Vector2.Angle(head.transform.right, dirToNest) < viewAngle)
                {
                    targetNest = nestTransform;
                }
            }
        }
        else if (Distance(this.position, nest.transform.position) < 2)
        {
            DropFood();
        }
        else
        {
            desiredDirection = (targetNest.position - new Vector3(head.position.x, head.position.y)).normalized;
        }
    }

    void DropFood() {
        nest.food += 1;
        searchingForFood = true;
        targetNest = null;
        count = 0;
        body.material.color = Color.white;
        ReverseDirection();
    }

    void HandlePheromoneSteering() {
        centerSensor.UpdateSensor(searchingForFood);
        leftSensor.UpdateSensor(searchingForFood);
        rightSensor.UpdateSensor(searchingForFood);

        if (centerSensor.value > Mathf.Max(leftSensor.value, rightSensor.value))
        {
            desiredDirection = head.transform.right;
        }
        else if ((centerSensor.value < leftSensor.value) && (centerSensor.value < rightSensor.value))
        {
            if (Random.value < 0.5f)
            {
                desiredDirection = MathHelper.Rotate2D(desiredDirection, MathConstants.MATH_PI_4);
            }
            else
            {
                desiredDirection = MathHelper.Rotate2D(desiredDirection, -MathConstants.MATH_PI_4);
            }
        }
        else if (leftSensor.value > rightSensor.value) {
            desiredDirection = MathHelper.Rotate2D(desiredDirection, MathConstants.MATH_PI_4);
        }
        else if (rightSensor.value > leftSensor.value) {
            desiredDirection = MathHelper.Rotate2D(desiredDirection, -MathConstants.MATH_PI_4);
        }
    }

    float Distance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
    }

    public void ReverseDirection()
    {
        desiredDirection = MathHelper.Rotate2D(desiredDirection, MathConstants.MATH_PI);
        transform.Rotate(0, 0, MathConstants.MATH_PI, Space.World);
    }

    void CheckOutsideMap() {
        if (Mathf.RoundToInt(this.position.x) <= 0 || Mathf.RoundToInt(this.position.x) >= mapGenerator.width - 1 || Mathf.RoundToInt(this.position.y) <= 0 || Mathf.RoundToInt(this.position.y) >= mapGenerator.height - 1)
        {
            if (!searchingForFood)
            {
                DropFood();
            }
            this.transform.position = new Vector3(mapGenerator.width / 2, mapGenerator.height / 2, 0);
        }
        else if (mapGenerator.map[Mathf.RoundToInt(this.position.x), Mathf.RoundToInt(this.position.y)] != 0 
            && mapGenerator.map[Mathf.RoundToInt(this.position.x) - 1, Mathf.RoundToInt(this.position.y)] != 0 
            && mapGenerator.map[Mathf.RoundToInt(this.position.x) + 1, Mathf.RoundToInt(this.position.y)] != 0 
            && mapGenerator.map[Mathf.RoundToInt(this.position.x), Mathf.RoundToInt(this.position.y) - 1] != 0 
            && mapGenerator.map[Mathf.RoundToInt(this.position.x), Mathf.RoundToInt(this.position.y) + 1] != 0)
        {
            if (!searchingForFood)
            {
                DropFood();
            }
            this.transform.position = new Vector3(mapGenerator.width / 2, mapGenerator.height / 2, 0);
        }
    }
}
