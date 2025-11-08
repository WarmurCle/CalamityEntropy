using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.Scarlet
{
    public class PunishmentHammer: BaseHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<PunishmentHammerProj>();
        public override void ExSD()
        {
            Item.width = Item.height = 58;
            Item.damage = 32;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 12);
        }
        //临时写一下，用于调试
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Pwnhammer).
                AddIngredient(ItemID.SoulofNight, 10).
                AddIngredient(ItemID.DarkShard, 2).
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Amethyst, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}