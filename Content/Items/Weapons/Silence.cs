using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Silence : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 1600;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 33;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.ArmorPenetration = 50;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<SilenceThrow>();
            Item.shootSpeed = 15f;
            Item.DamageType = CEUtils.RogueDC;
        }

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                type = ModContent.ProjectileType<SilenceThrow>();
        }


        public override float StealthDamageMultiplier => 0.3f;
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
                    p.ToProj().penetrate = 9 + player.Entropy().WeaponBoost * 4;
                }

                return false;
            }
            return true;
        }
    }
}
