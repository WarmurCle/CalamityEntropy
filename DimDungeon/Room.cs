using Microsoft.Xna.Framework;

namespace CalamityEntropy.DimDungeon;

public class Room
{
    public Rectangle Bounds { get; set; }

    public Room()
    {
        
    }
    
    public Room(Rectangle bounds)
    {
        Bounds = bounds;
    }
}