using CalamityEntropy.Utilities;
using CalamityMod.Items;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkSunkenSea : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("SunkenSea");
        public override EBookProjectileEffect getEffect()
        {
            return new SunkenSeaBMEffect();
        }

        public override Color tooltipColor => Color.SkyBlue;
    }

    public class SunkenSeaBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            Vector2 shotDir = Utilities.Util.randomRot().ToRotationVector2();
            Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center + shotDir * 32, shotDir * 6, ModContent.ProjectileType<AquashardSplit>(), damageDone / 6, projectile.knockBack / 3, projectile.owner).ToProj().DamageType = projectile.DamageType;
            SoundEngine.PlaySound(in SoundID.Item27, projectile.Center);
        }
    }
}