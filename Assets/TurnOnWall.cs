using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnWall : MonoBehaviour
{
    [SerializeField] GameObject wall;
    [SerializeField] int areaNumber = 0;

    private PersistantData PD;

    void Awake(){
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
    }

    void Start(){
        if(PD && PD.area1BossKilled == true && areaNumber == 1)
            Destroy(gameObject);
        else if(PD && PD.area2BossKilled == true && areaNumber == 2)
            Destroy(gameObject);
    }
        
    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Player" || layerName == "Sliding"){
            wall.SetActive(true);
        }
    }
}
