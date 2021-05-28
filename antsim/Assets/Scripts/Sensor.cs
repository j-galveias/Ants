using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sensor : MonoBehaviour
{

    public float radius;

    public float value;
    public LayerMask foodMarker;
    public LayerMask homeMarker;
    public float pheromoneEvaporateTime;
    public Renderer sensorRenderer;

    public void UpdateSensor(bool searchingForFood)
    {   
        this.value = 0;
        Collider2D[] cols = new Collider2D[200];
        int numPhero;

        if (searchingForFood)
        {
            numPhero = Physics2D.OverlapCircleNonAlloc(sensorRenderer.bounds.center, this.radius, cols, foodMarker);
        }
        else
        {
            numPhero = Physics2D.OverlapCircleNonAlloc(sensorRenderer.bounds.center, this.radius, cols, homeMarker);
        }

        if (numPhero > 0)
        {
            foreach (Collider2D col in cols)
            {
                if (col == null)
                {
                    break;
                }
                this.value = Mathf.Max(col.gameObject.GetComponent<Pheromone>().intensity, this.value);
            }
        }
    }
}
