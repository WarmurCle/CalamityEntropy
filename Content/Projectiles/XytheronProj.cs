using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class XytheronProj : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Xytheron";
        List<float> odr = new List<float>();
        List<float> ods = new List<float>();
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
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 80;
            Projectile.timeLeft = 100000;
            Projectile.extraUpdates = 7;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotSpeed);
        }
        public int addcharge = 3;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Util.Util.PlaySound("xhit", Main.rand.NextFloat(0.8f, 1.1f), Projectile.Center, 8, volume: 0.32f);
            Util.Util.PlaySound("DevourerDeathImpact", Main.rand.NextFloat(0.8f, 1f), Projectile.Center, 8, volume: 0.32f);
            CalamityEntropy.Instance.screenShakeAmp = 5;
            for (int i = 0; i < 3; i++)
            {
                EParticle.spawnNew(new AbyssalLine(), target.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, Util.Util.randomRot());
            }
            if (Projectile.owner.ToPlayer().HeldItem.ModItem is Xytheron xr)
            {
                if (addcharge > 0)
                {
                    xr.charge += 1;
                    if (xr.charge > 20)
                    {
                        xr.charge = 20;
                    }
                    addcharge--;
                }
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotSpeed = reader.ReadSingle();
        }
        public float scaleD = 0.64f;
        public float rotSpeed = 0f;
        public float rotSpeedJ = 0;
        float glowalpha = 0;
        public bool playsound = true;
        public override void AI()
        {

            float updates = Projectile.MaxUpdates + 1;
            if (Projectile.ai[0] == 0)
            {
                Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.rotation -= 2.42f * Projectile.direction;
            }
            Player owner = Projectile.owner.ToPlayer();
            float meleeSpeed = owner.GetTotalAttackSpeed(Projectile.DamageType) * 1.56f;

            Projectile.Center = owner.MountedCenter + owner.gfxOffY * Vector2.UnitY;
            Projectile.rotation += rotSpeed * meleeSpeed * 0.32f;
            if (Projectile.ai[0] >= 74 && playsound)
            {
                Util.Util.PlaySound("xswing", Main.rand.NextFloat(0.9f, 1.4f), Projectile.Center, 8, 0.8f);
                playsound = false;
            }
            if (Projectile.ai[0] < 60 * updates)
            {
                Projectile.ai[0] = 60 * updates;
            }
            else
            {
                if (Projectile.ai[0] < 86 * updates)
                {
                    rotSpeed += 0.0006f * Projectile.direction * meleeSpeed;
                }
                else
                {
                    rotSpeed *= (float)Math.Pow(0.94, 1.0 / meleeSpeed);
                    if (Projectile.ai[0] > 86 * updates)
                    {
                        rotSpeed *= 0.6f;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.direction = (Main.MouseWorld - owner.Center).X > 0 ? 1 : -1;
                            float targetrot = (Main.MouseWorld - owner.Center).ToRotation() - 2.42f * Projectile.direction;
                            Projectile.rotation = Util.Util.rotatedToAngle(Projectile.rotation, targetrot, 0.05f * meleeSpeed, false);
                        }
                        if (odr.Count > 0)
                        {
                            odr.RemoveAt(0);
                            ods.RemoveAt(0);
                        }
                    }
                    if (Projectile.ai[0] > 94 * updates)
                    {
                        owner.itemTime = 0;
                        owner.itemAnimation = 0;
                        Projectile.Kill();
                        return;
                    }
                }
            }
            Projectile.ai[0] += meleeSpeed;
            odr.Add(Projectile.rotation);
            ods.Add(scaleD);
            if (odr.Count > 60)
            {
                odr.RemoveAt(0);
                ods.RemoveAt(0);
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
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            drawSlash();
            return false;
        }
        public void drawSword()
        {
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Player player = Main.player[Projectile.owner];
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(Util.Util.getExtraTex("StarlessNightGlow"), Projectile.owner.ToPlayer().MountedCenter - Main.screenPosition, null, new Color(180, 180, 255) * glowalpha * 0.8f, Projectile.rotation + (float)Math.PI * 0.25f, new Vector2(32, 168), Projectile.scale * 3f * scaleD, SpriteEffects.None, 0);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.owner.ToPlayer().MountedCenter - Main.screenPosition - Projectile.rotation.ToRotationVector2() * 8, null, Color.White, Projectile.rotation + (float)Math.PI * 0.25f, new Vector2(0, TextureAssets.Projectile[Projectile.type].Value.Height), Projectile.scale * 3f * scaleD, SpriteEffects.None, 0);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public void drawSlash()
        {
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Player player = Main.player[Projectile.owner];

            Texture2D tail = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Extra_201").Value;
            Texture2D tail2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/B1").Value;
            Texture2D tail3 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Noise_10").Value;
            var r = Main.rand;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(100, 100, 100);
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(708 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3((float)i / (float)odr.Count, 1, 1),
                      b));
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(0 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3((float)i / (float)odr.Count, 0, 1),
                      b));
            }

            if (ve.Count >= 3)
            {
                gd.Textures[0] = tail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                ve.Clear();
                for (int i = 0; i < odr.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(708 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((float)i / (float)odr.Count, 1, 1),
                          b));
                    ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(0 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((float)i / (float)odr.Count, 0, 1),
                          b));
                }
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SlashTrans2", AssetRequestMode.ImmediateLoad).Value;

                sb.End();
                sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, shader, Main.GameViewMatrix.TransformationMatrix);
                shader.CurrentTechnique.Passes["EnchantedPass"].Apply();
                gd.Textures[1] = Util.Util.getExtraTex("xt_colormap");

                shader.Parameters["ofs"].SetValue(Main.GlobalTimeWrappedHourly * 1.6f);
                gd.Textures[0] = tail2;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                shader.Parameters["ofs"].SetValue(Main.GlobalTimeWrappedHourly * 3);
                gd.Textures[0] = tail3;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                sb.End();
                sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            }

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            drawSword();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 720 * Projectile.scale * scaleD, targetHitbox, 64);
        }
    }

}