using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkAstral : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Astral");
        public override void modifyShootCooldown(ref int shootCooldown)
        {
            shootCooldown *= 3;
        }

        public override int modifyProjectile(int pNow)
        {
            return ModContent.ProjectileType<AstralBullet>();
        }
        public override Color tooltipColor => new Color(122, 122, 190);
    }

}