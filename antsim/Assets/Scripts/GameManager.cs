using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mode;

    public TMP_Text text;

    public float timer, refresh, avgFramerate;
    string display = "{0} FPS";
    public TMP_Text m_Text;

    List<Ant> listAnts;

    // Use this for initialization
    void Start()
    {
        mode = FindObjectOfType<SeedAndMode>().Mode;
        listAnts = FindObjectOfType<MapGenerator>().listAnts;
        mode = FindObjectOfType<SeedAndMode>().Mode;
    }

    // Update is called once per frame
    void Update()
    {
        if (listAnts != null)
        {
            foreach (Ant ant in listAnts)
            {
                ant.OnUpdate();
            }
        }

        text.text = mode.ToString();

        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        m_Text.text = string.Format(display, avgFramerate.ToString());

        if (Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale += 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = Mathf.Max(1, Time.timeScale - 1);
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
