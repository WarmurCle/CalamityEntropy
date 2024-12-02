namespace CalamityEntropy.DimDungeon;

public class Corridor
{
    public int Width { get; set; }
    public int Length { get; set; }
    public Room NextRoom { get; init; }
    public Direction Direction { get; init; }
    public Corridor(int width, int length, Room nextRoom, Direction direction)
    {
        Width = width;
        Length = length;
        NextRoom = nextRoom;
        Direction = direction;
    }
}