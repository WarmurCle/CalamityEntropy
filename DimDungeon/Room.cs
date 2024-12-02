namespace CalamityEntropy.DimDungeon;

public class Room
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Room(int width, int height)
    {
        Width = width;
        Height = height;
    }
}