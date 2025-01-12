using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{	
	public class VoidRelics : ModItem
	{
       public override void SetStaticDefaults()
	   {
		ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
		ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		ItemID.Sets.ItemNoGravity[Item.type] = true;
	   }
		
		public override void SetDefaults()
		{
			Item.damage = 116;
			Item.crit = 0;
			Item.DamageType = DamageClass.Summon;
			Item.width = 64;
			Item.height = 64;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.shoot = ModContent.ProjectileType<VoidMark> ();
			Item.shootSpeed = 2f;
			Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            SoundStyle s = new("CalamityEntropy/Assets/Sounds/vmspawn");
            s.Volume = 0.6f;
            s.Pitch = 1f;
			Item.UseSound = s;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.mana = 10;
			Item.buffType = ModContent.BuffType<VoidStorm>();
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(20, 26, 92);
            Item.Entropy().tooltipStyle = 4;
            Item.Entropy().NameColor = new Color(60, 80, 140);
            Item.Entropy().Legend = true;
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float availableSummonSlots = player.maxMinions - player.slotsMinions;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidMark>()] > 0)
			{
				foreach (Projectile p in Main.projectile)
				{
					if (p.type == ModContent.ProjectileType<VoidMark>() && p.owner == player.whoAmI)
					{
						if (p.ai[1] < 10)
						{
							p.ai[1] += 1;
						}
					}
				}
				return false;
			}
			if (availableSummonSlots > 0)
			{
                //player.slotsMinions += availableSummonSlots;

                player.AddBuff(Item.buffType, 3);
				int projectile = Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, (int)(Item.damage * availableSummonSlots), knockback, player.whoAmI, 0, 0, availableSummonSlots);
				Main.projectile[projectile].originalDamage = (int)(Item.damage * availableSummonSlots);
				Main.projectile[projectile].OriginalCritChance = Item.crit;
            }
            return false;
        }

        public override void AddRecipes()
        {
            //CreateRecipe().
              //  AddIngredient(ModContent.ItemType<DarkPlasma>(), 3).
                //AddIngredient(ItemID.EmpressBlade, 1).
                //AddIngredient(ModContent.ItemType<TomeofFates>(), 1).
                //AddTile(ModContent.TileType<VoidCondenser>()).
                //Register();
        }
    }
}