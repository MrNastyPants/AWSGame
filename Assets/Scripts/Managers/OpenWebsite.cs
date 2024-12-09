using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenWebsite : MonoBehaviour
{
    // Choose URL to website?
    private string websiteURL = "https://awsdevchallenge.devpost.com/?ref_feature=challenge&ref_medium=discover";

    // Function to open 
    public void OpenWebsiteFunc()
    {
        Application.OpenURL(websiteURL);
    }
}


