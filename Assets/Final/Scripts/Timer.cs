using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    int minutes;
    int seconds;
    int milliseconds;

    Text timerDisplay;

    [SerializeField]
    float offsetX = 148;

    
    float offsetY = 34;

    // Start is called before the first frame update
    void Start()
    {
        timerDisplay = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        minutes = (int)(Time.timeSinceLevelLoad / 60f);
        seconds = (int)(Time.timeSinceLevelLoad % 60f);
        milliseconds = (int)((Time.timeSinceLevelLoad * 1000f) % 1000);
        timerDisplay.text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + milliseconds.ToString("D3");
    }
}
