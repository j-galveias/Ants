using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mode = 2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale += 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = Mathf.Max(1, Time.timeScale - 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mode = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mode = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            mode = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            mode = 4;
        }
    }
}
