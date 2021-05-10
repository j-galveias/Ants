using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneMap : MonoBehaviour
{
    public LayerMask foodMarker;
    public LayerMask homeMarker;

    public Pheromone[] GetAllInCircle(Vector2 position, float radius, bool searchingForFood)
    {
        LayerMask marker = searchingForFood ? foodMarker : homeMarker;

        Collider2D[] allMarkers = Physics2D.OverlapCircleAll(position, radius, marker);

        Pheromone[] pheromones = new Pheromone[allMarkers.Length];

        for (int i = 0; i < allMarkers.Length; i++)
        {
            pheromones[i] = allMarkers[i].GetComponent<Pheromone>();
        }

        return pheromones;
    }
}
