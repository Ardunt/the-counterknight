﻿using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool destroyEnemy = false;
    private static Color enemyColor;
    private static Color ghostColor = new Color(45, 0, 255);

    private const float INCREASE_AMOUNT = -0.25f;
    private const float INCREASE_LIMIT = -2.75f;
    private const float DEFAULT_SPEED = -2;
    private static float enemySpeed = DEFAULT_SPEED;

    private const float DEFAULT_SHOOT_DELAY = 2;
    private static float shootDelay = DEFAULT_SHOOT_DELAY;
    
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    
    private GameObject arrowPrefab;
    private Animator animator;
    private AudioSource aud; // NOTE: Default audioClip of "aud" is "EnemyAttack" 

    void Start()
    {
        if(enemyColor == ghostColor)
        {
            GetComponent<SpriteRenderer>().color = ghostColor;
        }

        boxCol = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        arrowPrefab = (GameObject)Resources.Load("Prefabs/Arrow");
        
        rb.gravityScale = 0;

        if(transform.position.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            boxCol.offset = new Vector2( -boxCol.offset.x, boxCol.offset.y);
        }

        animator = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        StartCoroutine(fireArrowAfterDelay());

        int supriseAttackChance = Random.Range(0, 3);

        if(supriseAttackChance == 0)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            arrow.GetComponent<Arrow>().setArcherObj(gameObject);
            
            if(UIHandler.soundOn)
            {
                aud.Play();
            } 
        }
    }

    void FixedUpdate()
    {
        if(destroyEnemy)
        {
            Destroy(gameObject);
        }
        else
        {
            rb.velocity = transform.up * enemySpeed * Time.deltaTime;

            transform.Translate(0, enemySpeed * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.GetComponent<Lighting>())
        {
            animator.SetBool("isDead", true);
            GetComponent<SpriteRenderer>().color = Color.white;
            Destroy(boxCol);            
        }    
    }

    IEnumerator fireArrowAfterDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        animator.SetTrigger("attack");

        animator.SetBool("haveShot", true);
        // Wait for animation playing
        AnimationClip attackClip = (AnimationClip)Resources.Load("Animations/DarkElf/DarkElfAttack");
        yield return new WaitForSeconds( attackClip.length - 0.2f ); // -0.2f for shoot delay

        if(!animator.GetBool("isDead"))
        {
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            arrow.GetComponent<Arrow>().setArcherObj(gameObject);
            
            if(UIHandler.soundOn)
            {
                aud.Play();
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    
    public static void resetEnemy()
    {
        enemySpeed = DEFAULT_SPEED;
        shootDelay = DEFAULT_SHOOT_DELAY;
        enemyColor = new Color(0, 0, 0);
    }

    public static void buffEnemy()
    {
        if(enemySpeed > INCREASE_LIMIT)
        {
            enemySpeed += INCREASE_AMOUNT;
            shootDelay += INCREASE_AMOUNT;
        }        
    }

    public static void setGhostColor()
    {
        enemyColor = ghostColor;
    }
}