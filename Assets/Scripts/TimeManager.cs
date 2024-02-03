using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.Rendering;


public class TimeManager : MonoBehaviour
{
    [SerializeField]private VolumeProfile postProcessVolume;
    private ColorAdjustments colorAdjustments;

    public float dayTime = 0.375f; // 7 AM
    public float nightTime = 0.99f; // 12 AM
    public float dayDuration = 90f; // Duration of one full day in seconds
    [Range(0, 1)]
    public float currentTime = 0.25f;
    [DoNotSerialize]
    
    public string timeString;

    void Start()
    {
        DontDestroyOnLoad(this);
        postProcessVolume.TryGet(out colorAdjustments);
        colorAdjustments.postExposure.value = 0f;
    }

    void Update()
    {
        currentTime += Time.deltaTime / dayDuration;

        if(currentTime >= nightTime && GameManager.Instance.currentGameState == GameManager.GameState.Day)
        {
            GameManager.Instance.SwitchGameState(GameManager.GameState.Night);
        }

        if(currentTime >= dayTime && currentTime < nightTime && GameManager.Instance.currentGameState == GameManager.GameState.Night)
        {
            GameManager.Instance.SwitchGameState(GameManager.GameState.Day);
        }   

        if (currentTime >= 1f) //Clamp time 0 to 1
        {
            currentTime -= 1f;
        }

        if(GameManager.Instance.currentGameState == GameManager.GameState.Day)
            colorAdjustments.postExposure.value -= 0.01f * Time.deltaTime; //magic number i know
        else
            colorAdjustments.postExposure.value = 0f;
            

        //Updating text string
        int hours = Mathf.FloorToInt(currentTime * 24);
        int minutes = Mathf.FloorToInt((currentTime * 24 * 60) % 60);
        string amPm = (hours < 12) ? "AM" : "PM";
        hours = (hours % 12 == 0) ? 12 : hours % 12;

        timeString = $"Time: {hours:D2}:{minutes:D2} {amPm}";
    }

}

