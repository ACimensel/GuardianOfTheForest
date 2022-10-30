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
        meleeText = GameObject.Find("Melee");
        moveText = GameObject.Find("Move");
        rangedText = GameObject.Find("Ranged");
        slideText = GameObject.Find("Slide");
        jumpText = GameObject.Find("Jump");
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
