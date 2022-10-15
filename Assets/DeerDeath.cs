using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerDeath : StateMachineBehaviour
{
    Deer deer;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
               deer = animator.GetComponent<Deer>();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            //    Destroy(animator.gameObject);
    }


}
