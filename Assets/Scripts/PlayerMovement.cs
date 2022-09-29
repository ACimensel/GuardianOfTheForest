using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	private AnimatorClipInfo[] animCurrentClipInfo;
	private float horizontalMove = 0f;
	private float prevHorizontalMove = 0f;
	private bool jump = false;
	private bool crouch = false;
	
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
	}

	void FixedUpdate(){
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
		jump = false;
	}

	void ControlSpeed(){
		animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
		if(animCurrentClipInfo[0].clip.name == "FemWarrior_attack"){
			horizontalMove = (animator.GetBool("Jumping")) ? prevHorizontalMove : 0f;
			// horizontalMove = 0f;
		}
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
}
