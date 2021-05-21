using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sensor : MonoBehaviour
{

    public float radius;
    //public Vector2 position;

    public float value;
    public LayerMask foodMarker;
    public LayerMask homeMarker;
    public float pheromoneEvaporateTime;
    public Renderer sensorRenderer;
    /*[SerializeField]
    int max = 0;*/

    public void UpdateSensor(bool searchingForFood)
    {   
        this.value = 0;
        //Pheromone[] pheromones;
        Collider2D[] cols = new Collider2D[200];
        int numPhero;

        if (searchingForFood)
        {
            numPhero = Physics2D.OverlapCircleNonAlloc(sensorRenderer.bounds.center, this.radius, cols, foodMarker);
            //Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, radius, foodMarker);
        }
        else
        {
            numPhero = Physics2D.OverlapCircleNonAlloc(sensorRenderer.bounds.center, this.radius, cols, homeMarker);
            //Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, radius, homeMarker);
        }

        if (numPhero > 0)
        {
            //pheromones = cols.Select<Collider2D, Pheromone>(col => col.gameObject.GetComponent<Pheromone>()).ToArray();

            foreach (Collider2D col in cols)
            {
                if (col == null)
                {
                    break;
                }
                this.value = Mathf.Max(col.gameObject.GetComponent<Pheromone>().intensity, this.value);
                //float lifetime = Time.time - pheromone.creationTime;
                //float evaporateAmount = Mathf.Min(1, lifetime / pheromone.disappearTime);
                //this.value += 1 - evaporateAmount;
                //this.value = Mathf.Max(pheromone.intensity, this.value);
            }
        }
    }
}
