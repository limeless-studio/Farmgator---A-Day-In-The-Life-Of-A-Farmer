using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartScene : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void RestartGame()
    {
        gameManager.RestartGame();
    }
}
