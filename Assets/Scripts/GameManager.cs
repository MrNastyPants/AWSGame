using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Manager;

    [Header("Built in Components")]
    public LevelManager LevelManager = new LevelManager();

    [Header("References")]
    private HUDManager _hud;
    private PlayerController _player;
    private CameraController _camera;
    private Item _heldItem;

    [Header("Inventory")]
    [SerializeField] public List<Item> Inventory = new List<Item>();
    [SerializeField] public float PlayerMoney = 0;


    //Properties
    public Item HeldItem {
        get => _heldItem;
        set {
            //Changes the Animation for the Player
            Player.ChangeCarry(value != null);

            //Plays the Sound
            HUD.PlaySound("Box");

            //Sets the Value
            _heldItem = value;
        }
    }
    public HUDManager HUD {
        get {
            //Checks to see if a HUD already exists
            if (_hud != null) return _hud;

            //Looks for one in the Scene
            if (GameObject.Find("InGameCanvas")) { 
                _hud = GameObject.Find("InGameCanvas").GetComponent<HUDManager>();
                return _hud;
            }

            //Creates one from scratch if there still isn't one
            var obj = GameObject.Instantiate(Resources.Load<GameObject>("InGameCanvas"));
            return _hud = obj.GetComponent<HUDManager>();
        }
    }
    public PlayerController Player {
        get {
            //The player already exists
            if (_player != null) return _player;

            //Finds the player out in the real world
            if(GameObject.Find("Player") != null) return _player = GameObject.Find("Player").GetComponent<PlayerController>();

            //Creates a new player
            var obj = GameObject.Instantiate(Resources.Load<GameObject>("Player")); 
            return _player = obj.GetComponent<PlayerController>();
        }
    }
    public CameraController Camera {
        get {
            //The player already exists
            if (_camera != null) return _camera;

            //Finds the player out in the real world
            if (GameObject.Find("Main Camera") != null) return _camera = GameObject.Find("Main Camera").GetComponent<CameraController>();

            //Creates a new player
            var obj = GameObject.Instantiate(Resources.Load<GameObject>("Main Camera"));
            return _camera = obj.GetComponent<CameraController>();
        }
    }

    //Functions
    private void Awake() {
        //Creates a Singleton of the GameManager
        if (Manager == null) {
            Manager = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        //Iniatializes the Level Manager
        LevelManager.Init();
        SceneManager.sceneLoaded += LevelManager.OnSceneLoaded;
    }

    //Public Functions
    [SerializeField] private NPC _speaker;
    public void StartDialogue(ResponseOutPut npc, NPCPrompt prompt, NPC speaker) {
        //Starts the dialogue
        Player.CanMove = false;                 //Stops the player from moving
        HUD.OpenDialogue(npc, prompt);          //Opens the dialogue window
        _speaker = speaker;
    }
    public void StopDialogue(int rating) { 
        //Stops the dialogue
        Player.CanMove = true;              //Frees the player
        HUD.OpenMenu(0);                    //Closes the dialogue menu
        _speaker.FinishedTalking(rating);
        _speaker = null;
    }
    public void GiveUpEquippedItem() {
        //Removes the Held Item then destroys the held item
        for (int i = 0; i < Inventory.Count; i++) {
            if (Inventory[i].Name == HeldItem.Name) {
                Inventory.RemoveAt(i);
                break;
            }
        }

        //Turns the held item off
        HeldItem = null;
    }
    public bool IsTalking() { return _speaker != null; }
    public void UpdateQuests(bool pass) {
        //Sends in the extra amount to refresh the UI
        LevelManager.RefreshQuests(true, pass);
    }
}
