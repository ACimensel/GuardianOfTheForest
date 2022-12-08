using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossCharge : MonoBehaviour
{
    [SerializeField] float destroyTime = 5f;

    void Start()
    {
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


}
