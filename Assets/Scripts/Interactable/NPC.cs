using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable{

    [Header("General NPC Stats")]
    [SerializeField] private string NPCName;

    [Header("References")]
    [SerializeField] private GameObject _alertBox;
    [SerializeField] public NPCPrompt _thisNPC;

    [Header("Dialogue Settings")]
    [SerializeField] ResponseOutPut _introResponse;
    [SerializeField] private bool _firstTime = true, _finishedQuest = false, _checkQuest = false, _passedQuest = false;
    [SerializeField] ResponseOutPut _questResponse;
    [SerializeField] List<ResponseOutPut> _goodResponseQueue = new();           //The player successfully gave them an item
    [SerializeField] List<ResponseOutPut> _badResponseQueue = new();            //The player unsuccessfully game them an item [DEFAULT]

    //Properties
    protected GameObject AlertBox {
        get => _alertBox != null ? _alertBox : _alertBox = transform.Find("Alert").gameObject;
    }

    //Functions
    private void Start() {
        //Initializes the NPC
        AlertBox.SetActive(false);
    }

    //Dialogue Options
    public void FinishedTalking(int rating) {
        //Sets the Variables
        _firstTime = false;

        //Checks the Quest
        if (_checkQuest) {
            _passedQuest = rating > 5;
            _finishedQuest = rating != -1;
            _checkQuest = false;
            _thisNPC._runPrompt = false;
        }

        //Sends an Alert
        print("Received: " + rating + ". You " + (_passedQuest ? "Passed!" : "did not pass."));
    }

    //Interaction Functions
    public override void Interact() {
        //Turns to face the player
        transform.Find("Mesh").LookAt(GameManager.Manager.Player.transform.position);

        //First time 
        if (_firstTime && _introResponse != null) {
            GameManager.Manager.StartDialogue(_introResponse, _thisNPC, this);
            return;
        }

        //You have to finish the quest in order to continue
        if (!_finishedQuest && _questResponse != null) {
            _thisNPC._runPrompt = true;
            _checkQuest = true;
            GameManager.Manager.StartDialogue(_questResponse, _thisNPC, this);
            return;
        }

        //Exits if responses are empty
        if (_goodResponseQueue.Count == 0 && _badResponseQueue.Count == 0) return;


        //Initialize Values
        var temp = _passedQuest ? _goodResponseQueue : _badResponseQueue;
        int responseID = Random.Range(0, temp.Count);

        //Opens up the dialogue box
        GameManager.Manager.StartDialogue(temp[responseID], _thisNPC, this);
    }
    public override void Hover() {
        AlertBox.SetActive(true);
    }
    public override void EndHover() {
        AlertBox.SetActive(false);
    }
}
