using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Prominence : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 6));
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 80;
            Item.damage = 80;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = CEUtils.GetSound("ProminenceShoot");
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-28, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Color impactColor = Main.rand.NextBool(3) ? Color.Firebrick : Color.OrangeRed;
            float impactParticleScale = Main.rand.NextFloat(1f, 1.75f) * 1.65f;
            SparkleParticle impactParticle = new SparkleParticle(position + velocity, Vector2.Zero, impactColor, Color.OrangeRed, impactParticleScale, 8, 0.16f, 2f);
            GeneralParticleHandler.SpawnParticle(impactParticle);


            Main.LocalPlayer.Calamity().GeneralScreenShakePower = 4;
            for (int i = 0; i < 64; i++)
            {
                EParticle.NewParticle(new Smoke() { timeleftmax = 16, Lifetime = 16 }, position, velocity.RotatedByRandom(0.74) * 0.6f * Main.rand.NextFloat(0.4f, 1f), Color.OrangeRed, Main.rand.NextFloat(0.06f, 0.14f), 1, true, BlendState.Additive, CEUtils.randomRot());
            }
            for (int i = 0; i < 3; i++)
            {
                int p = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.08f), type, damage, knockback, player.whoAmI);

                p.ToProj().Entropy().ProminenceArrow = true;
                CEUtils.SyncProj(p);
            }
            for (int i = 0; i < 26; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.64f) * 0.6f * Main.rand.NextFloat(0.4f, 1f), ModContent.ProjectileType<ProminenceSplitShot>(), damage / 6, knockback * 2, player.whoAmI);
                Vector2 vel = velocity.RotatedByRandom(0.84f) * 1.4f * Main.rand.NextFloat(0.4f, 1f);
                EParticle.NewParticle(new StrikeParticle(), position, vel, Color.Lerp(Color.OrangeRed, new Color(255, 231, 66), Main.rand.NextFloat()), 0.24f, 1, true, BlendState.Additive, vel.ToRotation());
            }
            player.velocity -= velocity * 0.08f;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.FragmentSolar, 16)
                .AddIngredient<TheBallista>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + new Vector2(0, -24);
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-14, 0);

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
    public class ProminenceSplitShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.6f;
            Projectile.timeLeft = 160;
            Projectile.MaxUpdates = 8;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 64;
        }
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            odp.Add(Projectile.Center);
            if (odp.Count > 12)
            {
                odp.RemoveAt(0);
            }
        }
        public List<Vector2> odp = new List<Vector2>();
        public Color clr = Color.Lerp(Color.OrangeRed, new Color(255, 231, 66), Main.rand.NextFloat());
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D circle = CEUtils.getExtraTex("BasicCircle");
            for (int i = 0; i < odp.Count; i++)
            {
                float s = (i + 1f) / (float)odp.Count * ((float)Projectile.timeLeft / 160f);
                Main.spriteBatch.Draw(circle, odp[i] - Main.screenPosition, null, Color.Lerp(clr, Color.White, (i + 1f) / (float)odp.Count), 0, circle.Size() * 0.5f, 0.18f * s, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
