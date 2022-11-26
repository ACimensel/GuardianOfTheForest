using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonDrones : StateMachineBehaviour
{
    [SerializeField] GameObject dronePrefab;
    Transform player;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject drone = Instantiate(dronePrefab, new Vector3(player.transform.position.x + 0.2f, player.transform.position.y + 9f, 0f), Quaternion.identity);
        GameObject drone2 = Instantiate(dronePrefab, new Vector3(player.transform.position.x + 0.4f, player.transform.position.y + 9f, 0f), Quaternion.identity);
        GameObject drone3 = Instantiate(dronePrefab, new Vector3(player.transform.position.x + 0.6f, player.transform.position.y + 9f, 0f), Quaternion.identity);
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
