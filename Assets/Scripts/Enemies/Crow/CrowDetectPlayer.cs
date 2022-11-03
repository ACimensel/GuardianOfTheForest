using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowDetectPlayer : MonoBehaviour
{

    private Animator animator;
    public Crow crow;

    void Start()
    {

    }

    void Awake()
    {
        animator = crow.GetComponent<Animator>();
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
