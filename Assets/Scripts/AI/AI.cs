using System;
using System.Collections.Generic;
using System.IO;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks; // Add this to resolve the Task issue

public static class AI {
    [Header("AWS Credentials")]
    [SerializeField] private string accessKeyId;
    [SerializeField] private string secretAccessKey;

    [Header("Experience Settings")]
    [SerializeField] private Text promptText;
    [SerializeField] private Text responseText;
    [SerializeField] private Text ratingText;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button submitButton;

    [Header("Character Settings")]
    [SerializeField] private string characterName;
    [SerializeField] private string characterBackground;
    [SerializeField] private string characterTraits;
    [SerializeField] private string scenarioDescription;

    private AmazonBedrockRuntimeClient client;
    private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;
    private const string ModelId = "amazon.titan-text-premier-v1:0";

    private void Awake() {
        var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
        client = new AmazonBedrockRuntimeClient(credentials, RegionEndpoint);
        submitButton.onClick.AddListener(call: () => SendPrompt(inputField.text));
    }

    public async void SendPrompt(string playerInput) {
        // Build the prompt for the rating
        string ratingPrompt = $"Background: {characterBackground}\n" +
                              $"Character: {characterName}, Traits: {characterTraits}\n" +
                              $"Scenario: {scenarioDescription}\n" +
                              $"Player Input: {playerInput}\n\n" +
                              $"Instructions: Generate a number from 1 to 10 to indicate how effective the player input item is for dealing with the scenario. Return the number with no additional text.";

        // Build the prompt for the character's speech
        string speechPrompt = $"Background: {characterBackground}\n" +
                              $"Character: {characterName}, Traits: {characterTraits}\n" +
                              $"Scenario: {scenarioDescription}\n" +
                              $"Player Input: {playerInput}\n\n" +
                              $"Instructions: Generate one sentence of the character's speech in response to the player's input. Keep the response brief and to the point.";

        Titan(speechPrompt, ratingPrompt);
    }

    public static async void Titan(string speechPrompt, string ratingPrompt) {
        ratingPrompt = "inputText: " + ratingPrompt + " Assistant:";
        speechPrompt = "inputText: " + speechPrompt + " Assistant:";

        // Create the request for the rating
        var ratingRequest = new InvokeModelRequest {
            ModelId = ModelId,
            ContentType = "application/json",
            Accept = "application/json",
            Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
                inputText = ratingPrompt,
            })))
        };

        // Create the request for the character's speech
        var speechRequest = new InvokeModelRequest {
            ModelId = ModelId,
            ContentType = "application/json",
            Accept = "application/json",
            Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
                inputText = speechPrompt,
            })))
        };

        try {
            // Send requests and await responses
            var ratingResponse = await client.InvokeModelAsync(ratingRequest);
            var speechResponse = await client.InvokeModelAsync(speechRequest);

            var ratingResponseBody = await new StreamReader(ratingResponse.Body).ReadToEndAsync();
            var speechResponseBody = await new StreamReader(speechResponse.Body).ReadToEndAsync();

            // Log the raw responses for debugging
            Debug.Log("Rating Response Body: " + ratingResponseBody);
            Debug.Log("Speech Response Body: " + speechResponseBody);

            // Parse the responses
            var ratingModelResponse = JObject.Parse(ratingResponseBody);
            var speechModelResponse = JObject.Parse(speechResponseBody);

            // Extract the rating and the speech
            string rating = null;
            string speech = null;

            // For Titan, check the structure of the response
            if (ratingModelResponse["results"] != null && ratingModelResponse["results"].HasValues) {
                var result = ratingModelResponse["results"][0];  // Get the first result
                rating = result["outputText"]?.ToString()?.Trim();
            }

            if (speechModelResponse["results"] != null && speechModelResponse["results"].HasValues) {
                var result = speechModelResponse["results"][0];  // Get the first result
                speech = result["outputText"]?.ToString()?.Trim();
            }

            // Check if we received valid responses
            if (!string.IsNullOrEmpty(rating) && !string.IsNullOrEmpty(speech)) {
                // Display the results in the UI
                ratingText.text = $"Effectiveness Rating: {rating}";
                responseText.text = speech;
            } else {
                // If parsing failed, log the raw responses
                Debug.LogError("Error: Could not parse the responses correctly.");
                Debug.LogError("Rating Response (Raw): " + ratingResponseBody);
                Debug.LogError("Speech Response (Raw): " + speechResponseBody);

                responseText.text = "Error: Could not parse the responses correctly.";
                ratingText.text = "Error: Rating not available.";
            }
        } catch (Exception ex) {
            // Log the exception message to the terminal
            Debug.LogError($"Error during API call: {ex.Message}");

            // Log any other details of the exception if necessary
            Debug.LogError($"Exception Details: {ex.ToString()}");

            responseText.text = "Error: Something went wrong.";
            ratingText.text = "Error: Rating not available.";
        }
    }
}