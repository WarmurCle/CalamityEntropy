using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class BloodyAmulet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        private float PotionMultie = 0.25f;
        private float MaxHealth = 0.15f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PotionMultie.ToPercentReal(), MaxHealth.ToPercentReal(), "50%", 20);
        public override void SetDefaults()
        {
            Item.width = 65;
            Item.height = 54;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<PureGreen>();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var calPlayer = player.Calamity();
            calPlayer.fleshTotem = true;
            calPlayer.healingPotionMultiplier += PotionMultie;
            player.statLifeMax2 += (int)(player.statLifeMax2 * MaxHealth);
            player.noKnockback = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FleshTotem>().
                AddIngredient<BloodPact>().
                AddIngredient<Bloodstone>(15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
