using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class DivineIntervention : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = -1;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<DivingShield>();
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 5;

        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<DivingShieldCooldown>());
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<DivingShieldCooldown>(), 3600, true, false);
            player.AddCooldown(DivingCd.ID, 3600);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 40).
                AddIngredient(ItemID.Ruby, 5).
                AddIngredient(ItemID.HolyWater, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
