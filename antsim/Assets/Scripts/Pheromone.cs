using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour 
{ 
    public bool searchingForFoodMarker;
    
    public float creationTime;

    public float disappearTime;

    public float intensity;

    public int maxIntensity;

    public SpriteRenderer _renderer;

    const float coef = 0.01f;

    [SerializeField] private float alpha;

    // Start is called before the first frame update
    void Awake()
    {
        creationTime = 0;
        intensity = 0;
        maxIntensity = 1000;
    }

    private void OnEnable()
    {
        creationTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        creationTime += deltaTime;

        if (creationTime > disappearTime || intensity <= 0)
        {
            gameObject.SetActive(false);
        }


        intensity -= deltaTime;
    }

    public void createPheromone(float count)
    {
        creationTime = 0;
        intensity = 1000.0f * Mathf.Exp(-coef * count);
        Color temp = _renderer.color;
        alpha = intensity / maxIntensity;
        temp.a = alpha;
        _renderer.color = temp;
    }
}
