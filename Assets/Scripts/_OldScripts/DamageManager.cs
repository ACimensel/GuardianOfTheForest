using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageManager : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    [SerializeField] int currentHealth = 10;
    [SerializeField] float flashTime = 0.2f;
    [SerializeField] float invulnerabilityTime = 3f;
	private Renderer rend;
	private Color startColor;
    private Animator animator;
	private Rigidbody2D rb;
	public bool damageEnabled = true;

    void Awake(){
		rend = GetComponent<Renderer>();
		startColor = rend.material.color;
        animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col){
		if(LayerMask.LayerToName(col.gameObject.layer) == "PlayerAttack" && damageEnabled){
            TakeDamage(1); // TODO change damage of ranged projectile, or make projectile call TakeDamage() and get rid of OnTriggerEnter
        }
    }

    public void TakeDamage(int damageTaken){
        if(damageEnabled){
            damageEnabled = false;
            currentHealth -= damageTaken;

            if(currentHealth > 0){
                // if(col.gameObject.transform.position.x < this.gameObject.transform.position.x) 
                //     rb.AddForce(new Vector2(50f, 30f));
                // else 
                //     rb.AddForce(new Vector2(-50f, 30f));

                StartCoroutine ("BecomeInvulnerable");
            }
            else{
                SetAllCollidersAndRbStatus(false);
                animator.SetBool("isDead", true);
            }
        }
    }

	void SetAllCollidersAndRbStatus(bool active){
		GetComponent<Rigidbody2D>().isKinematic = !active;

		foreach(Collider2D c in GetComponents<Collider2D>())
			c.enabled = active;
 	}

    void Reset(){
        currentHealth = maxHealth;
        animator.SetBool("isDead", false);
        SetAllCollidersAndRbStatus(true);
        damageEnabled = true;
    }

	IEnumerator BecomeInvulnerable(){
		animator.SetTrigger("isHurt");
		Coroutine flash = StartCoroutine ("Flash");

		yield return new WaitForSeconds(invulnerabilityTime);

        StopCoroutine(flash);
		startColor.a = 1f;
		rend.material.color = startColor;
        damageEnabled = true;
	}

    IEnumerator Flash(){
        startColor.a = 0.6f;
        float delta = 0.2f;

        while(true){
            delta *= -1;
            startColor.a += delta;
            rend.material.color = startColor;
            yield return new WaitForSeconds(flashTime);
        }
	}
}
