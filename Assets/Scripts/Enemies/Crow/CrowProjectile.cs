using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowProjectile : MonoBehaviour
{
    [SerializeField] float destroyTime = 3f;
    [SerializeField] float moveSpeed = 10f;
    public const int damage = 1;
    private Transform player;
    private Vector2 target;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector2(player.position.x, player.position.y);
        StartCoroutine("DestroyAfterTime");
    }
    
    void Update() {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);
        if (layerName == "Player" || layerName == "Ground"){
            Destroy(this.gameObject);
        }
    }

    IEnumerator DestroyAfterTime(){
        yield return new WaitForSeconds(destroyTime);

        Destroy(this.gameObject);
    }
}
