using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class TimerControler : MonoBehaviour
{
    public static TimerControler instace;
    public TMP_Text text;
    private TimeSpan time;
    private float timePassed;
    bool timerActive;

    private void Awake()
    {
        instace = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        text.text = "00:00.00";
    }

    public void StartTimer()
    {
        timePassed = 0;
        timerActive = true;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer() {
        while (timerActive)
        {
            timePassed += Time.unscaledDeltaTime;
            time = TimeSpan.FromSeconds(timePassed);
            text.text = time.ToString("mm':'ss'.'ff");

            yield return null;
        }
    }

    public void StopTimer() {
        timerActive = false;
    }
}
