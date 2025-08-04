using SQLite4Unity3d;

public class Inventory
{
    [PrimaryKey]
    public string itemName {  get; set; }

    [NotNull]
    public ItemType itemType { get; set; }

    [NotNull]
    public int count { get; set; }

}