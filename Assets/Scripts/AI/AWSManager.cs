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

public static class AWSManager
{
    public static string AIFinal = "NA";
    public static async void Titan(string Prompt, string ModelId, AmazonBedrockRuntimeClient client)
    {
        Prompt = "inputText: " + Prompt + " Assistant:";

        // Create the request for the rating
        var Request = new InvokeModelRequest
        {
            ModelId = ModelId,
            ContentType = "application/json",
            Accept = "application/json",
            Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                inputText = Prompt,
            })))
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
                
                AIFinal = AIResponse;
                Debug.Log("AIFINAL:" + AIFinal);
            }

            // Check if we received valid responses
            if (!string.IsNullOrEmpty(AIResponse))
            {
                AIFinal = AIResponse;
            }
        }
        catch (Exception ex)
        {
        }
    }
}
