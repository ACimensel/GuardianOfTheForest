using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    [SerializeField] float invulnerabilityTime = 1f;
    [SerializeField] float flashTime = 0.8f;

    public int health = 60;
    public float walkSpeed = 1f;
    public bool isFlipped = false;
    public bool facingRight = false;
    public Transform player;

    private Renderer rend;
    private Animator animator;

    private Color startColor;
    private bool isDamageEnabled = true;
    private float destroyTime = 1.4f;

    public bool hitWall = false;

    AnimatorClipInfo[] animCurrentClipInfo;
    private string animationName;
    Rigidbody2D rb;

    private float attackRange = 2f;
    public bool isIdle = true;

    Vector3 localScale;

    bool detectedPlayer;
    bool isDead;
    bool isPlayerFar;

    private float dodgeTime;
    private float startDodgeTime = 1f;


    private float walkTime = 4f;
    private float idleTime = 2f;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        rb = animator.GetComponent<Rigidbody2D>();
        startColor = rend.material.color;
        localScale = this.transform.localScale;
        dodgeTime = startDodgeTime;
    }

    void Start() { }

    void Update()
    {
        animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        animationName = animCurrentClipInfo[0].clip.name;

        if (walkTime == 4f && animator.GetBool("walk") == false)
        {
            Idle();
        }

        if (animator.GetBool("walk") == true)
        {
            Walk();
        }

    }


    void Idle()
    {
        animator.SetBool("idle", true);

        if (idleTime > 0f)
        {
            idleTime -= Time.deltaTime;
            Debug.Log("Idle time: " + idleTime);
        }

        else
        {
            animator.SetBool("idle", false);
            LookAtPlayer();
            animator.SetTrigger("chargeAttack");
        }
    }

    void Walk()
    {
        idleTime = 2f;

        if (animationName != "FinalBoss_death")
        {
            if (walkTime > 0f)
            {
                if (facingRight)
                {
                    rb.velocity = new Vector2(walkSpeed, 0f);
                }
                else
                {
                    rb.velocity = new Vector2(-walkSpeed, 0f);
                }
                walkTime -= Time.deltaTime;
                Debug.Log("Walk time: " + walkTime);
            }

            else
            {
                animator.SetBool("walk", false);
                walkTime = 4f;
                rb.velocity = new Vector2(0, 0);
                LookAtPlayer();
                animator.SetTrigger("rangedAttack");                
            }
        }



    }


    void FixedUpdate()
    {
        detectedPlayer = animator.GetBool("detectedPlayer");
        isPlayerFar = animator.GetBool("isPlayerFar");
    }

    public void LookAtPlayer()
    {
        if (animationName != "FinalBoss_death")
        {
            if (transform.position.x > player.position.x && facingRight)
            {
                transform.Rotate(0f, 180f, 0f);
                facingRight = false;
            }
            else if (transform.position.x < player.position.x && !facingRight)
            {
                transform.Rotate(0f, 180f, 0f);
                facingRight = true;
            }

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
                animator.SetBool("detectedPlayer", true);
                isIdle = false;
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
                animator.ResetTrigger("chargeAttack");
                animator.ResetTrigger("rangedAttack");
                animator.SetTrigger("isDead");
                gameObject.layer = LayerMask.NameToLayer("Dead");
                GetComponent<DropOrbs>().Drop();
            }
        }
    }

    IEnumerator Flash()
    {
        startColor.a = 0.9f;
        float delta = 0.3f;

        while (true)
        {
            delta *= -1;
            startColor.a += delta;
            rend.material.color = startColor;
            yield return new WaitForSeconds(flashTime);
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
