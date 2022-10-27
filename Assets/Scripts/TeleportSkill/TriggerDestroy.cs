using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy : MonoBehaviour
{
    public void DestroyAfterXSeconds(float waitTime){
        StartCoroutine(DelayedDestroy(waitTime));
    }

    IEnumerator DelayedDestroy(float waitTime){
        yield return new WaitForSeconds(waitTime);

        GetComponent<Animator>().SetBool("Destroy", true);
    }
}
