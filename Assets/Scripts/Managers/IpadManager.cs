using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IpadManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> _apps = new List<GameObject>();
    [SerializeField] private bool _currentFocus = true;

    //Properties
    protected List<GameObject> Apps {
        get {

            //Gathers all of the screens
            if (_apps.Count == 0) {
                for (int i = 0; i < transform.Find("BackDrop").childCount; i++)
                    _apps.Add(transform.Find("BackDrop").GetChild(i).gameObject);
            }
            
            //Returns the list of apps
            return _apps;
        }
    }
    private bool WindowChange {
        get {
            bool current = Application.isFocused;

            //Returns flase if it's the same value
            if (current == _currentFocus) return false;

            //Not Open
            if (!current) return _currentFocus = current;

            return _currentFocus = current;
        }
    }
    
    private void Start() {
        OpenApp(0);
    }
    private void FixedUpdate() {
        //Opens the correct window
        if (WindowChange) FocusChange();
    }
    private void OnEnable() {
        OpenApp(0);
    }

    //Opens the correct window
    public void OpenApp(int ID) {
        //Activates the Apps
        for (int i = 0; i < Apps.Count; i++) Apps[i].SetActive(i == ID);

        //Checks to make sure the Game Manager is not missing
        if(GameManager.Manager != null && GameManager.Manager.HUD != null)
            GameManager.Manager.HUD.PlaySound("App");

        //Refreshes the Inventory
        if (ID == 4) RefreshInventory(0);

        //If it opens on the Amazon Web Page App
        if (ID == 1) FocusChange();
    }

    [Header("Amazon")]
    [SerializeField] private Transform _amazonCheckOut;
    [SerializeField] private Item _currentItem;
    
    //Properties
    private Transform AmazonCheckOut {
        get => _amazonCheckOut != null ? _amazonCheckOut : _amazonCheckOut = transform.Find("BackDrop/Amazon/CheckOut");
    }

    //Functions
    public void OpenAmazon() {
        //Opens the URL
        Application.OpenURL("https://www.Amazon.com");
    }
    public void FocusChange() {
        //Initialize Variables
        string amazonDesc = GUIUtility.systemCopyBuffer;
        ClipboardParser.ParseClipboardText(amazonDesc, out string name, out float price);

        //Checks the Clipboard
        if (name == "" || price == 0) {
            Debug.Log("Clipboard not parsed correctly.");
            return;
        }

        //Updates the Amazon List
        UpdateAmazonProduct(name, price);
    }
    public void UpdateAmazonProduct(string name, float price) {
        //Updates the Product Name
        AmazonCheckOut.Find("ProductName").GetComponent<Text>().text = name;
        AmazonCheckOut.Find("ProductPrice").GetComponent<Text>().text = "$" + price.ToString("#0.00");

        float playerMoney = GameManager.Manager.PlayerMoney;
        AmazonCheckOut.Find("PlayerMoney").GetComponent<Text>().text = "$" + playerMoney.ToString("#0.00");
        AmazonCheckOut.Find("Subtract").GetComponent<Text>().text = "$" + price.ToString("#0.00");

        //Money Left
        playerMoney -= price;
        AmazonCheckOut.Find("MoneyLeft").GetComponent<Text>().text = "$" + playerMoney.ToString("#0.00");

        //Creates the Item
        _currentItem = new Item(name, price);
    }
    public void PurchaseItem() {
        //The player doesn't have enough money to buy this
        if (_currentItem.Price > GameManager.Manager.PlayerMoney) {
            OpenApp(2);
            return;
        }

        //Checks to make sure there is an item they are purchasing
        if(_currentItem.Name == "" || _currentItem.Price == 0) return;

        //If they do have enough money
        GameManager.Manager.HUD.PlaySound("Cash");
        GameManager.Manager.PlayerMoney -= _currentItem.Price;
        GameManager.Manager.Inventory.Add(new Item(_currentItem.Name, _currentItem.Price));
        GameManager.Manager.HUD.UpdateMoney(GameManager.Manager.PlayerMoney);

        //Opens up the Purchase Screen
        OpenApp(3);
    }

    [Header("Inventory")]
    [SerializeField] private Transform _itemHolder;
    [SerializeField] private int _pageNumber;

    //Properties
    private Transform ItemHolder {
        get => _itemHolder != null ? _itemHolder : _itemHolder = transform.Find("BackDrop/Notepad/ItemHolder");
    }

    //Functions
    public void RefreshInventory(int page) {
        //Intialize Variables
        _pageNumber = page;
        var startID = _pageNumber * 5;

        //Enters the Information
        for (int i = 0; i < 5; i++) {
            //Fills in the item if there is one in the Inventory
            if (GameManager.Manager.Inventory.Count > (startID + i)) {
                //Adds it to the list
                ItemHolder.Find("Item_" + i).gameObject.SetActive(true);
                ItemHolder.Find("Item_" + i + "/ProductName").GetComponent<Text>().text = GameManager.Manager.Inventory[startID + i].Name;
                ItemHolder.Find("Item_" + i + "/ProductPrice").GetComponent<Text>().text = "$" + GameManager.Manager.Inventory[startID + i].Price.ToString("#0.00");

                //Checks to see if it's equipped
                if(GameManager.Manager.HeldItem != null)
                FixEquip(i, GameManager.Manager.Inventory[startID + i].Name == GameManager.Manager.HeldItem.Name);

                continue;
            }

            //Disables the Inventory Item Slot
            ItemHolder.Find("Item_" + i).gameObject.SetActive(false);
        }

        //Sets the previous and next buttons
        ItemHolder.parent.Find("Next").GetComponent<Button>().interactable = GameManager.Manager.Inventory.Count > (page + 1) * 5;
        ItemHolder.parent.Find("Previous").GetComponent<Button>().interactable = (page != 0);
        ItemHolder.parent.Find("PageNumber").GetComponent<Text>().text = (_pageNumber + 1) + "/" + (((GameManager.Manager.Inventory.Count - (GameManager.Manager.Inventory.Count % 5)) / 5) + 1);

    }
    public void FlipPage(int direction) {
        RefreshInventory(_pageNumber + direction);
        GameManager.Manager.HUD.PlaySound("Switch");
    }
    public void EquipItem(int buttonID) {
        //Initialize Variables
        var item = GameManager.Manager.Inventory[(_pageNumber * 5) + buttonID];

        //Unequipes the Item if double clicking on the button
        if (GameManager.Manager.HeldItem != null && GameManager.Manager.HeldItem.Name == item.Name) {
            GameManager.Manager.HeldItem = null;
            FixEquip(buttonID, false);
            return;
        }

        //Equipped an Item
        GameManager.Manager.HeldItem = new Item(item.Name, item.Price);
        FixEquip(buttonID, true);
    }
    private void FixEquip(int id, bool active) {
        ItemHolder.Find("Item_" + id + "/Equip/Text").GetComponent<Text>().text = active ? "X" : "-";
        ItemHolder.Find("Item_" + id + "/Equip").GetComponent<Image>().color = active ? Color.green : Color.cyan;
    }
}
