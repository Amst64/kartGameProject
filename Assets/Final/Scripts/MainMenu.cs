using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Button_Exit()
    {
        Application.Quit();
        Debug.Log("The game has been closed");
    }

    public void Button_Play()
    {
        SceneManager.LoadScene("Scene_Canyon");
    }
}
