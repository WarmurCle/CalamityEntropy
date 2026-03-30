using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
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
            player.statLifeMax2 += MaxHealthAddition(level);
            player.lifeRegen += (int)(Math.Round(GetRegen(level * 2)));
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sunflower)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.Ruby, 2)
                .AddTile(TileID.WorkBenches)
                .AddCondition(Mod.GetLocalization("NearShimmer", () => "Near shimmer"), () => (Main.LocalPlayer.ZoneShimmer))
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int level = Level();
            tooltips.Replace("[1]", GetDefense(level));
            tooltips.Replace("[2]", GetRegen(level).ToString());
            tooltips.Replace("[3]", GetEndurance(level).ToPercent());
            tooltips.Replace("[4]", DoTDmgMult(level).ToPercent());
            tooltips.Replace("[5]", MaxHealthAddition(level));
            tooltips.Replace("[L]", level);
            tooltips.Replace("[ML]", 9);
        }
        public static List<int> ApplyBuffType()
        {
            var l = new List<int>();
            if (DownedBossSystem.downedCalamitas)
                l.Add(ModContent.BuffType<TrueVulnerabilityHex>());
            if (DownedBossSystem.downedExoMechs)
                l.Add(ModContent.BuffType<MiracleBlight>());
            if (DownedBossSystem.downedYharon)
                l.Add(ModContent.BuffType<Dragonfire>());
            if (DownedBossSystem.downedDoG)
                l.Add(ModContent.BuffType<GodSlayerInferno>());
            if (DownedBossSystem.downedProvidence)
                l.Add(ModContent.BuffType<HolyFlames>());
            if (DownedBossSystem.downedBoomerDuke)
                l.Add(ModContent.BuffType<SulphuricPoisoning>());
            if (DownedBossSystem.downedPlaguebringer)
                l.Add(ModContent.BuffType<Plague>());
            if (DownedBossSystem.downedCryogen)
                l.Add(BuffID.Frostburn2);
            if (NPC.downedBoss2)
                l.Add(BuffID.Venom);
            l.Add(BuffID.Poisoned);
            return l;
        }
        public static int MaxHealthAddition(int level) => level switch
        {
            0 => 10,
            1 => 15,
            2 => 20,
            3 => 25,
            4 => 30,
            5 => 35,
            6 => 40,
            7 => 50,
            8 => 60,
            9 => 70,
            _ => 10
        };
        public static float DoTDmgMult(int level) => level switch
        {
            0 => 0.5f,
            1 => 0.75f,
            2 => 1f,
            3 => 1.2f,
            4 => 1.4f,
            5 => 1.5f,
            6 => 1.6f,
            7 => 1.8f,
            8 => 2f,
            9 => 2.5f,
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
            8 => 5.5f,
            9 => 6f,
            _ => 0.5f
        }; 
        public static float GetEndurance(int level) => level switch
        {
            0 => 0.02f,
            1 => 0.03f,
            2 => 0.04f,
            3 => 0.05f,
            4 => 0.06f,
            5 => 0.08f,
            6 => 0.09f,
            7 => 0.1f,
            8 => 0.12f,
            9 => 0.14f,
            _ => 0.02f
        };
        public static int GetDefense(int level) => level switch
        {
            0 => 1,
            1 => 2,
            2 => 3,
            3 => 5,
            4 => 6,
            5 => 8,
            6 => 10,
            7 => 12,
            8 => 14,
            9 => 16,
            _ => 1
        };
        public static int Level()
        {
            if (DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs)
                return 9;
            if (DownedBossSystem.downedDoG)
                return 8;
            if (DownedBossSystem.downedProvidence)
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
