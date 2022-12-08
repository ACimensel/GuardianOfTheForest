using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantData : MonoBehaviour
{
    public static PersistantData PD;

    // Guardian Data
    [Header("Guardian Data")] [Space]
    public int guardianMaxHealth = 10;
    public int guardianCurrentHealth = 10;
    public int orbCount = 0;

    // Boss Data
    [Header("Boss Data")] [Space]
    public bool area1BossCinematicPlayed = false;
    public bool area1BossKilled = false;
    public bool area2BossCinematicPlayed = false;
    public bool area2BossKilled = false;
    public bool finalBossCinematicPlayed = false;
    public bool finalBossKilled = false;

    // Hub Tree Data
    [Header("Hub Tree Data")] [Space]
    public bool firstTransitionPlayed = false;
    public bool secondTransitionPlayed = false;
    public bool portalOpenTransitionPlayed = false;
    public bool fullyHealedTransitionPlayed = false;
    public int lastAreaFromHub = 0;

    void Awake(){
        if(PD != null){
            Destroy(this.gameObject);
        }
        else{
            PD = this;   
            DontDestroyOnLoad(this);
        }
    }
}
