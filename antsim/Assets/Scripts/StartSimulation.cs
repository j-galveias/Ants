using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSimulation : MonoBehaviour
{

    SeedAndMode seedAndMode;

    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        seedAndMode = FindObjectOfType<SeedAndMode>();
    }

    public void OnButtonPressed()
    {
        if (!string.IsNullOrEmpty(inputField.text) && seedAndMode.Mode != 0)
        {
            seedAndMode.UpdateSeed(inputField.text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
