using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryDroneDetectPlayer : MonoBehaviour
{
    private Animator animator;
    public StationaryDrone stationaryDrone;

    void Start()
    {

    }

    void Awake()
    {
        animator = stationaryDrone.GetComponent<Animator>();
    }



    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Player" || LayerMask.LayerToName(col.gameObject.layer) == "Climbing")
        {
            animator.SetBool("detectedPlayer", true);
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Player" || LayerMask.LayerToName(col.gameObject.layer) == "Climbing")
        {
            animator.SetBool("detectedPlayer", false);
        }
    }
}
