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
    [SerializeField] GameObject chargePrefab;
    [SerializeField] GameObject essence;


    [SerializeField] float startPjCooldown;
    private float pjCooldown;

    [SerializeField] float startChargeCooldown;
    private float chargeCooldown;

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
    bool isPlayerFar;

    Vector3 localScale;

    AnimatorClipInfo[] animCurrentClipInfo;
    public string animationName;

    private float dashTime;
    private float startDashTime = 1f;

    BoxCollider2D bc2d;
    CircleCollider2D cc2d;

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
        chargeCooldown = startChargeCooldown;
        localScale = this.transform.localScale;

        dashTime = startDashTime;
        bc2d = GetComponent<BoxCollider2D>();
        cc2d = GetComponent<CircleCollider2D>();

    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        detectedPlayer = animator.GetBool("detectedPlayer");
        isDead = animator.GetBool("isDead");
        isPlayerFar = animator.GetBool("isPlayerFar");
    }

    void Update()
    {
        animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        animationName = animCurrentClipInfo[0].clip.name;
    
        if (animationName == "a1_dash")
        {
            bc2d.enabled = false;
            cc2d.enabled = false;
        }

        if (animationName != "a1_dash")
        {
            bc2d.enabled = true;
            cc2d.enabled = true;
        }



        if (detectedPlayer && isDead == false)
        {
            LookAtPlayer();
            this.rb.velocity = new Vector3(0, 0, 0);

            if (dashTime <= 0)
            {
                dashTime = startDashTime;
                // rb.velocity = Vector2.zero;
            }

            else
            {
                if (Vector2.Distance(player.position, rb.position) < 0.8f && facingRight == false)
                {
                    animator.SetTrigger("isPlayerClose");
                    this.rb.velocity = new Vector2(-20f, 0);
                    this.transform.Translate(new Vector2(-2, 0));
                }

                if (Vector2.Distance(player.position, rb.position) < 0.8f && facingRight)
                {
                    animator.SetTrigger("isPlayerClose");
                    this.rb.velocity = new Vector2(20f, 0);
                    this.transform.Translate(new Vector2(2, 0));
                }
                dashTime -= Time.deltaTime;
            }



            if (Vector2.Distance(player.position, rb.position) > 7f)
            {
                animator.SetBool("isPlayerFar", true);
            }

            if (Vector2.Distance(player.position, rb.position) < 7f)
            {
                animator.SetBool("isPlayerFar", false);
            }


            if (pjCooldown <= 0 && !isPlayerFar && Vector2.Distance(player.position, rb.position) < 7f)
            {
                if (facingRight)
                {
                    GameObject bossProjectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x + 1f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                    bossProjectile.SendMessage("SetVelocity", "right");
                }
                else
                {
                    GameObject bossProjectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x + -1f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                    bossProjectile.SendMessage("SetVelocity", "left");
                }
                pjCooldown = startPjCooldown;
            }

            if (chargeCooldown <= 0 && isPlayerFar)
            {
                if (facingRight)
                {
                    GameObject bossCharge = Instantiate(chargePrefab, new Vector3(this.transform.position.x + 2.5f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                    bossCharge.SendMessage("SetVelocity", "right");
                }
                else
                {
                    GameObject bossCharge = Instantiate(chargePrefab, new Vector3(this.transform.position.x + -2.5f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                    bossCharge.SendMessage("SetVelocity", "left");
                }
                chargeCooldown = startChargeCooldown;
            }


            else
            {
                pjCooldown -= Time.deltaTime;
                chargeCooldown -= Time.deltaTime;
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
