using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSkillIdle : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.GetComponent<TeleportSkill>().Destroy();
    }
}
