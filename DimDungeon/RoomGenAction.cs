namespace CalamityEntropy.DimDungeon;

public enum Direction
{
    Left,
    Up,
    Down,
    Right
}

public record RoomGenAction
{
    public Corridor Corridor { get; init; }
    public Direction Direction { get; init; }
    public Room NextRoom { get; init; }

    public RoomGenAction(Corridor corridor, Direction direction, Room nextRoom)
    {
        Corridor = corridor;
        Direction = direction;
        NextRoom = nextRoom;
    }
}