using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public bool searchingForFoodMarker;
    
    public float creationTime;

    public float disappearTime;

    public float intensity;

    const float coef = 0.01f;

    // Start is called before the first frame update
    void Awake()
    {
        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - creationTime > disappearTime)
        {
            Destroy(transform.gameObject);
        }
    }

    public void createPheromone(int count)
    {
        intensity = 1000.0f * Mathf.Exp(-coef * count);
    }
}
