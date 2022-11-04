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
    public bool isShieldActive;
    private bool isCoolingDown;
    private Animator animator;

    [SerializeField] public float shieldHealth;
    private TMP_Text shieldTimer;
    private Image shieldHolder;
    private Image shieldImg;

    Guardian guardian;

    void Awake()
    {
        shield = GameObject.Find("Shield");
        isShieldActive = false;
        timeLeft = 6f;
        cooldownTime = 12f;
        isCoolingDown = false;
        animator = shield.GetComponent<Animator>();
        shieldHealth = 3f;
        shieldTimer = GameObject.Find("ShieldTimer").GetComponent<TMP_Text>();
        shieldTimer.text = "";
        shieldHolder = GameObject.Find("ShieldHolder").GetComponent<Image>();
        shieldImg = GameObject.Find("ShieldImg").GetComponent<Image>();
        guardian = GameObject.Find("Guardian").GetComponent<Guardian>();
    }

    void Start()
    {
                shield.SetActive(false);

    }

    void Update()
    {
        ManageShield();
        UpdateShieldImg();
    }

    void ShieldCountdown()
    {
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            if (shieldHealth <= 0)
            {
                animator.SetTrigger("shieldEnded");
                isShieldActive = false;
                guardian.isDamageEnabled = true;
                shieldHealth = 3f;

                timeLeft = 6f;
                isCoolingDown = true;
            }
        }
        else
        {
            animator.SetTrigger("shieldEnded");
            isShieldActive = false;
            guardian.isDamageEnabled = true;
            shieldHealth = 3f;

            timeLeft = 6f;
            isCoolingDown = true;
        }
    }

    void CooldownCountdown()
    {
        shieldTimer.text = cooldownTime.ToString("F0");
        if (cooldownTime > 0f)
        {
            cooldownTime -= Time.deltaTime;
        }
        else
        {
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
            guardian.isDamageEnabled = false;
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