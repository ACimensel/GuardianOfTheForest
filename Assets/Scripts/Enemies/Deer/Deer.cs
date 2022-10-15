using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour
{

    public float patrolSpeed = 5f;
    public float walkSpeed = 3.5f;

    public int health = 5;
    [SerializeField] float invulnerabilityTime = 3f;
    private bool isDamageEnabled = true;
    float destroyTime = 4f;

    public bool isFlipped = false;
    public bool facingRight = false;

    public Transform player;
    private Renderer renderer;
    private Animator animator;
    Deer deer;

    private Color startColor;
    [SerializeField] float flashTime = 0.2f;


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
        startColor = renderer.material.color;
    }



    IEnumerator Flash()
    {
        startColor.a = 0.6f;
        float delta = 0.2f;

        while (true)
        {
            delta *= -1;
            startColor.a += delta;
            renderer.material.color = startColor;
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
        renderer.material.color = startColor;
        isDamageEnabled = true;
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "PlayerAttack" && isDamageEnabled)
        {
            isDamageEnabled = false;
            health--;

            if (health > 0)
            {
                StartCoroutine("BecomeInvulnerable");
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

