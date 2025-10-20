﻿using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles.Chainsaw;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Chainsaw
{
    public class AzafurePowerSaw : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 42;
            Item.height = 42;
            Item.noUseGraphic = true;
            Item.useTime = 16;
            Item.useAnimation = 0;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 360;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item23;
            Item.channel = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<AzafurePowerSawProj>();
            Item.shootSpeed = 1f;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HellIndustrialComponents>(5).
                AddRecipeGroup(RecipeGroupID.IronBar, 6).
                AddIngredient(ItemID.Chain, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
    public class AzafurePowerSawProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0f;
            Projectile.scale = 1.6f;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.ArmorPenetration = 5;
        }
        int frame = 2;
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.localAI[0] < 5 ? false : null;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale += Projectile.owner.ToPlayer().Entropy().WeaponBoost * 0.8f;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 8 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
            }

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                HandleChannelMovement(player, playerRotatedPoint);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.localAI[0]++ > 3 ? 0 : Projectile.GetOwner().direction * -MathHelper.ToRadians(4 - Projectile.localAI[0]) * 48);
            if (Projectile.localAI[0] == 4)
            {
                CEUtils.PlaySound("chainsawHit", 1, Projectile.Center, volume: 0.4f);
            }
            Projectile.Center = player.Center + Projectile.rotation.ToRotationVector2() * 48 * Projectile.scale + player.gfxOffY * Vector2.UnitY;
            if (Projectile.Entropy().OnProj != -1)
            {
                Projectile.Center = Projectile.Entropy().OnProj.ToProj().Center + Projectile.rotation.ToRotationVector2() * 32 * Projectile.scale;
            }
            if (Projectile.velocity.X > 0)
            {
                player.direction = 1;

            }
            else
            {
                player.direction = -1;
            }
            player.itemRotation = (Projectile.velocity * player.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (!player.channel)
            {
                Projectile.timeLeft = 1;
            }
            soundCd--;

            Projectile.ai[2]++;
            if (Projectile.ai[2] > 480 * Projectile.MaxUpdates)
            {
                CEUtils.PlaySound("chainsaw_break", 1, Projectile.Center, 1, 0.6f * CEUtils.WeapSound);
                player.itemTime = 120;
                Projectile.Kill();
            }
        }
        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = newVelocity;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.GetOwner().Center, Projectile.GetOwner().Center + Projectile.rotation.ToRotationVector2() * 216, targetHitbox, 42);
        }
        public int soundCd = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[Projectile.owner];
            SoundStyle hitSound = new SoundStyle("CalamityEntropy/Assets/Sounds/chainsaw", SoundType.Ambient) { Volume = 0.3f };
            if (soundCd <= 0)
            {
                SoundEngine.PlaySound(hitSound, Projectile.Center);
                soundCd = 16;
            }
            if (Projectile.owner == Main.myPlayer && ModContent.GetInstance<Config>().ChainsawShakeScreen)
            {
                CalamityEntropy.Instance.screenShakeAmp = 1;
            }
            float sparkCount = 12;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = new Vector2(18, 0).RotatedByRandom(3.14159f) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(23, 35);
                float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                Color sparkColor2 = Color.DarkRed;

                float velc = 0.7f;
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? Color.Red : Color.Firebrick);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
        public override bool PreDraw(ref Color dc)
        {
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Chainsaw/AzafurePowerSaw" + (((int)(Projectile.ai[0] / 4)) % frame).ToString()).Value;
            var rand = Main.rand;
            SpriteEffects ef = SpriteEffects.None;
            if (Projectile.velocity.X < 0)
            {
                ef = SpriteEffects.FlipVertically;
            }
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition + new Vector2(rand.Next(-2, 3), rand.Next(-2, 3)), null, dc, Projectile.rotation, tx.Size() / 2, new Vector2(Projectile.scale, Projectile.scale), ef, 0);
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Chainsaw/AzafurePowerSaw0";
    }
}