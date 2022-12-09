using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossCinematicHandler : MonoBehaviour
{
    [SerializeField] GameObject guardian;
    [SerializeField] GameObject guardianCamera;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject bossIdle;
    [SerializeField] GameObject cinematic;
    [SerializeField] float cinematicWaitTime = 2f;
    private PersistantData PD;
    private Animator bossAnim;

    private void Awake() {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        bossAnim = cinematic.GetComponent<Animator>();
    }

    void Start()
    {
        if(!PD.finalBossCinematicPlayed){
            boss.SetActive(false);
            guardian.GetComponent<Guardian>().Freeze();
            StartCoroutine("WaitForCinematic");
        }
    }

    void Update()
    {
        if (cinematic && bossAnim.GetCurrentAnimatorStateInfo(0).IsName("bosscinem") && bossAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f){
            Destroy(cinematic); 

            PD.finalBossCinematicPlayed = true;
            boss.SetActive(true);

            guardian.GetComponent<SpriteRenderer>().enabled = true;
            Guardian gScr = guardian.GetComponent<Guardian>();
            gScr.isFrozen = false;
            gScr.isMovementEnabled = true;
        }
    }

    IEnumerator WaitForCinematic()
    {
        yield return new WaitForSeconds(cinematicWaitTime);

        guardian.GetComponent<SpriteRenderer>().enabled = false;
        bossIdle.SetActive(false);
        cinematic.SetActive(true);
    }
}
