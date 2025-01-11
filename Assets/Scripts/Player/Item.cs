[System.Serializable]
public class Item{
    //Initialize Variables
    public string Name = "";
    public float Price = 0;

    //Functions
    public Item() { }
    public Item(string name, float price) {
        Name = name;
        Price = price;
    }
}
