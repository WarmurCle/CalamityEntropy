using Microsoft.Xna.Framework;

namespace CalamityEntropy.DimDungeon;

public class CorridorMetadata
{
    public int Width { get; set; }
    public int Length { get; set; }
    
    public CorridorMetadata(int width, int length)
    {
        Width = width;
        Length = length;
    }

    public (int x, int y) GetOffset(int lastRoomX, int lastRoomY, int lastRoomWidth, int lastRoomHeight, Direction direction)
    {
        int offsetX = lastRoomX;
        int offsetY = lastRoomY;
        if (direction.IsHorizontal())
        {
            offsetY += lastRoomHeight / 2;
        }
        else
        {
            offsetX += lastRoomWidth / 2;
        }

        switch (direction)
        {
            case Direction.Up:
                offsetY -= Length;
                break;
            case Direction.Down:
                offsetY += lastRoomHeight;
                break;
            case Direction.Left:
                offsetX -= Length;
                break;
            case Direction.Right:
                offsetX += lastRoomWidth;
                break;
        }
        
        return (offsetX, offsetY);
    }

    public (int x, int y) GetOffset(Rectangle roomBounds, Direction direction)
    {
        return GetOffset(roomBounds.X, roomBounds.Y, roomBounds.Width, roomBounds.Height, direction);
    }
}