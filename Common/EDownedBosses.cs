using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Common
{
    public class EDownedBosses : ModSystem
    {
        public static bool downedCruiser = false;
        public static bool downedAbyssalWraith = false;
        public static bool downedNihilityTwin = false;
        public static bool EntropyMode = false;
        public static bool TDR = false;
        public static bool downedProphet = false;
        public static bool downedLuminaris = false;
        public static bool downedAcropolis = false;
        public static Point ForbiddenArchiveCenter = new Point(-1, -1);
        public override void ClearWorld()
        {
            EntropyMode = false;
            downedCruiser = false;
            downedAbyssalWraith = false;
            downedNihilityTwin = false;
            downedProphet = false;
            downedLuminaris = false;
            downedAcropolis = false;
            ForbiddenArchiveCenter = new Point(-1, -1);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (EntropyMode)
            {
                tag["EntropyMode"] = true;
            }
            if (TDR)
            {
                tag["TDR"] = true;
            }
            if (downedCruiser)
            {
                tag["downedCruiser"] = true;
            }
            if (downedAbyssalWraith)
            {
                tag["downedAbyssalWraith"] = true;
            }
            if (downedNihilityTwin)
            {
                tag["downedNihilityTwin"] = true;
            }
            if (downedProphet)
            {
                tag["downedProphet"] = true;
            }
            if (downedLuminaris)
            {
                tag["downedLuminaris"] = true;
            }
            if (downedAcropolis)
            {
                tag["downedAcropolis"] = true;
            }
            tag["DungeonArchiveCenterX"] = ForbiddenArchiveCenter.X;
            tag["DungeonArchiveCenterY"] = ForbiddenArchiveCenter.Y;
        }
        public static Vector2 GetDungeonArchiveCenterPos()
        {
            return ForbiddenArchiveCenter.ToVector2() * 16 + new Vector2(8, 1288);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedCruiser = tag.ContainsKey("downedCruiser");
            downedAbyssalWraith = tag.ContainsKey("downedAbyssalWraith");
            downedNihilityTwin = tag.ContainsKey("downedNihilityTwin");
            EntropyMode = tag.ContainsKey("EntropyMode");
            downedProphet = tag.ContainsKey("downedProphet");
            downedLuminaris = tag.ContainsKey("downedLuminaris");
            downedAcropolis = tag.ContainsKey("downedAcropolis");
            TDR = tag.ContainsKey("TDR");
            if (tag.ContainsKey("DungeonArchiveCenterX") && tag.ContainsKey("DungeonArchiveCenterY"))
            {
                ForbiddenArchiveCenter = new(tag.GetInt("DungeonArchiveCenterX"), tag.GetInt("DungeonArchiveCenterY"));
            }
            else
            {
                ForbiddenArchiveCenter = new(-1, -1);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            var flags2 = new BitsByte();
            flags[0] = downedCruiser;
            flags[1] = downedAbyssalWraith;
            flags[2] = downedNihilityTwin;
            flags[3] = downedProphet;
            flags[4] = downedLuminaris;
            flags[5] = downedAcropolis;
            flags2[0] = EntropyMode;
            flags2[1] = TDR;

            writer.Write(flags);
            writer.Write(flags2);
            writer.Write(ForbiddenArchiveCenter.X);
            writer.Write(ForbiddenArchiveCenter.Y);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            BitsByte flags2 = reader.ReadByte();

            downedCruiser = flags[0];
            downedAbyssalWraith = flags[1];
            downedNihilityTwin = flags[2];
            downedProphet = flags[3];
            downedLuminaris = flags[4];
            downedAcropolis = flags[5];
            EntropyMode = flags2[0];
            TDR = flags2[1];
            ForbiddenArchiveCenter = new Point(reader.ReadInt32(), reader.ReadInt32());
        }
    }
}