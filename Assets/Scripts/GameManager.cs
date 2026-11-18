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

    [Header("Shift Management")]
    public float shiftDuration = 300f;
    [SerializeField] private float shiftTimeRemaining;
    public float ShiftTimeRemaining => shiftTimeRemaining;
    private bool isShiftActive = false;
    public bool IsShiftActive => isShiftActive;

    public int currentShift = 0;

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
    private void Start()
    {
        
    }

    private void Update()
    {
        if(isShiftActive)
        {
            shiftTimeRemaining -= Time.deltaTime;

            if(shiftTimeRemaining <= 0)
            {
                EndShift();
            }
        }
    }

    private void AdvanceShift()
    {
        currentShift++;
        shiftTimeRemaining = shiftDuration;
        isShiftActive = true;
        WasteManager.Instance.CalculateSpawnInterval();

        Debug.Log($"Shift {currentShift} started. Duration: {shiftDuration} seconds.");
    }

    public void StartShift()
    {
        if(isGameOver)
        {
            Debug.Log("Cannot start a new shift. The game is over.");
            return;
        }

        if(!isShiftActive)
        {
            AdvanceShift();
        }
    }

    public void EndShift()
    {
        if(isShiftActive)
        {
            isShiftActive = false;
            Debug.Log($"Shift {currentShift} ended.");
        }
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
