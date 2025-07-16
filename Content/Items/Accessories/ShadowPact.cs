using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class ShadowPact : ModItem
    {
        public float RogueDamage = 0.06f;
        public static int BaseDamage = 16;

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.defense = 4;
            Item.height = 36;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().shadowPact = true;
            player.GetDamage<RogueDamageClass>() += RogueDamage;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", RogueDamage.ToPercent());
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.ShadowScale, 6)
                .AddIngredient(ItemID.Book, 4)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe().AddIngredient(ItemID.TissueSample, 6)
                .AddIngredient(ItemID.Book, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

}
