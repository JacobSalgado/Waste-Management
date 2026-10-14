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
    [SerializeField] private Button activeSimButton;

    [Header("Screenspace")]
    [SerializeField] private TextMeshProUGUI budgetText;
    [SerializeField] private TextMeshProUGUI dayCountText;
    [SerializeField] private TextMeshProUGUI landfillUsageText;
    [SerializeField] private TextMeshProUGUI cityDirtinessText;

    private TextMeshProUGUI activeSimLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameplayUI(false);
        startButton.onClick.AddListener(StartGame);

        GameManager.Instance.OnMoneyChanged += UpdateBudgetText;
        GameManager.Instance.OnDayChanged += UpdateDayText;
        GameManager.Instance.OnSimStateChanged += UpdateSimButton;

        UpdateBudgetText(GameManager.Instance.PlayerMoney);
        UpdateDayText(GameManager.Instance.CurrentDay);

        activeSimButton.onClick.AddListener(() => GameManager.Instance.SetSimActive(true));
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

        activeSimButton.gameObject.SetActive(visible && !GameManager.Instance.IsSimActive);
    }

    void UpdateBudgetText(int money)
    {
        budgetText.text = $"Budget: ${money:N0}"; // :NO formats the number with commas. So 500000 will appear as 500,000"
    }

    void UpdateDayText(int day)
    {
        dayCountText.text = $"Day {day}";
    }

    void UpdateSimButton(bool active)
    {
        activeSimButton.gameObject.SetActive(!active);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged -= UpdateBudgetText;
            GameManager.Instance.OnDayChanged -= UpdateDayText;
            GameManager.Instance.OnSimStateChanged -= UpdateSimButton;
        }
    }
}
