using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class ProphecyFlyingKnife : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 20;
            Item.ArmorPenetration = 20;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.6f;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<FutureKnife>();
            Item.shootSpeed = 18f;
            Item.DamageType = CEUtils.RogueDC;
        }
        public int altShotCount = 0;

        public override float StealthDamageMultiplier => 4.4f;
        public override float StealthVelocityMultiplier => 3f;
        public override float StealthKnockbackMultiplier => 2f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage * 2, knockback, player.whoAmI, altShotCount > 0 ? 1 : 0);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    p.ToProj().netUpdate = true;
                }
                Utilities.Util.PlaySound("bne0", 1, position);
                return false;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    int p = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.36f), type, (int)(damage * 0.8f), knockback, player.whoAmI, altShotCount > 0 ? 1 : 0);
                }
            }
            return false;
        }
    }
}
