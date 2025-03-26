using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkGoozma : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Master;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Goozma");
        public override void modifyShootCooldown(ref int shootCooldown)
        {
            shootCooldown = (int)(shootCooldown * 0.64f);
        }
        public override int modifyProjectile(int pNow)
        {
            if (pNow == ModContent.ProjectileType<AstralBullet>())
            {
                return -1;
            }
            return ModContent.ProjectileType<GoozmaStarShot>();
        }
        public override Color tooltipColor => Main.DiscoColor;
        public override EBookProjectileEffect getEffect()
        {
            return new GZMBMEffect();
        }
    }
    public class GZMBMEffect : EBookProjectileEffect
    {

    }

}