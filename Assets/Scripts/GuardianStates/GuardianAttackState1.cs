using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAttackState1 : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.GetComponent<Guardian>().PlayMeleeAttackSounds();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
    }
}
