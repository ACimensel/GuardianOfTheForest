using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GameOver() {
        gameOverUI.SetActive(true);
    }

    public void Revive() {
        gameOverUI.SetActive(false);
    }

}
