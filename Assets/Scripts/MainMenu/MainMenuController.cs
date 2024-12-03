using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> _menues = new List<GameObject>();
    [SerializeField] private int menuState = 0;

    //Properties
    private List<GameObject> Menues {
        get {
            //Fills up the List based on the Children of this object
            if (_menues.Count == 0)                 
                for(int i = 0; i < transform.childCount; i++) _menues.Add(transform.GetChild(i).gameObject);

            //Returns the List
            return _menues;
        }
    }

    private void Start() {
        ChangeMenu(0);
    }
    private void Update() {
        //On any key down exits the title screen
        if (menuState == 0 && Input.anyKeyDown) ChangeMenu(1);
    }

    public void ChangeMenu(int menu) {
        //Deactives un-wanted menues and opens the wanted menu
        for (int i = 0; i < Menues.Count; i++) Menues[i].SetActive(i == menu);
        menuState = menu;
    }
    public void StartGame() {
        //Loads the scene if the Game Manager Exists
        try {
            GameManager.Manager.LevelManager.ChangeScene(1);
        }
        
        //Creates a Game Manager if it does not exist
        catch {
            GameManager.Manager = new GameObject("GameManager").AddComponent<GameManager>();
            GameManager.Manager.LevelManager.ChangeScene(1);
        }
    }
}
