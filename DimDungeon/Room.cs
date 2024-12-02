using System.Collections.Generic;

namespace CalamityEntropy.DimDungeon;

public class Room
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Corridor> Corridor { get; init; }

    public Room(int width, int height, List<Corridor> corridors)
    {
        Width = width;
        Height = height;
        Corridor = corridors;
    }
}