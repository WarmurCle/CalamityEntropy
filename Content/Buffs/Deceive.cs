using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs
{
    public class Deceive : ModBuff
    {
        public int counter;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            
        }


        public override void Update(Player player, ref int buffIndex)
        {
            player.velocity *= 0.98f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.velocity *= 0.98f;
        }
    }
}