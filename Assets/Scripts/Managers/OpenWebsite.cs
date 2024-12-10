using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenWebsite : MonoBehaviour
{
    // Choose URL to website?
    private string websiteURL = "https://www.amazon.com";

    // Function to open 
    public void OpenWebsiteFunc()
    {
        Application.OpenURL(websiteURL);
    }
}


