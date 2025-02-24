using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class VoidPathology : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 104;
            Item.height = 104;
            Item.damage = 220;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.UseSound = Util.Util.GetSound("vp_use");
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.shoot = ModContent.ProjectileType<NihilityVirus>();
            Item.shootSpeed = 16f;
            Item.mana = 12  ;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.useTurn = false;
        }
        public override bool MagicPrefix()
        {
            return true;
        }
    }
}
