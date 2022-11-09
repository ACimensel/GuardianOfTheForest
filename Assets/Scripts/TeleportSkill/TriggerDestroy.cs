using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy : MonoBehaviour
{
    [SerializeField] float stayAliveTime = 10f;

    private GameObject guardian = null;
    private Guardian script = null;

    void Awake(){
        guardian = GameObject.Find("Guardian");
        if(guardian != null) script = guardian.GetComponent<Guardian>();
    }

    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);
        Debug.Log("Hit by layer: " + layerName);

        // if (layerName == "EnemyAttack" && isDamageEnabled){
        // }
    }

    public void Destroy(){
        StartCoroutine(DestroyAfterXSeconds(stayAliveTime));
    }

    IEnumerator DestroyAfterXSeconds(float stayAliveTime){
        yield return new WaitForSeconds(stayAliveTime);

        Destroy(gameObject.GetComponent<BoxCollider2D>());

        if(script != null){ 
            script.DequeueTeleport();
        }

        GetComponent<Animator>().SetBool("Destroy", true);
    }
}
