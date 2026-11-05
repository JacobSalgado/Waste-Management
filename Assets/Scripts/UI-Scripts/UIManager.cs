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
    [SerializeField] private TextMeshProUGUI trucksText;
    [SerializeField] private TextMeshProUGUI facilitiesText;
    [SerializeField] private TextMeshProUGUI cityDirtinessText;

    [Header("End of Week")]
    [SerializeField] private TextMeshProUGUI weekCompleteText;

    private TextMeshProUGUI activeSimLabel;

    [Header("Switch Camera Reference")]
    [SerializeField] private SwitchCameraView switchCameraView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeSimLabel = activeSimButton.GetComponentInChildren<TextMeshProUGUI>();
        weekCompleteText.gameObject.SetActive(false);

        SetGameplayUI(false);
        startButton.onClick.AddListener(StartGame);
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
        trucksText.gameObject.SetActive(visible);
        landfillUsageText.gameObject.SetActive(visible);
        facilitiesText.gameObject.SetActive(visible);
        cityDirtinessText.gameObject.SetActive(visible);
    }

    void UpdateSimButton(bool active)
    {
        switchCameraView.switchCam(); // <-- this switches the camera when the user activates the sim, making it more seamlessly
    }

    void ShowWeekComplete()
    {
        activeSimButton.gameObject.SetActive(false);
        weekCompleteText.gameObject.SetActive(true);
        weekCompleteText.text = "Week Complete!";
    }

    void OnDestroy()
    {
   
    }
}
