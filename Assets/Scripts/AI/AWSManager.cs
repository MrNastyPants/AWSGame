using Amazon.BedrockRuntime.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;
using Amazon.BedrockRuntime;
using Amazon;
using Amazon.Runtime;

public static class AWSManager
{
    //AI Applications
    public static string AIFinal = "";
    public static string AIScore = "";
    public const string ModelId = "amazon.titan-text-premier-v1:0";

    //General Stats
    private const string accessKey = "AKIA57VDLLRWBB5LSSWM";
    private const string accessKeySecret = "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";

    //Wakes up the AI
    public static AmazonBedrockRuntimeClient TurnOnAI() {
        var credentials = new BasicAWSCredentials(accessKey, accessKeySecret);
        return new AmazonBedrockRuntimeClient(credentials, RegionEndpoint.USEast1);
    }

    //Functions
    public static async void Titan(string Prompt, AmazonBedrockRuntimeClient client, Action sendResponse, bool rating = false)
    {
        Prompt = "inputText: " + Prompt + " Assistant:";

        // Create the request for the rating
        var Request = new InvokeModelRequest
        {
            ModelId = ModelId,
            ContentType = "application/json",
            Accept = "application/json",
            Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new{inputText = Prompt,})))
        };

        try
        {
            // Send requests and await responses
            var Response = await client.InvokeModelAsync(Request);
            var ResponseBody = await new StreamReader(Response.Body).ReadToEndAsync();

            // Log the raw responses for debugging
            Debug.Log("Response Body: " + ResponseBody);

            // Parse the responses
            var ratingModelResponse = JObject.Parse(ResponseBody);

            // Extract the rating and the speech
            string AIResponse = null;

            // For Titan, check the structure of the response
            if (ratingModelResponse["results"] != null && ratingModelResponse["results"].HasValues)
            {
                var result = ratingModelResponse["results"][0];  // Get the first result
                AIResponse = result["outputText"]?.ToString()?.Trim();
            }

            // Check if we received valid responses
            if (!string.IsNullOrEmpty(AIResponse)){
                //Sets the Proper Variables
                if (rating) AIScore = AIResponse;
                else AIFinal = AIResponse;
                
                //Calls the external Script
                sendResponse?.Invoke();
            }
        }
        catch (Exception exception){
            //Logs the Error that Occured
            Debug.Log(exception);
        }
    }
    public static string BuildPrompt(NPCPrompt prompt, string playerInput) {
        string speechPrompt = $"Background: {prompt.characterBackground}\n" +
                      $"Character: {prompt.characterName}, Traits: {prompt.characterTraits}\n" +
                      $"Scenario: {prompt.scenarioDescription}\n" +
                      $"Player Input: {playerInput}\n\n" +
                      $"Instructions: Generate one to three sentences of what the character would say in response to the player's input. " +
                      $"The response should be short, crisp, and to the point and written as if the character is speaking directly to the player." +
                      $"Do not include any dialogue or quotation marks. " +
                      $"Do not include any explanations or additional information beyond the character's speech in response to the player's input.";

        //Sends the Prompt to the AI
        return speechPrompt;
    }
    public static string BuildRating(NPCPrompt prompt, string playerInput) {
        string ratingPrompt = $"Background: {prompt.characterBackground}\n" +
                              $"Character: {prompt.characterName}, Traits: {prompt.characterTraits}\n" +
                              $"Scenario: {prompt.scenarioDescription}\n" +
                              $"Player Input: {playerInput}\n\n" +
                              $"Based on the input, provide how effective the player input item is for dealing with the scenario as a single-digit number (e.g., \"5\"). " +
                              $"On a scale from 0 to 10, without mentioning the scale. Do not include anything else except the final score in your response.";

        return ratingPrompt;
    }

}
