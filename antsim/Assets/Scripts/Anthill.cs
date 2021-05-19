using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Anthill : MonoBehaviour
{
    public int food = 0;

    public TMP_Text text;

    void Update()
    {
        text.text = food.ToString();
    }
}
