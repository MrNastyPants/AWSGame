using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelManager{

    //[Header("Level Manager Stats")]

    //Public Functions
    public void Init() { 
        //Initializes the Level
    }

    public void ChangeScene(int scene) {
        //Changes the Scene of the Game
        SceneManager.LoadScene(scene);
    }
}
