using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : MonoBehaviour
{
    [SerializeField] float invulnerabilityTime = 0.5f;
    [SerializeField] float flashTime = 0.2f;

    public float patrolSpeed = 5f;
    public float walkSpeed = 7.5f;
    public int health = 50;
    public bool isFlipped = false;
    public bool facingRight = false;
    public Transform player;

    private Renderer rend;
    private Animator animator;
    private Color startColor;
    private bool isDamageEnabled = true;
    private float destroyTime = 4f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
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

    void OnTriggerEnter2D(Collider2D col)
    {
		if(LayerMask.LayerToName(col.gameObject.layer) == "PlayerBolt" && isDamageEnabled){   
            TakeDamage(Bolt.boltDamage); 
        }

        if (LayerMask.LayerToName(col.gameObject.layer) == "Wall")
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
        }
    }


    public void TakeDamage(int damageTaken){
        if(isDamageEnabled){
            isDamageEnabled = false;
            health -= damageTaken;

            if(health > 0){
                StartCoroutine ("BecomeInvulnerable");
            }
            else{
                gameObject.layer = LayerMask.NameToLayer("Dead");
                foreach (Transform child in transform){
                    child.gameObject.layer = LayerMask.NameToLayer("Dead");
                }

                animator.SetBool("isDead", true);
                StartCoroutine("DestroyAfterTime");
                
                GetComponent<DropOrbs>().Drop();
            }
        }
    }
}
