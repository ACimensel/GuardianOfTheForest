using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A1Boss : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rb;
    public float patrolSpeed = 5f;
    public bool facingRight;
    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        facingRight = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (facingRight)
        {
            rb.velocity = new Vector2(patrolSpeed, 0f);
            spriteRenderer.flipX = true;
        }
        else
        {
            rb.velocity = new Vector2(-patrolSpeed, 0f);
            spriteRenderer.flipX = false;

        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Wall")
        {
            facingRight = !facingRight;
        }
    }




}
