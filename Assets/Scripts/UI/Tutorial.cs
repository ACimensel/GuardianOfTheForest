using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private GameObject meleeText;
    private GameObject moveText;
    private GameObject rangedText;
    private GameObject slideText;
    private GameObject jumpText;


    void Awake()
    {
        meleeText = GameObject.Find("MeleeHint");
        moveText = GameObject.Find("MoveHint");
        rangedText = GameObject.Find("RangedHint");
        slideText = GameObject.Find("SlideHint");
        jumpText = GameObject.Find("JumpHint");
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Melee Attack"))
        {
            meleeText.SetActive(false);
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            moveText.SetActive(false);
        }

        if (Input.GetButtonDown("Ranged Attack")) {
            rangedText.SetActive(false);
        }

        if (Input.GetButtonDown("Slide")) {
            slideText.SetActive(false);
        }

        if (Input.GetButtonDown("Jump")) {
            jumpText.SetActive(false);
        }

    }
}
