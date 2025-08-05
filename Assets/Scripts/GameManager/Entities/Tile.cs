using SQLite4Unity3d;

public class Tile
{
    [PrimaryKey]
    public TilePosType tilePos { get; set; } 

    [NotNull]
    public string tileName { get; set; }
}