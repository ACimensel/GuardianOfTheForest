using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossChargeAttack : StateMachineBehaviour
{
    FinalBoss finalBoss;
    Guardian guardian;
    [SerializeField] GameObject chargePrefab;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finalBoss = animator.GetComponent<FinalBoss>();
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
        float dist = Vector3.Distance(guardian.transform.position, finalBoss.transform.position);
        dist = dist - 0.5f;

        if (finalBoss.facingRight == false)
        {
            dist = -dist;
        }

        //finalBoss.GetComponent<Rigidbody2D>().velocity = new Vector2(dist, 40f); // jump;

        GameObject charge = Instantiate(chargePrefab, new Vector3(guardian.transform.position.x, guardian.transform.position.y + 9f, 0f), Quaternion.identity);
        GameObject charge2 = Instantiate(chargePrefab, new Vector3(guardian.transform.position.x - 1f, guardian.transform.position.y + 12f, 0f), Quaternion.identity);
        GameObject charge3 = Instantiate(chargePrefab, new Vector3(guardian.transform.position.x + 1f, guardian.transform.position.y + 10f, 0f), Quaternion.identity);

        animator.SetBool("idle", false);
        animator.SetBool("walk", true);
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
