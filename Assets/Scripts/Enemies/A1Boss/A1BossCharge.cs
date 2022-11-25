using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A1BossCharge : MonoBehaviour
{
    [SerializeField] float destroyTime = 5f;
    [SerializeField] float moveSpeed = 7f;
    private Transform player;
    private Vector2 target;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector2(player.position.x, 1.1f);     // 1 to lock y movement
        StartCoroutine("DestroyAfterTime");
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        string layerName = LayerMask.LayerToName(col.gameObject.layer);
        if (layerName == "Player")
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);

        Destroy(this.gameObject);
    }


    void SetVelocity(string direction)
    {
        if (direction == "right")
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0f);
        else if (direction == "left")
            GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, 0f);
    }


}
