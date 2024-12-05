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
    [SerializeField] private NPC _currentNPC;
    [SerializeField] private int _state = 0, _optionSelected = 0, _optionCount = 0;
    [SerializeField] private bool _finishedTyping = false;
    [SerializeField] private bool _openOptions = false;
    [SerializeField] private bool _optionsOpened = false;
    [SerializeField] Coroutine _typingCoroutine;

    //Controls the Dialogue Window
    private void Update() {
        //Continues if the key is pressed
        if (Input.GetKeyDown(KeyCode.E)) {
            if (_openOptions && _finishedTyping) ControlOptions();
            else TypingSpeedUp();
        }
        //Moves up and down the options
        if (_optionsOpened) {
            //Moves up and down the list
            HoverOption(_optionSelected, false);
            if (Input.GetKeyDown(KeyCode.W)) _optionSelected = _optionSelected - 1 != -1 ? _optionSelected - 1 : _optionCount - 1;
            if (Input.GetKeyDown(KeyCode.S)) _optionSelected = (_optionSelected + 1) % _optionCount;
            HoverOption(_optionSelected);
        }

    }

    //Initializes the Dialogue tree
    public void InitializeDialogue(NPC npc) {
        //Exits because nothing was sent over
        if (npc.DialogueTree.Count == 0) {
            StopDialogue();
            return;
        }

        //Turns off the options
        Options.SetActive(false);

        //Initializes the vlaues
        _state = -1;
        _currentNPC = npc;
        _openOptions = false;
        _finishedTyping = false;

        //Runs the Dialogue
        NextDialogue();
    }
    public void StopDialogue() {
        GameManager.Manager.StopDialogue();
    }
    private void NextDialogue() {
        //Increments the state
        _state++;

        //Checks to make sure it is not the end
        if (_state + 1 > _currentNPC.DialogueTree.Count) {
            StopDialogue();
            return;
        }

        //Starts the typing
        Name.text = _currentNPC.DialogueTree[_state].Name;
        _typingCoroutine = StartCoroutine(TypeText(_currentNPC.DialogueTree[_state].Text));

        //Checks to see if there were Options
        _openOptions = _currentNPC.DialogueTree[_state].Response1 != "";

    }

    //Speed up Typing
    private void TypingSpeedUp() {
        //Goes on to the next panel
        if (_finishedTyping) {
            NextDialogue();
            return;
        }

        //Stops the typing co-routine and sets the text to the full text
        StopCoroutine(_typingCoroutine);
        MainText.text = _currentNPC.DialogueTree[_state].Text;
        _finishedTyping = true;
    }
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
    private void ControlOptions() {
        //Opens up the Options
        if (_optionsOpened == false) {
            OpenOptions();
            return;
        }

        //Selects the Option
        switch (_optionSelected) {
            case 0: _state = _currentNPC.DialogueTree[_state].Option1 - 1; break;
            case 1: _state = _currentNPC.DialogueTree[_state].Option2 - 1; break;
            case 2: _state = _currentNPC.DialogueTree[_state].Option3 - 1; break;
        }

        //Closes the Options
        CloseOptions();
        NextDialogue();
    }
    private void OpenOptions() {
        //Sets up the Options Menu
        Options.SetActive(true);
        _optionSelected = 0;

        // Option 1 always active
        Option1.gameObject.SetActive(true);
        Option1.transform.Find("Text").GetComponent<Text>().text = _currentNPC.DialogueTree[_state].Response1;
        _optionCount = 1;

        //Option 2
        Option2.gameObject.SetActive(_currentNPC.DialogueTree[_state].Response2 != "");
        Option2.transform.Find("Text").GetComponent<Text>().text = _currentNPC.DialogueTree[_state].Response2;
        _optionCount = Option2.gameObject.activeInHierarchy ? _optionCount + 1 : _optionCount;

        //Option 3
        Option3.gameObject.SetActive(_currentNPC.DialogueTree[_state].Response3 != "");
        Option3.transform.Find("Text").GetComponent<Text>().text = _currentNPC.DialogueTree[_state].Response3;
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

}
