using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] public Transform playerTransform;

    private Vector2 spawnPosition = new Vector3(1f, 1f, 0f);

    public int enemiesDefeated = 0;

    public bool playerCanRestoreHealth;

    [SerializeField] private float spawnInterval = 2f;
    private float spawnTime = 0f;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Time.time >= 3f) //Delay so player can read controls
        {
            spawnPosition = new Vector2(playerTransform.position.x + 10, -1.7f);
            if (Time.time >= spawnTime)
            {
                spawnTime = spawnInterval + Time.time;
                GameObject spawnedEnemy = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)], spawnPosition, Quaternion.identity, transform);
                if (spawnedEnemy.tag == "Skeleton")
                    spawnedEnemy.transform.position = new Vector3(spawnedEnemy.transform.position.x, -2.19f, spawnedEnemy.transform.position.z);
            }
        }
    }

}
