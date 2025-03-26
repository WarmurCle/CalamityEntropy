namespace CalamityEntropy.Content.DimDungeon;

public class Corridor
{
    public CorridorMetadata Metadata { get; set; }
    public Rectangle Bounds { get; set; }
    public Direction Direction { get; set; }

    public Room RoomA { get; set; }
    public Room RoomB { get; set; }

    public Corridor(Rectangle bounds, Direction direction, CorridorMetadata metadata)
    {
        Bounds = bounds;
        Direction = direction;
        Metadata = metadata;
    }
}