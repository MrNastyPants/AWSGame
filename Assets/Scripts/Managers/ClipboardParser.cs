using System;
using System.Globalization; // For float parsing
using System.Text.RegularExpressions;
using UnityEngine;

public class ClipboardParser : MonoBehaviour
{
    private string lastClipboardText = ""; // Stores the last clipboard content to detect changes

    void Update()
    {
        // Retrieve clipboard text using Unity's GUIUtility
        string clipboardText = GUIUtility.systemCopyBuffer;

        // Check if clipboard content has changed
        if (!string.IsNullOrEmpty(clipboardText) && clipboardText != lastClipboardText)
        {
            lastClipboardText = clipboardText; // Update the stored clipboard text

            // Parse the clipboard text
            ParseClipboardText(clipboardText, out string productName, out float price);

            // Display the result in the console
            Debug.Log($"Updated Product Name: {productName}");
            Debug.Log($"Updated Price: {price}");
        }
    }

    /// <summary>
    /// Parses the clipboard text to extract the product name and price.
    /// </summary>
    /// <param name="clipboardText">The raw text copied from the clipboard.</param>
    /// <param name="productName">Output parameter for the product name.</param>
    /// <param name="price">Output parameter for the price as a float.</param>
    void ParseClipboardText(string clipboardText, out string productName, out float price)
    {
        productName = "";
        price = 0f;

        try
        {
            // Use regex to extract the product name
            Match nameMatch = Regex.Match(clipboardText, @"Product Name:\s*(.*)");
            if (nameMatch.Success)
            {
                productName = nameMatch.Groups[1].Value.Trim();
            }

            // Use regex to extract the price
            Match priceMatch = Regex.Match(clipboardText, @"Price\s*[:\s]*([\d,\.]+)");
            if (priceMatch.Success)
            {
                string priceString = priceMatch.Groups[1].Value.Trim();

                // Replace commas with dots to standardize the format
                priceString = priceString.Replace(",", ".");

                // Use InvariantCulture to ensure proper float parsing
                if (float.TryParse(priceString, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float parsedPrice))
                {
                    price = parsedPrice;
                }
                else
                {
                    Debug.LogWarning($"Failed to parse price: {priceString}");
                }
            }
            else
            {
                Debug.LogWarning("Price not found in clipboard text.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing clipboard text: {e.Message}");
        }
    }
}
