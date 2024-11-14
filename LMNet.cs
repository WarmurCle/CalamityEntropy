using CalamityEntropy.NPCs;
using CalamityEntropy.Projectiles;
using CalamityEntropy.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy
{

    public class AtlasItemNetS : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ProjectileType<AtlasItem>();
        }
        public override void SendExtraAI(Projectile npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (npc.type == ModContent.ProjectileType<AtlasItem>())
            {
                binaryWriter.Write(npc.Entropy().AtlasItemType);
                binaryWriter.Write(npc.Entropy().AtlasItemStack);

            }
        }


        public override void ReceiveExtraAI(Projectile npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (npc.type == ModContent.ProjectileType<AtlasItem>())
            {
                npc.Entropy().AtlasItemType = binaryReader.ReadInt32();
                npc.Entropy().AtlasItemStack = binaryReader.ReadInt32();

            }
        }
    }
}
