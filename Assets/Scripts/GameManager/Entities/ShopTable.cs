using SQLite4Unity3d;

public class ShopTable
{
    [PrimaryKey, AutoIncrement]   
    public int tableID { get; set; }
    
    public string blanketName { get; set; }

    [NotNull]
    public int count { get; set; }

}