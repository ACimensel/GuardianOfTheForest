using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOrbs : MonoBehaviour
{
    [SerializeField] GameObject orbPrefab;
    [SerializeField] int minAmountToDrop = 5;
    [SerializeField] int maxAmountToDrop = 10;


    public void Drop(){
        Vector3 pos = gameObject.transform.position;
        float numToDrop = Random.Range(minAmountToDrop, maxAmountToDrop);

        for(int i = 0; i < numToDrop; i++){
            Instantiate(orbPrefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        }

        Destroy(this);
    }
}
