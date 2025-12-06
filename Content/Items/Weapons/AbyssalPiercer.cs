using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AbyssalPiercer : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 60;
            Item.ArmorPenetration = 12;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.ArmorPenetration = 86;
            Item.knockBack = 1f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<AbyssalPiercerThrow>();
            Item.shootSpeed = 40f;
            Item.DamageType = CEUtils.RogueDC;
        }



        public override float StealthDamageMultiplier => 1.0f;
        public override float StealthVelocityMultiplier => 1.5f;
        public override float StealthKnockbackMultiplier => 3f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    p.ToProj().netUpdate = true;
                    p.ToProj().penetrate = 5;
                }
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<CorrodedFossil>(), 6).AddIngredient(ModContent.ItemType<Voidstone>(), 10).AddTile(TileID.Anvils).Register();
        }
    }
}
