using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantData : MonoBehaviour
{
    public static PersistantData PD;

    // Guardian Data
    public int guardianMaxHealth = 10;
    public int guardianCurrentHealth = 10;
    public int orbCount = 0;

    // Boss Data
    public bool area1BossKilled = true;     
    public bool area2BossKilled = true;     
     
    void Awake()
        {
            if(PD != null){
                Destroy(this.gameObject);
            }
            else
                PD = this;
            
            DontDestroyOnLoad(this);
        }
}
