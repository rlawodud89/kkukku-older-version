using SQLite4Unity3d;

public class QuestBox
{
    [PrimaryKey]
    public string questName { get; set; }

    [NotNull]
    public bool complete { get; set; }

    [NotNull]
    public int progress { get; set; }

    [NotNull]
    public bool getreward { get; set; }
}