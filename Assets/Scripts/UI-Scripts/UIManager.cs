using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Welcome Screen")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject directionsPanel;

    [Header("")]
    [SerializeField] private Button nextButtonToDirections;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject gameUI;

    public void ContinueToDirections()
    {
        welcomePanel.SetActive(false);
        directionsPanel.SetActive(true);

    }

    public void ContinueToGame()
    {
        directionsPanel.SetActive(false);
        gameUI.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //nextButtonToDirections.onClick.AddListener(ContinueToDirections);
    }


    void StartGame()
    {
        
    }
}
