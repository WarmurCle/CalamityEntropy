using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs
{
    public class VoidVirus : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if(Main.GameUpdateCount % 20 == 0)
            {
                npc.SimpleStrikeNPC(200, 0, false, 0, DamageClass.Default);
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            
            if(Main.GameUpdateCount % 12 == 0)
            {
                if(player.statLife > 5)
                {
                    player.statLife -= 5;
                }
                else
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.CalamityEntropy.KilledByVoidVirus1") + $"{player.name}" + Language.GetTextValue("Mods.CalamityEntropy.KilledByVoidVirus2")), 4, 0, dodgeable: false, armorPenetration: 114514, quiet: true);
                }
            }
        }
    }

    public class VoidVirusDebuffNPC : GlobalNPC
    {
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<VoidVirus>())
            {
                modifiers.ArmorPenetration += 48;
            }
        }
    }

}