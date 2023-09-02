using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
    }
    public void Play()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void FreePlay()
    {
        SceneManager.LoadScene("Free Play");
    }

    public void Easy()
    {
        SceneManager.LoadScene("Easy");
    }

    public void Main()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
