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
        public static bool downedProphet = false;
        public override void ClearWorld()
        {
            EntropyMode = false;
            downedCruiser = false;
            downedAbyssalWraith = false;
            downedNihilityTwin = false;
            downedProphet = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (EntropyMode)
            {
                tag["EntropyMode"] = true;
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
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedCruiser = tag.ContainsKey("downedCruiser");
            downedAbyssalWraith = tag.ContainsKey("downedAbyssalWraith");
            downedNihilityTwin = tag.ContainsKey("downedNihilityTwin");
            EntropyMode = tag.ContainsKey("EntropyMode");
            downedProphet = tag.ContainsKey("downedProphet");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            var flags2 = new BitsByte();
            flags[0] = downedCruiser;
            flags[1] = downedAbyssalWraith;
            flags[2] = downedNihilityTwin;
            flags[3] = downedProphet;
            flags2[0] = EntropyMode;
            writer.Write(flags);
            writer.Write(flags2);
            
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            BitsByte flags2 = reader.ReadByte();

            downedCruiser = flags[0];
            downedAbyssalWraith = flags[1];
            downedNihilityTwin = flags[2];
            downedProphet = flags[3];
            EntropyMode = flags2[0];
        }
    }
}