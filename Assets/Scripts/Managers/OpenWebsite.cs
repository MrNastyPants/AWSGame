using UnityEngine;
using UnityEngine.UI;

public class OpenWebsiteWithPopup : MonoBehaviour
{
    [Header("Website URL")]
    [SerializeField] private string websiteURL = "https://www.amazon.com";

    [Header("UI References")]
    [SerializeField] private GameObject popupPanel; //Panel
    [SerializeField] private Text popupMessage;     //Text Component
    [SerializeField] private Button confirmButton;  //Confirm Button
    [SerializeField] private Button cancelButton;   //Cancel Button

    private void Start()
    {
        // Set up the UI
        popupPanel.SetActive(false); // Ensure the popup is hidden initially

        // Add listeners to buttons
        confirmButton.onClick.AddListener(OnConfirm);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancel);
    }

    public void ShowPopup()
    {
        //message
        popupMessage.text = "To use the Amazon extension:\n\n" +
                            "1. Ensure the browser extension is installed and enabled.\n" +
                            "2. Navigate to a product page.\n" +
                            "3. Click the extension to copy product details.\n\n" +
                            "Press Confirm to proceed to Amazon.";

        // Display the popup panel
        popupPanel.SetActive(true);
    }

    private void OnConfirm()
    {
        // Open the website
        Application.OpenURL(websiteURL);

        // Hide the popup
        popupPanel.SetActive(false);
    }

    private void OnCancel()
    {
        // Hide the popup
        popupPanel.SetActive(false);
    }
}