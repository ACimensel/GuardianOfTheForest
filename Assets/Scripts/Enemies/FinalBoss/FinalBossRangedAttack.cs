using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossRangedAttack : StateMachineBehaviour
{
    FinalBoss finalBoss;
    Guardian guardian;
    Rigidbody2D rb;
    [SerializeField] GameObject projectilePrefab;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finalBoss = animator.GetComponent<FinalBoss>();
        guardian = GameObject.Find("Guardian").GetComponent<Guardian>();
        rb = animator.GetComponent<Rigidbody2D>();
        finalBoss.LookAtPlayer();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (finalBoss.facingRight)
        {
            GameObject bossProjectile = Instantiate(projectilePrefab, new Vector3(finalBoss.transform.position.x + 1f, finalBoss.transform.position.y + 0.2f, 0f), Quaternion.identity);
            bossProjectile.SendMessage("SetVelocity", "right");
        }
        else
        {
            GameObject bossProjectile = Instantiate(projectilePrefab, new Vector3(finalBoss.transform.position.x + -1f, finalBoss.transform.position.y + 0.2f, 0f), Quaternion.identity);
            bossProjectile.SendMessage("SetVelocity", "left");
        }
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
