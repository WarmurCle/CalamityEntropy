using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class TalismanOfTheAzureAbyss : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.defense = 15;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().accAzureAbyss = true;
            player.lifeRegen += 10;
            player.GetDamage(DamageClass.Generic) += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VoidScales>(5).
                AddIngredient<Lumenyl>(5).
                AddIngredient<AscendantSpiritEssence>(2).
                AddTile(ModContent.TileType<VoidWellTile>()).
                Register();
        }
    }
    public class AzureShield : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Generic, false, -1);
            Projectile.timeLeft = 480;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.localAI[1]++ == 0)
                CEUtils.PlaySound("vp_use", 1.25f, Projectile.Center);
            Player player = Projectile.GetOwner();
            if(player.dead)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.Center = player.Center;
            }
            player.Entropy().AzureShield = 3;
            Projectile.ai[0] = (float)(1f + 0.16f * Math.Sin(Main.GameUpdateCount * 0.12f));
            if (Projectile.timeLeft < 32)
                Projectile.ai[0] *= Projectile.timeLeft / 32f;
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
            DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(70, 70) * Projectile.ai[0], new Vector2(32, 32) * Projectile.ai[0], Color.LightBlue, BlendState.Additive);
            return false;
        }
    }
    public class AzureVortex : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Generic, false, -1);
            Projectile.width = Projectile.height = 120;
            Projectile.timeLeft = 250;
            Projectile.light = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.localAI[0] > 16;
        }
        public override void AI()
        {
            Projectile.pushByOther(0.6f);
            if (Projectile.timeLeft >= 50)
                Projectile.ai[0] = float.Lerp(Projectile.ai[0], 1.25f, 0.14f);
            else
            {
                Projectile.ai[0] -= 0.02f;
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
                    if (Projectile.timeLeft >= 60)
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
            CEUtils.DrawGlow(Projectile.Center, Color.LightBlue * 2, 0.4f * Projectile.ai[0]);
            CEUtils.DrawGlow(Projectile.Center, Color.LightBlue * 2, 0.4f * Projectile.ai[0]);
            DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(64, 64) * Projectile.ai[0], new Vector2(1, 1) * Projectile.ai[0], new Color(180, 180, 255), BlendState.Additive);
            return false;
        }
    }
}
