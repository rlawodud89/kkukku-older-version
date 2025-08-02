using SQLite4Unity3d;

public class QuestBox
{
    [PrimaryKey]
    public string questName { get; set; }

    [NotNull]
    public bool finish { get; set; }
}