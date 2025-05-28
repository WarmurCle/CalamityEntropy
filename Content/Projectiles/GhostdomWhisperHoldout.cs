using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Utilities;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class GhostdomWhisperHoldout : ModProjectile
    {
        public float back = 0;
        public float backspeed = 0;
        public float bamp = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Default;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.ArmorPenetration = 86;
        }
        int ammoID = -1;
        public override void AI()
        {
            bamp *= 0.87f;
            Player player = Projectile.owner.ToPlayer();
            if (player.dead)
            {
                Projectile.Kill();
            }
            if (Projectile.Entropy().IndexOfTwistedTwinShootedThisProj != -1 && !(Projectile.Entropy().IndexOfTwistedTwinShootedThisProj.ToProj().active))
            {
                Projectile.Kill();
            }
            if (player.HasAmmo(player.HeldItem))
            {
                player.PickAmmo(player.HeldItem, out int projID, out float shootSpeed, out int damage, out float kb, out ammoID, true);
            }
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                HandleChannelMovement(player, playerRotatedPoint);
            }
            Projectile.Center = player.MountedCenter + player.gfxOffY * Vector2.UnitY;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center += new Vector2(12, 6 * Projectile.direction).RotatedBy(Projectile.rotation);
            if (Projectile.velocity.X >= 0)
            {
                player.direction = 1;
                Projectile.direction = 1;
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(16 + Projectile.ai[1]));
            }
            else
            {
                player.direction = -1;
                Projectile.direction = -1;
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi - MathHelper.ToRadians(16 + Projectile.ai[1]));
            }
            if (player.HeldItem.type == ModContent.ItemType<GhostdomWhisper>())
            {
                Projectile.timeLeft = 3;
            }
            if (player.channel && !(Projectile.ai[1] >= maxCharge && player.HasBuff<SoyMilkBuff>()))
            {
                bamp = 0;
                player.itemAnimation = 3;
                player.itemTime = 3;
                if (Projectile.ai[1] < maxCharge)
                {
                    Projectile.ai[1] += 1.8f * player.GetTotalAttackSpeed(DamageClass.Ranged) * (1 + player.Entropy().WeaponBoost);
                    if (Projectile.ai[1] >= maxCharge)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/ArtAttackCast"), Projectile.Center);
                    }
                }

            }
            if (!player.channel || (Projectile.ai[1] >= maxCharge && (player.HasBuff<SoyMilkBuff>() || player.Entropy().WeaponBoost > 0)))
            {
                if (Projectile.ai[1] > 16 && player.HasAmmo(player.HeldItem))
                {
                    bamp = 24 * (Projectile.ai[1] / (float)maxCharge);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (player.HeldItem.ModItem is GhostdomWhisper gw)
                        {
                            gw.cs = true;
                        }
                        player.PickAmmo(player.HeldItem, out int projID, out float shootSpeed, out int damage, out float kb, out ammoID, false);
                        if (player.HeldItem.ModItem is GhostdomWhisper gw2)
                        {
                            gw2.cs = false;
                        }
                        int p = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoID), Projectile.Center, new Vector2(shootSpeed, 0).RotatedBy(Projectile.rotation) * (Projectile.ai[1] / (float)maxCharge), projID, (int)(damage * (Projectile.ai[1] / (float)maxCharge) * (Projectile.ai[1] >= maxCharge ? 1.8f : 1) * (Projectile.Entropy().IndexOfTwistedTwinShootedThisProj == -1 ? 1 : TwistedTwinMinion.damageMul)), kb * (Projectile.ai[1] / (float)maxCharge), Projectile.owner);
                        p.ToProj().scale = 1.6f * Projectile.scale;
                        if (Projectile.ai[1] >= maxCharge)
                        {
                            p.ToProj().CritChance = 100;
                            p.ToProj().OriginalCritChance = 100;
                            p.ToProj().Entropy().GWBow = true;
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);

                        }

                    }
                    backspeed = 3.5f * (Projectile.ai[1] / (float)maxCharge);
                    SoundStyle SwingSound = new SoundStyle("CalamityMod/Sounds/Item/TerratomereSwing");
                    SwingSound.Volume = 0.6f;
                    SwingSound.Pitch = 0.4f + 2f * (Projectile.ai[1] / (float)maxCharge);
                    if (Projectile.ai[1] >= maxCharge)
                    {
                        SwingSound = new("CalamityMod/Sounds/NPCKilled/DevourerDeathImpact");
                        SwingSound.Pitch = 1.2f;
                        SwingSound.Volume = 0.46f;

                    }
                    SoundEngine.PlaySound(SwingSound, Projectile.Center);
                }
                Projectile.ai[1] = 0;
            }
            back += backspeed;
            backspeed *= 0.9f;
            back *= 0.9f;
            if (Projectile.ai[1] >= maxCharge)
            {
                float sparkCount = 4;
                for (int i = 0; i < sparkCount; i++)
                {
                    Vector2 sparkVelocity2 = new Vector2(5, 0).RotatedBy((float)Main.rand.NextDouble() * 3.14159f * 2) * Main.rand.NextFloat(0.5f, 1.8f);
                    int sparkLifetime2 = Main.rand.Next(20, 24);
                    float sparkScale2 = Main.rand.NextFloat(0.1f, 0.5f);
                    Color sparkColor2 = Color.DarkBlue;

                    float velc = 0.4f;
                    if (Main.rand.NextBool())
                    {
                        AltSparkParticle spark = new AltSparkParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * 39 - sparkVelocity2 * 8, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        LineParticle spark = new LineParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * 39 - sparkVelocity2 * 8, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? Color.Purple : Color.Purple);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
            player.heldProj = Projectile.whoAmI;

            Projectile.ai[0]++;

        }
        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 16f;
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = newVelocity;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public int maxCharge = 70;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float scaleX = 1 + (1 + (float)Math.Cos((Projectile.ai[1] / (float)maxCharge) * MathHelper.PiOver2 - MathHelper.Pi)) * 0.05f;
            Vector2 scaled = new Vector2(scaleX, 1f / (scaleX * scaleX));
            Vector2 up = Projectile.Center + (new Vector2(-27, -64) * scaled).RotatedBy(Projectile.rotation) + new Vector2(-back, 0).RotatedBy(Projectile.rotation);
            Vector2 down = Projectile.Center + (new Vector2(-27, 64) * scaled).RotatedBy(Projectile.rotation) + new Vector2(-back, 0).RotatedBy(Projectile.rotation);
            Vector2 middle = Projectile.Center + new Vector2(-27 * scaleX - Projectile.ai[1] * 0.64f + Main.rand.NextFloat(-bamp, bamp), 0).RotatedBy(Projectile.rotation) + new Vector2(-back, 0).RotatedBy(Projectile.rotation);
            CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, middle, up, Color.Purple, 2, 2);
            CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, middle, down, Color.Purple, 2, 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vector2> v2ss = new List<Vector2>() { new Vector2(-2, -2), new Vector2(-2, 2), new Vector2(2, -2), new Vector2(2, 2) };
            for (int i = 0; i < 6 * (Projectile.ai[1] / (float)maxCharge) * (Projectile.ai[1] >= maxCharge ? 1f : 0.8f); i++)
            {
                foreach (Vector2 v in v2ss)
                {
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + v + new Vector2(-back, 0).RotatedBy(Projectile.rotation), null, Color.White * (Projectile.ai[1] / (float)maxCharge) * (Projectile.ai[1] >= maxCharge ? 1f : 0.95f), Projectile.rotation, texture.Size() / 2, Projectile.scale * scaled, (Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically));
                }
            }

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(-back, 0).RotatedBy(Projectile.rotation), null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale * scaled, (Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically));



            if (Projectile.ai[1] > 0)
            {
                if (ammoID >= 0)
                {
                    Main.spriteBatch.End();

                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    List<Vector2> v2s = new List<Vector2>() { new Vector2(-2, -2), new Vector2(-2, 2), new Vector2(2, -2), new Vector2(2, 2) };
                    for (int i = 0; i < 6 * (Projectile.ai[1] / (float)maxCharge) * (Projectile.ai[1] >= maxCharge ? 1f : 0.8f); i++)
                    {
                        foreach (Vector2 v in v2s)
                        {
                            Main.EntitySpriteDraw(TextureAssets.Item[ammoID].Value, Projectile.Center + v + new Vector2(-back, 0).RotatedBy(Projectile.rotation) + new Vector2(-Projectile.ai[1] * 0.64f - 32, 0).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.White * (Projectile.ai[1] / (float)maxCharge) * (Projectile.ai[1] >= maxCharge ? 1f : 0.95f), Projectile.rotation - MathHelper.PiOver2, TextureAssets.Item[ammoID].Value.Size() / 2 * new Vector2(1, 0), Projectile.scale * 1.5f * new Vector2(1f, 2.7f), (Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
                        }
                    }

                    Main.spriteBatch.End();

                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                }
                Main.EntitySpriteDraw(TextureAssets.Item[ammoID].Value, Projectile.Center + new Vector2(-back, 0).RotatedBy(Projectile.rotation) + new Vector2(-Projectile.ai[1] * 0.64f - 32 * scaleX, 0).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.White, Projectile.rotation - MathHelper.PiOver2, TextureAssets.Item[ammoID].Value.Size() / 2 * new Vector2(1, 0), Projectile.scale * 1.5f * new Vector2(1, 2.7f), (Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));

            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }

}