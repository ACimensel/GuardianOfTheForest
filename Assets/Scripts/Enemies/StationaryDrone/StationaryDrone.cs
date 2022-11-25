using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryDrone : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float startPjCooldown;
    private float pjCooldown;


    private float destroyTime = 0.5f;
    public int health = 1;
    public bool isFlipped = false;
    public bool facingRight = false;
    public Transform player;

    private Renderer rend;
    private Animator animator;
    private Color startColor;
    private Rigidbody2D rb;

    bool detectedPlayer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        rb = animator.GetComponent<Rigidbody2D>();
        startColor = rend.material.color;
        pjCooldown = startPjCooldown;
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
    { }


    void FixedUpdate()
    {
        detectedPlayer = animator.GetBool("detectedPlayer");
        LookAtPlayer();
    }


    void Update()
    {
        if (pjCooldown <= 0 && detectedPlayer)
        {
            if (facingRight)
            {
                GameObject droneProjectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x + 1f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                droneProjectile.SendMessage("SetVelocity", "right");

            }

            else
            {
                GameObject droneProjectile = Instantiate(projectilePrefab, new Vector3(this.transform.position.x + -1f, this.transform.position.y + 0.2f, 0f), Quaternion.identity);
                droneProjectile.SendMessage("SetVelocity", "left");
            }
            pjCooldown = startPjCooldown;

        }
        else
        {
            pjCooldown -= Time.deltaTime;
        }
    }


    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }

    public void TakeDamage(int damageTaken)
    {
        animator.SetBool("isDead", true);
        StartCoroutine("DestroyAfterTime");
        gameObject.layer = LayerMask.NameToLayer("Dead");
        GetComponent<DropOrbs>().Drop();

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
