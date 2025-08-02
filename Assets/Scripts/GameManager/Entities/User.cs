using SQLite4Unity3d;

public class User
{
    [PrimaryKey]
    public string name { get; set; }

    [NotNull]
    public int energyLevel {  get; set; }

    [NotNull]
    public int energyPercent { get; set; }

    [NotNull]
    public int gold {  get; set; }

    [NotNull]
    public int moonrock { get; set; }

    [NotNull]
    public float playTime { get; set; }
    
}