using CalamityEntropy.Buffs;
using CalamityEntropy.Projectiles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories
{
	public class SacrificalMask : ModItem
	{

		public override void SetDefaults() {
			Item.width = 86;
			Item.height = 86;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare =ItemRarityID.Red;
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = Color.Red;
            Item.Entropy().NameColor = Color.Black;
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SacrificalDagger>()] < 8)
            {
                Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<SacrificalDagger>(), (int)player.GetBestClassDamage().ApplyTo(210), 1, player.whoAmI);
            }
            player.Entropy().sacrMask = true;
        }

        public override void AddRecipes()
        {
        }
    }
}
