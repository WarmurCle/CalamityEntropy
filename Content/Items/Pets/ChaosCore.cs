using CalamityEntropy.Content.Buffs.Pets;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Projectiles.Pets;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Pets
{
    public class ChaosCore : ModItem, IDevItem
    {
        public string DevName => "ShadowWarrior";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Sorensen>();
            Item.buffType = ModContent.BuffType<ChaosTyrant>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(Item.buffType, 3600);
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofLight, 2).
                AddIngredient(ItemID.HallowedBar, 4).
                AddIngredient<EssenceofSunlight>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}