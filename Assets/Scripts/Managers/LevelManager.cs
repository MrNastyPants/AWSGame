using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelManager{

    [Header("Level Manager Stats")]
    [SerializeField] private bool _isLevelLoaded = false;
    [SerializeField] private string _currentScene = "Outside";

    [Header("Tweaker's House")]
    [SerializeField] private int _tweakerHouseLevel = 0;

    //Public Functions
    public void Init() { 
        //Initializes the Level
        _currentScene = SceneManager.GetActiveScene().name;
    }

    //Changinge Scenes
    public void ChangeScene(int scene) {
        //Changes the Scene of the Game
        _isLevelLoaded = false;
        SceneManager.LoadScene(scene);
    }
    public void ChangeScene(string scene) {
        //Changes the Scene of the Game
        _isLevelLoaded = false;
        SceneManager.LoadScene(scene);
    }

    //Loading in the Scene
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        //Loads the Scenes 
        switch (scene.name) {
            case "Outside": LoadOutSide(); break;
            default: break;
        }

        //Sets the Variables
        _currentScene = scene.name;
    }
    public void PlacePlayer(Vector3 position, Vector3 lookDirection) { 
        //Sets the position and rotation
        GameManager.Manager.Player.transform.position = position;
        GameManager.Manager.Player.transform.Find("Mesh").transform.LookAt(GameManager.Manager.Player.transform.position + lookDirection);
    }

    //Load Levels
    protected void LoadOutSide() {
        //Sets the Position of the Player
        switch (_currentScene) {
            case "Tweaker's House": PlacePlayer(new Vector3(0, 0, 0), new Vector3(0, 0, -1)); break;
            case "Grandma's House": PlacePlayer(new Vector3(7, 0, -6), new Vector3(-1, 0, 0)); break;
        }

        //Turns on the Tweaker if the player failed him.
    }
}
