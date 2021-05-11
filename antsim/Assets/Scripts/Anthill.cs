using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Anthill : MonoBehaviour
{
    int food = 0;

    public TMP_Text text;

    void Update()
    {
        text.text = food.ToString();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Food"))
        {
            food += 1;

            collision.gameObject.transform.parent.transform.parent.GetComponent<Ant>().searchingForFood = true;

            Destroy(collision.gameObject);
        }
    }
}
