using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.TextCore;

public class UIManager : MonoBehaviour
{   
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI transitionScreenText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI RestartText;
    [SerializeField] private TextMeshProUGUI enemiesDefeatedText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject statIncrease;
    [SerializeField] private CanvasGroup sceneChangeTransition;
    [SerializeField] private CanvasGroup controls;
    [SerializeField] private CanvasGroup gameOverScreen;
    [SerializeField] private CanvasGroup gameRestartTransition;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField]private TMP_ColorGradient dayGradient, nightGradient;

    private bool fadeInCalled = false;
    private bool controlsdisplayed = false;

    private void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(Fade(gameRestartTransition, 1f, 0f, 1f));
    }

    private void OnEnable() 
    {
        GameManager.OnGameStateChange += ShowGameOver;
    }

    private void OnDisable() 
    {
        GameManager.OnGameStateChange -= ShowGameOver;
    }

    private void Update()
    {
        ShowSceneChangeTransition();
        UpdateTimeText();
        UpdateGameOverText();
        if (Time.time >= 5f && !controlsdisplayed) //Make sure controls are not shown on restarts
            FadeControls();
    }

    private void ShowGameOver(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.GameOver)
        {
            StartCoroutine(Fade(gameOverScreen, gameOverScreen.alpha, 1f, 1f));
        }
    }

    private void FadeControls()
    {
        StartCoroutine(Fade(controls, 1f, 0f, 2f));
        controlsdisplayed = true;
        controls.gameObject.SetActive(false);
    }

    private void UpdateGameOverText()
    {
        gameOverScoreText.text = $"Your Score: <color=#00FF48>{enemyManager.enemiesDefeated}</color>";
    }

    private void UpdateTimeText()
    {
        timeText.text = timeManager.timeString;
    }

    private void UpdateColorForAllTexts(TMP_ColorGradient newGradient)
    {
        timeText.colorGradientPreset = newGradient;
        transitionScreenText.colorGradientPreset = newGradient;
        healthText.colorGradientPreset = newGradient;
        gameOverText.colorGradientPreset = newGradient;
        enemiesDefeatedText.colorGradientPreset = newGradient;
        gameOverScoreText.colorGradientPreset = newGradient;
        RestartText.colorGradientPreset = newGradient;
    }

    private void ShowSceneChangeTransition()
    {
        if(timeManager.currentTime >= 0.98f && GameManager.Instance.currentGameState == GameManager.GameState.Day)
        {
            if(!fadeInCalled)
            {
                UpdateColorForAllTexts(nightGradient);
                ShowStatIncrease(true);
                transitionScreenText.text = "Frenzy Begins!";
                FadeInSceneChangeTransition();
                fadeInCalled = true;
            }
        }

        else if(timeManager.currentTime >= 0.28f && GameManager.Instance.currentGameState == GameManager.GameState.Night)
        {
            if(!fadeInCalled)
            {
                UpdateColorForAllTexts(dayGradient);
                ShowStatIncrease(false);
                transitionScreenText.text = "Frenzy Ends!";
                FadeInSceneChangeTransition();
                fadeInCalled = true;
            }
        }
    }

    private void ShowStatIncrease(bool status)
    {
        statIncrease.SetActive(status);
    }

    private void FadeInSceneChangeTransition()
    {
        StartCoroutine(Fade(sceneChangeTransition, sceneChangeTransition.alpha, 1f, 0.5f));
        Invoke(nameof(FadeOutSceneChangeTransition), 2f);
    }

    public void FadeOutSceneChangeTransition()
    {
        StartCoroutine(Fade(sceneChangeTransition, sceneChangeTransition.alpha, 0f, 0.5f, () => {fadeInCalled = false;}));
    }

    IEnumerator Fade(CanvasGroup group, float startAlpha, float endAlpha, float duration, Action onComplete = null)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        group.alpha = endAlpha;
        onComplete?.Invoke();
    }
}

