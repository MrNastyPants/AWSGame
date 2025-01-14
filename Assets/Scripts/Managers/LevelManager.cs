using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelManager{

    [Header("Level Manager Stats")]
    [SerializeField] private string _currentScene = "Outside";

    [Header("Tweaker's House")]
    [SerializeField] private int _tweakerHouseLevel = 0;
    [SerializeField] private int _grandmaHouseLevel = 0;
    [SerializeField] private int _moverHouseLevel = 0;
    [SerializeField] private float _tweakerHouseMusic = 0;
    [SerializeField] private float _grandmaHouseMusic = 0;
    [SerializeField] private float _outsideMusic = 0;
    [SerializeField] private float _moverHouseMusic = 0;

    //Public Functions
    public void Init() { 
        //Initializes the Level
        _currentScene = SceneManager.GetActiveScene().name;
    }

    //Changinge Scenes
    public void ChangeScene(int scene) {
        //Turns the scene int into a string
        string sceneName = SceneManager.GetSceneByBuildIndex(scene).name;
        ChangeScene(sceneName);
    }
    public void ChangeScene(string scene) {
        //Checks to see if it should load the credits
        int finalScore = CheckPassedFailed(true) + CheckPassedFailed(false);
        if (finalScore == 3) {
            SceneManager.LoadScene("Credits");
            return;
        }

        //Changes the Scene of the Game
        SaveLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(scene);
    }

    //Loading in the Scene
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        //Finds the Game Object and Loads the Scene
        if (GameObject.FindGameObjectWithTag("LevelChanges") != null) {
            switch (scene.name) {
                case "Outside": LoadOutSide(); break;
                case "Tweaker's House": LoadTweakerHouse(); break;
                case "Grandma's House": LoadGrandmaHouse(); break;
                case "Mover's House":   LoadMoverHouse();   break;
                default: break;
            }
        }

        //Sets the Variables
        _currentScene = scene.name;

        //Updates the money
        GameManager.Manager.HUD.UpdateMoney(GameManager.Manager.PlayerMoney);

        //Updates the Quests on every level
        RefreshQuests();
    }
    public void PlacePlayer(Vector3 position, Vector3 lookDirection) { 
        //Sets the position and rotation
        GameManager.Manager.Player.transform.position = position;
        GameManager.Manager.Player.transform.Find("Mesh").transform.LookAt(GameManager.Manager.Player.transform.position + lookDirection);
    }

    //Quets
    public void RefreshQuests(bool extra = false, bool pass = true) {
        //Checks the Quests
        int passed = CheckPassedFailed(true);
        int failed = CheckPassedFailed(false);

        //Adds the Extra
        if (extra) {
            if (pass) passed++;
            else failed++;
        }

        //Scenes it to the HUD
        GameManager.Manager.HUD.UpdateQuests(passed, failed);
    }
    private int CheckPassedFailed(bool passed) {
        //Initialize Variables
        int finalScore = 0;
        int passGrade = passed ? 3 : 2;

        //Checks which ones passed and which ones failed
        if (_grandmaHouseLevel == passGrade) finalScore++;
        if (_tweakerHouseLevel == passGrade) finalScore++;
        if (_moverHouseLevel == passGrade) finalScore++;

        //Returns the Final
        return finalScore;
    }

    //Save Levels
    private void SaveLevel(string levelName) {
        //Finds the Game Object and exits if it doesn't exist
        if (GameObject.FindGameObjectWithTag("LevelChanges") == null) return;

        switch (levelName) {
            case "Tweaker's House": SaveTweakerHouse(); break;
            case "Grandma's House": SaveGrandmaHouse(); break;
            case "Mover's House":   SaveMoverHouse();   break;
            case "Outside":         SaveOutSide();      break;
            default: break;
        }
    }

    //Load Levels
    protected void LoadOutSide() {
        //Sets the Position of the Player
        switch (_currentScene) {
            case "Tweaker's House": PlacePlayer(new Vector3(0, 0, 0), new Vector3(0, 0, -1));   break;
            case "Grandma's House": PlacePlayer(new Vector3(7, 0, -6), new Vector3(-1, 0, 0));  break;
            case "Mover's House": PlacePlayer(new Vector3(-6, 0, -6), new Vector3(1, 0, 0));    break;
        }

        //Sets the Variables
        var par = GameObject.FindGameObjectWithTag("LevelChanges");
        par.transform.Find("MusicManager").GetComponent<AudioSource>().time = _outsideMusic;

        //Turns on the Tweaker and closes the door
        if (_tweakerHouseLevel == 2) {
            //Checks to see if it's the first time
            if (_currentScene == "Tweaker's House") GameManager.Manager.HUD.PlaySound("Bomb");

            par.transform.Find("TweakerDoor").GetComponent<Door>()._isLocked = true;
            par.transform.Find("Tweaker").gameObject.SetActive(true);
        }

        //Changes the Location to Match
        GameManager.Manager.HUD.UpdateLocation("Apartment Square");
    }
    protected void LoadTweakerHouse() {
        //Sets Variables
        Transform par = GameObject.FindGameObjectWithTag("LevelChanges").transform;

        //Sets the Tweaker
        var temp = par.Find("Tweaker").GetComponent<NPC>();
        NPCLoader(temp, ref _tweakerHouseLevel, ref _tweakerHouseMusic);

        //Turns off the fire
        if (_tweakerHouseLevel == 3) {
            par.Find("Fire").gameObject.SetActive(false);

            //Edits the Player Animation
            temp.Anim.SetBool("Talking", true);
            temp._animate = false;

            //Changes the music
            par.Find("MusicManager").GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Music/Tweaker_Good");
            par.Find("MusicManager").GetComponent<AudioSource>().time = _tweakerHouseMusic;
            par.Find("MusicManager").GetComponent<AudioSource>().Play();
        }

        //Changes the Location to Match
        GameManager.Manager.HUD.UpdateLocation("Tweaker's House");
    }
    protected void LoadGrandmaHouse() {
        //Sets Variables
        Transform par = GameObject.FindGameObjectWithTag("LevelChanges").transform;
        NPCLoader(par.Find("Grandma").GetComponent<NPC>(), ref _grandmaHouseLevel, ref _grandmaHouseMusic);

        //Unlocks the door
        if (_grandmaHouseLevel == 3) par.transform.Find("Door").GetComponent<Door>()._isLocked = false;

        //Changes the Location to Match
        GameManager.Manager.HUD.UpdateLocation("Grandma's House");
    }
    protected void LoadMoverHouse() {
        //Sets Variables
        Transform par = GameObject.FindGameObjectWithTag("LevelChanges").transform;
        NPCLoader(par.Find("Mover").GetComponent<NPC>(), ref _moverHouseLevel, ref _moverHouseMusic);

        //Enables the Box 
        if (_moverHouseLevel == 3) par.Find("Package").transform.Find("Mesh").GetComponent<MeshRenderer>().enabled = true;

        //Changes the Location to Match
        GameManager.Manager.HUD.UpdateLocation("Mover's House");
    }

    //Additional Functions
    private void NPCLoader(NPC _npc, ref int value, ref float music) {
        //Sets the Variables
        _npc._firstTime = value == 0;
        _npc._finishedQuest = value >= 2;
        _npc._passedQuest = value == 3;

        //Set the music
        GameObject.FindGameObjectWithTag("LevelChanges").transform.Find("MusicManager").GetComponent<AudioSource>().time = music;
    }
    private void NPCSaver(NPC _npc, ref int value, ref float music) {
        //Sets the Variables
        if (_npc._passedQuest) value = 3;
        else if (_npc._finishedQuest) value = 2;
        else if (!_npc._firstTime) value = 1;
        else value = 0;

        //Saves the music
        music = GameObject.FindGameObjectWithTag("LevelChanges").transform.Find("MusicManager").GetComponent<AudioSource>().time;
    }

    //Save Levels
    protected void SaveOutSide() {
        //Sets the Variables
        var par = GameObject.FindGameObjectWithTag("LevelChanges");
        _outsideMusic = par.transform.Find("MusicManager").GetComponent<AudioSource>().time;
    }
    protected void SaveTweakerHouse() {
        //Sets the Variables
        Transform par = GameObject.FindGameObjectWithTag("LevelChanges").transform;
        NPCSaver(par.Find("Tweaker").GetComponent<NPC>(), ref _tweakerHouseLevel, ref _tweakerHouseMusic);
    }
    protected void SaveGrandmaHouse() {
        //Sets the Variables
        Transform par = GameObject.FindGameObjectWithTag("LevelChanges").transform;
        NPCSaver(par.Find("Grandma").GetComponent<NPC>(), ref _grandmaHouseLevel, ref _grandmaHouseMusic);
    }
    protected void SaveMoverHouse() {
        //Sets the Variables
        Transform par = GameObject.FindGameObjectWithTag("LevelChanges").transform;
        NPCSaver(par.Find("Mover").GetComponent<NPC>(), ref _moverHouseLevel, ref _moverHouseMusic);
    }
}
