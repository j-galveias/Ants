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
    public Renderer renderer;

    public void UpdateSensor(bool searchingForFood)
    {   
        this.value = 0;
        Pheromone[] pheromones;
        if (searchingForFood)
        {
            pheromones = Physics2D.OverlapCircleAll(renderer.bounds.center, this.radius, foodMarker).Select<Collider2D, Pheromone>(col => col.gameObject.GetComponent<Pheromone>()).ToArray();
            //Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, radius, foodMarker);
        }
        else
        {
            pheromones = Physics2D.OverlapCircleAll(renderer.bounds.center, this.radius, homeMarker).Select<Collider2D, Pheromone>(col => col.gameObject.GetComponent<Pheromone>()).ToArray();
            //Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, radius, homeMarker);
        }

        /*if (pheromones.Length > 0)
        {
            renderer.material.color = Color.cyan;
        }
        else
        {
            renderer.material.color = Color.white;
        }*/

        foreach (Pheromone pheromone in pheromones)
        {
            //float lifetime = Time.time - pheromone.creationTime;
            //float evaporateAmount = Mathf.Min(1, lifetime / pheromone.disappearTime);
            //this.value += 1 - evaporateAmount;
            this.value = Mathf.Max(pheromone.intensity, this.value);
        }
    }
}
