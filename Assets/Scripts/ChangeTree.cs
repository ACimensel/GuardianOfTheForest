using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class ChangeTree : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerDetectPoint;
    [SerializeField] private Guardian guardianScr;
    private int bossesKilled = 0;
    private PersistantData PD;
    private Animator anim;
    private float freezeTime = 4f;

    private void Awake() {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        anim = GetComponent<Animator>();
    }

    void Start() {
        if(PD.area1BossKilled)
            bossesKilled++;
        if(PD.area2BossKilled)
            bossesKilled++;
        if(PD.finalBossKilled)
            bossesKilled++;

        if(bossesKilled == 1){
            if(!PD.firstTransitionPlayed){
                anim.SetTrigger("0to2");
                PD.firstTransitionPlayed = true;
            }
            else{
                anim.SetTrigger("2idle");
            }
        }
        else if(bossesKilled == 2){
            if(!PD.secondTransitionPlayed){
                anim.SetTrigger("2to3");
                PD.secondTransitionPlayed = true;
            }
            else if(PD.portalOpenTransitionPlayed){
                anim.SetTrigger("4idle");
            }
            else{
                anim.SetTrigger("3idle");
            }
        }
        else if(bossesKilled == 3){
            if(!PD.fullyHealedTransitionPlayed){
                anim.SetTrigger("4to3"); // TODO this doesn't have an animation
                PD.fullyHealedTransitionPlayed = true;
            }
            else{
                anim.SetTrigger("3idle");
            }
        }
    }

    private void Update() {
        Vector3 diff = player.transform.position - playerDetectPoint.transform.position;
        if(bossesKilled == 2 && !PD.portalOpenTransitionPlayed && diff.magnitude < 10f){
            PD.portalOpenTransitionPlayed = true;
            guardianScr.Freeze();
            anim.SetTrigger("3to4");
            CameraShaker.Instance.ShakeOnce(4f, 4f, freezeTime, freezeTime);
            StartCoroutine("UnfreezePlayer");
        }
    }

    private void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if(PD.portalOpenTransitionPlayed && (layerName == "Player" || layerName == "Sliding")) {
            SceneManager.LoadScene("AreaX-FinalBoss");
        }
    }
    
    IEnumerator UnfreezePlayer()
    {
        yield return new WaitForSeconds(freezeTime);

        guardianScr.isFrozen = false;
        guardianScr.isMovementEnabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
