using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerPatrol : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;
    Deer deer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        deer = animator.GetComponent<Deer>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (deer.facingRight) {
            rb.velocity = new Vector2(deer.patrolSpeed, 0f);
        } else {
            rb.velocity = new Vector2(-deer.patrolSpeed, 0f);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {

    // }

}