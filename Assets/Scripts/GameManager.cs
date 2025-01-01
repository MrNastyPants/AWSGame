using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Inventory")]
    [SerializeField] public Item heldItem;
    [SerializeField] public float PlayerMoney = 0;

    //Properties
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

    private bool _currentFocus = true;
    private bool FocusChange {
        get {
            bool current = Application.isFocused;

            //Returns false if it's the same value
            if (current == _currentFocus) return false;

            //Not open
            if (!current) return _currentFocus = current;

            return _currentFocus = current;
        }
    }

    private void FixedUpdate() {
        if (FocusChange) {
            string amazonDesc = GUIUtility.systemCopyBuffer;
            print(amazonDesc);
        }
    }

    //Public Functions
    public void StartDialogue(ResponseOutPut npc) {
        //Starts the dialogue
        Player.CanMove = false;             //Stops the player from moving
        HUD.OpenDialogue(npc);              //Opens the dialogue window
    }
    public void StopDialogue() { 
        //Stops the dialogue
        Player.CanMove = true;              //Frees the player
        HUD.OpenMenu(0);                    //Closes the dialogue menu
    }
}
