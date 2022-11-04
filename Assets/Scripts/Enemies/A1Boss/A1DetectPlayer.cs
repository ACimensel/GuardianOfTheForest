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
