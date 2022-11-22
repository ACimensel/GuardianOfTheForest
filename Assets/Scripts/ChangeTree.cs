using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTree : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    private int bossesKilled = 0;
    private PersistantData PD;

    private void Awake() {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
    }

    void Start() {
        if(PD.area1BossKilled)
            bossesKilled++;
        if(PD.area2BossKilled)
            bossesKilled++;

        if(bossesKilled == 1)
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
        else if(bossesKilled == 2)
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
    }
}
