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
    public int todayEnergy { get; set; }

    [NotNull]
    public int todayGold { get; set; }

    [NotNull]
    public int todayMoonrock { get; set; }

    [NotNull]
    public int designshopLevel { get; set; }

    [NotNull]
    public int itemshopLevel { get; set; }

    [NotNull]
    public int loomLevel { get; set; }

    [NotNull]
    public int fillerLevel { get; set; }

    [NotNull]
    public int decoLevel { get; set; }

    [NotNull]
    public string endScene { get; set; }

    [NotNull]
    public bool isOpen { get; set; }
}