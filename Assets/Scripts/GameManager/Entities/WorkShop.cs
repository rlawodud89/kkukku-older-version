using SQLite4Unity3d;

public class WorkShop
{
    [PrimaryKey, AutoIncrement]
    public int tableID { get; set; }

    [NotNull]
    public string tableName { get; set; }

    [NotNull]
    public int x { get; set; }

    [NotNull]
    public int y { get; set; }
}