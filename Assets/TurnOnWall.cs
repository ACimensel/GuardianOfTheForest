using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnWall : MonoBehaviour
{
    [SerializeField] GameObject wall;

    private PersistantData PD;

    void Awake(){
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
    }

    void Start(){
        if(PD && PD.area1BossKilled == true)
            Destroy(gameObject);
    }
        
    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Player"){
            wall.SetActive(true);
        }
    }
}
