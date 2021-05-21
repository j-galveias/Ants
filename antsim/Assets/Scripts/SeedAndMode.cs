using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedAndMode : MonoBehaviour
{

    public string Seed;

    public int Mode;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void UpdateSeed(string newSeed)
    {
        Seed = newSeed;
    }

    public void UpdateMode(int newMode)
    {
        Mode = newMode;
    }
}
