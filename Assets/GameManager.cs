using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject finishLine; 
    // Start is called before the first frame update
    void Start()
    {
        Invoke("spawnFinishLine", 3);
    }

    void spawnFinishLine() 
    {
        finishLine.SetActive(true);
    }
}
