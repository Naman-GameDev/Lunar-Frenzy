using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Health : MonoBehaviour
{
    [SerializeField] private Player playerController;
    private TextMeshProUGUI healthText;

    private void Start()
    {
        healthText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (playerController.health >= 0)
            healthText.text = playerController.health.ToString();
        else
            healthText.text = "0";

    }
}
