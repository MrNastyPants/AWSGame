using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable{

    [Header("General NPC Stats")]
    [SerializeField] private string NPCName;

    [Header("References")]
    [SerializeField] private GameObject _alertBox;
    [SerializeField] public NPCPrompt _thisNPC;
    [SerializeField] private AudioSource _alertSound;

    [Header("Dialogue Settings")]
    [SerializeField] ResponseOutPut _introResponse;
    [SerializeField] private bool _firstTime = true, _finishedQuest = false, _checkQuest = false, _passedQuest = false;
    [SerializeField] ResponseOutPut _questResponse;
    [SerializeField] List<ResponseOutPut> _goodResponseQueue = new();           //The player successfully gave them an item
    [SerializeField] List<ResponseOutPut> _badResponseQueue = new();            //The player unsuccessfully game them an item [DEFAULT]

    [Header("Animation")]
    [SerializeField] private bool _animate = false;
    [SerializeField] private Animator _anim;

    //Properties
    protected Animator Anim { 
        get => _anim != null ? _anim : _anim = transform.Find("Mesh").GetComponent<Animator>();
    }
    protected GameObject AlertBox {
        get => _alertBox != null ? _alertBox : _alertBox = transform.Find("Alert").gameObject;
    }
    protected AudioSource AlertSound {
        get => _alertSound != null ? _alertSound : _alertSound = GetComponent<AudioSource>();
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

        //Ends the Talking Animation
        if (_animate) {
            Anim.SetBool("Talking", false);
            transform.Find("Mesh").LookAt(transform.position - transform.right);
        }
    }

    //Interaction Functions
    public override void Interact() {
        //Turns to face the player
        transform.Find("Mesh").LookAt(GameManager.Manager.Player.transform.position);

        //First time 
        if (_firstTime && _introResponse != null) {
            GameManager.Manager.StartDialogue(_introResponse, _thisNPC, this);

            //Starts the Talking Animation
            if (_animate) Anim.SetBool("Talking", true);

            return;
        }

        //You have to finish the quest in order to continue
        if (!_finishedQuest && _questResponse != null) {
            _thisNPC._runPrompt = true;
            _checkQuest = true;
            GameManager.Manager.StartDialogue(_questResponse, _thisNPC, this);

            //Starts the Talking Animation
            if (_animate) Anim.SetBool("Talking", true);

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
        if (_finishedQuest) return;
        AlertBox.SetActive(true);
        _alertSound.Play();
    }
    public override void EndHover() {
        AlertBox.SetActive(false);
    }
}
