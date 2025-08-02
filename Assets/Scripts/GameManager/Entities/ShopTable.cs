using SQLite4Unity3d;
using static UnityEditor.Progress;

public class ShopTable
{
    [PrimaryKey]
    public string CompositeKey => tableID + "_" + blanketName;
    //PK: (tableID, blanketName)
    
    public int tableID { get; set; }
    
    public string blanketName { get; set; }

    [NotNull]
    public int count { get; set; }

}