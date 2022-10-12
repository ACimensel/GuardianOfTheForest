using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageManager : MonoBehaviour
{
	[System.Serializable] public class BoolEvent : UnityEvent<bool> { }
	public BoolEvent OnChangeMovement;

    [SerializeField] float flashTime = 0.2f;
    [SerializeField] float invulnerabilityTime = 3f;
	private Renderer renderer;
	private Color startColor;
    private Animator animator;
	private Rigidbody2D rb;
	private PlayerStats ps;
	private bool damageEnabled = true;

    void Awake(){
		renderer = GetComponent<Renderer>();
		startColor = renderer.material.color;
        animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
        ps = GetComponent<PlayerStats>();

		if(OnChangeMovement == null)
			OnChangeMovement = new BoolEvent();
    }

    // Update is called once per frame
    void OnTriggerEnter2D (Collider2D col){
        Debug.Log("HIT");
		if (LayerMask.LayerToName(col.gameObject.layer) == "Damage" && damageEnabled){
            damageEnabled = false;
            OnChangeMovement.Invoke(false);

            Debug.Log("DAMAGED");
            ps.health--;

            if(ps.health > 0){
                if(col.gameObject.transform.position.x < this.gameObject.transform.position.x) 
                    rb.AddForce(new Vector2(300f, 100f));
                else 
                    rb.AddForce(new Vector2(-300f, 100f));

                StartCoroutine ("BecomeInvulnerable");
		        StartCoroutine ("TurnOnMovement");
            }
            else{
                animator.SetTrigger("isDead");
            }
        }
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

    IEnumerator TurnOnMovement(){
        // AnimatorClipInfo[] animatorinfo = animator.GetCurrentAnimatorClipInfo(0);
		// string current_animation;

    	// do{
		// 	animatorinfo = animator.GetCurrentAnimatorClipInfo(0);
		// 	current_animation = animatorinfo[0].clip.name;
		// } while(current_animation != "FemWarrior_hurt");
        
        yield return new WaitForSeconds(5f);
        
        OnChangeMovement.Invoke(true);
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
