using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageManager : MonoBehaviour
{
    [SerializeField] int health = 1;
    [SerializeField] float flashTime = 0.2f;
    [SerializeField] float invulnerabilityTime = 3f;
	private Renderer renderer;
	private Color startColor;
    private Animator animator;
	private Rigidbody2D rb;
	private bool damageEnabled = true;

    void Awake(){
		renderer = GetComponent<Renderer>();
		startColor = renderer.material.color;
        animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D (Collider2D col){
        Debug.Log(LayerMask.LayerToName(col.gameObject.layer));
		if (LayerMask.LayerToName(col.gameObject.layer) == "PlayerAttack" && damageEnabled){
            damageEnabled = false;
            health--;

            if(health > 0){
                if(col.gameObject.transform.position.x < this.gameObject.transform.position.x) 
                    rb.AddForce(new Vector2(50f, 30f));
                else 
                    rb.AddForce(new Vector2(-50f, 30f));

                StartCoroutine ("BecomeInvulnerable");
            }
            else{
				SetAllCollidersAndRbStatus(false);
                animator.SetTrigger("isDead");
            }
        }
    }
	void SetAllCollidersAndRbStatus(bool active){
		GetComponent<Rigidbody2D>().isKinematic = !active;

		foreach(Collider2D c in GetComponents<Collider2D>())
			c.enabled = active;
 	}

	IEnumerator BecomeInvulnerable(){
		animator.SetTrigger("isHurt");
		Coroutine flash = StartCoroutine ("Flash");

		yield return new WaitForSeconds (invulnerabilityTime);

        StopCoroutine(flash);
		startColor.a = 1f;
		renderer.material.color = startColor;
        damageEnabled = true;
	}

    IEnumerator Flash(){
        startColor.a = 0.6f;
        float delta = 0.2f;

        while(true){
            delta *= -1;
            startColor.a += delta;
            renderer.material.color = startColor;
            yield return new WaitForSeconds(flashTime);
        }
	}
}
