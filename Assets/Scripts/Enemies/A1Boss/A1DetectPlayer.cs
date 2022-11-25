using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A1DetectPlayer : MonoBehaviour
{

    private Animator animator;
    public A1Boss a1boss;

    void Start()
    {

    }

    void Awake()
    {
        animator = a1boss.GetComponent<Animator>();
    }

    void Update()
    {
        if (this.GetComponent<A1Boss>().animationName == "a1_dash")
        {
            BoxCollider2D bc2d = GetComponent<BoxCollider2D>();
            bc2d.enabled = false;

            CircleCollider2D cc2d = GetComponent<CircleCollider2D>();
            cc2d.enabled = false;
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Player")
        {
            animator.SetBool("detectedPlayer", true);
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Player")
        {
            animator.SetBool("detectedPlayer", false);
        }
    }
}
