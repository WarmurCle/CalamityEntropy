using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class NightEpic : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 86;
            Item.useAnimation = Item.useTime = 18;
            Item.crit = 10;
            Item.mana = 8;
            Item.rare = Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark4").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<NightEpicHeld>();
        public override int SlotCount => 4;

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<RedemptionBible>()
                .AddIngredient<AstralBar>(6)
                .AddIngredient<AstralMonolith>(6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class NightEpicHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/NightEpic/NightEpicOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/NightEpic/NightEpicPage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/NightEpic/NightEpicUI";

        public override bool Shoot()
        {
            base.Shoot();
            return base.Shoot();
        }

        public override EBookStatModifer getBaseModifer()
        {
            var mdf = base.getBaseModifer();
            mdf.Homing += 1f;
            mdf.HomingRange += 0.8f;
            return mdf;
        }

        public override float randomShootRotMax => 0.5f;
        public override int baseProjectileType => ModContent.ProjectileType<NightStar>();

        public override int frameChange => 3;

    }
}
