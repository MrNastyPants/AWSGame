using Amazon.BedrockRuntime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Text _name;
    [SerializeField] private Text _mainText;
    [SerializeField] private GameObject _options;
    [SerializeField] private Button _option1, _option2, _option3;

    //Properties
    private Text Name {
        get => _name != null ? _name : _name = transform.Find("Name/Text").GetComponent<Text>();
    }
    private Text MainText {
        get => _mainText != null ? _mainText : _mainText = transform.Find("MainText").GetComponent<Text>();
    }
    private GameObject Options {
        get => _options != null ? _options : _options = transform.Find("Options").gameObject;
    }
    private Button Option1 { 
        get => _option1 != null ? _option1 : _option1 = Options.transform.Find("OptionOne").GetComponent<Button>();
    }
    private Button Option2 {
        get => _option2 != null ? _option2 : _option2 = Options.transform.Find("OptionTwo").GetComponent<Button>();
    }
    private Button Option3 {
        get => _option3 != null ? _option3 : _option3 = Options.transform.Find("OptionThree").GetComponent<Button>();
    }

    [Header("Read-Only Stats")]
    [SerializeField] private ResponseOutPut _currentResponse;
    [SerializeField] private int _currentID = 0, _optionSelected = 0, _optionCount = 0;
    [SerializeField] private bool _finishedTyping = false;
    [SerializeField] private bool _openOptions = false;
    [SerializeField] private bool _optionsOpened = false;
    [SerializeField] private bool _exitAfter = false;
    [SerializeField] private string _specialComment = "";
    [SerializeField] private int _responseRating = -1;
    [SerializeField] Coroutine _typingCoroutine;

    [Header("AI Variables")]
    private AmazonBedrockRuntimeClient client;

    //Controls the Dialogue Window
    private void Update() {
        //Continues if the key is pressed
        if (Input.GetKeyDown(KeyCode.E)) StateMachine(_currentID);
        //Moves up and down the options
        if (_optionsOpened) {
            //Moves up and down the list
            HoverOption(_optionSelected, false);
            if (Input.GetKeyDown(KeyCode.W)) _optionSelected = _optionSelected - 1 != -1 ? _optionSelected - 1 : _optionCount - 1;
            if (Input.GetKeyDown(KeyCode.S)) _optionSelected = (_optionSelected + 1) % _optionCount;
            HoverOption(_optionSelected);
        }

    }
    private void Awake() {
        client = AWSManager.TurnOnAI();
    }

    //Initializes and controls the Dialogue tree
    public void InitializeDialogue(ResponseOutPut response, NPCPrompt prompt) {
        //Exits because nothing was sent over
        if (response == null || response.OutPutResponse.Count == 0) {
            StopDialogue();
            return;
        }

        //Turns off the options
        Options.SetActive(false);

        //Initializes the vlaues
        _currentResponse = response;
        _openOptions = false;
        _finishedTyping = false;
        _exitAfter = false;
        _specialComment = "";
        _responseRating = -1;

        //Sends in the Info to get the AI Response
        if (GameManager.Manager.HeldItem != null && GameManager.Manager.HeldItem.Name != "" && prompt._runPrompt) CallTheAI(prompt);

        //Runs the Dialogue
        NextDialogue(0);
    }
    public void StopDialogue() {      
        //Stops Everything
        GameManager.Manager.StopDialogue(_responseRating);
    }
    private void StateMachine(int ID) {
        //Stops the typing co-routine and sets the text to the full text
        if (!_finishedTyping) {
            StopCoroutine(_typingCoroutine);
            MainText.text = _currentResponse.OutPutResponse[ID].AIResponse ? _specialComment : _currentResponse.OutPutResponse[ID].Text;
            _finishedTyping = true;
            return;
        }

        //The options are open and is taking in the input
        if (_openOptions) {
            ControlOptions(ID);
            return;
        }

        //Checks to see if it's finished with the tree
        if (_currentResponse.OutPutResponse[ID].isEnd || _exitAfter) {
            StopDialogue();
            return;
        }

        //Checks the Response Rating and sets the pass fail parameters
        int nextOp = _currentResponse.OutPutResponse[ID].Option1;
        if (_currentResponse.OutPutResponse[ID].AIResponse)
            nextOp = _responseRating > 5 ? _currentResponse.OutPutResponse[ID].Option1 : _currentResponse.OutPutResponse[ID].Option2;

        //Continues on to the next Dialogue Branch
        NextDialogue(nextOp);
    }
    private void NextDialogue(int ID) {
        //Checks the Current ID
        _currentID = ID;

        //Gives the player Money if there is any
        if (_currentResponse.OutPutResponse[ID].giveMoney != 0) GivePlayerMoney(_currentResponse.OutPutResponse[ID].giveMoney);

        //Starts the typing
        Name.text = _currentResponse.OutPutResponse[ID].Name;
        string speech = _currentResponse.OutPutResponse[ID].Text;

        //Checks to see if there is an AI response and if not check to see if there were options
        if (_currentResponse.OutPutResponse[ID].AIResponse) speech = AIResponse();
        else _openOptions = _currentResponse.OutPutResponse[ID].Response1 != "";

        //Starts typing
        _typingCoroutine = StartCoroutine(TypeText(speech));
    }

    //Types in the text
    IEnumerator TypeText(string text) {
        //Initializes Variables
        var newString = "";
        _finishedTyping = false;

        //Types in the text
        for (int i = 0; i < text.Length; i++) {
            newString += text[i];
            MainText.text = newString;
            yield return new WaitForSeconds(0.05f);
        }

        //Is done typing
        _finishedTyping = true;
    }

    //Options
    private void ControlOptions(int ID) {
        //Opens up the Options
        if (_optionsOpened == false) {
            OpenOptions(ID);
            return;
        }

        //Selects the Option
        var _optionSelected = 0; 
        switch (this._optionSelected) {
            case 0: _optionSelected = _currentResponse.OutPutResponse[ID].Option1; break;
            case 1: _optionSelected = _currentResponse.OutPutResponse[ID].Option2; break;
            case 2: _optionSelected = _currentResponse.OutPutResponse[ID].Option3; break;
        }

        //Closes the Options
        CloseOptions();
        NextDialogue(_optionSelected);
    }
    private void OpenOptions(int ID) {
        //Sets up the Options Menu
        Options.SetActive(true);
        _optionSelected = 0;

        // Option 1 always active
        Option1.gameObject.SetActive(true);
        Option1.transform.Find("Text").GetComponent<Text>().text = _currentResponse.OutPutResponse[ID].Response1;
        _optionCount = 1;

        //Option 2
        Option2.gameObject.SetActive(_currentResponse.OutPutResponse[ID].Response2 != "");
        Option2.transform.Find("Text").GetComponent<Text>().text = _currentResponse.OutPutResponse[ID].Response2;
        _optionCount = Option2.gameObject.activeInHierarchy ? _optionCount + 1 : _optionCount;

        //Option 3
        Option3.gameObject.SetActive(_currentResponse.OutPutResponse[ID].Response3 != "");
        Option3.transform.Find("Text").GetComponent<Text>().text = _currentResponse.OutPutResponse[ID].Response3;
        _optionCount = Option3.gameObject.activeInHierarchy ? _optionCount + 1 : _optionCount;

        //Returns that it was opened
        HoverOption(_optionSelected);
        _optionsOpened = true;
    }
    private void CloseOptions() {
        HoverOption(_optionSelected, false);
        Options.SetActive(false);
        _optionsOpened = false;
    }
    private void HoverOption(int value, bool hover = true) {
        switch (value) {
            case 0:
                Option1.transform.Find("Text").GetComponent<Text>().fontSize += hover ? 5 : -5;
                Option1.transform.Find("Text").GetComponent<Text>().fontStyle = hover ? FontStyle.Bold : FontStyle.Normal;
                break;
            case 1:
                Option2.transform.Find("Text").GetComponent<Text>().fontSize += hover ? 5 : -5;
                Option2.transform.Find("Text").GetComponent<Text>().fontStyle = hover ? FontStyle.Bold : FontStyle.Normal;
                break;
            case 2:
                Option3.transform.Find("Text").GetComponent<Text>().fontSize += hover ? 5 : -5;
                Option3.transform.Find("Text").GetComponent<Text>().fontStyle = hover ? FontStyle.Bold : FontStyle.Normal;
                break;
        }
    }

    //AI Functions
    private void CallTheAI(NPCPrompt prompt) {
        //Calls the Ai
        print("Calling the AI");

        //Sets the Variables
        string finalPrompt = AWSManager.BuildPrompt(prompt, GameManager.Manager.HeldItem.Name);
        string scorePrompt = AWSManager.BuildRating(prompt, GameManager.Manager.HeldItem.Name);
        _specialComment = "";
        _responseRating = -1;

        //Sends the Information to the AI and awaits for a response
        AWSManager.Titan(finalPrompt, client, ReceiveAIInput);
        AWSManager.Titan(scorePrompt, client, ReceiveAIScore, true);
    }
    private void GivePlayerMoney(float Amount) {
        GameManager.Manager.PlayerMoney += Amount;
        GameManager.Manager.HUD.UpdateMoney(GameManager.Manager.PlayerMoney);
    }
    private string ExtractDigits(string text) {
        return string.Join("", System.Text.RegularExpressions.Regex.Matches(text, @"\d+"));
    }
    private string AIResponse() {
        //Checks to see if they are holding an item. 
        if (_specialComment == "") {
            _exitAfter = true;
            return _specialComment = "You aren't holding anything. Come back when you have something for me!";
        }

        //Removes the Item that the player is holding
        GameManager.Manager.GiveUpEquippedItem();

        //Returns the AI Response
        return _specialComment;
    }

    //External Functions
    public void ReceiveAIInput() {
        _specialComment = AWSManager.AIFinal;
    }
    public void ReceiveAIScore() {
        //Retreives the number from the Score
        string score = AWSManager.AIScore;

        //Checks to make sure there is only digits left and parses it into an int
        string temp = ExtractDigits(score);
        _responseRating = int.Parse(temp);
    }
}
