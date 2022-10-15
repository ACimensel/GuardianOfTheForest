using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour
{

    public float patrolSpeed = 5f;
    public float walkSpeed = 3.5f;

    public int health = 5;
    public bool isInvulnerable = false;
    float destroyTime = 4f;

    public bool isFlipped = false;
    public bool facingRight = false;

    public Transform player;
    private Renderer renderer;
    private Animator animator;
    Deer deer;


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


    void Awake()
    {
        animator = GetComponent<Animator>();
        deer = animator.GetComponent<Deer>();
        renderer = GetComponent<Renderer>();

    }

    void Die()
    {
        animator.SetTrigger("isDead");

    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "PlayerAttack")
        {
            health--;

            if (health > 0) {
                animator.SetTrigger("isHurt");
            }

            else
            {
                animator.SetTrigger("isDead");
                StartCoroutine("DestroyAfterTime");
            }
        }

        else if (LayerMask.LayerToName(col.gameObject.layer) == "PlayerPresence")
        {
            animator.SetBool("detectedPlayer", true);
        }

        else if (LayerMask.LayerToName(col.gameObject.layer) == "Wall")
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
        }

    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "PlayerPresence")
        {
            animator.SetBool("detectedPlayer", false);
        }
    }


}

