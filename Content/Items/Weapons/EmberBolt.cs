using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class EmberBolt : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 36;
            Item.DamageType = DamageClass.Magic;
            Item.width = 44;
            Item.height = 46;
            Item.useTime = 42;
            Item.useAnimation = 42;
            Item.knockBack = 4;
            Item.UseSound = CEUtils.GetSound("beast_lavaball_rise1", 1.6f);
            Item.shoot = ModContent.ProjectileType<TectinicShardHoming>();
            Item.shootSpeed = 26f;
            Item.mana = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for(int i = 0; i < 4; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.6f, 1f), type, damage, knockback, player.whoAmI).ToProj().DamageType = Item.DamageType;
            }
            return false;
        }
        public override bool MagicPrefix()
        {
            return true;
        }
    }
}
