using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Shield : MonoBehaviour
{
    private GameObject shield;
    private float timeLeft;
    private float cooldownTime;
    private bool isShieldActive;
    private bool isCoolingDown;
    private Animator animator;

    [SerializeField] float shieldHealth;
    private TMP_Text shieldTimer;
    private Image shieldHolder;
    private Image shieldImg;


    void Awake()
    {
        shield = GameObject.Find("Shield");
        shield.SetActive(false);
        isShieldActive = false;
        timeLeft = 3f;
        cooldownTime = 12f;
        isCoolingDown = false;
        animator = shield.GetComponent<Animator>();
        shieldHealth = 3f;
        shieldTimer = GameObject.Find("ShieldTimer").GetComponent<TMP_Text>();
        shieldTimer.text = "";
        shieldHolder = GameObject.Find("ShieldHolder").GetComponent<Image>();
        shieldImg = GameObject.Find("ShieldImg").GetComponent<Image>();
    }

    void Start()
    {
    }

    void Update()
    {
        ManageShield();
        UpdateShieldImg();
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
            timeLeft = 3f;
            isCoolingDown = true;
        }
    }

    void CooldownCountdown()
    {
        shieldTimer.text = cooldownTime.ToString("F0");
        Debug.Log("COOL: " + cooldownTime);
        if (cooldownTime > 0f)
        {
            cooldownTime -= Time.deltaTime;
        } else {
            isCoolingDown = false;
        }
    }

    void UpdateShieldImg()
    {
        if (isShieldActive == false && isCoolingDown)
        {
            ChangeTextOpacity(shieldTimer, 1f);
            CooldownCountdown();
            ChangeImgOpacity(shieldImg, 0.2f);
            ChangeImgOpacity(shieldHolder, 0.2f);
        }
        else
        {
            ChangeImgOpacity(shieldImg, 1f);
            ChangeImgOpacity(shieldHolder, 1f);
            if (cooldownTime < 0) { cooldownTime = 12f; }
            ChangeTextOpacity(shieldTimer, 0f);
        }
    }

    void ManageShield()
    {
        if (Input.GetButtonDown("Skill_Shield") && !isCoolingDown)
        {
            shield.SetActive(true);
            isShieldActive = true;
        }
        if (isShieldActive)
        {
            ShieldCountdown();
        }
    }


    void ChangeImgOpacity(Image img, float opacity)
    {
        Color tmp = img.GetComponent<Image>().color;
        tmp.a = opacity;
        img.GetComponent<Image>().color = tmp;
    }

    void ChangeTextOpacity(TMP_Text tmpText, float opacity)
    {
        Color tmp = tmpText.GetComponent<TMP_Text>().color;
        tmp.a = opacity;
        tmpText.GetComponent<TMP_Text>().color = tmp;
    }


}