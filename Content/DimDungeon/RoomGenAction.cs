namespace CalamityEntropy.Content.DimDungeon;

public enum Direction
{
    Left,
    Up,
    Down,
    Right
}

public record RoomGenAction
{
    public Direction Direction { get; init; }
    public CorridorMetadata Corridor { get; init; }
    public RoomMetadata NextRoom { get; init; }
    public RoomGenAction(CorridorMetadata corridor, Direction direction, RoomMetadata nextRoom)
    {
        Direction = direction;
        Corridor = corridor;
        NextRoom = nextRoom;
    }
}