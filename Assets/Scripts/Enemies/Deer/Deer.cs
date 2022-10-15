using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour
{
    public Transform player;
    public bool isFlipped = false;

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }


    private Animator animator;
    Deer deer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        deer = animator.GetComponent<Deer>();
    }


    public int health = 1;
    public bool isInvulnerable = false;

    void Die()
    {
        animator.SetTrigger("isDead");

    }

    float destroyTime = 3f;
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
            animator.SetTrigger("isDead");
            StartCoroutine("DestroyAfterTime");

        }

    }

}

