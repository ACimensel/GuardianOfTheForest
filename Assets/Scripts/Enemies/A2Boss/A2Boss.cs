

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2Boss : MonoBehaviour
{
    [SerializeField] float invulnerabilityTime = 1f;
    [SerializeField] float flashTime = 0.8f;
    [SerializeField] GameObject dronePrefab;
    [SerializeField] GameObject essence;

    public int health = 60;
    public float walkSpeed = 5f;
    public bool isFlipped = false;
    public bool facingRight = false;
    public Transform player;

    private Renderer rend;
    private Animator animator;

    private Color startColor;
    private PersistantData PD;


    private bool isDamageEnabled = true;
    private float destroyTime = 1.4f;

    public bool hitWall = false;


    AnimatorClipInfo[] animCurrentClipInfo;
    private string animationName;
    Rigidbody2D rb;

    private float attackRange = 2f;
    public bool isIdle = true;

    Vector3 localScale;

    public bool isInAir = false;


    void Awake()
    {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        if(PD.area2BossKilled)
            Destroy(gameObject);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        rb = animator.GetComponent<Rigidbody2D>();
        startColor = rend.material.color;
        localScale = this.transform.localScale;
    }

    void Start()
    {
        if(!PD.area2BossCinematicPlayed)
            gameObject.SetActive(false);
    }

    void Update()
    {
        animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        animationName = animCurrentClipInfo[0].clip.name;        

        if (Vector2.Distance(player.position, rb.position) < attackRange)
        {
            animator.SetTrigger("attack");
            isIdle = false;
            animator.SetBool("detectedPlayer", true);
        }

        if (animator.GetBool("detectedPlayer") && isIdle == false && Vector2.Distance(player.position, rb.position) < 4f && Vector2.Distance(player.position, rb.position) > attackRange)
        {
            float randomNumber = Random.Range(-10.0f, 10.0f);
            if (randomNumber < 0)
            {
                animator.SetTrigger("jumpAttack");
            }
            else
            {
                animator.SetTrigger("summonDronesAttack");
            }
        }

        if (animator.GetBool("detectedPlayer") && isIdle == false)
        {
            LookAtPlayer();
            if (Vector2.Distance(player.position, rb.position) > attackRange)
            {
                Vector2 target = new Vector2(player.position.x, rb.position.y);
                Vector2 newPos = Vector2.MoveTowards(rb.position, target, walkSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
            }

        }
    }

    public void LookAtPlayer()
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
                animator.SetBool("isDead", true);
                StartCoroutine("DestroyAfterTime");
                gameObject.layer = LayerMask.NameToLayer("Dead");
                essence.SetActive(true);

                GetComponent<DropOrbs>().Drop();
                PD.area2BossKilled = true;
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
    }





}










