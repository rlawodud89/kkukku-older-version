using SQLite4Unity3d;
using System;

public class WorkRoom
{
    [PrimaryKey, AutoIncrement]
    public int workerID { get; set; }

    [NotNull]
    public int stamina { get; set; } = 0;

    public DateTime startTime { get; set; }

    public float workingPercent { get; set; } = 0f;

    public string workItem { get; set; } = null;

}