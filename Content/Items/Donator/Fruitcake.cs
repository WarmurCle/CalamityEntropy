using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.SummonItems;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public class Fruitcake : ModItem, IDonatorItem
    {
        public static Dictionary<int, List<int>> ammoList = new();
        public string DonatorName => "永霞伊";
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().fruitCake = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OverloadedSludge>()
                .AddIngredient(ItemID.WoodenArrow)
                .AddIngredient(ItemID.SlimeCrown)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = tooltips.Count - 1; i >= 0; i--)
            {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Text.StartsWith("#"))
                {
                    bool hide = true;
                    if (int.TryParse(tooltips[i].Text[1].ToString(), out int n))
                    {
                        if (Level() >= n)
                        { hide = false; }
                    }
                    tooltips[i].Text = tooltips[i].Text.Substring(2);
                    if (hide)
                    {
                        tooltips.RemoveAt(i);
                    }
                }
            }
        }
        public static int Level()
        {
            int l = 0;
            if (NPC.downedSlimeKing || NPC.downedBoss1 || NPC.downedBoss2 || DownedBossSystem.downedDesertScourge)
            {
                l = 1;
            }
            if (NPC.downedBoss2)
            {
                l = 2;
            }
            if (DownedBossSystem.downedSlimeGod)
            {
                l = 3;
            }
            if (DownedBossSystem.downedCryogen || DownedBossSystem.downedBrimstoneElemental)
            {
                l = 4;
            }
            if (EDownedBosses.downedProphet)
            {
                l = 5;
            }
            if (NPC.downedMoonlord)
            {
                l = 6;
            }
            if(DownedBossSystem.downedPolterghast)
            {
                l = 7;
            }
            return l;
        }
    }
}
