using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.DimDungeon
{
    public static class MapGen
    {
        public static List<RoomGenAction> Gen()
        {
            List<RoomGenAction> map = new List<RoomGenAction>();
            int maxSize = ModContent.GetInstance<Config>().DungeonMax;
            Room start = new Room(64, 64, new List<Corridor>() { new Corridor(12, 44, CreateRoom(maxSize), Direction.Left), new Corridor(8, 10, CreateRoom(maxSize), Direction.Right), new Corridor(8, 10, CreateRoom(maxSize), Direction.Up) });
            map.Add(new RoomGenAction(start));
            return map;
        }

        public static Room CreateRoom(int connects)
        {
            if (connects == 0)
            {
                return new Room(64, 64, new List<Corridor>());
            }
            else
            {
                Room r = CreateRoom(connects - 1);
                return new Room(64, 64, new List<Corridor>() { new Corridor(12, 44, r, (Direction)(Main.rand.Next(0, 4))) });
            }
        }
    }
}
