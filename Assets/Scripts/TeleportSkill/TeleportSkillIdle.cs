using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSkillIdle : StateMachineBehaviour
{
    [SerializeField] float stayAliveTime = 10f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.GetComponent<TriggerDestroy>().DestroyAfterXSeconds(stayAliveTime);
    }
}
