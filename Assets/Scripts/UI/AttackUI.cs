using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackUI : MonoBehaviour
{

    [SerializeField] Sprite meleeImg0;
    [SerializeField] Sprite meleeImg1;
    [SerializeField] Sprite meleeImg2;
    [SerializeField] Sprite meleeImg3;
    [SerializeField] Sprite rangedImg0;

    private Image rangedIcon;
    private Image meleePlaceholder;
    private Image rangedPlaceholder;
    private AnimatorClipInfo[] guardianAnimator;

    private string currentAnimation;


    void Awake()
    {
        meleePlaceholder = GameObject.Find("MeleeImg").GetComponent<Image>();   
        rangedPlaceholder = GameObject.Find("RangedImg").GetComponent<Image>();        
     
    }

    void Start()
    {
        meleePlaceholder.sprite = meleeImg1;
        rangedPlaceholder.sprite = rangedImg0; 
    }

    void Update()
    {
        guardianAnimator = GameObject.Find("Guardian").GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        currentAnimation = guardianAnimator[0].clip.name;
        UpdateMeleeImg();
    }




    void ChangeImg(Image placeholder, Sprite img)
    {
        placeholder.sprite = img;
    }

    void UpdateMeleeImg()
    {
        switch (currentAnimation)
        {
            case "Guardian_melee1":
                ChangeImg(meleePlaceholder, meleeImg1);
                break;
            case "Guardian_melee2":
                ChangeImg(meleePlaceholder, meleeImg2);
                break;
            case "Guardian_melee3":
                ChangeImg(meleePlaceholder, meleeImg3);
                break;
            default:
                ChangeImg(meleePlaceholder, meleeImg0);
                break;
        }
    }

    void UpdateRangedImg() {

    }


}
