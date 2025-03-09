using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkOfNight : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Night");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Damage += 0.1f;
        }
        public override Color tooltipColor => Color.DarkRed;
        public override EBookProjectileEffect getEffect()
        {
            return new DarkSoulBMEffect();
        }
    }
    public class DarkSoulBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                (Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(30, 34), ModContent.ProjectileType<DarkSoul>(), damageDone / 2, projectile.knockBack / 3, projectile.owner).ToProj().ModProjectile as EBookBaseProjectile).homing = ((EBookBaseProjectile)projectile.ModProjectile).homing;
            }
        }
    }
}