using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
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
    public class ThiefsPocketwatchOfEclipse : ModItem
    {
        public static float damage = 0.2f;
        public static float MoveSpeed = 0.15f;
        public static float stealthGen = 0.25f;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", damage.ToPercent());
            tooltips.Replace("[B]", MoveSpeed.ToPercent());
            tooltips.Replace("[C]", stealthGen.ToPercent());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RogueDamageClass>() += damage;
            player.Entropy().moveSpeed += MoveSpeed;
            player.Entropy().RogueStealthRegenMult += stealthGen;
            player.Entropy().ExtraStealthBar = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SolarVeil>(8)
                .AddIngredient<ExodiumCluster>(12)
                .AddIngredient(ItemID.GoldWatch)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient<SolarVeil>(8)
                .AddIngredient<ExodiumCluster>(12)
                .AddIngredient(ItemID.PlatinumWatch)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
