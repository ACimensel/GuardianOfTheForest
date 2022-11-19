using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPhysics : MonoBehaviour
{
    [SerializeField] float minUpForce = 3.5f;
    [SerializeField] float maxUpForce = 2f;
    [SerializeField] float minSideForce = -0.4f;
    [SerializeField] float maxSideForce = 0.4f;
    [SerializeField] float pickUpRadius = 4f;

    private Rigidbody2D rb;
    private bool trackPlayer = false;
    private GameObject player;
    private float step = 0.01f; 

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Guardian");
        
        float xF = Random.Range(minSideForce, maxSideForce);
        float yF = Random.Range(minUpForce, maxUpForce);
        rb.AddForce(new Vector3(xF, yF, 0f));
    }


    void FixedUpdate(){
        if(!trackPlayer && Vector3.Distance(player.transform.position, transform.position) < pickUpRadius){
            trackPlayer = true;
        }

        if(trackPlayer){
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
            step += 0.002f;
        }        
    }

    void OnCollisionEnter2D(Collision2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Player"){
            Destroy(gameObject);
        }
    }
}
