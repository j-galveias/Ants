using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mode;

    public TMP_Text text;

    public DictionaryObjectPool objectPool;
    [SerializeField]
    GameObject[] gos;
    [SerializeField]
    public int numberstospawn;
    // Use this for initialization
    void Start()
    {
        objectPool = new DictionaryObjectPool();
        foreach (var item in gos)
        {
            objectPool.AddObjectPool(item.name, item, this.transform, numberstospawn/2);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
