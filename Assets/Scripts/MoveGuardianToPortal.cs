using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGuardianToPortal : MonoBehaviour
{
    [SerializeField] GameObject guardian;
    [SerializeField] GameObject guardianWake;
    [SerializeField] Transform portalToArea1;
    [SerializeField] Transform portalToArea2;
    private PersistantData PD;
    private bool isPlayingOpening = false;
    private Animator anim;

    private void Awake() {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        anim = guardianWake.GetComponent<Animator>();
    }

    void Start()
    {
        if(PD.lastAreaFromHub == 0){
            guardian.GetComponent<Guardian>().Freeze();
            guardian.GetComponent<SpriteRenderer>().enabled = false;
            guardianWake.SetActive(true);
            isPlayingOpening = true;
        }
        else if(PD.lastAreaFromHub == 1){
            Transform trs = guardian.transform;
            trs.position = new Vector3(portalToArea1.position.x - 6f, trs.position.y, trs.position.z);
        }
        else if(PD.lastAreaFromHub == 2){
            Transform trs = guardian.transform;
            trs.position = new Vector3(portalToArea2.position.x + 2f, trs.position.y, trs.position.z);
        }
    }

    private void Update() {
        if (isPlayingOpening && anim.GetCurrentAnimatorStateInfo(0).IsName("wakeUp") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f){
            isPlayingOpening = false;
            guardianWake.SetActive(false);

            guardian.GetComponent<SpriteRenderer>().enabled = true;
            guardian.GetComponent<Guardian>().Unfreeze();
        }
    }
}
