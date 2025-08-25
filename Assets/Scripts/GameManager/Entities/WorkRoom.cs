using SQLite4Unity3d;

public class WorkRoom
{
    [PrimaryKey, AutoIncrement]
    public int workerID { get; set; }

    [NotNull]
    public int stamina { get; set; } = 0;

    [NotNull]
    public bool working { get; set; } = false;

    public float workingPercent { get; set; } = 0f;

    public string workItem { get; set; } = null;

}