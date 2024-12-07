using CalamityEntropy.Util;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
	public class RustExpeditioner : ModItem {
		public override void SetStaticDefaults() {
			AmmoID.Sets.SpecificLauncherAmmoProjectileFallback[Type] = ItemID.RocketLauncher;

		}

		public override void SetDefaults() {
			Item.DefaultToRangedWeapon(ProjectileID.RocketI, AmmoID.Rocket, singleShotTime: 30, shotVelocity: 20f, hasAutoReuse: true);
			Item.width = 50;
			Item.height = 20;
			Item.damage = 40;
			Item.knockBack = 4f;
			Item.crit = 15;
			Item.UseSound = SoundID.Item11;
			Item.value = Item.buyPrice(gold: 3);
			Item.rare = ItemRarityID.Orange;
		}

		public override Vector2? HoldoutOffset() {
			return new Vector2(-8f, 2f); // Moves the position of the weapon in the player's hand.
		}

        public override void AddRecipes()
        {
			CreateRecipe().AddIngredient(ModContent.ItemType<DubiousPlating>(), 5).AddIngredient(ItemID.IronBar, 15).AddTile(TileID.Anvils).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			p.ToProj().Entropy().withGrav = true;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
            }

            return false;
        }
    }
}