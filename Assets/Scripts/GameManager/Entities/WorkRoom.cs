using SQLite4Unity3d;

public class WorkRoom
{
    [PrimaryKey, AutoIncrement]
    public int workerID { get; set; }

    [NotNull]
    public int stamina { get; set; }

    [NotNull]
    public bool working { get; set; }

    public int workingPercent { get; set; }

    public string workItem { get; set; }

}