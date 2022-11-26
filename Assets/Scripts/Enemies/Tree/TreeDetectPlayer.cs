
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDetectPlayer : MonoBehaviour
{
    private Animator animator;
    public GameObject tree;

    private bool isTreeIdle;

    void Awake()
    {
        animator = tree.GetComponent<Animator>();
    }

    void Update() {
        isTreeIdle = tree.GetComponent<Tree>().isIdle;
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
            tree.GetComponent<Tree>().isIdle = true;
        }
    }
    
}

