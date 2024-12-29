using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResponseFunctions : ResponseManager{

    //Start Functions
    public void Start() {
        ResponseUnits = new GameObject[100];
        ResponseUnits[0] = StartResponse;
    }

    //Unit Controller
    public void ClearUnits() { 
        
    }
    public void AddUnit() {
        //Creates the button
        var obj = Instantiate(Resources.Load<GameObject>("UI/ResponseTemplate"));
        obj.transform.SetParent(WorkSpace);
        obj.GetComponent<RectTransform>().position = new Vector3(250, 250, 0);

        //Initializes the Variables
        int currentID = 0;
        for (int i = 0; i < ResponseUnits.Length; i++) {
            //Checks to see if it is an empty spot
            if (ResponseUnits[i] == null) break;
            
            //Increments the ID
            currentID++;
        }

        //Hooks up the buttons
        obj.transform.Find("Close").GetComponent<Button>().onClick.AddListener(() => RemoveUnit(currentID));
        obj.transform.Find("Drag").GetComponent<Button>().onClick.AddListener(() => Drag(currentID));

        //Adds the object to the list
        ResponseUnits[currentID] = obj;
    }

    //Button Functions
    public void RemoveUnit(int ID) {
        //Destroys the Unit
        Destroy(ResponseUnits[ID]);
        ResponseUnits[ID] = null;
    }
    public void Drag(int ID) {
        //Starts the drag
        if (_currentID == -1) _currentID = ID;
        else _currentID = -1;
    }

    //Movement
    private Vector3 _oldPos, _oldMouse;
    private int _currentID = -1;
    public void Update() {
        //Moving Around the Scene
        if (Input.GetMouseButtonDown(2)) {
            _oldPos = WorkSpace.GetComponent<RectTransform>().position;
            _oldMouse = Input.mousePosition;
        }
        if (Input.GetMouseButton(2)) {
            WorkSpace.GetComponent<RectTransform>().position = _oldPos + (Input.mousePosition - _oldMouse);
        }

        //Drags the Item
        if (_currentID != -1) {
            ResponseUnits[_currentID].GetComponent<RectTransform>().position = Input.mousePosition + new Vector3(125, -80, 0);
        }
    }

}
