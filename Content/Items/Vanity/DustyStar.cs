﻿using CalamityEntropy.Content.Items.Donator;
using CalamityMod;
using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity
{
    public class DustyStar : ModItem, IDevItem
    {
        public string DevName => "Polaris";
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (DateTime.Now.Month == 12)
                {
                    EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/polaris_Head_s", EquipType.Head, this);
                }
                else
                {
                    EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/polaris_Head", EquipType.Head, this);
                }

                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/polaris_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/polaris_Legs", EquipType.Legs, this);
            }
        }

        public override void SetStaticDefaults()
        {

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.accessory = true;
            Item.value = CalamityMod.Items.CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
            Item.Calamity().devItem = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<DustyStarPlayer>().vanityEquipped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<DustyStarPlayer>().vanityEquipped = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DemonicBoneAsh>(), 1)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.WorkBenches).Register();
        }

    }

    public class DustyStarPlayer : ModPlayer
    {
        public bool vanityEquipped = false;

        public override void ResetEffects()
        {
            vanityEquipped = false;
        }

        public override void FrameEffects()
        {
            if (vanityEquipped)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "DustyStar", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "DustyStar", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "DustyStar", EquipType.Head);

            }
        }
    }
}
