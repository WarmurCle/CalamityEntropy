using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items;

namespace CalamityEntropy.Content.Items.Weapons.Fractal
{
    public class WelkinFractal : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.crit = 3;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 48;
            Item.height = 60;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WelkinFractalHeld>();
            Item.shootSpeed = 12f;
        }
        public int atkType = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, atkType == 0 ? -1 : atkType);
            atkType++;
            if(atkType > 2)
            {
                atkType = 0;
            }
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ShatteredFractal>()
                .AddIngredient<CalamityMod.Items.Materials.AerialiteBar>(6)
                .AddIngredient(ItemID.Feather, 2)
                .AddIngredient(ItemID.SunplateBlock, 4)
                .AddIngredient(ItemID.Starfury)
                .AddIngredient<WindBlade>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class WelkinFractalHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Fractal/WelkinFractal";
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 100000;
            Projectile.extraUpdates = 3;
        }
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;
        public float spawnFeatherCounter = 0;
        public override void AI()
        {
            Player owner = Projectile.getOwner();
            float MaxUpdateTimes = owner.itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            counter++;
            if (init)
            {
                if (Projectile.ai[0] == 2)
                {
                    Util.PlaySound("powerwhip", 1, Projectile.Center, volume: 0.6f);
                }
                if(Projectile.ai[0] < 2)
                {
                    Util.PlaySound("sf_use", 1 + Projectile.ai[0] * 0.12f, Projectile.Center, volume: 0.6f);
                }
                init = false;
            }
            odr.Add(Projectile.rotation);
            Projectile.timeLeft = 3;
            float RotF = 4f;
            if (Projectile.ai[0] == 2)
            {
                if(shoot)
                {
                    shoot = false;
                    if(Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.normalize() * 10, ModContent.ProjectileType<FractalBeam>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    }
                    Util.PlaySound("sf_shoot", 1, Projectile.Center);
                }
                float l = (float)(Math.Cos(progress * MathHelper.Pi - MathHelper.PiOver2));
                Projectile.rotation = Projectile.velocity.ToRotation();
                scale = 1f + l * 3f;
                alpha = l;
                Projectile.Center = Projectile.getOwner().MountedCenter + Projectile.velocity.normalize() * (-34 + l * 34);
            }
            else
            {
                spawnFeatherCounter += owner.GetTotalAttackSpeed(Projectile.DamageType);
                if (spawnFeatherCounter >= 16f)
                {
                    spawnFeatherCounter -= 16f;
                    Vector2 spawnPos = Projectile.Center + new Vector2(0, 600) * Projectile.ai[0] + Util.randomPointInCircle(34);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, (Main.MouseWorld - spawnPos).normalize() * 28, ModContent.ProjectileType<FractalFeather>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, Main.rand.NextFloat() * 6.28f);
                    }
                }
                alpha = 1;
                scale = 1f * (1 + (float)(Math.Cos(Util.GetRepeatedCosFromZeroToOne(progress, 3) * MathHelper.Pi - MathHelper.PiOver2)) * 0.8f);
                Projectile.rotation = Projectile.velocity.ToRotation() + (RotF * -0.5f + RotF * Util.GetRepeatedCosFromZeroToOne(progress, 3)) * Projectile.ai[0] * (Projectile.velocity.X > 0 ? -1 : 1);
                Projectile.Center = Projectile.getOwner().MountedCenter; 
            }

            if (odr.Count > 60)
            {
                odr.RemoveAt(0);
            }
            if (Projectile.velocity.X > 0)
            {
                owner.direction = 1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            else
            {
                owner.direction = -1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            if(counter > MaxUpdateTimes)
            {
                owner.itemTime = 1;
                owner.itemAnimation = 1;
                Projectile.Kill();
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public bool playHitSound = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (playHitSound)
            {
                playHitSound = false;
                Util.PlaySound(Projectile.ai[0] == 2 ? "sf_hit1" : "sf_hit", 1, Projectile.Center);
                if (Projectile.ai[0] != 2)
                {
                    Util.PlaySound("FractalHit", 1, Projectile.Center);
                }
            }
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero
            });
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            int dir = (int)(Projectile.ai[0]) * (Projectile.velocity.X > 0 ? -1 : 1);
            if (Projectile.ai[0] == 2)
            {
                dir = Math.Sign(Projectile.velocity.X);
            }
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            float MaxUpdateTime = Projectile.getOwner().itemTimeMax * Projectile.MaxUpdates;
            if (Projectile.ai[0] < 2)
            {
                Texture2D bs = Util.getExtraTex("SemiCircularSmear");
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(bs, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.Lerp(new Color(50, 140, 160), new Color(200, 255, 66), counter / MaxUpdateTime) * (float)(Math.Cos(Util.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, 3) * MathHelper.Pi - MathHelper.PiOver2)) * 0.5f, Projectile.rotation + MathHelper.ToRadians(32) * -dir, bs.Size() / 2f, Projectile.scale * 1.2f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.ExitShaderRegion();
                Texture2D glow = this.getTextureGlow();
                Main.spriteBatch.Draw(glow, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.White * alpha, rot, origin, Projectile.scale * scale, effect, 0);

            }
            else
            {
                Texture2D glow = this.getTextureGlow();
                Main.spriteBatch.Draw(glow, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.White * alpha * (float)(Math.Cos(Util.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, 3) * MathHelper.Pi - MathHelper.PiOver2) * 0.5f + 0.5f), rot, origin, Projectile.scale * scale * 1.4f * 0.34f, effect, 0);
            }
            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale, effect);
            
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (86 * (Projectile.ai[0] == 2 ? 0.9f : 1)) * Projectile.scale * scale, targetHitbox, 64);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (86 * (Projectile.ai[0] == 2 ? 1.24f : 1)) * Projectile.scale * scale, 84, DelegateMethods.CutTiles);
        }
    }

}