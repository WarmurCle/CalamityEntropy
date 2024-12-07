using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace CalamityEntropy.Content.DimDungeon;

public class Room
{
    public Rectangle Bounds { get; set; }
    
    public RoomMetadata Metadata { get; set; }
    
    public List<Corridor> CorridorUp { get; set; }
    public List<Corridor> CorridorDown { get; set; }
    public List<Corridor> CorridorLeft { get; set; }
    public List<Corridor> CorridorRight { get; set; }
    public bool SpawnedEnemies = false;
    public Rectangle getRect()
    {
        Room room = this;
        return new Rectangle(16 * ((DimDungeonGenPass.START_POINT + room.Metadata.genPos).X + 1), 16 * ((DimDungeonGenPass.START_POINT + room.Metadata.genPos).Y + 1), 16 * (room.Metadata.Width - 2), 16 * (room.Metadata.Height - 2));
    }
    public IEnumerable<Corridor> Corridors => CorridorUp.Union(CorridorDown.Union(CorridorLeft.Union(CorridorRight)));
    public List<Room> ConnectedRooms
    {
        get
        {
            List<Room> rooms = new();
            foreach (Corridor corridor in Corridors)
            {
                rooms.Add(corridor.RoomA == this ? corridor.RoomB : corridor.RoomA);
            }
            return rooms;
        }
    }

    public Room()
    {
        CorridorUp = new();
        CorridorDown = new();
        CorridorLeft = new();
        CorridorRight = new();
    }
    
    public Room(Rectangle bounds, RoomMetadata metadata)
    {
        CorridorUp = new();
        CorridorDown = new();
        CorridorLeft = new();
        CorridorRight = new();
        
        Bounds = bounds;
        Metadata = metadata;
    }

    public Room AddCorridor(ref Corridor corridor, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                CorridorUp.Add(corridor);
                break;
            case Direction.Down:
                CorridorDown.Add(corridor);
                break;
            case Direction.Left:
                CorridorLeft.Add(corridor);
                break;
            case Direction.Right:
                CorridorRight.Add(corridor);
                break;
        }
        
        return this;
    }

    public Room ConnectsWith(Corridor corridor, Direction direction, Room room)
    {
        corridor.RoomA = this;
        corridor.RoomB = room;

        if (room.Corridors.All(c => c != corridor))
        {
            room.AddCorridor(ref corridor, direction.Reverse());
        }
        
        AddCorridor(ref corridor, direction);
        
        return this;
    }
}