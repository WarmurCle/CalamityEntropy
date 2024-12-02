using System.Collections.Generic;

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
    public Room room;
    public RoomGenAction(Room room)
    {
        this.room = room;
    }
}