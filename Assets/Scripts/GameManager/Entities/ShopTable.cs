using SQLite4Unity3d;

public class ShopTable
{
    // 실제로는 PK: (tableID, blanketName)

    [NotNull]
    public int tableID { get; set; }

    [NotNull]
    public string blanketName { get; set; }

    [NotNull]
    public int count { get; set; }

}