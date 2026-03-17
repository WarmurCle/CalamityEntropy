using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public class Leyla : ModItem, IDonatorItem
    {
        public string DonatorName => "Fortun3Rod1on";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int level = Level();
            player.Entropy().addEquip("Leyla", !hideVisual);
            player.statDefense += GetDefense(level);
            player.endurance += GetEndurance(level);
            player.lifeRegen += (int)(Math.Round(GetRegen(level * 2)));
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sunflower)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.Ruby, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int level = Level();
            tooltips.Replace("[1]", GetDefense(level));
            tooltips.Replace("[2]", GetRegen(level).ToString());
            tooltips.Replace("[3]", GetEndurance(level).ToPercent());
            tooltips.Replace("[4]", DoTDmgMult(level).ToPercent());
            tooltips.Replace("[L]", level);
        }
        public static float DoTDmgMult(int level) => level switch
        {
            0 => 0.5f,
            1 => 0.5f,
            2 => 0.5f,
            3 => 0.6f,
            4 => 0.7f,
            5 => 0.8f,
            6 => 0.9f,
            7 => 1f,
            _ => 0.5f
        };
        public static float GetRegen(int level) => level switch
        {
            0 => 0.5f,
            1 => 1f,
            2 => 1.5f,
            3 => 2f,
            4 => 2.5f,
            5 => 3f,
            6 => 4f,
            7 => 5f,
            _ => 0.5f
        }; 
        public static float GetEndurance(int level) => level switch
        {
            0 => 0.02f,
            1 => 0.04f,
            2 => 0.05f,
            3 => 0.06f,
            4 => 0.08f,
            5 => 0.1f,
            6 => 0.12f,
            7 => 0.14f,
            _ => 0.02f
        };
        public static int GetDefense(int level) => level switch
        {
            0 => 1,
            1 => 2,
            2 => 4,
            3 => 5,
            4 => 6,
            5 => 8,
            6 => 10,
            7 => 15,
            _ => 1
        };
        public static int Level()
        {
            if (DownedBossSystem.downedDoG)
                return 7;
            if (NPC.downedMoonlord)
                return 6;
            if (NPC.downedPlantBoss)
                return 5;
            if (DownedBossSystem.downedCryogen || DownedBossSystem.downedBrimstoneElemental)
                return 4;
            if (DownedBossSystem.downedSlimeGod)
                return 3;
            if (NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind)
                return 2;
            if (NPC.downedSlimeKing || NPC.downedBoss1 || DownedBossSystem.downedDesertScourge)
                return 1;

            return 0;
        }
    }
}
