using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    TaskManager taskManager;
    PlayerController controller;
    GameManager gameManager;
    public bool sleep;
    public bool isSleeping;
    float delay = 3f;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        taskManager = FindAnyObjectByType<TaskManager>();
        controller = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (sleep)
        {
            controller.canMove = false;
            if (!isSleeping)
            {
                taskManager.ConfirmTaskCompletion(5);
                isSleeping = true;
            }

            if (delay > 0)
            {
                delay -= Time.deltaTime;
            }
            else
            {
                gameManager.ScoreMenu();
            }
        }
    }
}
