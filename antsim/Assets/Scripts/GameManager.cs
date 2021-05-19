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

    public DictionaryObjectPool objectPool;
    [SerializeField]
    GameObject[] gos;
    [SerializeField]
    public int numberstospawn = 1000;

    List<Ant> listAnts;

    // Use this for initialization
    void Start()
    {
        objectPool = new DictionaryObjectPool();
        foreach (var item in gos)
        {
            objectPool.AddObjectPool(item.name, item, this.transform, numberstospawn);
        }

        listAnts = FindObjectOfType<MapGenerator>().listAnts;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Ant ant in listAnts)
        {
            ant.OnUpdate();
        }

        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        m_Text.text = string.Format(display, avgFramerate.ToString());

        if (Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale += 1f;
            text.text = Time.timeScale.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = Mathf.Max(1, Time.timeScale - 1);
            text.text = Time.timeScale.ToString();
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
