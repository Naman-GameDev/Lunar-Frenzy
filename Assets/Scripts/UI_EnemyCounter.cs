using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_EnemyCounter : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    private TextMeshProUGUI enemiesDefeatedText;
    private void Start()
    {
        enemiesDefeatedText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        enemiesDefeatedText.text = $"Enemies Defeated: <color=#00FF48>{enemyManager.enemiesDefeated}</color>";
    }
}