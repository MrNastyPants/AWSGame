using System;
using System.Collections;
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
using System.Threading.Tasks; 

public class AI : MonoBehaviour{
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

    private int promptnumber = 0;
    private void Awake() {
        var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
        client = new AmazonBedrockRuntimeClient(credentials, RegionEndpoint);
        submitButton.onClick.AddListener(call: () => SendPrompt(inputField.text));
    }

    public void SendPrompt(string playerInput) {

        string ratingPrompt = $"Background: {characterBackground}\n" +
                              $"Character: {characterName}, Traits: {characterTraits}\n" +
                              $"Scenario: {scenarioDescription}\n" +
                              $"Player Input: {playerInput}\n\n" +
                              $"Instructions: Generate a number from 1 to 10 to indicate how effective the player input item is for dealing with the scenario. Return the number with no additional text.";

        string speechPrompt = $"Background: {characterBackground}\n" +
                              $"Character: {characterName}, Traits: {characterTraits}\n" +
                              $"Scenario: {scenarioDescription}\n" +
                              $"Player Input: {playerInput}\n\n" +
                              $"Instructions: Generate one sentence of the character's speech in response to the player's input. Keep the response brief and to the point.";

        promptnumber = 0;
        AWSManager.Titan(speechPrompt, ModelId, client);
        StartCoroutine(checkprompt(promptnumber));

        promptnumber = 1;
        AWSManager.Titan(ratingPrompt, ModelId, client);
        StartCoroutine(checkprompt(promptnumber));

    }

    public void recieveprompt(string response, int number) { 
        if (number == 0)
        {
            responseText.text = AWSManager.AIFinal;
        }
        else if (number == 1){
            ratingText.text = AWSManager.AIFinal;
        }
    }
    
    private IEnumerator checkprompt(int number) {
        while (AWSManager.AIFinal == "NA")
        {
            if (AWSManager.AIFinal != "NA")
            {
                recieveprompt(AWSManager.AIFinal, number);
                break;
            }
            yield return new WaitForSeconds(0.001f);
        }

    }
}