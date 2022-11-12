using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Slide : MonoBehaviour
{
    private Guardian guardian;
    private Image slideImg;
    private Image slideHolder;
    private TMP_Text slideTimer;
    private float cooldownTime;
    private float timeLeft;

    void Awake()
    {
        guardian = GameObject.Find("Guardian").GetComponent<Guardian>();

        slideImg = GameObject.Find("SlideImg").GetComponent<Image>();
        slideHolder = GameObject.Find("SlideHolder").GetComponent<Image>();
        slideTimer = GameObject.Find("SlideTimer").GetComponent<TMP_Text>();

        slideTimer.text = "";
        timeLeft = guardian.slideCooldownCounter;

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

    void UpdateTeleportImg()
    {
        if (guardian.slideCooldownCounter > 0f)
        {
            ChangeTextOpacity(slideTimer, 1f);
            slideTimer.text = guardian.slideCooldownCounter.ToString("F0");
            ChangeImgOpacity(slideImg, 0.2f);
            ChangeImgOpacity(slideHolder, 0.2f);
        }
        else
        {
            ChangeImgOpacity(slideImg, 1f);
            ChangeImgOpacity(slideHolder, 1f);
            ChangeTextOpacity(slideTimer, 0f);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        UpdateTeleportImg();
    }
}
