using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class TheDeadCutProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 86;
            Projectile.height = 86;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 20;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public float counter = 0;
        public float scale = 0f;
        public List<float> oldRots = new List<float>();
        public List<float> oldScales = new List<float>();
        public float LScale = 1;
        public Vector2 JW = Vector2.Zero;
        public override void AI()
        {
            Player player = Projectile.GetOwner();

            float MaxUpdateTime = Projectile.MaxUpdates * player.itemTimeMax;
            counter++;
            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            float RotF = 4.8f;
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.ai[0] = Math.Sign(Projectile.velocity.X);
                RotF = MathHelper.TwoPi * 0.75f;
                float dr = RotF * CEUtils.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, 3);
                JW = (MathHelper.Pi * -0.75f).ToRotationVector2().RotatedBy(dr) * new Vector2(1f, 0.32f);

                LScale = JW.Length();
            }

            float l = (float)(Math.Cos(CEUtils.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, 2) * MathHelper.Pi - MathHelper.PiOver2));
            scale = 1.5f + l * 1.5f;
            if (Projectile.Calamity().stealthStrike)
            {
                scale = 5.8f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (RotF * -0.5f + RotF * CEUtils.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, Projectile.Calamity().stealthStrike ? 3 : 2)) * Projectile.ai[0];
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.ai[0] = Math.Sign(Projectile.velocity.X);
                RotF = MathHelper.Pi;
                float dr = RotF * (counter / MaxUpdateTime);
                Projectile.rotation = Projectile.velocity.ToRotation() + JW.ToRotation() * Projectile.ai[0];
            }
            oldRots.Add(Projectile.rotation);
            oldScales.Add(scale * LScale);
            if (oldRots.Count > 160)
            {
                oldScales.RemoveAt(0);
                oldRots.RemoveAt(0);
            }

            Projectile.Center = player.MountedCenter + Vector2.UnitY * player.gfxOffY;
            if (counter > MaxUpdateTime)
            {
                Projectile.Kill();
            }

            if (Projectile.velocity.X > 0)
            {
                player.direction = 1;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            else
            {
                player.direction = -1;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 120 * scale * Projectile.scale, targetHitbox, (int)(140 * Projectile.scale));
        }
        public int addStealth = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.Entropy().WhiteLerp = 2;
            CEUtils.PlaySound("MurasamaHitInorganic", Main.rand.NextFloat(1f, 1.3f), Projectile.Center, 3, 0.6f);
            if (!Projectile.Calamity().stealthStrike)
            {
                Projectile.GetOwner().Calamity().rogueStealth += Projectile.GetOwner().Calamity().rogueStealthMax * (addStealth == 0 ? 0.1f : (addStealth == 1 ? 0.04f : addStealth == 2 ? 0.016f : 0));
                if (Projectile.GetOwner().Calamity().rogueStealth > Projectile.GetOwner().Calamity().rogueStealthMax)
                {
                    Projectile.GetOwner().Calamity().rogueStealth = Projectile.GetOwner().Calamity().rogueStealthMax;
                }
                addStealth++;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D white = this.getTextureGlow();
            Texture2D stealth = CEUtils.getExtraTex("TheDeadCutRotated");
            Texture2D stealthW = CEUtils.getExtraTex("TheDeadCutRotatedWhite");
            if (Projectile.Calamity().stealthStrike)
            {
                tex = stealth; white = stealthW;
            }
            int dir = (int)(Projectile.ai[0]);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            if (Projectile.Calamity().stealthStrike)
            {
                origin = new Vector2(0, tex.Height / 2);
                rot = Projectile.rotation;
                effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            }
            if (!Projectile.Calamity().stealthStrike)
            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    float prog = (float)i / oldRots.Count;
                    Color c = Color.Lerp(Color.Black, Color.White, prog) * prog * prog * 0.05f;
                    float oRot = Projectile.Calamity().stealthStrike ? oldRots[i] : (dir > 0 ? oldRots[i] + MathHelper.PiOver4 : oldRots[i] + MathHelper.Pi * 0.75f);
                    Main.EntitySpriteDraw(white, Projectile.GetOwner().MountedCenter + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, c, oRot, origin, Projectile.Calamity().stealthStrike ? new Vector2(Projectile.scale * oldScales[i] * (0.8f + 0.2f * prog), 1 / 5f * scale) : (Vector2.One * Projectile.scale * oldScales[i] * (0.8f + 0.2f * prog)), effect);
                }
            }
            else
            {
                Texture2D tail = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MotionTrail2").Value;
                var r = Main.rand;
                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                List<ColoredVertex> ve = new List<ColoredVertex>();

                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = Color.Lerp(Color.DarkGray, Color.LightBlue, (float)i / (float)oldRots.Count) * 0.8f * ((float)i / (float)oldRots.Count);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(520 * oldScales[i] * Projectile.scale * 0.2f, 0).RotatedBy(oldRots[i])),
                          new Vector3(i / (float)oldRots.Count, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition,
                          new Vector3(i / (float)oldRots.Count, 0, 1),
                          b));
                }

                if (ve.Count >= 3)
                {
                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    gd.Textures[0] = tail;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
                Main.spriteBatch.ExitShaderRegion();
            }
            float MaxUpdateTime = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            float l = (float)(Math.Cos(CEUtils.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, 3) * MathHelper.Pi - MathHelper.PiOver2));

            Main.EntitySpriteDraw(tex, (Vector2)(CEUtils.GetOwner(Projectile).MountedCenter + CEUtils.GetOwner(Projectile).gfxOffY * Vector2.UnitY - Main.screenPosition), null, lightColor, rot, origin, Projectile.Calamity().stealthStrike ? new Vector2(CEUtils.getDistance(JW, Vector2.Zero), (1 / 5f) * scale * (0.32f + 0.68f * (1 - l)) * 0.4f) : (Vector2.One * Projectile.scale * scale * LScale), effect);
            return false;
        }
    }

}