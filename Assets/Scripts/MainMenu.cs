using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private AudioSource audioSource;
    public Button button;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
    }

    public void PlayClicked()
    {
        LoadMainScene();
    }

    public void QuitClicked()
    {
        Quit();
    }


    private void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void Quit()
    {
        Application.Quit();
    }
}
