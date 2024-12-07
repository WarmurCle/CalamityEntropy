using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{	
	public class FriendBox : ModItem
	{
       public override void SetStaticDefaults()
	   {
		ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
		ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		ItemID.Sets.ItemNoGravity[Item.type] = true;
	   }
		
		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.crit = 0;
			Item.DamageType = DamageClass.Summon;
			Item.width = 64;
			Item.height = 64;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.shoot = ModContent.ProjectileType<TwistedTwinMinion>();
			Item.shootSpeed = 2f;
			Item.value = 100000;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.mana = 10;
			Item.buffType = ModContent.BuffType<TwistedTwinBuff>();
            Item.rare = ModContent.RarityType<HotPink>();
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 3);
			int projectile = Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, (int)(damage), knockback, player.whoAmI, 0, 1, 0);
            Main.projectile[projectile].originalDamage = damage;

            projectile = Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, (int)(damage), knockback, player.whoAmI, 0, -1, 0);
            Main.projectile[projectile].originalDamage = damage;
            
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