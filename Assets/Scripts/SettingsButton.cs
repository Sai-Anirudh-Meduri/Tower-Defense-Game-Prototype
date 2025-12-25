using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsButton : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject settingsButton;    // The "Settings" button
    public GameObject sensSlider;        // Slider object
    public GameObject sensText;          // "Mouse Sensitivity" text

    [Header("Player Reference")]
    public PlayerMovement playerMovement;

    private Slider slider;
    private float originalSensitivity;

    void Start()
    {
        slider = sensSlider.GetComponent<Slider>();

        // Save original sensitivity
        originalSensitivity = playerMovement.mouseSensitivity;

        // Initialize slider value
        slider.minValue = 1f;
        slider.maxValue = 200f;
        slider.value = originalSensitivity;

        // Hide slider + text at start
        sensSlider.SetActive(false);
        sensText.SetActive(false);

        // Add event listener
        slider.onValueChanged.AddListener(ChangeSensitivity);
    }

    // Called when pressing Settings button
    public void OpenSettings()
    {
        settingsButton.SetActive(false);
        sensSlider.SetActive(true);
        sensText.SetActive(true);
    }

    // Slider updates sensitivity in real time
    public void ChangeSensitivity(float value)
    {
        playerMovement.mouseSensitivity = value;
    }

    // Called by PauseManager when leaving pause menu
    public void ResetToDefault()
    {
        settingsButton.SetActive(true);
        sensSlider.SetActive(false);
        sensText.SetActive(false);

        slider.value = playerMovement.mouseSensitivity;
    }
}
