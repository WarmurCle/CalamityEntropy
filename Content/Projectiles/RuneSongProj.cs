using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class RuneSongProj : ModProjectile
    {
        List<float> odr = new List<float>();
        List<float> ods = new List<float>();
        public int noSlowTime = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.ArmorPenetration = 80;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotSpeed);

        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotSpeed = reader.ReadSingle();
        }
        public float scaleD = 1f;
        public float rotSpeed = 0f;
        public override void AI()
        {
            Player owner = Projectile.owner.ToPlayer();
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] == 0 || Projectile.ai[0] == 40)
                {
                    odr.Clear();

                    ods.Clear();
                    rotSpeed = 0;
                    Projectile.velocity = (Main.MouseWorld - Main.LocalPlayer.Center).SafeNormalize(Vector2.One) * 12;
                    if (Projectile.velocity.X > 0)
                    {
                        Projectile.rotation = (Main.MouseWorld - Main.LocalPlayer.Center).ToRotation() - (float)Math.PI * 0.5f;
                    }
                    else
                    {
                        Projectile.rotation = (Main.MouseWorld - Main.LocalPlayer.Center).ToRotation() + (float)Math.PI * 0.5f;
                    }
                    Projectile.netUpdate = true;
                }
                if (Projectile.ai[0] == 20 || Projectile.ai[0] == 60)
                {
                    odr.Clear();
                    ods.Clear();
                    rotSpeed = 0;
                    Projectile.velocity = (Main.MouseWorld - Main.LocalPlayer.Center).SafeNormalize(Vector2.One) * 12;
                    if (Projectile.velocity.X < 0)
                    {
                        Projectile.rotation = (Main.MouseWorld - Main.LocalPlayer.Center).ToRotation() - (float)Math.PI * 0.5f;
                    }
                    else
                    {
                        Projectile.rotation = (Main.MouseWorld - Main.LocalPlayer.Center).ToRotation() + (float)Math.PI * 0.5f;
                    }
                    Projectile.netUpdate = true;
                }
                if (Projectile.ai[0] == 80)
                {

                    odr.Clear();
                    ods.Clear();
                    rotSpeed = 0;
                    Projectile.velocity = (Main.MouseWorld - Main.LocalPlayer.Center).SafeNormalize(Vector2.One) * 12;
                    if (Projectile.velocity.X > 0)
                    {
                        Projectile.rotation = (Main.MouseWorld - Main.LocalPlayer.Center).ToRotation() - (float)Math.PI * 0.7f;
                    }
                    else
                    {
                        Projectile.rotation = (Main.MouseWorld - Main.LocalPlayer.Center).ToRotation() + (float)Math.PI * 0.7f;
                    }
                    Projectile.netUpdate = true;
                }

            }
            if (Projectile.ai[0] == 83)
            {
                noSlowTime = 4;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/CometShardUse"), Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/rswave"), Projectile.Center);
                Projectile.damage *= 2;
                odr.Clear();
                ods.Clear();
                if (Projectile.velocity.X > 0)
                {
                    rotSpeed = 0.74f;
                }
                else
                {
                    rotSpeed = -0.74f;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    float angle = 0;
                    for (int i = 0; i < 6 + Projectile.owner.ToPlayer().Entropy().WeaponBoost * 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(angle) * 5f, ModContent.ProjectileType<Rune>(), (int)(Projectile.damage * 0.6f), 4, Projectile.owner, 0, Main.rand.Next(1, 12));
                        angle += (float)(Math.PI * 2f / (float)(6 + Projectile.owner.ToPlayer().Entropy().WeaponBoost * 2));
                    }
                }
            }
            if (Projectile.ai[0] == 2 || Projectile.ai[0] == 42)
            {
                noSlowTime = 4;
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/sword_spin1"), Projectile.Center);
                odr.Clear();
                ods.Clear();
                if (Projectile.velocity.X > 0)
                {
                    rotSpeed = 0.54f;
                }
                else
                {
                    rotSpeed = -0.54f;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 2 + Projectile.owner.ToPlayer().Entropy().WeaponBoost; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f) * 3, ModContent.ProjectileType<RuneArrow>(), (int)(Projectile.damage * 0.6f), 4, Projectile.owner);
                    }
                }
            }
            if (Projectile.ai[0] == 22 || Projectile.ai[0] == 62)
            {
                noSlowTime = 4;
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/sword_spin1"), Projectile.Center);
                odr.Clear();
                ods.Clear();
                if (Projectile.velocity.X < 0)
                {
                    rotSpeed = 0.54f;
                }
                else
                {
                    rotSpeed = -0.54f;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 2 + Projectile.owner.ToPlayer().Entropy().WeaponBoost; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f) * 3, ModContent.ProjectileType<RuneArrow>(), (int)(Projectile.damage * 0.6f), 4, Projectile.owner);
                    }
                }
            }
            if (noSlowTime <= 0)
            {
                rotSpeed *= 0.8f;
            }
            else
            {
                noSlowTime--;
            }
            scaleD = 0.6f + Math.Abs(rotSpeed) * 2f;
            Projectile.rotation += rotSpeed;
            if (rotSpeed == 0)
            {
                scaleD = 1.6f;
            }
            if (!owner.channel && (Projectile.ai[0] == 110 || Projectile.ai[0] == 19 || Projectile.ai[0] == 39 || Projectile.ai[0] == 59 || Projectile.ai[0] == 79))
            {
                Projectile.Kill();
            }
            if (Projectile.ai[0] == 116)
            {
                Projectile.ai[0] = -1;
                Projectile.damage /= 2;
            }
            owner.itemAnimation = 2;
            owner.itemTime = 2;
            owner.heldProj = Projectile.whoAmI;
            Projectile.ai[0]++;
            Projectile.Center = owner.MountedCenter + owner.gfxOffY * Vector2.UnitY;
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

            if (rotSpeed != 0)
            {
                float ds;
                if (ods.Count > 0)
                {
                    ds = ods[ods.Count - 1];
                }
                else
                {
                    ds = scaleD;
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.8f);
                ods.Add(MathHelper.Lerp(ds, scaleD, 0.2f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.6f);
                ods.Add(MathHelper.Lerp(ds, scaleD, 0.4f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.4f);
                ods.Add(MathHelper.Lerp(ds, scaleD, 0.6f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.2f);
                ods.Add(MathHelper.Lerp(ds, scaleD, 0.8f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
            }
            odr.Add(Projectile.rotation);
            ods.Add(scaleD);
            if (odr.Count > 17)
            {
                odr.RemoveAt(0);
                ods.RemoveAt(0);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Player player = Main.player[Projectile.owner];

            Texture2D tail = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/SplitTrail").Value;
            var r = Main.rand;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = Color.Lerp(new Color(105, 105, 255), new Color(195, 195, 255), (float)i / (float)odr.Count) * 0.8f;
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(280 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3(i / (float)odr.Count, 1, 1),
                      b));
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(0 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3(i / (float)odr.Count, 0, 1),
                      b));
            }

            if (ve.Count >= 3)
            {
                gd.Textures[0] = tail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.owner.ToPlayer().MountedCenter - Main.screenPosition, null, Color.White, Projectile.rotation + (float)Math.PI * 0.25f, new Vector2(0, TextureAssets.Projectile[Projectile.type].Value.Height), Projectile.scale * 3f * scaleD, SpriteEffects.None, 0);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (scaleD < 0.9f || rotSpeed == 0)
            {
                return false;
            }
            return Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 300 * Projectile.scale * scaleD, targetHitbox, 100) || Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + (Projectile.rotation - rotSpeed * 0.5f).ToRotationVector2() * 300 * Projectile.scale * scaleD, targetHitbox, 100);
        }
    }

}