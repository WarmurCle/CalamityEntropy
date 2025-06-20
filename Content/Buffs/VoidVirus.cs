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
            if (Main.GameUpdateCount % 20 == 0)
            {
                if (npc.life > 80)
                {
                    npc.life -= 80;
                    CombatText.NewText(npc.getRect(), Color.SkyBlue, 80, false, true);
                }
                else
                {
                    npc.SimpleStrikeNPC(80, 0, false, 0, DamageClass.Default);
                }
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {

            if (Main.GameUpdateCount % 12 == 0)
            {
                if (player.statLife > 2)
                {
                    player.statLife -= 2;
                }
                else
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral(Language.GetTextValue("Mods.CalamityEntropy.KilledByVoidVirus1") + $"{player.name}" + Language.GetTextValue("Mods.CalamityEntropy.KilledByVoidVirus2"))), 4, 0, dodgeable: false, armorPenetration: 114514, quiet: true);
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
                modifiers.ArmorPenetration += 10;
            }
        }
    }

}
