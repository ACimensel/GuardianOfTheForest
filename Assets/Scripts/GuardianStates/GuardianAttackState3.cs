using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAttackState3 : StateMachineBehaviour
{
    private Guardian guardian;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        guardian = animator.GetComponent<Guardian>();
        guardian.PlayMeleeAttackSounds();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        // if (Input.GetButtonDown("Melee Attack") && animator.GetInteger("nextAttackState") != 0){
        //     animator.SetInteger("nextAttackState", 1);
        // }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        // animator.SetTrigger("MeleeAttack1");
    }
}
