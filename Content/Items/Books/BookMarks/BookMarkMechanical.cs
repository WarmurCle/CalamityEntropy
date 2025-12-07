using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkMechanical : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Mechanical");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Homing += 0.45f;
        }
        public override Color tooltipColor => Color.LightGray;
        public override EBookProjectileEffect getEffect()
        {
            return new MechanicalBMEffect();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<HellIndustrialComponents>(), 10)
            .AddIngredient(ItemID.SoulofNight, 10)
            .AddIngredient(ItemID.SoulofLight, 10)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
    public class MechanicalBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            if (ownerClient && (projectile.ModProjectile is EBookBaseProjectile eb && eb.mainProj) && Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 5 : 8) && CECooldowns.CheckCD("MechanicalBookmark", 30))
            {
                Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.UnitY * -8, ModContent.ProjectileType<Detector>(), projectile.damage / 6, projectile.knockBack, projectile.owner);
            }
        }
    }
}
