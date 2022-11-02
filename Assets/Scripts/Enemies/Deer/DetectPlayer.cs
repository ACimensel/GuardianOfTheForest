using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private Animator animator;
    private GameObject deer;


    void Awake()
    {
        deer = GameObject.Find("Deer");
        animator = deer.GetComponent<Animator>();
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
