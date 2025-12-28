using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo
{
    public class CharredMissle : ModItem, IDonatorItem
    {
        public string DonatorName => "Ovasa";

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = BaseMissleProj.AmmoType;
            Item.damage = 8;
            Item.shoot = ModContent.ProjectileType<CharredMissleProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ModContent.ItemType<OsseousRemains>())
                .AddIngredient(ItemID.IronBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class CharredMissleProj : BaseMissleProj
    {
        public override float StickDamageAddition => 0.05f;
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/CharredMissle";
    }
}
