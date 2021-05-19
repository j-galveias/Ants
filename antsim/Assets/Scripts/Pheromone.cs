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
        creationTime = Time.time;
        intensity = 0;
        maxIntensity = 1000;
    }

    private void OnEnable()
    {
        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - creationTime > disappearTime || intensity <= 0)
        {
            //Destroy(transform.gameObject);
            /*Color temp = _renderer.color;
            alpha = 0;
            temp.a = alpha;
            _renderer.color = temp;
            intensity = 0;*/
            gameObject.SetActive(false);
        }


        intensity -= Time.deltaTime;
    }

    public void createPheromone(float count)
    {
        creationTime = Time.time;
        intensity = 1000.0f * Mathf.Exp(-coef * count);
        Color temp = _renderer.color;
        alpha = intensity / maxIntensity;
        temp.a = alpha;
        _renderer.color = temp;
    }
}
