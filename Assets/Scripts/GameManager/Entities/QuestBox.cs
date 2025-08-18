using SQLite4Unity3d;

public class QuestBox
{
    [PrimaryKey]
    public string questName { get; set; }

    [NotNull]
    public bool isCompleted { get; set; } = false;

    [NotNull]
    public int process { get; set; } = 0;

    [NotNull]
    public bool getReward { get; set; } = false;
}