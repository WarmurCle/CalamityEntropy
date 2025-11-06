using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookmarkSulphurous : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Sulphurous");
        public override EBookProjectileEffect getEffect()
        {
            return new BookmarkSulphurousBMEffect();
        }

        public override Color tooltipColor => Color.LimeGreen;
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<SulphuricScale>(4)
                .AddIngredient<SulphurousSand>(40)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }

    public class BookmarkSulphurousBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (projectile.GetOwner().Entropy().SulphurousBubbleRecharge < 3600)
            {
                target.AddBuff<SulphuricPoisoning>(5 * 60);
            }
        }
    }
}