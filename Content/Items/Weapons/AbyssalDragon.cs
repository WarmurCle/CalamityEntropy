using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AbyssalDragon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 630;
            Item.noMelee = true;
            Item.crit = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item28;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<AbyssDragonProj>();
            Item.shootSpeed = 20f;
            Item.mana = 28;
            Item.DamageType = DamageClass.Magic;
        }
        public override bool MagicPrefix()
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, ModContent.ProjectileType<AbyssDragonProj>(), damage, knockback, player.whoAmI);
            for (int i = 0; i < 6 + player.Entropy().WeaponBoost * 3; i++)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity.RotatedByRandom(MathHelper.ToRadians(6)), ModContent.ProjectileType<AbyssalStar>(), (int)(damage * 0.3f), knockback, player.whoAmI);

            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 8)
                .AddIngredient(ModContent.ItemType<ReaperTooth>(), 4)
                .AddIngredient(ModContent.ItemType<DeathhailStaff>())
                .AddIngredient(ModContent.ItemType<ClamorNoctus>())
                .AddTile(ModContent.TileType<CosmicAnvil>())
                .Register();
        }
        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + new Vector2(0, 48);
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-16, 0);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (-0.5f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        #endregion
    }
}
