using System.Collections.Generic;

namespace CalamityEntropy.DimDungeon;

public class RoomMetadata
{
    public int Width { get; set; }
    public int Height { get; set; }

    public RoomMetadata(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public (int x, int y) GetOffset(int corridorX, int corridorY, int corridorWidth, int corridorLength, Direction direction)
    {
        int offsetX = corridorX;
        int offsetY = corridorY;

        if (direction.IsHorizontal())
        {
            offsetY -= Height / 2;
        }
        else
        {
            offsetX -= Width / 2;
            
        }
        
        switch (direction)
        {
            case Direction.Up:
                offsetY -= Height;
                break;
            case Direction.Down:
                offsetY += corridorLength;
                break;
            case Direction.Left:
                offsetX -= Width;
                break;
            case Direction.Right:
                offsetX += corridorLength;
                break;
        }
        
        return (offsetX, offsetY);
    }
}