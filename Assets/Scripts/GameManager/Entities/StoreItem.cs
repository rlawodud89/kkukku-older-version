using SQLite4Unity3d;

public class StoreItem
{
    [NotNull]
    public StoreType storeType { get; set; }

    [NotNull]
    public string itemName { get; set; }
}