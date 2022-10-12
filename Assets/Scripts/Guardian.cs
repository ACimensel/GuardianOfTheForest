using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : MonoBehaviour {
	[SerializeField] float moveSpeed = 5f;
	[SerializeField] float jumpForce = 800f;
	[SerializeField] float fallTolerance = -5f;
    [SerializeField] float flashTime = 0.2f;
    [SerializeField] float staggerTime = 0.2f;
    [SerializeField] float invulnerabilityTime = 3f;
	[SerializeField] int healthPoints = 3;

	private Renderer renderer;
	private Color startColor;
	private Rigidbody2D rb;
	private Animator anim;
	private float dirX = 0f;
	private bool isMovementEnabled = true;
	private bool facingRight = true;
	private Vector3 localScale;
	private float tolerance = 0.01f;
	private bool isDamageEnabled = true;

	// Use this for initialization
	void Awake(){
		renderer = GetComponent<Renderer>();
		startColor = renderer.material.color;
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		localScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update(){
		if (Input.GetButtonDown ("Jump") && isMovementEnabled && Mathf.Abs(rb.velocity.y) < tolerance)
			rb.AddForce (Vector2.up * jumpForce);

		if (isMovementEnabled)
			dirX = Input.GetAxisRaw ("Horizontal") * moveSpeed;

		SetAnimationState ();
	}

	void FixedUpdate(){
		if (isMovementEnabled)
			rb.velocity = new Vector2 (dirX, rb.velocity.y);
	}

	void LateUpdate(){
		CheckWhereToFace();
	}

	void SetAnimationState(){
		float absX = Mathf.Abs(dirX);
		anim.SetFloat("Speed", absX);

		if (Mathf.Abs(rb.velocity.y) < tolerance){
			anim.SetBool("isJumping", false);
			anim.SetBool("isFalling", false);
		}

		if (Input.GetKey(KeyCode.S) && absX > tolerance){
			anim.SetBool("isSliding", true);
			Debug.Log("SLIDE WEEE");
		}
		else
			anim.SetBool("isSliding", false);

		if (Input.GetButtonDown("Jump") && isMovementEnabled)
			anim.SetBool("isJumping", true);
		
		if (rb.velocity.y < fallTolerance){
			anim.SetBool("isJumping", false);
			anim.SetBool("isFalling", true);
		}
	}

	void CheckWhereToFace()
	{
		if (dirX > 0)
			facingRight = true;
		else if (dirX < 0)
			facingRight = false;

		if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
			localScale.x *= -1;

		transform.localScale = localScale;
	}

	void OnTriggerEnter2D (Collider2D col){
        Debug.Log("HIT");
		if (LayerMask.LayerToName(col.gameObject.layer) == "Damage" && isDamageEnabled){
            isDamageEnabled = false;
			disableMovement();

            Debug.Log("DAMAGED");
            healthPoints--;

            if(healthPoints > 0){
                if(col.gameObject.transform.position.x < this.gameObject.transform.position.x) 
                    rb.AddForce(new Vector2(300f, 100f));
                else 
                    rb.AddForce(new Vector2(-300f, 100f));

                StartCoroutine ("BecomeInvulnerable");
                StartCoroutine ("EnableMovementDelayed");
            }
            else{
				SetAllCollidersAndRbStatus(false);
                anim.SetBool("isDead", true);
            }
        }
    }

	void disableMovement(){
			isMovementEnabled = false;
			dirX = 0;
			rb.velocity = Vector2.zero;
	}

	void enableMovement(){
			isMovementEnabled = true;
	}

	void SetAllCollidersAndRbStatus(bool active){
		GetComponent<Rigidbody2D>().isKinematic = !active;

		foreach(Collider2D c in GetComponents<Collider2D>())
			c.enabled = active;
 	}

	IEnumerator BecomeInvulnerable(){
		anim.SetTrigger("isHurt"); //
		Coroutine flash = StartCoroutine ("Flash");

		yield return new WaitForSeconds (invulnerabilityTime);

        StopCoroutine(flash);
		startColor.a = 1f;
		renderer.material.color = startColor;
        isDamageEnabled = true;
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

	IEnumerator EnableMovementDelayed(){
        yield return new WaitForSeconds(staggerTime);

		enableMovement();
	}
}
