using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]private Sprite guySprite, wwSprite; 
    [SerializeField]private RuntimeAnimatorController guyAnimController, wwAnimController;
    [SerializeField]private CapsuleCollider2D guyCapsuleCollider, wwCapsuleCollider;
    [SerializeField]private GameObject guyGroundCheck, wwGroundCheck;
    [SerializeField] private EnemyManager enemyManager;
    private Rigidbody2D rb;
    private Animator animator;
    private SoundManager.Sounds attack1Sound;
    private SoundManager.Sounds attack2Sound;

    [SerializeField]private float jumpForce = 5f;
    [SerializeField]private float moveSpeed = 5f;
    private float attack2Speed = 0.5f;
    private float attack1Speed = 0.2f;
    private float attackRange = 1.4f;
    private float attack1Time = 0f;
    private float attack2Time = 0f;

    private int playerCurrentState;
    public int health = 100;
    private int attack1Damage = 15;
    private int attack2Damage = 25;
    
    private bool isGrounded;
    private bool jumped;
    private bool attacked1;
    private bool attacked2;
    private bool jumpAnim;
    private bool hurt;
    private bool dead;

    public static readonly string ENEMY_NIGHTBORNE = "NightBorne";
    public static readonly string ENEMY_SKELETON = "Skeleton";
    public static readonly string ENEMY_BAT = "Bat";

    private void OnEnable()
    {
        GameManager.OnGameStateChange += TransformPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= TransformPlayer;
    }
    private void Start()
    { 
        DontDestroyOnLoad(this);
        playerCurrentState = 0;
        attack1Sound = SoundManager.Sounds.GuyAttack1;
        attack2Sound = SoundManager.Sounds.GuyAttack2;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump") && isGrounded) //Caching input in Update to avoid animation delay
            jumped = true;

        if(!dead)
            Attack();
        GroundCheck();
        AnimationHandler();
        RestoreHealth();
    }

    private void FixedUpdate()
    {
        if(!dead)
        {
            Movement();
            Jump();
        }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new Vector2(horizontalInput, 0f);

        rb.velocity = new Vector2(movement.x * moveSpeed , rb.velocity.y);

        if(rb.velocity.x > 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else if(rb.velocity.x < 0)
            GetComponent<SpriteRenderer>().flipX = true;
        
    }

    private void GroundCheck()
    {
        if(playerCurrentState == 0)
            isGrounded = Physics2D.Raycast(guyGroundCheck.transform.position, Vector2.down, 0.2f);
        else
            isGrounded = Physics2D.Raycast(wwGroundCheck.transform.position, Vector2.down, 0.2f);

    }

    private void Jump()
    {
        if(jumped && isGrounded)
        {
            jumpAnim = true;
            SoundManager.Instance.PlaySound(SoundManager.Sounds.Jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumped = false;
        }
    }

    private void Attack()
    {
        if(Input.GetButtonDown("Attack1") && Time.time >= attack1Time)
        {
            attacked1 = true;

            attack1Time = Time.time + attack1Speed;

            SoundManager.Instance.PlaySound(attack1Sound);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

            foreach(var enemy in hitEnemies)
            {
                if(enemy.CompareTag(ENEMY_NIGHTBORNE))
                    enemy.GetComponent<Enemy_NightBorne>().TakeDamage(attack1Damage);
                
                if(enemy.CompareTag(ENEMY_SKELETON))
                    enemy.GetComponent<Enemy_Skeleton>().TakeDamage(attack1Damage);

                if(enemy.CompareTag(ENEMY_BAT))
                    enemy.GetComponent<Enemy_Bat>().TakeDamage(attack1Damage);
            }
        }

        if(Input.GetButtonDown("Attack2") && Time.time >= attack2Time)
        {
            attacked2 = true;

            attack2Time = Time.time + attack2Speed;

            SoundManager.Instance.PlaySound(attack2Sound);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

            foreach(var enemy in hitEnemies)
            {
                if(enemy.CompareTag(ENEMY_NIGHTBORNE))
                    enemy.GetComponent<Enemy_NightBorne>().TakeDamage(attack2Damage);
                
                if(enemy.CompareTag(ENEMY_SKELETON))
                    enemy.GetComponent<Enemy_Skeleton>().TakeDamage(attack2Damage);

                if(enemy.CompareTag(ENEMY_BAT))
                    enemy.GetComponent<Enemy_Bat>().TakeDamage(attack2Damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if(health > 0)
        {
            health -= damage;
            hurt = true;
            if(health <= 0)
                Die();
        }
    }

    protected virtual void Die()
    {
        dead = true;
        SoundManager.Instance.PlaySound(SoundManager.Sounds.GameOver);
        GameManager.Instance.SwitchGameState(GameManager.GameState.GameOver);
    }

    private void AnimationHandler()
    {
        if(rb.velocity.x != 0f && isGrounded)
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);

        if(jumpAnim)
        {
            animator.SetTrigger("Jumped");
            animator.SetTrigger("Fell");
            jumpAnim = false;
        }

        if(attacked1)
        {
            animator.SetTrigger("Attacked1");
            attacked1 = false;
        }

        if(attacked2)
        {
            animator.SetTrigger("Attacked2");
            attacked2 = false;
        }

        if(hurt)
        {
            animator.SetTrigger("Hurt");
            hurt = false;
        }

        if(dead)
        {
            animator.SetTrigger("Dead");
            dead = false;
        }
    }

    private void TransformPlayer(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.Night)
        {
            playerCurrentState = 1;
            if (health <= 50)
                health += 50;
            else
                health = 100;
            attack1Sound = SoundManager.Sounds.WWAttack1;
            attack2Sound = SoundManager.Sounds.WWAttack2;
            attack1Damage = 25;
            attack2Damage = 50;
            transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            wwCapsuleCollider.enabled = true;
            guyCapsuleCollider.enabled = false;
            GetComponent<SpriteRenderer>().sprite = wwSprite;
            animator.runtimeAnimatorController = wwAnimController;
        }

        else if(gameState == GameManager.GameState.Day)
        {
            playerCurrentState = 0;
            attack1Sound = SoundManager.Sounds.GuyAttack1;
            attack2Sound = SoundManager.Sounds.GuyAttack2;
            attack1Damage = 15;
            attack2Damage = 25;
            transform.localScale = new Vector3(4f, 4f, 4f);
            guyCapsuleCollider.enabled = true;
            wwCapsuleCollider.enabled = false;
            GetComponent<SpriteRenderer>().sprite = guySprite;
            animator.runtimeAnimatorController = guyAnimController;
        }
    }

    private void RestoreHealth()
    {
        if(enemyManager.playerCanRestoreHealth)
            if (health <= 80)
                health += 20;
            else
                health = 100;
        enemyManager.playerCanRestoreHealth = false;
    }   

}