using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkIce : BookMark
    {
        public override Texture2D UITexture => BookMark.GetUITexture("Ice");
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Color tooltipColor => new Color(160, 250, 255);
        public override EBookProjectileEffect getEffect()
        {
            return new IceBMEffect();
        }
    }
    public class IceBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            if (ownerClient)
            {
                Vector2 pos = projectile.Center - projectile.velocity.normalize() * 168 + CEUtils.randomVec(128);
                int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), pos, (Main.MouseWorld - pos).normalize() * 32, ModContent.ProjectileType<IceEdge2>(), projectile.damage / 5, projectile.knockBack, projectile.owner);
                (p.ToProj().ModProjectile as EBookBaseProjectile).homing = (projectile.ModProjectile as EBookBaseProjectile).homing;
            }
        }
    }
}