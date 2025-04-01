﻿using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Mercy : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<VoidEcho>();
        }
        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.width = 96;
            Item.noUseGraphic = true;
            Item.height = 96;
            Item.useTime = 1;
            Item.useAnimation = 0;
            Item.channel = true;
            Item.knockBack = 4;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType<MercySpawner>();
            Item.shootSpeed = 1f;
            Item.mana = 110;
            Item.useStyle = -1;
            Item.noMelee = true;
            /* Item.Entropy().stroke = true;
             Item.Entropy().strokeColor = new Color(66, 7, 20);
             Item.Entropy().tooltipStyle = 4;
             Item.Entropy().NameColor = new Color(255, 0, 70);
             Item.Entropy().HasCustomStrokeColor = true;
             Item.Entropy().HasCustomNameColor = true;*/
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<HelhieimBlaster>()] <= 0;
        }

        public override bool MagicPrefix()
        {
            return true;
        }
    }
}
