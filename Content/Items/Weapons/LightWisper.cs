﻿using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class LightWisper : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 98;
            Item.height = 46;
            Item.damage = 315;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 3;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightWisperFlame>();
            Item.shootSpeed = 11f;
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-28, 0);

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<VoidBar>(), 8)
                .AddIngredient(ModContent.ItemType<CleansingBlaze>())
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
