using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : MonoBehaviour {
	[SerializeField] float moveSpeed = 6f;

	Rigidbody2D rb;
	Animator anim;
	float dirX = 0f;
	int  healthPoints = 3;
	bool isHurting, isDead;
	bool facingRight = true;
	Vector3 localScale;

	private float tolerance = 0.01f;

	// Use this for initialization
	void Awake(){
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		localScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update(){		
		if (Input.GetButtonDown ("Jump") && !isDead && rb.velocity.y == 0)
			rb.AddForce (Vector2.up * 600f);

		if (!isDead)
			dirX = Input.GetAxisRaw ("Horizontal") * moveSpeed;

		SetAnimationState ();
	}

	void FixedUpdate(){
		if (!isHurting)
			rb.velocity = new Vector2 (dirX, rb.velocity.y);
	}

	void LateUpdate(){
		CheckWhereToFace();
	}

	void SetAnimationState(){
		float absX = Mathf.Abs(dirX);
		anim.SetFloat("Speed", absX);

		if (absX < tolerance){
			anim.SetBool("isWalking", false);
			anim.SetBool("isRunning", false);
		}

		if (Mathf.Abs(rb.velocity.y) < tolerance){
			anim.SetBool("isJumping", false);
			anim.SetBool("isFalling", false);
		}

		if (Input.GetKey(KeyCode.DownArrow) && absX > tolerance)
			anim.SetBool("isSliding", true);
		else
			anim.SetBool("isSliding", false);

		if (Input.GetButtonDown("Jump"))
			anim.SetBool("isJumping", true);
		
		if (rb.velocity.y < 0){
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

	// void OnTriggerEnter2D (Collider2D col)
	// {
	// 	if (col.gameObject.name.Equals ("Fire")) {
	// 		healthPoints -= 1;
	// 	}

	// 	if (col.gameObject.name.Equals ("Fire") && healthPoints > 0) {
	// 		anim.SetTrigger ("isHurting");
	// 		StartCoroutine ("Hurt");
	// 	} else {
	// 		dirX = 0;
	// 		isDead = true;
	// 		anim.SetTrigger ("isDead");
	// 	}
	// }

	IEnumerator Hurt()
	{
		isHurting = true;
		rb.velocity = Vector2.zero;

		if (facingRight)
			rb.AddForce (new Vector2(-200f, 200f));
		else
			rb.AddForce (new Vector2(200f, 200f));
		
		yield return new WaitForSeconds (0.5f);

		isHurting = false;
	}

}
