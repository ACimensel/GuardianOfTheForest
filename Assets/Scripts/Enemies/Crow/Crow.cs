using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Crow : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Sprite pj0;
    [SerializeField] Sprite pj1;
    [SerializeField] Sprite pj2;
    [SerializeField] Sprite pj3;

    [SerializeField] float startPjCooldown;
    private float pjCooldown;

    [SerializeField] float invulnerabilityTime = 1f;
    [SerializeField] float flashTime = 0.8f;
    private float destroyTime = 1.2f;
    public int health = 50;
    public float patrolSpeed = 50f;
    public bool isFlipped = false;
    public bool facingRight = false;
    public Transform player;

    private bool isDamageEnabled;
    private Renderer rend;
    private Animator animator;
    private Color startColor;
    private Rigidbody2D rb;

    public float nextWaypointDist = 2f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    bool detectedPlayer;

    void Awake()
    {
        isDamageEnabled = true;
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        rb = animator.GetComponent<Rigidbody2D>();
        startColor = rend.material.color;
        pjCooldown = startPjCooldown;
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
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

    void Start()
    {
        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, 1f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, player.position, OnPathComplete);
        }
    }

    void FixedUpdate()
    {

        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        // for freezing y pos movement. Currently using rb constraints under inspector instead
        // Vector2 pathPosition = new Vector2(path.vectorPath[currentWaypoint].x, 0);
        // Vector2 crowPosition = new Vector2(rb.position.x, 0);
        // Vector2 direction = (pathPosition - crowPosition).normalized;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * patrolSpeed * Time.deltaTime;

        detectedPlayer = animator.GetBool("detectedPlayer");

        if (detectedPlayer)
        {
            if (Mathf.Abs(rb.position.x - player.position.x) > 5)
            {
                rb.AddForce(force);
            }
        }


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDist)
        {
            currentWaypoint++;
        }

    }

    void AdjustDirection()
    {
        if (rb.velocity.x > 0)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            facingRight = true;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            facingRight = false;
        }
    }

    void Update()
    {
        AdjustDirection();

        if (pjCooldown <= 0 && detectedPlayer)
        {
            GameObject crowProjectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x + 0.4f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
            pjCooldown = startPjCooldown;
        }
        else
        {
            pjCooldown -= Time.deltaTime;
        }
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
                
                GetComponent<DropOrbs>().Drop();
            }
        }
    }



    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "PlayerBolt" && isDamageEnabled)
        {
            TakeDamage(Bolt.boltDamage);
        }

        if (LayerMask.LayerToName(col.gameObject.layer) == "Wall")
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
        }
    }


}
