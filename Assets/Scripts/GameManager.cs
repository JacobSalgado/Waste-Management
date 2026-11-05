using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set; }

    [Header("Landfill")]
    [SerializeField] private int currentLandfillAmount;
    [SerializeField] private int maxLandfillAmount = 50;


    [Header("Economy")]
    [SerializeField] private int playerMoney = 500000;
    public int currentShift = 1;

    private bool isGameOver = false;


    public int PlayerMoney => playerMoney;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void AdvanceShift()
    {
        currentShift++;
    }

    public bool TrySpendMoney(int amount)
    {
        if(playerMoney >= amount)
        {
            playerMoney -= amount;
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
