using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUpdate : MonoBehaviour
{

    SeedAndMode seedAndMode;

    public int modeValue;

    // Start is called before the first frame update
    void Start()
    {
        seedAndMode = FindObjectOfType<SeedAndMode>();   
    }


    public void OnValueChange()
    {
        seedAndMode.UpdateMode(modeValue);
    }
}
