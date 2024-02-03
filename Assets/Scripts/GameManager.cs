using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Day, Night,GameOver,   
    }

    public static event Action<GameState> OnGameStateChange;

    public GameState currentGameState;
    public static GameManager Instance;

    void Start()
    {
        currentGameState = GameState.Day;

        //Making sure there can only be one instance, also avoids duplication when restarting
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        RestartGame();
    }

    public void SwitchGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Day:
                SceneManager.LoadScene("DayScene");
                break;

            case GameState.Night:
                SceneManager.LoadScene("NightScene");
                break;

            case GameState.GameOver:
                Invoke(nameof(PauseGame), 0.8f);
                break;
        }
        currentGameState = newState;
        Debug.Log(currentGameState);
        OnGameStateChange?.Invoke(newState);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void RestartGame()
    {
        if(Input.GetKeyDown(KeyCode.Return) && currentGameState == GameState.GameOver)
        {
            SceneManager.LoadScene("MainScene");
            DestroyPersistentGameObjectsOnRestart();
            currentGameState = GameState.Day;
            if(Time.timeScale != 1)
                Time.timeScale = 1;
        }
    }

    private void DestroyPersistentGameObjectsOnRestart()
    {
        Destroy(GameObject.Find("Player"));
        Destroy(GameObject.Find("Enemy Manager"));
        Destroy(GameObject.Find("UI"));
        Destroy(GameObject.Find("Time Manager"));
        Destroy(GameObject.Find("Virtual Camera"));
        Destroy(GameObject.Find("Main Camera"));
        Destroy(GameObject.Find("Sound Manager"));

    }
}
