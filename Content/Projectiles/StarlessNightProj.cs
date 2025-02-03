using System;
using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class StarlessNightProj : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/StarlessNight";
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
            Projectile.timeLeft = 40 * 8;
            Projectile.extraUpdates = 7;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotSpeed);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), 0, 0, Projectile.owner);
            Util.Util.PlaySound("he" + (Main.rand.NextBool() ? 1 : 3).ToString(), 1, Projectile.Center);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotSpeed = reader.ReadSingle();
        }
        public float scaleD = 0.64f;
        public float rotSpeed = 0f;
        public float rotSpeedJ = 0;
        public override void AI()
        {
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.ai[1] == 0)
                {
                    Projectile.rotation += 2.2f * Projectile.direction;
                }
                else
                {
                    Projectile.rotation -= 2.6f * Projectile.direction;
                }
            }
            Player owner = Projectile.owner.ToPlayer();
            Projectile.Center = owner.MountedCenter + owner.gfxOffY * Vector2.UnitY;
            Projectile.rotation += rotSpeed;
            if (Projectile.ai[0] == 50)
            {
                EParticle.spawnNew(new Sn(), Projectile.Center, Projectile.velocity * 2.4f, Color.White, 1, 1, true, BlendState.Additive, Projectile.velocity.ToRotation());
            }
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] < 30 + 35 && Projectile.ai[0] > 30)
                {
                    rotSpeedJ += Projectile.direction * -0.00024f;
                }
                else if(Projectile.ai[0] > 30 + 35)
                {
                    rotSpeedJ *= 0.9f;
                    rotSpeed *= 0.916f;
                }
                else
                {
                    rotSpeedJ += Projectile.direction * -0.000005f;
                }
            }
            else
            {
                if (Projectile.ai[0] < 30 + 35 && Projectile.ai[0] > 30)
                {
                    rotSpeedJ -= Projectile.direction * -0.0002365f;
                }
                else if (Projectile.ai[0] > 30 + 35)
                {
                    rotSpeedJ *= 0.9f;
                    rotSpeed *= 0.916f;
                }
                else
                {
                    rotSpeedJ -= Projectile.direction * -0.000005f;
                }
            }
            rotSpeed += rotSpeedJ;
            Projectile.ai[0]++;
            odr.Add(Projectile.rotation);
            ods.Add(scaleD);
            if (odr.Count > 52)
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
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.owner.ToPlayer().MountedCenter - Main.screenPosition, null, Color.White, Projectile.rotation + (float)Math.PI * 0.25f, new Vector2(0, TextureAssets.Projectile[Projectile.type].Value.Height), Projectile.scale * 3f * scaleD, SpriteEffects.None, 0);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public void drawSlash()
        {
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Player player = Main.player[Projectile.owner];

            Texture2D tail = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Extra_201").Value;
            Texture2D tail2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/SwordSlashTexture").Value;
            var r = Main.rand;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(255, 255, 255);
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(570 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3(i / (float)odr.Count, 1, 1),
                      b));
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(0 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3(i / (float)odr.Count, 0, 1),
                      b));
            }

            if (ve.Count >= 3)//因为顶点需要围成一个三角形才能画出来 所以需要判顶点数>=3 否则报错
            {
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SlashTrans", AssetRequestMode.ImmediateLoad).Value;
                Main.instance.GraphicsDevice.Textures[1] = Util.Util.getExtraTex("sn_colormap");
                shader.CurrentTechnique.Passes["EnchantedPass"].Apply();

                sb.End();
                sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, shader, Main.GameViewMatrix.TransformationMatrix);

                gd.Textures[0] = tail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                gd.Textures[0] = tail2;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                sb.End();
                sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            }

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 554 * Projectile.scale * scaleD, targetHitbox, 64);
        }
    }

}