using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eolienne : MonoBehaviour
{
    float angle = 0.05f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotQ = Quaternion.AngleAxis(angle, transform.up);
        transform.rotation = rotQ * transform.rotation;
    }
}
