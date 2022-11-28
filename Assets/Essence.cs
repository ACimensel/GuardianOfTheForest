using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    [SerializeField] int areaNumber = 0;
    private PersistantData PD;

    void Awake()
    {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
    }

    void Start()
    {
        if(PD && !PD.area1BossKilled && areaNumber == 1)
            gameObject.SetActive(false);
        else if(PD && !PD.area2BossKilled && areaNumber == 2)
            gameObject.SetActive(false);
    }
}
