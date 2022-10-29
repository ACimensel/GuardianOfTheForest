using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackUI : MonoBehaviour
{

    [SerializeField] Sprite meleeImg0;
    [SerializeField] Sprite meleeImg1;
    [SerializeField] Sprite meleeImg2;
    [SerializeField] Sprite meleeImg3;
    [SerializeField] Sprite rangedImg0;

    private Image rangedIcon;
    private Image meleeImg;
    private Image rangedImg;
    private Image rangedHolder;
    private AnimatorClipInfo[] guardianAnimator;

    private string currentAnimation;
    private Guardian guardian;

    private float timeLeft;
    private TMP_Text rangedTimer;

    void Awake()
    {
        meleeImg = GameObject.Find("MeleeImg").GetComponent<Image>();
        rangedImg = GameObject.Find("RangedImg").GetComponent<Image>();
        rangedHolder = GameObject.Find("RangedHolder").GetComponent<Image>();
        guardian = GameObject.Find("Guardian").GetComponent<Guardian>();
        rangedTimer = GameObject.Find("RangedTimer").GetComponent<TMP_Text>();
        meleeImg.sprite = meleeImg1;
        rangedImg.sprite = rangedImg0;
        timeLeft = guardian.rangedCooldownTime;
        rangedTimer.text = "";
    }

    void Start() {}

    void Update()
    {
        guardianAnimator = GameObject.Find("Guardian").GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        currentAnimation = guardianAnimator[0].clip.name;
        UpdateMeleeImg();
        UpdateRangedImg();
    }

    void ChangeImg(Image placeholder, Sprite img)
    {
        placeholder.sprite = img;
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

    void UpdateMeleeImg()
    {
        switch (currentAnimation)
        {
            case "Guardian_melee1":
                ChangeImg(meleeImg, meleeImg1);
                break;
            case "Guardian_melee2":
                ChangeImg(meleeImg, meleeImg2);
                break;
            case "Guardian_melee3":
                ChangeImg(meleeImg, meleeImg3);
                break;
            default:
                ChangeImg(meleeImg, meleeImg0);
                break;
        }
    }

    void UpdateRangedImg()
    {
        if (guardian.isRangedEnabled == false)
        {
            ChangeTextOpacity(rangedTimer, 1f);
            RangedCountdown();
            ChangeImgOpacity(rangedImg, 0.2f);
            ChangeImgOpacity(rangedHolder, 0.2f);
        }
        else
        {
            ChangeImgOpacity(rangedImg, 1f);
            ChangeImgOpacity(rangedHolder, 1f);
            if (timeLeft < 0) { timeLeft = guardian.rangedCooldownTime; }
            ChangeTextOpacity(rangedTimer, 0f);
        }
    }

    void RangedCountdown()
    {
        rangedTimer.text = timeLeft.ToString("F0");
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
        }
    }

}
