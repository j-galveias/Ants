using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.utils;
using UnityEngine;

public class PheromoneMap : MonoBehaviour
{
    public LayerMask foodMarker;
    public LayerMask homeMarker;

    public GameObject foodMarkerObj;
    public GameObject homeMarkerObj;

    public Pheromone[,] homeMap;
    public Pheromone[,] foodMap;

    public Dictionary<Pair<float, float>, Pheromone> homeDic;
    public Dictionary<Pair<float, float>, Pheromone> foodDic;

    public void Awake()
    {
        homeDic = new Dictionary<Pair<float, float>, Pheromone>();
        foodDic = new Dictionary<Pair<float, float>, Pheromone>();
    }
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

    public void CreatePheromoneMaps(int[,] map, int width, int height) {
        homeMap = new Pheromone[width, height];
        foodMap = new Pheromone[width, height];

        /*for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                    Pheromone temp1 = Instantiate(homeMarkerObj).GetComponent<Pheromone>();
                    temp1.transform.position = new Vector2(x, y);
                    temp1.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                    Pheromone temp2 = Instantiate(foodMarkerObj).GetComponent<Pheromone>();
                    temp2.transform.position = new Vector2(x, y);
                    temp2.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                    homeMap[x, y] = temp1;
                    foodMap[x, y] = temp2;
            }
        }*/
    }



}
