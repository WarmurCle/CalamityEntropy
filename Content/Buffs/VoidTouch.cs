using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs
{
    public class VoidTouch : ModBuff
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
            if (Main.GameUpdateCount % 18 == 0)
            {
                player.statLife -= 12;
                if (player.statLife <= 12)
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral($"{player.name}" + Language.GetTextValue("Mods.CalamityEntropy.KilledByVoidTouch"))), 16, 0);
                }
            }
            var r = Main.rand;
            Dust.NewDust(player.Center, player.width, player.height, DustID.CorruptSpray, (float)r.NextDouble() * 6 - 3, (float)r.NextDouble() * 6 - 3);
            if (!player.GetModPlayer<EPlayerDash>().velt)
            {
                player.velocity *= 0.99f;
            }
            for (int i = 0; i < 1; i++)
            {
                Particle p = new Particle();
                p.position = player.Center;
                p.alpha = 0.5f;

                p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * 6), (float)((r.NextDouble() - 0.5) * 6));
                VoidParticles.particles.Add(p);
            }
            player.GetModPlayer<EPlayerDash>().velt = false;
        }
    }
}
