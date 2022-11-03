using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowDetectPlayer : MonoBehaviour
{

    private Animator animator;
    private GameObject crow;

    void Start()
    {

    }

    void Awake()
    {
        crow = GameObject.Find("Crow (1)");
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
