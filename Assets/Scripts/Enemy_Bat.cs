using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bat : EnemyBase
{
    private Transform playerTransform;
    
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _attackRange = 1.7f;
    [SerializeField] private float _stoppingDistance = 1.3f;
    [SerializeField] private int _attackDamage = 2;

    private void Start()
    {
        //Initialize fields
        playerTransform = GetComponentInParent<EnemyManager>().playerTransform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackPoint = gameObject.transform.GetChild(0);
    }

    private void Update()
    {
        attackDamage = _attackDamage;
        moveSpeed = _moveSpeed;
        attackSpeed = _attackSpeed;
        attackRange = _attackRange;
        stoppingDistance = _stoppingDistance;
        MoveTowards(playerTransform);
        AnimationHandler();
    }

}
