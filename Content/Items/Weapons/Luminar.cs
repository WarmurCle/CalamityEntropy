using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Plates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Luminar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 80;
            Item.damage = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.UseSound = SoundID.DD2_SkyDragonsFuryShot;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.autoReuse = true;
            Item.ArmorPenetration = 10;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-18, 0);
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool(6);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            var pj = p.ToProj();
            pj.Entropy().LuminarArrow = true;
            CEUtils.SyncProj(p);
            return false;
        }
    }
}
