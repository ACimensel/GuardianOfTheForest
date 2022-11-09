using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    [SerializeField] float destroyTime = 5f;
    [SerializeField] float moveSpeed = 10f;
    public const int boltDamage = 10;

    private bool enteredTeleport = false;

    void Start(){
        StartCoroutine("DestroyAfterTime");
    }

    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Enemy" || layerName == "Ground"){
            Destroy(this.gameObject);
        }
        else if (layerName == "Teleport"){
            EnterTeleport();
        }
    }

    IEnumerator DestroyAfterTime(){
        yield return new WaitForSeconds(destroyTime);

        Destroy(this.gameObject);
    }

    void SetVelocity(string direction){
        if (direction == "right")
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0f);
        else if (direction == "left"){
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0f);
        }
    }

    public void EnterTeleport(){
        if(!enteredTeleport){
            enteredTeleport = true;
            Vector3 scale = transform.localScale;

            if (GetComponent<Rigidbody2D>().velocity.x > 0f){
                // FLIP LEFT
                transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0f);
            }
            else{
                // FLIP RIGHT
                transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0f);
            }

        }
    }

    void GoRight(){
        GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0f);
    }

    void GoLeft(){
        GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0f);
    }
}
