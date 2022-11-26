
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2DetectPlayer : MonoBehaviour
{
    private Animator animator;
    public GameObject A2Boss;

    private bool isTreeIdle;

    void Awake()
    {
        animator = A2Boss.GetComponent<Animator>();
    }

    void Update() {
        isTreeIdle = A2Boss.GetComponent<A2Boss>().isIdle;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // if (LayerMask.LayerToName(col.gameObject.layer) == "Player" && isTreeIdle == false)
        // {
        //     animator.SetBool("detectedPlayer", true);
        // }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Player")
        {
            animator.SetBool("detectedPlayer", false);
            A2Boss.GetComponent<A2Boss>().isIdle = true;
        }
    }
    
}

