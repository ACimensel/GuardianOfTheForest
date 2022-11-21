using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantData : MonoBehaviour
{
    public static PersistantData PD;
     
    public GameObject soundManager;
    public int score = 0;
     
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
