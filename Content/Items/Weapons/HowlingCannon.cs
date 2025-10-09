﻿using CalamityEntropy.Content.NPCs.SpiritFountain;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class HowlingCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 126;
            Item.height = 66;
            Item.damage = 900;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 1f;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<Soulight>();
            Item.shoot = ModContent.ProjectileType<HowlingBullet>();
            Item.shootSpeed = 16f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
        }
        public bool JustShooted = false;
        public int usecount = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            usecount++;
            JustShooted = true;
            if(usecount % 3 == 1)
            {
                Projectile.NewProjectile(source, position + velocity * 3.8f, velocity, ModContent.ProjectileType<HowlingLaser>(), damage * 3, knockback, player.whoAmI);
            }
            for(int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, position, velocity + CEUtils.randomRot().ToRotationVector2() * 0.6f, type, damage / 2, knockback, player.whoAmI);
            }
            return false;
        }
        public override bool RangedPrefix()
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            int heldType = ModContent.ProjectileType<HowlingCannonHeld>();
            if (player.ownedProjectileCounts[heldType] < 1)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, heldType, 0, 0, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Norfleet>()
                .AddIngredient<RuinousSoul>(4)
                .AddIngredient<AscendantSpiritEssence>(6)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }

    public class HowlingCannonHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/HowlingCannon";
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.width = Projectile.height = 6;
        }
        public float heldOffset = 0;
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            if(player.HeldItem.ModItem is HowlingCannon hc)
            {
                Projectile.timeLeft = 2;
                if(hc.JustShooted)
                {
                    CEUtils.PlaySound("howlingShoot", Main.rand.NextFloat(0.7f, 1.3f), Projectile.Center);
                    hc.JustShooted = false;
                    heldOffset += -18;
                    if (hc.usecount % 3 == 1)
                    {
                        heldOffset += -12;
                    }
                    if (hc.usecount % 3 == 0)
                    {
                        player.itemTimeMax *= 2;
                        player.itemAnimationMax *= 2;
                        player.itemAnimation *= 2;
                        player.itemTime *= 2;
                    }
                    CEUtils.SetShake(Projectile.Center, 4);
                }

                player.Calamity().mouseWorldListener = true;
                Projectile.Center = player.GetDrawCenter();
                Projectile.rotation = (player.Calamity().mouseWorld - Projectile.Center).ToRotation();
                player.heldProj = Projectile.whoAmI;
                player.SetHandRot(Projectile.rotation);
            }
            heldOffset *= 0.86f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            int dir = Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1;
            Vector2 origin = new Vector2(50, tex.Height / 2f);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(heldOffset, 0).RotatedBy(Projectile.rotation), null, lightColor, Projectile.rotation + dir * heldOffset * 0f, origin, Projectile.scale, effect);
            return false;
        }
    }
    public class HowlingBullet : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public List<Vector2> oldPos = new List<Vector2>();
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.light = 0.4f;
            Projectile.timeLeft = 1240;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.PlaySound("blackholeEnd", 3f, Projectile.Center, volume:0.6f);
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.AliceBlue, 0.1f, 0.8f, 8));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.SkyBlue * 0.8f, 1.5f, 1, true, BlendState.Additive, 0, 16);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 0.4f, 1, true, BlendState.Additive, 0, 16);
            if (Projectile.owner == Main.myPlayer)
            {
                CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.owner.ToPlayer(), Projectile.Center, Projectile.damage, 120, Projectile.DamageType);
            }
            CEUtils.SetShake(Projectile.Center, 8);
        }
        public TrailParticle t1;
        public TrailParticle t2;
        public override void AI()
        {
            if (t1 == null || t2 == null)
            {
                t1 = new TrailParticle();
                t2 = new TrailParticle();
                EParticle.spawnNew(t1, Projectile.Center, Vector2.Zero, Color.AliceBlue, 0.8f, 1, true, BlendState.Additive);
                EParticle.spawnNew(t2, Projectile.Center, Vector2.Zero, Color.AliceBlue, 0.8f, 1, true, BlendState.Additive);

            }
            t1.AddPoint(Projectile.Center + Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * 12 * Projectile.scale);
            t2.AddPoint(Projectile.Center + Projectile.velocity.normalize().RotatedBy(-MathHelper.PiOver2) * 12 * Projectile.scale);
            oldPos.Add(Projectile.Center);
            if (oldPos.Count > 20)
            {
                oldPos.RemoveAt(0);
            }
            if (Projectile.timeLeft % 4 == 0)
            {
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.2f, new Color(100, 100, 160), new Vector2(0.3f, 1f), Projectile.velocity.ToRotation(), 0.4f, 0.2f, 30);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            float scale = 2 * Projectile.scale;
            DrawEnergyBall(Projectile.Center, scale, Projectile.Opacity);
            for (int i = 0; i < oldPos.Count; i++)
            {
                float c = (i + 1f) / oldPos.Count;
                DrawEnergyBall(oldPos[i], scale * c, Projectile.Opacity * c);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public void DrawEnergyBall(Vector2 pos, float size, float alpha)
        {
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(90, 90, 165) * alpha, Projectile.rotation, tex.Size() * 0.5f, new Vector2(1 + (Projectile.velocity.Length() * 0.01f), 1) * size * 0.25f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(40, 40, 255) * alpha, Projectile.rotation, tex.Size() * 0.5f, new Vector2(1 + (Projectile.velocity.Length() * 0.01f), 1) * size * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
        }
    }

    public class HowlingLaser : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(target.Center, Vector2.Zero, new Color(200, 160, 200), new Vector2(1, 1f), 0, 0.2f, 0.8f, 42);
            GeneralParticleHandler.SpawnParticle(pulse);
            for (float i = 0; i < 360; i += 4)
            {
                var d = Dust.NewDustDirect(target.Center, 1, 1, DustID.AncientLight, 0, 0, 0);
                d.velocity = i.ToRadians().ToRotationVector2() * 8;
                d.position = target.Center + i.ToRadians().ToRotationVector2() * 12;
                d.scale = 1.2f;
                
                d.noGravity = true;
            }
            EParticle.NewParticle(new HeavenfallStar(), target.Center, Vector2.Zero, Color.White * 0.6f, 4, 1, true, BlendState.Additive, Projectile.velocity.ToRotation() + MathHelper.PiOver2);
            EParticle.NewParticle(new HeavenfallStar(), target.Center, Vector2.Zero, Color.AliceBlue * 0.6f, 5, 1, true, BlendState.Additive, Projectile.velocity.ToRotation() + MathHelper.PiOver2);
            EParticle.NewParticle(new HeavenfallStar(), target.Center, Vector2.Zero, Color.Blue, 6, 1, true, BlendState.Additive, Projectile.velocity.ToRotation() + MathHelper.PiOver2);
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity.normalize() * 3000, targetHitbox, 16);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.localAI[1]++ == 0)
            {
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(255, 255, 255), 0.5f, 1, true, BlendState.Additive, 0, 12);
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(80, 80, 255), 0.7f, 1, true, BlendState.Additive, 0, 12);

                CEUtils.PlaySound("CrystalBallActive", 1.2f, Projectile.Center);
                for(int i = 80; i < 2000; i += 10)
                {
                    EParticle.NewParticle(new HeavenfallStar(), Projectile.Center + Projectile.velocity.normalize() * i, Vector2.Zero, new Color(20, 20, 160), 2f, 1, true, BlendState.Additive, Projectile.velocity.ToRotation(), 12);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D laser = CEUtils.getExtraTex("DeathRay");
            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/ColorLerp", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, Main.Rasterizer, shader, Main.GameViewMatrix.ZoomMatrix);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(laser, Projectile.Center - Main.screenPosition, new Rectangle(-(int)((Main.GameUpdateCount * 400) % laser.Width), 0, 8000, laser.Height), Color.Blue, Projectile.velocity.ToRotation(), new Vector2(0, laser.Height / 2), new Vector2(1, CEUtils.Parabola(Projectile.timeLeft / 10f, 1)) * Projectile.scale * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();
            
            return false;
        }
    }
}
