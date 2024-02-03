using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected Transform attackPoint;
    protected Rigidbody2D rb;
    protected Animator animator;
    
    protected int health = 100;
    protected int attackDamage = 10;

    protected float moveSpeed = 10f;
    protected float stoppingDistance = 2.2f;
    protected float attackRange = 2.5f;
    protected float attackSpeed = 0.5f;
    protected float attackTime = 0f;
    
    protected bool hurt;
    protected bool attacked;
    protected bool dead;
    protected bool isFacingLeft = true;

    public virtual void MoveTowards(Transform target)
    {
        Vector3 directionToPlayer = target.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        //Flip Sprite
        if (rb.velocity.x > 0 && isFacingLeft || rb.velocity.x < 0 && !isFacingLeft)
        {
            isFacingLeft = !isFacingLeft;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        if (distanceToPlayer > stoppingDistance)
        {
            rb.velocity = directionToPlayer.normalized * moveSpeed;
        }
        else
        {
            //Stop the enemy and call the attack function
            rb.velocity = new Vector2(0f, 0f);
            Attack();
        }

    }

    public virtual void TakeDamage(int damage)
    {
        if(health > 0)
        {
            if(damage > health)
                health = 0;
            else
                health -= damage;

            attackTime += 0.3f; //Stun enemy
            hurt = true;
            if(health <= 0)
                Die();
        }
    }

    protected virtual void Die()
    {
        dead = true;
        SoundManager.Instance.PlaySound(SoundManager.Sounds.EnemyDie);
        GetComponentInParent<EnemyManager>().enemiesDefeated += 1;
            if(GetComponentInParent<EnemyManager>().enemiesDefeated % 10 == 0)
            {
                GetComponentInParent<EnemyManager>().playerCanRestoreHealth = true;
            }
        Destroy(gameObject, 1f);
        
    }


    protected virtual void Attack()
    {
        if(Time.time >= attackTime)
        {
            attacked = true;
            attackTime = Time.time + attackSpeed;
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, attackRange);
            
            foreach(var collider in hitPlayer)
            {
                if(collider.CompareTag("Player"))
                    collider.GetComponent<Player>().TakeDamage(attackDamage);
            }
        }
    }
    

    protected void AnimationHandler()
    {
        if(rb.velocity.x != 0f)
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);
        
        if(hurt)
        {
            animator.SetTrigger("Hurt");
            hurt = false;
        }

        if(attacked)
        {
            animator.SetTrigger("Attacked");
            attacked = false;
        }

        if(dead)
        {
            animator.SetTrigger("Dead");
            dead = false;
        }
    }

}
