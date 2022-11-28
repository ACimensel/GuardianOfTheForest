using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpUp : StateMachineBehaviour
{
    A2Boss a2boss;
    Guardian guardian;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        a2boss = animator.GetComponent<A2Boss>();
        guardian = GameObject.Find("Guardian").GetComponent<Guardian>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        a2boss.LookAtPlayer();

        float dist = Vector3.Distance(guardian.transform.position, a2boss.transform.position);
        dist = dist - 0.5f;

        if (a2boss.facingRight == false)
        {
            dist = -dist;
        }

        a2boss.GetComponent<Rigidbody2D>().velocity = new Vector2(dist, 40f); // jump;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
