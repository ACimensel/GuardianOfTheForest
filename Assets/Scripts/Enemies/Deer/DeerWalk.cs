using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerWalk : StateMachineBehaviour
{
    public float attackRange = 2f;
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
        // chase player if not close enough to swipe
        if (Vector2.Distance(player.position, rb.position) > attackRange)
        {
            deer.LookAtPlayer();
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, deer.walkSpeed * Time.fixedDeltaTime);

            rb.MovePosition(newPos);
        }

        else
        {
            animator.SetTrigger("swipe");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("swipe");
    }

}
