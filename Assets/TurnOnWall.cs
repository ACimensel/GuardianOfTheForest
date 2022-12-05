using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnWall : MonoBehaviour
{
    [SerializeField] GameObject fightBoss;
    [SerializeField] GameObject cinematicBoss;
    [SerializeField] Guardian guardianScr;
    [SerializeField] GameObject wall;
    [SerializeField] int areaNumber = 0;

    private PersistantData PD;
    private Animator bossAnim;
    private bool isCinematicPlaying = false;

    void Awake(){
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        bossAnim = cinematicBoss.GetComponent<Animator>();
    }

    void Start(){
        if(PD && PD.area1BossKilled && areaNumber == 1){
            Destroy(cinematicBoss);
            Destroy(gameObject);
        }
        else if(PD && PD.area2BossKilled && areaNumber == 2){
            Destroy(gameObject);
        }
        else if(PD.area1BossCinematicPlayed && areaNumber == 1){
            Destroy(cinematicBoss);
        }
        else if(PD.area2BossCinematicPlayed && areaNumber == 2){
            Destroy(cinematicBoss);
        }
    }

    void Update(){
        if(bossAnim && bossAnim.GetCurrentAnimatorStateInfo(0).IsName("cinematic_transform"))
            Debug.Log(bossAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        if (bossAnim && bossAnim.GetCurrentAnimatorStateInfo(0).IsName("cinematic_transform") && bossAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f){
            Destroy(cinematicBoss); 
            fightBoss.SetActive(true);
            guardianScr.EnableMovement(false);

            if(areaNumber == 1)
                PD.area1BossCinematicPlayed = true;
            else if(areaNumber == 2)
                PD.area2BossCinematicPlayed = true;
        }
    }
        
    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Player" || layerName == "Sliding"){
            if((areaNumber == 1 && PD.area1BossKilled) || (areaNumber == 2 && PD.area2BossKilled))
                return;

            wall.SetActive(true);

            if(areaNumber == 1 && !isCinematicPlaying && !PD.area1BossCinematicPlayed){
                isCinematicPlaying = true;
                guardianScr.DisableMovement(false);
                bossAnim.SetTrigger("transform");
            }
            else if(areaNumber == 2 && !isCinematicPlaying && !PD.area2BossCinematicPlayed){
                isCinematicPlaying = true;
                guardianScr.DisableMovement(false);
                bossAnim.SetTrigger("transform");
            }
        }
    }
}
