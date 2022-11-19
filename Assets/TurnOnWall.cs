using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnWall : MonoBehaviour
{
    [SerializeField] GameObject wall;

    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Player"){
            wall.SetActive(true);
        }
    }
}
