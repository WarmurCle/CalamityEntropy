using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs.Wyrm
{
    public class WyrmPhantom : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PhantomWyrm>()] <= 0)
            {
                if(Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Util.Util.randomVec(14), ModContent.ProjectileType<PhantomWyrm>(), (int)(player.GetTotalDamage(DamageClass.Summon).ApplyTo(800)), 1, player.whoAmI);
                }
            }
        }
    }
}