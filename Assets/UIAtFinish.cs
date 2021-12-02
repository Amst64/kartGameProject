using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIAtFinish : MonoBehaviour
{
    public void Button_Menu()
    {
        SceneManager.LoadScene("Scene_Menus");
    }

    public void Button_Next()
    {
        Debug.Log("Load next level");
    }
}
