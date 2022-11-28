using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Drone : MonoBehaviour
{
    public float explosionCountdown;

    private float destroyTime = 0.5f;
    public int health = 1;
    public float patrolSpeed = 50f;
    public bool isFlipped = false;
    public bool facingRight = false;
    public Transform player;

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
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        rb = animator.GetComponent<Rigidbody2D>();
        startColor = rend.material.color;
        player = GameObject.Find("Guardian").GetComponent<Transform>();
        explosionCountdown = 5f;
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

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * patrolSpeed * 10f * Time.deltaTime;

        detectedPlayer = animator.GetBool("detectedPlayer");

        if (detectedPlayer)
        {
            rb.AddForce(force);
        }


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDist)
        {
            currentWaypoint++;
        }

    }

    void CountdownToExplode()
    {
        if (explosionCountdown > 0f)
        {
            explosionCountdown -= Time.deltaTime;
            Debug.Log(explosionCountdown);
        }

        if (explosionCountdown < 0f)
        {
            TakeDamage(1);
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
        if (detectedPlayer)
        {
            CountdownToExplode();
        }
    }


    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }

    public void TakeDamage(int damageTaken)
    {
        StopMoving();
        animator.SetBool("isDead", true);
        StartCoroutine("DestroyAfterTime");
        gameObject.layer = LayerMask.NameToLayer("Dead");
        GetComponent<DropOrbs>().Drop();
    }

    private void StopMoving()
    {
        rb.velocity = new Vector2(0, 0);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "PlayerBolt")
        {
            TakeDamage(Bolt.boltDamage);
        }

        if (layerName == "Player")
        {
            TakeDamage(1);
        }

        if (layerName == "Wall")
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("Do something here");
        }

    }


}
