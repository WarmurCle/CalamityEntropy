using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using CalamityEntropy.Projectiles;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityEntropy.Util;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using Terraria.GameContent;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Security.Policy;
using CalamityMod.Items.Weapons.Magic;

namespace CalamityEntropy.Items
{
    public class TargetingWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 1;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item28;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<TargetSetProj>();
            Item.shootSpeed = 20f;
        }


    }
}
