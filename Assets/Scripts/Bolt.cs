using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    [SerializeField] float destroyTime = 5f;
    [SerializeField] float moveSpeed = 10f;
    public const int boltDamage = 10;
    public bool enteredTeleport = false;

    private TeleportSkill teleport = null;
    private int isBoltGoingRight = 0; // 0 = false, 1 = true

    void Start(){
        StartCoroutine("DestroyAfterTime");
    }

    void OnTriggerEnter2D(Collider2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Enemy" || layerName == "Ground"){
            Destroy(this.gameObject);
        }
        else if (layerName == "Teleport" && !enteredTeleport && col.gameObject.GetComponent<TeleportSkill>().hasPair){
            enteredTeleport = true;

            teleport = col.gameObject.GetComponent<TeleportSkill>();
            teleport.PlayWarpSound();
            ChangeBolt();
        }
    }

    IEnumerator DestroyAfterTime(){
        yield return new WaitForSeconds(destroyTime);

        Destroy(this.gameObject);
    }

    void SetVelocity(string direction){
        if (direction == "right"){
            isBoltGoingRight = 1;
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0f);
        }
        else if (direction == "left"){
            isBoltGoingRight = 0;
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0f);
        }
    }

    public void ChangeBolt(){
        int portalPos = teleport.PortalPosition(); // -1 = no pair, 0 = hit portal on left, 1 = hit portal on right, 2 = same spot
        
        if(isBoltGoingRight == portalPos){
            // Move Position
            transform.position = new Vector3(teleport.pairPos.x, teleport.pairPos.y, transform.position.z);
        }
        else if((isBoltGoingRight == 0 && portalPos == 1) || (isBoltGoingRight == 1 && portalPos == 0)){
            // Move Position
            transform.position = new Vector3(teleport.pairPos.x, teleport.pairPos.y, transform.position.z);

            // Flip direction and velocity
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);

            GetComponent<Rigidbody2D>().velocity = (GetComponent<Rigidbody2D>().velocity.x > 0f) ? new Vector2(-moveSpeed, 0f) : new Vector2(moveSpeed, 0f);
        }

        // Change color
        GetComponent<SpriteRenderer>().color = new Color(0/255f, 247/255f, 189/255f);
    }

    void GoRight(){
        GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0f);
    }

    void GoLeft(){
        GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0f);
    }
}
