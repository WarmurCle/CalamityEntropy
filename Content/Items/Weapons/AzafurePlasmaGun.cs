using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafurePlasmaGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 76;
            Item.crit = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 194;
            Item.height = 42;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 16;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = CEUtils.GetSound("ofshoot");
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<PlasmaBall>();
            Item.shootSpeed = 6;
            var modItem = Item.Calamity();
            modItem.UsesCharge = true;
            modItem.MaxCharge = 80f;
            modItem.ChargePerUse = 0.05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AerialiteBar>(8)
                .AddIngredient<DubiousPlating>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }


        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 76f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(0 + ((player.itemAnimation >= (player.itemAnimationMax * 0.75f)) ? CEUtils.Parabola(4 * (player.itemAnimation - player.itemAnimationMax * 0.75f) / (float)player.itemAnimationMax, 24) : 0), 0);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }
        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        #endregion
    }
}
