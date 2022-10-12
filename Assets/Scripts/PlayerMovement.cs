using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;

	public float runSpeed = 40f;

	private Animator animator;
	private Rigidbody2D rb;
	private AnimatorClipInfo[] animCurrentClipInfo;
	private float horizontalMove = 0f;
	private float prevHorizontalMove = 0f;
	private bool jump = false;
	private bool crouch = false;
	private bool movementEnabled = true;
	
	void Awake(){
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update(){
		ControlSpeed();
		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump") && !animator.GetBool("Crouching")){
			jump = true;
			animator.SetBool("Jumping", true);
		}

		if (Input.GetButton("Crouch") && !animator.GetBool("Jumping")){
			crouch = true;
		} 
		else if (Input.GetButtonUp("Crouch")){
			crouch = false;
		}
		
		if (Input.GetButtonDown("Attack") && !animator.GetBool("Crouching")){
			animator.SetTrigger("Attack");
		}

		// if (Input.GetButtonDown("Dash")){
		// 	animator.SetTrigger("Dash");
		// 	controller.Dash(Input.GetAxisRaw("Horizontal"));
		// }
	}

	void FixedUpdate(){
		// Move our character
		if(movementEnabled) controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
		jump = false;
	}

	void ControlSpeed(){
		animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
		string animationName = animCurrentClipInfo[0].clip.name;
		if(animationName == "FemWarrior_attack"){
			horizontalMove = (animator.GetBool("Jumping")) ? prevHorizontalMove : 0f;
			// horizontalMove = 0f;
		}
		// else if(animationName == "FemWarrior_dash"){
		// 	horizontalMove = 0f;
		// }
		else{
			prevHorizontalMove = horizontalMove;
			horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		}
	}

	public void OnLanding(){
		animator.SetBool("Jumping", false);
	}

	public void OnCrouching(bool isCrouching){
		animator.SetBool("Crouching", isCrouching);
	}
	public void OnChangeMovement(bool enableMovement){
		movementEnabled = enableMovement;

		if(!movementEnabled) rb.velocity = new Vector2 (0f, rb.velocity.y);
		Debug.Log(movementEnabled);
	}
}
