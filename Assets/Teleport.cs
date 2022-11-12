using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{

    private Guardian guardian;
    private Image teleportImg;
    private Image teleportHolder;
    private TMP_Text teleportTimer;
    private float cooldownTime;
    private float timeLeft;

    void Awake()
    {
        guardian = GameObject.Find("Guardian").GetComponent<Guardian>();

        teleportImg = GameObject.Find("TeleportImg").GetComponent<Image>();
        teleportHolder = GameObject.Find("TeleportHolder").GetComponent<Image>();
        teleportTimer = GameObject.Find("TeleportTimer").GetComponent<TMP_Text>();

        teleportTimer.text = "";
        timeLeft = guardian.teleportCooldownTime;

    }

    void Start()
    {

    }

    void Update()
    {
        UpdateTeleportImg();
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

    void TeleportCountdown()
    {
        teleportTimer.text = timeLeft.ToString("F0");
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            Debug.Log("TIMELEFT:" + timeLeft);

        }
    }


    void UpdateTeleportImg()
    {
        if (guardian.isTeleportEnabled == false)
        {
            ChangeTextOpacity(teleportTimer, 1f);
            TeleportCountdown();
            ChangeImgOpacity(teleportImg, 0.2f);
            ChangeImgOpacity(teleportHolder, 0.2f);
        }
        else
        {
            ChangeImgOpacity(teleportImg, 1f);
            ChangeImgOpacity(teleportHolder, 1f);
            if (timeLeft < 0) { timeLeft = guardian.teleportCooldownTime; }
            ChangeTextOpacity(teleportTimer, 0f);
        }
    }



}
