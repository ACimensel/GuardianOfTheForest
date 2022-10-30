using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDamageManager : MonoBehaviour
{
    private Shield shield;
    private Animator animator;


    void Awake()
    {
        shield = GameObject.Find("ShieldSkill").GetComponent<Shield>();
        animator = this.GetComponent<Animator>();
    }

    void Start()
    {
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "EnemyAttack")
        {
            shield.shieldHealth--;
            animator.SetInteger("shieldHealth", (int)shield.shieldHealth);
            Debug.Log(shield.shieldHealth);

        }
    }



}
