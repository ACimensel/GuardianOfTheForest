using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDetectPlayer : MonoBehaviour
{

    private Animator animator;
    public Drone drone;

    void Start()
    {

    }

    void Awake()
    {
        animator = drone.GetComponent<Animator>();
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
