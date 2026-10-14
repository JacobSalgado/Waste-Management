using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Welcome Screen")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI projectTitle;

    [Header("Office Gameplay")]
    [SerializeField] private Button purchaseFacility;
    [SerializeField] private Button purchaseTruck;

    [Header("Screenspace")]
    [SerializeField] private TextMeshProUGUI budgetText;
    [SerializeField] private TextMeshProUGUI dayCountText;
    [SerializeField] private TextMeshProUGUI landfillUsageText;
    [SerializeField] private TextMeshProUGUI cityDirtinessText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameplayUI(false);
        startButton.onClick.AddListener(StartGame);

        GameManager.Instance.OnMoneyChanged += UpdateBudgetText;
        GameManager.Instance.OnDayChanged += UpdateDayText;

        UpdateBudgetText(GameManager.Instance.PlayerMoney);
        UpdateDayText(GameManager.Instance.CurrentDay);
    }


    void StartGame()
    {
        startButton.gameObject.SetActive(false);
        projectTitle.gameObject.SetActive(false);
        SetGameplayUI(true);
    }

    void SetGameplayUI(bool visible)
    {
        purchaseFacility.gameObject.SetActive(visible);
        purchaseTruck.gameObject.SetActive(visible);
        budgetText.gameObject.SetActive(visible);
        dayCountText.gameObject.SetActive(visible);
        landfillUsageText.gameObject.SetActive(visible);
        cityDirtinessText.gameObject.SetActive(visible);
    }

    void UpdateBudgetText(int money)
    {
        budgetText.text = $"Budget: ${money:N0}"; // :NO formats the number with commas. So 500000 will appear as 500,000"
    }

    void UpdateDayText(int day)
    {
        dayCountText.text = $"Day {day}";
    }
}
