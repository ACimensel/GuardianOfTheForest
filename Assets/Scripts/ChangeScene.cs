using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private int loadLevel;
    private PersistantData PD;

    private void Awake() {
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.CompareTag("Player")) {
            if(loadLevel == 1)
                PD.lastAreaFromHub = 1;
            else if(loadLevel == 2)
                PD.lastAreaFromHub = 2;

            SceneManager.LoadScene(loadLevel);
        }
    }
}
