using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    AnimatorClipInfo[] animCurrentClipInfo;

    void Start()
    {
        player = GameObject.Find("Guardian");
        animator = player.GetComponent<Animator>();
    }

    void Update()
    {
        animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string animationName = animCurrentClipInfo[0].clip.name;
        // if (animationName == "Guardian_melee1") {
        //     this.gameObject.GetComponent<Animator>().SetTrigger("shakeCamera");
        // }
    }
}
