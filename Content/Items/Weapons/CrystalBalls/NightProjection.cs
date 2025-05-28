using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.CrystalBalls
{
    public class NightProjection : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 120;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 1f;
            Item.UseSound = CEUtils.GetSound("soulshine");
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<NightProjectionHoldout>();
            Item.shootSpeed = 16f;
            Item.mana = 3;
            Item.DamageType = DamageClass.Magic;
        }
        public override bool MagicPrefix()
        {
            return true;
        }
    }
}
