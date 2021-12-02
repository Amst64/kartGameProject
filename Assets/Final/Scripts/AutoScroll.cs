using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoScroll : MonoBehaviour
{
    float ScrollSpeed = 50.0f;
    float StartPosition=-400;
    float EndPosition = 1000.0f;

    RectTransform MyPlane;

    [SerializeField]
    TextMeshProUGUI MyText;


    // Start is called before the first frame update
    void Start()
    {
        MyPlane = gameObject.GetComponent<RectTransform>();
        StartCoroutine(AutoScrollText());

    }

    IEnumerator AutoScrollText()
    {
        while (MyPlane.localPosition.y<EndPosition)
        {
            MyPlane.Translate(Vector3.up * ScrollSpeed * Time.deltaTime);

            if (MyPlane.localPosition.y >= EndPosition) {
                MyPlane.localPosition = Vector3.up * StartPosition;
            }

            yield return null;

        }
    }
}
