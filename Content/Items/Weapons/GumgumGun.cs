using CalamityEntropy.Content.Projectiles.Gum;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
	public class GumgumGun : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults() {
			
			Item.width = 22;
			Item.height = 22;
			Item.scale = 1.4f;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;

			/*Item.UseSound = new SoundStyle($"{nameof(ExampleMod)}/Assets/Sounds/Items/Guns/ExampleGun") {
				Volume = 0.9f,
				PitchVariance = 0.2f,
				MaxInstances = 8,
			};*/
			Item.value = Item.buyPrice(gold: 2);
			Item.DamageType = ModContent.GetInstance<NoneTypeDamageClass>();
			Item.damage = 25;
			Item.knockBack = 0.2f;
			Item.noMelee = true;
			Item.Calamity().devItem = true;
			Item.shoot = ModContent.ProjectileType<GumProj>();
			Item.shootSpeed = 24f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/Gum" + Main.rand.Next(1, 3).ToString()));
            }
            return true;
        }
        public override Vector2? HoldoutOffset() {
			return new Vector2(2f, -2f);
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.RedDye)
				.AddIngredient(ItemID.OrangeDye)
				.AddIngredient(ItemID.YellowDye)
				.AddIngredient(ItemID.GreenDye)
				.AddIngredient(ItemID.CyanDye)
				.AddIngredient(ItemID.BlueDye)
				.AddIngredient(ItemID.PurpleDye)
				.AddIngredient(ItemID.PainterPaintballGun)
				.AddTile(TileID.MythrilAnvil)
				.Register();

        }
    }
}
