﻿using CalamityEntropy.Content.Items.Donator;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity
{
    public class CrimsonNight : ModItem, IDevItem
    {
        public string DevName => "AwayAstral";
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/cr_Head", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/cr_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/cr_Legs", EquipType.Legs, this);
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
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<CrimsonNightPlayer>().vanityEquipped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<CrimsonNightPlayer>().vanityEquipped = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BloodMoonStarter)
                .AddIngredient(ItemID.Silk, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

    }

    public class CrimsonNightPlayer : ModPlayer
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
                Player.legs = EquipLoader.GetEquipSlot(Mod, "CrimsonNight", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "CrimsonNight", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "CrimsonNight", EquipType.Head);

            }
        }
    }
}
