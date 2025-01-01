using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResponseFunctions : ResponseManager {

    //Start Functions
    public void Awake() {
        ResponseUnits = new GameObject[100];
        OutPutResponse = new Dialogue[100];
    }

    //Unit Controller
    public void ClearUnits() {
        //Deletes Everything
        for (int i = ResponseUnits.Length - 1; i >= 0; i--) {
            Destroy(ResponseUnits[i]);
            ResponseUnits[i] = null;
            OutPutResponse[i] = new Dialogue();
        }
    }
    public void Save() {
        //Exits if the file doesn't exist
        if (_file == null) {
            Debug.Log("There is no file to save to.");
            return;
        }

        var temp = new List<Dialogue>();
        var tempPositions = new List<Vector2>();

        //Adds everything to the file
        for (int i = 0; i < OutPutResponse.Length; i++) {
            if (ResponseUnits[i] == null) continue;

            //Adds the Value
            temp.Add(OutPutResponse[i]);
            tempPositions.Add(ResponseUnits[i].GetComponent<RectTransform>().position);
        }

        //Saves the File
        _file.OutPutResponse = new List<Dialogue>(temp);
        _file.Positions = new List<Vector2>(tempPositions);
    }
    public void Load() {
        //Exits if there is no file to load from
        if (_file == null) {
            Debug.Log("There is no file to load.");
            return;
        }

        //Removes Everything First and then starts loading the objects
        ClearUnits();

        //Starts the Loading
        for(int i = 0; i < _file.OutPutResponse.Count; i++) {
            //Adds the Unit
            AddUnit(false);

            //Sets the Dialogue
            OutPutResponse[i] = _file.OutPutResponse[i];

            //Copies the Text
            ResponseUnits[i].transform.Find("Name/NameField").GetComponent<InputField>().text = OutPutResponse[i].Name;
            ResponseUnits[i].transform.Find("Dialogue").GetComponent<InputField>().text = OutPutResponse[i].Text;

            //Copies the Responses
            ResponseUnits[i].transform.Find("Responses/Response1").GetComponent<InputField>().text = OutPutResponse[i].Response1;
            ResponseUnits[i].transform.Find("Responses/Response2").GetComponent<InputField>().text = OutPutResponse[i].Response2;
            ResponseUnits[i].transform.Find("Responses/Response3").GetComponent<InputField>().text = OutPutResponse[i].Response3;

            //Refreshes the Responses
            RefreshResponseMenu(i);

            //Final
            ResponseUnits[i].transform.Find("Money/MoneyField").GetComponent<InputField>().text= OutPutResponse[i].giveMoney.ToString();
            ResponseUnits[i].transform.Find("End/End").GetComponent<Toggle>().isOn = OutPutResponse[i].isEnd;
            ResponseUnits[i].transform.Find("AIResponse/AI").GetComponent<Toggle>().isOn = OutPutResponse[i].AIResponse;

            //Sets the Positions
            ResponseUnits[i].GetComponent<RectTransform>().position = _file.Positions[i];

        }

        //Sets up the Lines
        for (int i = 0; i < _file.OutPutResponse.Count; i++) 
            for(int j = 0; j < OutPutResponse[i].Next.Count; j++)
                Attach(i, OutPutResponse[i].Next[j].y, OutPutResponse[i].Next[j].x);
        
    }
    public void AddUnit(bool autoLine = true) {
        //Creates the button
        var obj = Instantiate(Resources.Load<GameObject>("UI/ResponseTemplate"));
        obj.transform.SetParent(WorkSpace);

        //Initializes the Variables
        int currentID = 0;
        for (int i = 0; i < ResponseUnits.Length; i++) {
            //Checks to see if it is an empty spot
            if (ResponseUnits[i] == null) break;

            //Increments the ID
            currentID++;
        }

        //Hooks up the buttons and Sets the position
        SetUpUnit(obj, currentID);
        var prev = currentID - 1;
        obj.GetComponent<RectTransform>().position = currentID != 0 ? ResponseUnits[prev].GetComponent<RectTransform>().position + new Vector3(450, 0, 0) : new Vector3(0, 0, 0);

        //Adds the object to the list
        ResponseUnits[currentID] = obj;

        //Exits to not create lines Automatically
        if (!autoLine || currentID == 0 || ResponseUnits[prev].transform.Find("Lines").childCount != 0) return;

        //Variables
        int spot = ResponseUnits[prev].transform.Find("Responses").gameObject.activeInHierarchy ? 1 : 0;

        //Makes a Line to the Previous
        OutPutResponse[prev].Next.Add(new Vector2Int(currentID, spot));
        OutPutResponse[currentID].Prior.Add(new Vector2Int(prev, spot));

        //Creates a new Line
        var temp = Instantiate(Resources.Load<GameObject>("UI/Line"), ResponseUnits[prev].transform.Find("Lines"));
        temp.name = spot.ToString();

        //Sets the Line
        FollowLines(prev, spot, currentID);
        ChangeOption(prev, spot, currentID);
    }
    private void SetUpUnit(GameObject unit, int ID) {
        //Sets up the unit
        unit.transform.Find("Close").GetComponent<Button>().onClick.AddListener(() => RemoveUnit(ID));
        unit.transform.Find("Drag").GetComponent<Button>().onClick.AddListener(() => Drag(ID));
        OutPutResponse[ID] = new Dialogue();

        //Hides the Responses and Sets up the buttons
        unit.transform.Find("Responses").gameObject.SetActive(false);
        unit.transform.Find("Main_Attach").GetComponent<Button>().onClick.AddListener(() => Attach(ID, 0));
        unit.transform.Find("Responses/Response1/Attach").GetComponent<Button>().onClick.AddListener(() => Attach(ID, 1));
        unit.transform.Find("Responses/Response2/Attach").GetComponent<Button>().onClick.AddListener(() => Attach(ID, 2));
        unit.transform.Find("Responses/Response3/Attach").GetComponent<Button>().onClick.AddListener(() => Attach(ID, 3));

        //Responses Buttons
        unit.transform.Find("AddOption").GetComponent<Button>().onClick.AddListener(() => AddResponseOption(ID));
        unit.transform.Find("RemoveOption").GetComponent<Button>().onClick.AddListener(() => RemoveResponseOption(ID));

        //Text Options
        unit.transform.Find("Name/NameField").GetComponent<InputField>().onEndEdit.AddListener((text) => NameChange(text, ID));
        unit.transform.Find("Dialogue").GetComponent<InputField>().onEndEdit.AddListener((text) => DialogueChange(text, ID));
        unit.transform.Find("Money/MoneyField").GetComponent<InputField>().onEndEdit.AddListener((text) => MoneyChange(text, ID));
        unit.transform.Find("End/End").GetComponent<Toggle>().onValueChanged.AddListener((value) => OnEndChange(value, ID));
        unit.transform.Find("AIResponse/AI").GetComponent<Toggle>().onValueChanged.AddListener((value) => OnAIChange(value, ID));


        //Response Text Options
        unit.transform.Find("Responses/Response1").GetComponent<InputField>().onEndEdit.AddListener((text) => ResponseChange(text, ID, 0));
        unit.transform.Find("Responses/Response2").GetComponent<InputField>().onEndEdit.AddListener((text) => ResponseChange(text, ID, 1));
        unit.transform.Find("Responses/Response3").GetComponent<InputField>().onEndEdit.AddListener((text) => ResponseChange(text, ID, 2));
    }

    //Responses
    public void Attach(int ID, int output) {
        //Extis if it has clicked an attachment already
        if (_attachingID != -1) {
            //Resets the attaching values
            _attachingID = -1;
            Destroy(_currentLine);
            return;
        }

        //Gets the Line or Creates a new one
        if (ResponseUnits[ID].transform.Find("Lines/" + output) != null) _currentLine = ResponseUnits[ID].transform.Find("Lines/" + output).gameObject;
        else {
            _currentLine = Instantiate(Resources.Load<GameObject>("UI/Line"), ResponseUnits[ID].transform.Find("Lines"));
            _currentLine.name = output.ToString();
        }
        
        //Sets the ID if it's starting here
        _attachingID = ID;
        _attachingPlace = output;

        //Clears the Values from this already
        DeleteConnectionsTo(ID, output);

        //Changes the Final Numbers
        ChangeOption(ID, output, 0);
    }
    public void Attach(int ID, int output, int targetID) {
        //Create a Line
        var line = Instantiate(Resources.Load<GameObject>("UI/Line"), ResponseUnits[ID].transform.Find("Lines"));
        line.name = output.ToString();
        FollowLines(ID, output, targetID);
    }
    private string SelectAttachment(int value) {
        //Initialize Variables
        var temp = "Main_Attach";
        
        //Selects which one
        switch (value) {
            case 0: temp = "Main_Attach"; break;
            case 1: temp = "Responses/Response1/Attach"; break;
            case 2: temp = "Responses/Response2/Attach"; break;
            case 3: temp = "Responses/Response3/Attach"; break;
        }

        //Returns the number
        return temp;
    }
    public void AddResponseOption(int ID) {
        //Variables
        bool stepOne = ResponseUnits[ID].transform.Find("Responses").gameObject.activeInHierarchy;
        bool StepTwo = ResponseUnits[ID].transform.Find("Responses/Response2").gameObject.activeInHierarchy;

        //Enables and Disables the Responses
        ResponseUnits[ID].transform.Find("Responses/Response3").gameObject.SetActive(StepTwo);
        ResponseUnits[ID].transform.Find("Responses/Response2").gameObject.SetActive(stepOne);
        ResponseUnits[ID].transform.Find("Responses/Response1").gameObject.SetActive(true);

        ResponseUnits[ID].transform.Find("Main_Attach").gameObject.SetActive(false);
        ResponseUnits[ID].transform.Find("Responses").gameObject.SetActive(true);


        //Initializes the Values
        SwitchConnections(ID, true);
    }
    public void RefreshResponseMenu(int ID) {
        //Variables
        bool StepOne = OutPutResponse[ID].Response1 != "";
        bool StepTwo = OutPutResponse[ID].Response2 != "";
        bool StepThree = OutPutResponse[ID].Response3 != "";

        //Enables and Disables the Responses
        ResponseUnits[ID].transform.Find("Responses/Response1").gameObject.SetActive(StepOne);
        ResponseUnits[ID].transform.Find("Responses/Response2").gameObject.SetActive(StepTwo);
        ResponseUnits[ID].transform.Find("Responses/Response3").gameObject.SetActive(StepThree);

        ResponseUnits[ID].transform.Find("Main_Attach").gameObject.SetActive(!StepOne);
        ResponseUnits[ID].transform.Find("Responses").gameObject.SetActive(StepOne);
    }
    public void RemoveResponseOption(int ID) {

        //Step 1
        if (ResponseUnits[ID].transform.Find("Responses/Response3").gameObject.activeInHierarchy) {
            ResponseUnits[ID].transform.Find("Responses/Response3").gameObject.SetActive(false);
            ResponseUnits[ID].transform.Find("Responses/Response3").GetComponent<InputField>().text = "";
            OutPutResponse[ID].Response3 = "";
            ChangeOption(ID, 3, 0);
            return;
        }

        //Step 2
        if (ResponseUnits[ID].transform.Find("Responses/Response2").gameObject.activeInHierarchy) {
            ResponseUnits[ID].transform.Find("Responses/Response2").gameObject.SetActive(false);
            ResponseUnits[ID].transform.Find("Responses/Response2").GetComponent<InputField>().text = "";
            OutPutResponse[ID].Response2 = "";
            ChangeOption(ID, 2, 0);
            return;
        }

        //Step 3
        ResponseUnits[ID].transform.Find("Responses/Response1").gameObject.SetActive(false);
        ResponseUnits[ID].transform.Find("Responses/Response1").GetComponent<InputField>().text = "";
        OutPutResponse[ID].Response1 = "";
        ChangeOption(ID, 1, 0);
        ResponseUnits[ID].transform.Find("Main_Attach").gameObject.SetActive(true);
        ResponseUnits[ID].transform.Find("Responses").gameObject.SetActive(false);

        //Fixes the Connections
        SwitchConnections(ID, false);
    }
    private void ChangeOption(int ID, int output, int value) {
        if (output == 0 || output == 1) OutPutResponse[ID].Option1 = value;
        if (output == 2) OutPutResponse[ID].Option2 = value;
        if (output == 3) OutPutResponse[ID].Option3 = value;
    }

    //Connections
    private void SwitchConnections(int ID, bool toOne) {
        //Initialize Variables
        var FirstIndex = -1;
        var newConState = toOne ? 1 : 0;
        FirstIndex = OutPutResponse[ID].Next.FindIndex((x) => x.y == (toOne ? 0 : 1));

        //Exits if nothing was found
        if (FirstIndex == -1) return;
        OutPutResponse[ID].Next[FirstIndex] = new Vector2Int(OutPutResponse[ID].Next[FirstIndex].x, newConState);

        //Fixes the Next ID of the Prior
        var NextIndex = OutPutResponse[OutPutResponse[ID].Next[FirstIndex].x].Prior.FindIndex((x) => x.x == ID);
        OutPutResponse[OutPutResponse[ID].Next[FirstIndex].x].Prior[NextIndex] = new Vector2Int(ID, newConState);

        //Changes the Name and ID of the Line
        ResponseUnits[ID].transform.Find("Lines/" + (toOne ? "0" : "1")).name = toOne ? "1" : "0";
        FollowLines(ID, newConState, OutPutResponse[ID].Next[FirstIndex].x);
    }
    private void DeleteAllConnections(int ID) {
        //Delete Forward
        DeleteConnectionsTo(ID, 0);
        DeleteConnectionsTo(ID, 1);
        DeleteConnectionsTo(ID, 2);
        DeleteConnectionsTo(ID, 3);
        
        //Delete Backward
        for(int i = OutPutResponse[ID].Prior.Count -1; i >= 0; i--) {
            //Removes the Connection
            var nextUnit = OutPutResponse[OutPutResponse[ID].Prior[i].x].Next.FindIndex((x) => x.x == ID);

            //Changes the Value of the Previous
            ChangeOption(OutPutResponse[ID].Prior[i].x, OutPutResponse[OutPutResponse[ID].Prior[i].x].Next[nextUnit].y, 0);

            //Deletes the Line
            Destroy(ResponseUnits[OutPutResponse[ID].Prior[i].x].transform.Find("Lines/" + OutPutResponse[OutPutResponse[ID].Prior[i].x].Next[nextUnit].y).gameObject);
            OutPutResponse[OutPutResponse[ID].Prior[i].x].Next.RemoveAt(nextUnit);
        }

        //Deletes itself
        OutPutResponse[ID].Next.Clear();
        OutPutResponse[ID].Prior.Clear();
    }
    private void DeleteConnectionsTo(int ID, int spot) {
        //Initialize Variables
        var nextUnit = OutPutResponse[ID].Next.FindIndex((x) => x.y == spot);

        //Exits if there is no next Unit
        if (nextUnit == -1) return;

        //Removes the Connections instance to this
        var prior = OutPutResponse[OutPutResponse[ID].Next[nextUnit].x].Prior.FindIndex((x) => x == new Vector2Int(ID, spot));
        OutPutResponse[OutPutResponse[ID].Next[nextUnit].x].Prior.RemoveAt(prior);

        //Removes the Current Connection
        OutPutResponse[ID].Next.RemoveAt(nextUnit);
    }

    //Text Functions
    public void NameChange(string text, int ID) {
        OutPutResponse[ID].Name = text;
    }
    public void DialogueChange(string text, int ID) {
        OutPutResponse[ID].Text = text;
    }
    public void MoneyChange(string text, int ID) {
        OutPutResponse[ID].giveMoney = float.Parse(text);
    }
    public void ResponseChange(string text, int ID, int responseID) {
        if (responseID == 0) OutPutResponse[ID].Response1 = text;
        if (responseID == 1) OutPutResponse[ID].Response2 = text;
        if (responseID == 2) OutPutResponse[ID].Response3 = text;
    }

    //Toggle Options
    public void OnEndChange(bool value, int ID) {
        OutPutResponse[ID].isEnd = value;
    }
    public void OnAIChange(bool value, int ID) {
        OutPutResponse[ID].AIResponse = value;
    }

    //Button Functions
    public void RemoveUnit(int ID) {
        //Removes all the connections it had
        DeleteAllConnections(ID);

        //Destroys the Unit
        Destroy(ResponseUnits[ID]);
        ResponseUnits[ID] = null;
        OutPutResponse[ID] = new Dialogue();
    }
    public void Drag(int ID) {
        //Checks to see if this was the target for an attach
        if (_attachingID != -1) {

            //Changes this one
            OutPutResponse[_attachingID].Next.Add(new Vector2Int(ID, _attachingPlace));
            OutPutResponse[ID].Prior.Add(new Vector2Int(_attachingID, _attachingPlace));

            //Changes the value
            ChangeOption(_attachingID, _attachingPlace, ID);

            //Resets
            FollowLines(_attachingID, _attachingPlace, ID);
            _attachingID = -1;

            return;
        }

        //Starts the drag
        if (_currentID == -1) _currentID = ID;
        else _currentID = -1;
    }

    //Movement
    [Header("Movement")]
    [SerializeField] private Vector3 _oldPos, _oldMouse;
    [SerializeField] private int _currentID = -1;

    [Header("Attaching")]
    [SerializeField] private int _attachingID = -1, _attachingPlace = 0;
    [SerializeField] private GameObject _currentLine = null;
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
            //Moves the Unit Around
            ResponseUnits[_currentID].GetComponent<RectTransform>().position = Input.mousePosition + new Vector3(125, -80, 0);

            //Updates the lines behind
            for (int i = 0; i < OutPutResponse[_currentID].Prior.Count; i++) 
                FollowLines(OutPutResponse[_currentID].Prior[i].x, OutPutResponse[_currentID].Prior[i].y, _currentID);

            //Update the Lines in front
            for (int i = 0; i < OutPutResponse[_currentID].Next.Count; i++) 
                FollowLines(_currentID, OutPutResponse[_currentID].Next[i].y, OutPutResponse[_currentID].Next[i].x);
        }

        //Checks the attaching number
        if (_attachingID != -1) { 
            var start = ResponseUnits[_attachingID].transform.Find(SelectAttachment(_attachingPlace)).GetComponent<RectTransform>().position;
            var end = Input.mousePosition;
            UpdateLine(start, end);
        }
    }

    //Line Movement
    public void FollowLines(int Prior, int output, int Current) {
        //Sets up the Variables
        var line = ResponseUnits[Prior].transform.Find("Lines/" + output).gameObject;
        var start = ResponseUnits[Prior].transform.Find(SelectAttachment(output)).GetComponent<RectTransform>().position;
        var end = ResponseUnits[Current].transform.Find("Drag").GetComponent<RectTransform>().position;
        UpdateLine(start, end, line);
    }
    public void UpdateLine(Vector2 startPos, Vector2 endPos, GameObject line = null) {
        //There is no current Line Active
        if (line == null) line = _currentLine;
        if (line == null) return;

        //Gets the Rotation
        var dir = endPos - startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //Sets everything
        line.GetComponent<RectTransform>().position = (startPos + endPos) / 2;
        line.GetComponent<RectTransform>().rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        line.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector2.Distance(startPos, endPos), 5);

    }

}
