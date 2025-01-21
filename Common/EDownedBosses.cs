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
        public override void ClearWorld()
        {
            downedCruiser = false;
            downedAbyssalWraith = false;
            downedNihilityTwin = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
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
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedCruiser = tag.ContainsKey("downedCruiser");
            downedAbyssalWraith = tag.ContainsKey("downedAbyssalWraith");
            downedNihilityTwin = tag.ContainsKey("downedNihilityTwin");
        }

        public override void NetSend(BinaryWriter writer)
        {
            // Order of operations is important and has to match that of NetReceive
            var flags = new BitsByte();
            flags[0] = downedCruiser;
            flags[1] = downedAbyssalWraith;
            flags[2] = downedNihilityTwin;
            writer.Write(flags);
            
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedCruiser = flags[0];
            downedAbyssalWraith = flags[1];
            downedNihilityTwin = flags[2];
        }
    }
}