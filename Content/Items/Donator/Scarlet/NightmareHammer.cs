using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.MainHammer;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.ExtraProj;
using CalamityMod;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.Scarlet
{
    public class NightmareHammer: BaseHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<NightmareHammerProj>();
        public override void ExSD()
        {
            Item.width = 88;
            Item.height = 94;
            Item.damage = 66;
            Item.useTime = 18;
            //这里的UseTime是有意改的很慢的
            Item.useAnimation = 18;
            Item.shootSpeed = 24f;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item103;
            Item.value = Item.buyPrice(gold: 12);
        }
        //临时写一下，用于调试
        public override void UpdateInventory(Player player)
        {
            Item.damage = 66;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FallenHammer>().
                AddIngredient<AshesofCalamity>(30).
                AddIngredient<Necroplasm>(30).
                AddIngredient(ItemID.LunarBar, 15).
                AddIngredient(ItemID.LargeAmethyst).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}