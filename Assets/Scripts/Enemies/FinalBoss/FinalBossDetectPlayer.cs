using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossDetectPlayer : MonoBehaviour
{
 
    private Animator animator;
    public FinalBoss finalBoss;

    void Start()
    {

    }

    void Awake()
    {
        animator = finalBoss.GetComponent<Animator>();
    }

    void Update()
    {
        // if (finalBoss.GetComponent<FinalBoss>().animationName == "a1_dash")
        // {
        //     BoxCollider2D bc2d = GetComponent<BoxCollider2D>();
        //     bc2d.enabled = false;

        //     CircleCollider2D cc2d = GetComponent<CircleCollider2D>();
        //     cc2d.enabled = false;
        // }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Player")
        {
            //animator.SetBool("detectedPlayer", true);
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Player")
        {
            //animator.SetBool("detectedPlayer", false);
        }
    }
}
