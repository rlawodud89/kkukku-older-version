using SQLite4Unity3d;

public class Interior
{
    [NotNull]
    public string interiorName {  get; set; }

    [NotNull]
    public InteriorType interiorType { get; set; }

    [NotNull]
    public bool isSet { get; set; }

    public int ID { get; set; }

    public float x { get; set; }

    public float y { get; set; }
}