using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGuardianToPortal : MonoBehaviour
{
    [SerializeField] Transform guardian;
    [SerializeField] Transform portalToArea1;
    [SerializeField] Transform portalToArea2;
    private PersistantData PD;

    private void Awake() {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
    }

    void Start()
    {
        Vector3 pos = guardian.position;
        if(PD.lastAreaFromHub == 1){
            guardian.position = new Vector3(portalToArea1.position.x - 2f, pos.y, pos.z);
        }
        else if(PD.lastAreaFromHub == 2){
            guardian.position = new Vector3(portalToArea2.position.x + 2f, pos.y, pos.z);
        }
    }
}
