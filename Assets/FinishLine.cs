using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    GameObject timerUI;

    [SerializeField]
    GameObject finishUI;

    [SerializeField]
    TextMeshProUGUI endTime;

    [SerializeField]
    Text timer;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Time.timeScale = 0;
            timerUI.SetActive(false);
            finishUI.SetActive(true);
            endTime.text = timer.text;
        }
    }
}
