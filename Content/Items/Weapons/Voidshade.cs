using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
	public class Voidshade : ModItem
	{
        public int attackType = 0; // keeps track of which attack it is
        public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.scale = 2f;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.damage = 200;
			Item.knockBack = 4;
			Item.crit = 6;
			Item.shoot = ModContent.ProjectileType<VoidshadeHeld>();
			Item.shootSpeed = 16;
			Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
			Item.rare = ModContent.RarityType<VoidPurple>();
            Item.Calamity().devItem = true;
		}
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                attackType = 3;
                damage = (int)(damage * 1.5f);
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/cr_dash") { Pitch = -0.2f }, player.Center);
            }
            else
            {
                if (player.Entropy().voidshadeBoostTime > 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/sword_spin1"), player.Center);
                }
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
            }
            // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
            attackType = (attackType + 1) % 2; // Increment attackType to make sure next swing is different
            if (player.altFunctionUse == 2)
            {
                attackType = (attackType + 1) % 2;

            }
            comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
            return false; // return false to prevent original projectile from being shot
        }

        public override void UpdateInventory(Player player)
        {
            if (comboExpireTimer++ >= 120) // after 120 ticks (== 2 seconds) in inventory, reset the attack pattern
                attackType = 0;
        }

        public override bool MeleePrefix()
        {
            return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(426).AddIngredient(ModContent.ItemType<Voidstone>(), 12).AddTile(TileID.Anvils).Register();
        }
    }
}
