using SQLite4Unity3d;

public class User
{
    [PrimaryKey]
    public string name { get; set; }

    [NotNull]
    public int energy { get; set; }

    [NotNull]
    public int gold { get; set; }

    [NotNull]
    public int moonrock { get; set; }

    [NotNull]
    public float playTime { get; set; }

    [NotNull]
    public int designshopLevel { get; set; }

    [NotNull]
    public int itemshopLevel { get; set; }

    [NotNull]
    public int workroomLevel { get; set; }

    [NotNull]
    public string endScene { get; set; }
}