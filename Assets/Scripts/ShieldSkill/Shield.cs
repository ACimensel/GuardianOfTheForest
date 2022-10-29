using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// u to enable -
// 3 hits to break 
// lasts 12s -
// 5s cooldown

public class Shield : MonoBehaviour
{
    private GameObject shield;
    private float timeLeft;
    private float cooldownTime;
    private bool isShieldActive;
    private bool isCoolingDown;
    private Animator animator;
    [SerializeField] float shieldHealth;


    void Awake()
    {
        shield = GameObject.Find("Shield");
        shield.SetActive(false);
        isShieldActive = false;
        timeLeft = 3f;
        cooldownTime = 6f;
        isCoolingDown = false;
        animator = shield.GetComponent<Animator>();
        shieldHealth = 3f;
    }

    void Start()
    {
    }

    void Update()
    {
        ManageShield();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) != "dne")
        {
            Physics2D.IgnoreCollision(col.GetComponent<Collider2D>(), shield.GetComponent<Collider2D>());
        }
    }


    void ShieldCountdown()
    {
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
        }
        else
        {
            animator.SetTrigger("shieldEnded");
            isShieldActive = false;
            //shield.SetActive(false);
            timeLeft = 3f;
        }
    }

    void Cooldown()
    {
        if (cooldownTime > 0f)
        {
            cooldownTime -= Time.deltaTime;
        }
        else
        {
        }
    }

    void ManageShield()
    {
        if (Input.GetButtonDown("Skill_Shield") && !isShieldActive)
        {
            shield.SetActive(true);
            isShieldActive = true;
        }
        if (isShieldActive)
        {
            ShieldCountdown();
        }
    }


}