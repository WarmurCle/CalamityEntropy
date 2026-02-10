using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Miracle
{
    public class MiracleVortex : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 260;
            Projectile.light = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.localAI[0] > 16 && Projectile.numHits == 0;
            ;
        }
        public override void AI()
        {
            if(Projectile.numHits == 0)
                Projectile.ai[0] = float.Lerp(Projectile.ai[0], 1, 0.14f);
            else
            {
                Projectile.ai[0] -= 0.07f;
                if (Projectile.ai[0] <= 0)
                    Projectile.Kill();
            }
                Projectile.localAI[0]++;
            NPC target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 1800);
            if (target != null)
            {
                if (Projectile.localAI[0] < 26)
                {
                    Projectile.velocity *= 0.98f;
                    if (Projectile.localAI[0] > 5)
                        Projectile.velocity = (CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 0.1f)).ToRotationVector2() * Projectile.velocity.Length();
                }
                else
                {
                    if (Projectile.numHits == 0)
                    {
                        if (Projectile.localAI[0] > 38)
                        {
                            Projectile.velocity *= 0.96f;
                            Projectile.velocity += (target.Center - Projectile.Center).normalize() * 5f;
                        }
                        else
                        {
                            Projectile.velocity *= 0.97f;
                        }
                    }
                    else
                    {
                        Projectile.velocity *= 0.86f;
                    }
                }
            }
            
            Vector2 d = Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * Projectile.ai[0] * 28;

            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center + d * 1, Projectile.velocity * -0.1f, false, 8, 0.03f * Projectile.ai[0], Color.Violet, new Vector2(0.7f, 1)));
            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center + d * -1, Projectile.velocity * -0.1f, false, 8, 0.03f * Projectile.ai[0], Color.Violet, new Vector2(0.7f, 1)));
            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center + d * 1 + Projectile.velocity / 2f, Projectile.velocity * -0.1f, false, 8, 0.03f * Projectile.ai[0], Color.Violet, new Vector2(0.9f, 1)));
            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center + d * -1 + Projectile.velocity / 2f, Projectile.velocity * -0.1f, false, 8, 0.03f * Projectile.ai[0], Color.Violet, new Vector2(0.9f, 1)));
        }
        public void DrawRing(Vector2 position, Texture2D trail, Vector2 scaleOutside, Vector2 scaleInside, Color color, BlendState blend, bool? drawUpside = null)
        {
            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<Vector2> points1 = new List<Vector2>();
            List<Vector2> points2 = new List<Vector2>();
            for (float i = 0; i <= 1; i += 0.01f)
            {
                Vector2 rv = (i * MathHelper.TwoPi).ToRotationVector2();
                Vector2 p = rv * scaleOutside;
                if (drawUpside.HasValue)
                {
                    if (drawUpside.Value)
                    {
                        if (i == 0)
                            i = 0.5f;
                    }
                    else
                    {
                        if (i > 0.5f)
                            break;
                    }
                }
                rv = (i * MathHelper.TwoPi).ToRotationVector2();
                p = rv * scaleOutside;
                points1.Add(p);
                points2.Add((i * MathHelper.TwoPi).ToRotationVector2() * scaleInside);
            }
            for (int i = 0; i < points1.Count; i++)
            {
                ve.Add(new ColoredVertex(position + points1[i], color, new Vector3(i / 50f + Main.GlobalTimeWrappedHourly, 0, 1)));
                ve.Add(new ColoredVertex(position + points2[i], color, new Vector3(i / 50f + Main.GlobalTimeWrappedHourly, 1, 1)));
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            gd.Textures[0] = trail;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            Main.spriteBatch.ExitShaderRegion();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trail1 = CEUtils.getExtraTex("Streak2Trans");
            CEUtils.DrawGlow(Projectile.Center, Color.MediumVioletRed * 2, 0.4f * Projectile.ai[0]);
            CEUtils.DrawGlow(Projectile.Center, Color.MediumVioletRed * 2, 0.4f * Projectile.ai[0]);
            DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(64, 64) * Projectile.ai[0], new Vector2(1, 1) * Projectile.ai[0], Color.Violet * 1.3f, BlendState.Additive);
            return false;
        }
    }
    public class Blackhole : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.width = Projectile.height = 470;
            if(Main.zenithWorld)
            {
                Projectile.width *= 6;
                Projectile.height *= 6;
            }
            Projectile.timeLeft = 260;
            Projectile.light = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }
        public float scale1 = 1;
        public float alpha1 = 0;
        public float scale2 = 0;
        public float scale3 = 0;
        public override void AI()
        {
            int w = ((int)(Projectile.ai[1]));
            if (w >= 0 && Main.npc.Length > w)
            {
                NPC npc = w.ToNPC();
                if(npc.active)
                {
                    Projectile.Center = npc.Center;
                }
                else
                {
                    Projectile.ai[1] -= -1;
                }
            }
            Projectile.localAI[0]++;
            scale1 -= 0.05f;
            alpha1 += 1 / 8f;
            if (alpha1 > 1)
                alpha1 = 1;
            if (scale1 < 0)
                scale1 = 0;
            if (Projectile.localAI[0] > 9)
            {
                scale2 = float.Lerp(scale2, 1, 0.1f);
            }
            if (Projectile.localAI[0] == 52 || Projectile.localAI[0] == 180)
            {
                EParticle.spawnNew(new AbyssalLine() { xadd = 2.6f, lx = 2.6f, spawnColor = Color.White, endColor = new Color(255, 255, 140) }, Projectile.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, 0, 36);
            }
            if (Projectile.localAI[0] >= 180)
            {
                scale3 = float.Lerp(scale3, 0, 0.3f);
                scale2 *= 0;
            }
            else if (Projectile.localAI[0] >= 60)
            {
                scale3 = float.Lerp(scale3, Main.zenithWorld ? 6 : 1, 0.24f);
                scale2 *= 0.86f;
            }
            if (Projectile.localAI[0] == 60)
                CEUtils.PlaySound("BlackholeSpawn", 1f, Projectile.Center);

            if (CanDamage().Value)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (!n.friendly && !n.boss && !(n.realLife >= 0) && n.Distance(Projectile.Center) < 3000)
                    {
                        n.velocity *= 0.9f;
                        var vec = (Projectile.Center - n.Center).normalize();
                        n.velocity += vec * 6;
                        n.Center += vec * 4;
                    }
                }
            }
        }
        public override bool? CanDamage()
        {
            return Projectile.localAI[0] > 60 && Projectile.localAI[0] < 190;
        }
        public void DrawRing(Vector2 position, Texture2D trail, Vector2 scaleOutside, Vector2 scaleInside, Color color, BlendState blend, bool? drawUpside = null)
        {
            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<Vector2> points1 = new List<Vector2>();
            List<Vector2> points2 = new List<Vector2>();
            for(float i = 0; i <= 1; i += 0.01f)
            {
                Vector2 rv = (i * MathHelper.TwoPi).ToRotationVector2();
                Vector2 p = rv * scaleOutside;
                if(drawUpside.HasValue)
                {
                    if (drawUpside.Value)
                    {
                        if (i == 0)
                            i = 0.5f;
                    }
                    else
                    {
                        if (i > 0.5f)
                            break;
                    }
                }
                rv = (i * MathHelper.TwoPi).ToRotationVector2();
                p = rv * scaleOutside;
                points1.Add(p);
                points2.Add((i * MathHelper.TwoPi).ToRotationVector2() * scaleInside);
            }
            for (int i = 0; i < points1.Count; i++)
            {
                ve.Add(new ColoredVertex(position + points1[i], color, new Vector3(i / 50f + Main.GlobalTimeWrappedHourly, 0, 1)));
                ve.Add(new ColoredVertex(position + points2[i], color, new Vector3(i / 50f + Main.GlobalTimeWrappedHourly, 1, 1)));
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            gd.Textures[0] = trail;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            Main.spriteBatch.ExitShaderRegion();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += target.defense;
            target.Entropy().Decrease20DR = 4;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trail1 = CEUtils.getExtraTex("Streak2Trans");
            Texture2D trail2 = CEUtils.getExtraTex("MegaStreakBacking2b");
            if (scale2 > 0)
            {
                CEUtils.DrawGlow(Projectile.Center, Color.Black, scale2 * 6f, false);
                CEUtils.DrawGlow(Projectile.Center, Color.Black, scale2 * 6f, false);
            }
            if (scale3 > 0)
            {
                float scale = scale3 * 2f;
                float dist = 60;
                float width = 260;
                float hmul = 1.9f;
                float wm = 0.82f;
                Texture2D circle = CEUtils.getExtraTex("VoidMask");
                Vector2 sq = new Vector2(1, (float)Math.Pow(Math.Abs(Projectile.Center.Y - Main.LocalPlayer.Center.Y) * 0.00016f, 0.5f) * ((Projectile.Center.Y - Main.LocalPlayer.Center.Y) > 0 ? 1 : -1));
                CEUtils.DrawGlow(Projectile.Center, Color.White * 0.8f, 6 * scale);
                float num = 2.4f;
                DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(dist + width * wm, dist + width * wm) * scale * sq, new Vector2(dist, dist) * scale * sq, new Color(225, 225, 160, 255), BlendState.Additive, true);
                DrawRing(Projectile.Center - Main.screenPosition, CEUtils.pixelTex, new Vector2(dist + width * 0.46f, dist + width * 0.46f) / hmul * scale, Vector2.Zero, Color.Black, BlendState.NonPremultiplied);
                DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(dist + width, dist + width) / hmul * scale, new Vector2(dist, dist) * scale / hmul, new Color(255, 255, 200), BlendState.Additive);
                DrawRing(Projectile.Center - Main.screenPosition, trail2, new Vector2(dist * num + width * 0.08f, dist * num + width * 0.08f) / hmul * scale, new Vector2(dist * num, dist * num) * scale / hmul, new Color(255, 255, 140) * 0.5f, BlendState.Additive);
                DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(dist + width * wm, dist + width * wm) * scale * sq, new Vector2(dist, dist) * scale * sq, new Color(225, 225, 160, 255), BlendState.Additive, false);
            }
            if(alpha1 > 0)
            {
                DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(1200, 1200) * scale1, new Vector2(800, 800) * scale1, new Color(255, 255, 230) * alpha1, BlendState.Additive);
            }
            return false;
        }
    }

}
