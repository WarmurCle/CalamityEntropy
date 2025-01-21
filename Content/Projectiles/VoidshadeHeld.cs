using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
	public class VoidshadeHeld : ModProjectile
	{
        public override void SetDefaults()
        {
			Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.timeLeft = 120;
			Projectile.extraUpdates = 1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 55;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
		public int attackType { get { return (int)Projectile.ai[0]; } }
        public List<Vector2> oldPos = new List<Vector2>();
        public List<float> oldRots = new List<float>();
		public List<float> oldScale = new List<float>();
        public bool init = false;
		public float rotSpeed = 0;
		public int counter = 0;
        public float cspeed = 0;
        public float c = 0;
        public int dash = 30;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			if(counter > 6 && counter < 60)
			{
				return Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 130 * Projectile.scale * getScale(), targetHitbox, 36);
			}
			return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(dash > 0 && attackType == 3)
            {
                dash = -32;

                Projectile.owner.ToPlayer().velocity *= -0.2f;
                Projectile.owner.ToPlayer().Entropy().voidshadeBoostTime = 90;
            }
        }
        public bool vsboost = false;
        public override void AI()
        {
            
			Player player = Projectile.owner.ToPlayer();
            if (attackType == 3)
            {
                if (counter > 9)
                {
                    dash--;

                    if (dash > 0)
                    {
                        player.velocity = Projectile.velocity * 2;
                    }
                    else
                    {
                        if (dash > -30)
                        {
                            player.velocity *= 0.88f;
                        }
                    }
                }
                if (counter == 20)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Projectile.velocity, ModContent.ProjectileType<WohLaser>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 0, 0, 2).ToProj().DamageType = Projectile.DamageType;
                    }
                }
                player.heldProj = Projectile.whoAmI;
                if (!init)
                {
                    init = true;
                    Projectile.direction = (Projectile.velocity.X > 0 ? 1 : -1);
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(16 * Projectile.direction);
                }
                if (counter < 20)
                {
                    cspeed += 0.01f;
                }
                else
                {
                    cspeed -= 0.014f;
                }
                Projectile.Center = player.Center + Projectile.rotation.ToRotationVector2() * c * 18 * getScale() - Projectile.rotation.ToRotationVector2() * 60 * getScale();
                c += cspeed;
                Projectile.rotation += -0.003f * c * Projectile.direction;
                counter++;
                if(counter > 60)
                {
                    Projectile.Kill();
                }
                if (counter < 36)
                {
                    if (counter % 2 == 1)
                    {
                        oldPos.Add(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-10) * Projectile.direction) * 130 * getScale() * Projectile.scale);
                    }
                    else
                    {
                        oldPos.Add(Projectile.Center + player.velocity / 2 + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-10) * Projectile.direction) * 130 * getScale() * Projectile.scale);

                    }
                    oldRots.Add(Projectile.rotation);
                    oldScale.Add(1);
                }
            }
            else
            {
                if(counter == 1 && player.Entropy().voidshadeBoostTime > 0)
                {
                    player.Entropy().voidshadeBoostTime = 0;
                    vsboost = true;
                    Projectile.damage *= 2;
                }
                if(counter == 16)
                {
                    if(Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Projectile.velocity * 0.8f, ModContent.ProjectileType<VoidImpact>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack, Projectile.owner);
                    }
                }
                if (!init)
                {
                    init = true;
                    Projectile.direction = (Projectile.velocity.X > 0 ? 1 : -1);
                    if (Projectile.direction == 1)
                    {
                        if (attackType == 0)
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180 - 20);
                        }
                        else
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(180 - 20);
                        }
                    }
                    else
                    {
                        if (attackType != 0)
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(180 - 20);
                        }
                        else
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180 - 20);
                        }
                    }
                }
                counter++;
                rotSpeed *= 0.9f;
                if (counter < 30)
                {
                    if (attackType == 0)
                    {
                        rotSpeed += -0.02f * Projectile.direction;
                    }
                    else
                    {
                        rotSpeed += 0.02f * Projectile.direction;
                    }
                }
                if (counter > 90)
                {
                    Projectile.Kill();
                }
                Projectile.rotation += rotSpeed * Projectile.direction;
                Player owner = player;
                if (counter <= 60)
                {
                    if (Projectile.velocity.X > 0)
                    {
                        owner.direction = 1;
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI / 2));
                    }
                    else
                    {
                        owner.direction = -1;
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI / 2));
                    }
                }
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
                player.heldProj = Projectile.whoAmI;
                if (counter < 49)
                {
                    if (counter % 2 == 1)
                    {
                        oldPos.Add(Projectile.Center);
                    }
                    else
                    {
                        oldPos.Add(Projectile.Center + player.velocity / 2);
                    }
                    oldRots.Add(Projectile.rotation - rotSpeed * Projectile.direction);
                    oldScale.Add(getScale() * Projectile.scale);
                }
                
            }
            if (oldPos.Count > 24 || (counter >= 49 && oldPos.Count > 0))
            {
                oldPos.RemoveAt(0);
                oldRots.RemoveAt(0);
                oldScale.RemoveAt(0);
            }
        }
        public float trailOffset = 0;
        public float getScale()
		{
			/*float x = (float)counter / 60f;
            x = Math.Clamp(x, 0, 1);

            double deviation = Math.Abs(x - 0.5);

            float value = (float)Math.Exp(-deviation * 6);
*/          
            if(attackType == 3)
            {
                return 2;
            }
            return 1.2f + Math.Abs(rotSpeed) * 6 * (vsboost ? 1.5f : 1);
		}
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.06f;
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Texture2D trail;
            var r = Main.rand;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();

            for (int i = 0; i < oldRots.Count; i++)
            {
                Color b = Color.Lerp(Color.Purple * 0.01f, Color.Purple, ((float)(i)) / (float)oldRots.Count) * 1f;
                if (attackType == 3)
                {
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 16 * Projectile.scale * getScale() * ((float)(oldPos.Count - i - 1) / (float)oldPos.Count),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 1, 1),
                          b));
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 16 * Projectile.scale * getScale() * ((float)(oldPos.Count - i - 1) / (float)oldPos.Count),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 1, 1),
                          b));
                }
                else
                {
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2() * (42 * oldScale[i] + 80 * oldScale[i] * (1 - (float)(i) / (float)oldRots.Count) * 0.5f),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 1, 1),
                          b));
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2() * (42 * oldScale[i] + 80 * oldScale[i] - 80 * oldScale[i] * (1 - ((float)(i) / (float)oldRots.Count)) * 0.5f),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 0, 1),
                          b));
                }
            }

            if (ve.Count >= 3)
            {
                trail = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            ve = new List<Vertex>();

            for (int i = 0; i < oldRots.Count; i++)
            {
                Color b = Color.Lerp(Color.White * 0.01f, Color.White, ((float)(i)) / (float)oldRots.Count) * 1f;
                if (attackType == 3)
                {
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 16 * Projectile.scale * getScale() * ((float)(oldPos.Count - i - 1) / (float)oldPos.Count),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 1, 1),
                          b));
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 16 * Projectile.scale * getScale() * ((float)(oldPos.Count - i - 1) / (float)oldPos.Count),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 1, 1),
                          b));
                }
                else
                {
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2() * (42 * oldScale[i] + 80 * oldScale[i] * (1 - (float)(i) / (float)oldRots.Count) * 0.5f),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 1, 1),
                          b));
                    ve.Add(new Vertex(oldPos[i] - Main.screenPosition + oldRots[i].ToRotationVector2() * (42 * oldScale[i] + 80 * oldScale[i] - 80 * oldScale[i] * (1 - ((float)(i) / (float)oldRots.Count)) * 0.5f),
                          new Vector3(i / (float)oldRots.Count + trailOffset, 0, 1),
                          b));
                }
            }

            if (ve.Count >= 3)
            {
                trail = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/SwordSlashTexture").Value;
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (counter <= 60)
            {
                Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
                int dir = 1;
                if (attackType == 0)
                {
                    dir = -1;
                }
                if(attackType == 3)
                {
                    if(Projectile.velocity.X < 0)
                    {
                        dir *= -1;
                    }
                }
                Main.EntitySpriteDraw(tex, Projectile.Center + (Projectile.rotation.ToRotationVector2() * -4) * getScale() * Projectile.scale - Main.screenPosition, null, Color.White, Projectile.rotation + (dir > 0 ? MathHelper.ToRadians(50) : MathHelper.ToRadians(130)), (dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height)), Projectile.scale * getScale(), (dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
            }
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}