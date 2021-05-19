using UnityEngine;

public class Food : MonoBehaviour
{
    public int count;
    MapGenerator mapGenerator;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    private void Update()
    {
        if (count == 0)
        {
            mapGenerator.listFoods.Remove(this);
            Destroy(this.gameObject);
        }
    }
}
