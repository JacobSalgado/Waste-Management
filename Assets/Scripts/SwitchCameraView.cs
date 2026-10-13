using UnityEngine;
using UnityEngine.UI;

public class SwitchCameraView : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera officeCamera;
    [SerializeField] private Camera terrainCamera;

    [Header("UI Button")]
    [SerializeField] private Button switchCamButton; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        officeCamera.enabled = true;
        terrainCamera.enabled = false;

        switchCamButton.onClick.AddListener(switchCam);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void switchCam()
    {
        officeCamera.enabled = !officeCamera.enabled;
        terrainCamera.enabled = !terrainCamera.enabled;
    }
}
