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
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject essence;


    [SerializeField] float startPjCooldown;
    private float pjCooldown;

    [SerializeField] float invulnerabilityTime = 1f;
    [SerializeField] float flashTime = 0.8f;
    private float destroyTime = 1.2f;
    public int health = 10;

    public bool isFlipped = false;
    public Transform player;
    private bool isDamageEnabled;
    private Renderer rend;
    private Color startColor;

    bool detectedPlayer;
    bool isDead;

    Vector3 localScale;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        facingRight = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        isDamageEnabled = true;
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        pjCooldown = startPjCooldown;
        localScale = this.transform.localScale;
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        detectedPlayer = animator.GetBool("detectedPlayer");
        isDead = animator.GetBool("isDead");
    }

    void Update()
    {
        if (detectedPlayer && isDead == false)
        {
            LookAtPlayer();
            this.rb.velocity = new Vector3(0, 0, 0);

            if (pjCooldown <= 0)
            {
                if (facingRight)
                {
                    GameObject crowProjectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x + 1f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                    crowProjectile.SendMessage("SetVelocity", "right");
                }
                else
                {
                    GameObject crowProjectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x + -1f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                    crowProjectile.SendMessage("SetVelocity", "left");

                }
                pjCooldown = startPjCooldown;
            }
            else
            {
                pjCooldown -= Time.deltaTime;
            }
        }

        else
        {
            if (isDead != true)
            {
                Patrol();
            }
        }

    }

    void LateUpdate()
    {
        if (((facingRight) && (localScale.x > 0)) || ((!facingRight) && (localScale.x < 0)))
        {
            localScale.x *= -1;
        }
        transform.localScale = localScale;
    }

    public void LookAtPlayer()
    {
        if (transform.position.x > player.position.x && facingRight)
        {
            facingRight = false;
        }
        else if (transform.position.x < player.position.x && !facingRight)
        {
            facingRight = true;
        }
    }

    void Patrol()
    {
        if (facingRight)
        {
            rb.velocity = new Vector2(patrolSpeed, 0f);
        }
        else
        {
            rb.velocity = new Vector2(-patrolSpeed, 0f);
        }
    }

    public void TakeDamage(int damageTaken)
    {
        if (isDamageEnabled)
        {
            isDamageEnabled = false;
            health -= damageTaken;

            if (health > 0)
            {
                StartCoroutine("BecomeInvulnerable");
            }
            else
            {
                animator.SetBool("isDead", true);
                StartCoroutine("DestroyAfterTime");
                gameObject.layer = LayerMask.NameToLayer("Dead");
                rb.velocity = new Vector2(0f, 0f);
                essence.SetActive(true);
            }
        }
    }

    IEnumerator BecomeInvulnerable()
    {
        animator.SetTrigger("isHurt");
        Coroutine flash = StartCoroutine("Flash");

        yield return new WaitForSeconds(invulnerabilityTime);

        StopCoroutine(flash);
        startColor.a = 1f;
        rend.material.color = startColor;
        isDamageEnabled = true;
    }

    IEnumerator Flash()
    {
        startColor.a = 0.6f;
        float delta = 0.2f;

        while (true)
        {
            delta *= -1;
            startColor.a += delta;
            rend.material.color = startColor;
            yield return new WaitForSeconds(flashTime);
        }
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "PlayerBolt" && isDamageEnabled)
        {
            TakeDamage(Bolt.boltDamage);
        }

        if (LayerMask.LayerToName(col.gameObject.layer) == "Wall")
        {
            facingRight = !facingRight;
        }
    }




}
