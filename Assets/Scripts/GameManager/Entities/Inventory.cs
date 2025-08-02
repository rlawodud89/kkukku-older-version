using SQLite4Unity3d;

public class Inventory
{
    [PrimaryKey]
    public string CompositeKey => itemName + "_" + itemType;
    //PK: (itemName, itemType)

    public string itemName {  get; set; }

    public ItemType itemType { get; set; }

    [NotNull]
    public int count { get; set; }

}