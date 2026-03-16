using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class AshesCore : ModItem
    {
        public static int BaseDamage = 12;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 58;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.expertOnly = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().ashesCore = true;
            player.maxMinions += 1;
            if (Main.myPlayer == player.whoAmI)
                if (player.ownedProjectileCounts[ProjType] < 1)
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjType, BaseDamage, 0, player.whoAmI);
        }
        private static int projType = -1;
        public static int ProjType { get { if (projType == -1) { projType = ModContent.ProjectileType<AshesSpirit>(); } return projType; } }

    }
    public class AshesSpirit : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2500;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.width = Projectile.height = 26;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public NPC target = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CEUtils.RequestTex("CalamityEntropy/Content/Items/Weapons/TectinicShardHoming");
            UnifiedRandom rand = new UnifiedRandom(Projectile.Name.GetHashCode());
            for(int i = 0; i < 9; i++)
            {
                float tr = rand.NextFloat() * MathHelper.TwoPi + (rand.NextBool() ? 1 : -1) * Main.GlobalTimeWrappedHourly * 1;
                SpriteEffects effect = rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float rot = (MathHelper.TwoPi / 9) * i + Projectile.ai[1];
                Vector2 pos = Projectile.Center + rot.ToRotationVector2() * vdist * Projectile.scale;
                Rectangle frame = CEUtils.GetCutTexRect(tex, 3, i % 3, false);
                Vector2 origin = new Vector2(frame.Width * 0.5f, frame.Height * 0.5f);
                Main.EntitySpriteDraw(tex, pos - Main.screenPosition, frame, Color.White, tr, origin, Projectile.scale, effect, 0);
            }
            CEUtils.DrawGlow(Projectile.Center, Color.OrangeRed, 0.8f);
            CEUtils.DrawGlow(Projectile.Center, Color.OrangeRed, 0.5f);
            return false;
        }
        public float vdist = 60;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Center.getRectCentered(vdist, vdist).Intersects(targetHitbox);
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            int distance = 1600;
            if (CEUtils.getDistance(Projectile.Center, player.Center) > 3200)
                Projectile.Center = player.Center;
            if (player.Entropy().ashesCore)
            {
                Projectile.timeLeft = 5;
            }
            else
            {
                Projectile.Kill();
            }
            if (target != null)
            {
                if (!target.active || target.dontTakeDamage || target.Distance(Projectile.Center) > distance)
                    target = null;
            }
            if(target == null)
            {
                target = CEUtils.FindMinionTarget(Projectile, distance);
            }
            if(target != null)
            {
                vdist = float.Lerp(vdist, 80, 0.1f);
                Vector2 targetPos = target.Center + (Projectile.Center - target.Center).normalize() * 320;
                Projectile.velocity *= 0.98f;
                Projectile.velocity += (targetPos - Projectile.Center) * 0.005f;
                
                if (Projectile.ai[0]-- <= 0 && Main.myPlayer == player.whoAmI)
                {
                    Projectile.ai[0] = 30;
                    Vector2 shootPos = Projectile.Center + CEUtils.randomRot().ToRotationVector2() * vdist;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), shootPos, (target.Center - shootPos).normalize() * 36, ModContent.ProjectileType<TectinicShardAshesCore>(), (int)player.GetDamage<SummonDamageClass>().ApplyTo(AshesCore.BaseDamage.ApplyAccArmorDamageBonus()), 5, player.whoAmI);
                }
            }
            else
            {
                Vector2 targetPos = player.Center + new Vector2(-player.direction * 80, -100);
                if(Projectile.Distance(targetPos) > 20)
                {
                    Projectile.velocity *= 0.96f;
                    Projectile.velocity += (targetPos - Projectile.Center).normalize() * 1;
                }
                vdist = float.Lerp(vdist, Projectile.Distance(targetPos) > 160 ? 60 : 40, 0.1f);
            }
            for(float i = 0; i < 1; i += 0.25f)
            {
                var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.FlameBurst);
                d.position = Projectile.Center + Projectile.velocity * i + CEUtils.randomPointInCircle(14); ;
                d.velocity *= 0;
                d.noGravity = true;
                d.scale = 1.24f;
            }
            Projectile.ai[1] += vdist * 0.0032f;
        }
    }
    #region shoot
    public class TectinicShardAshesCore : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/TectinicShardHoming";
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            Main.projFrames[Projectile.type] = 1;
        }
        public override void PostAI()
        {
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 5)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
        }
        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            Color result = new Color(255, 255, 255) * completionRatio;
            return result;
        }

        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 12 * Projectile.scale, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("RockCrumble", Main.rand.NextFloat(1.8f, 2.4f), Projectile.Center, 8, 0.15f);

            float scale = 30 / 40f;
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.OrangeRed, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 15));

        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.timeLeft = 400;
        }
        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Color color = Color.OrangeRed;
            var mp = this;
            if (mp.odp.Count > 1)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = color * 0.66f;
                b.A = 255;
                float a = 0;
                float lr = 0;
                ve.Add(new ColoredVertex(mp.odp[0] - Main.screenPosition + (mp.odp[1] - mp.odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 14 * Projectile.scale,
                          new Vector3(0, 1, 1),
                        b * (1f / (float)mp.odp.Count)));
                ve.Add(new ColoredVertex(mp.odp[0] - Main.screenPosition + (mp.odp[1] - mp.odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 14 * Projectile.scale,
                      new Vector3(0, 0, 1),
                      b * (1f / (float)mp.odp.Count)));
                for (int i = 1; i < mp.odp.Count; i++)
                {
                    a += 1f / (float)mp.odp.Count;

                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 14 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 14 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                          b * a));
                    lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                }
                a = 1;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/wohslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                    ve = new List<ColoredVertex>();
                    b = color;

                    a = 0;
                    lr = 0;
                    for (int i = 1; i < mp.odp.Count; i++)
                    {
                        a += 1f / (float)mp.odp.Count;

                        ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 8 * Projectile.scale,
                              new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 8 * Projectile.scale,
                              new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                              b * a));
                        lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                    }
                    tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/wohslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            tofs++;

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            Main.spriteBatch.ExitShaderRegion();
            if (odp.Count > 1)
            {
                Texture2D texture = Projectile.GetTexture();
                Rectangle frame = CEUtils.GetCutTexRect(texture, 3, Projectile.whoAmI % 3, false);
                Vector2 position = odp[odp.Count - 1] - Main.screenPosition + Vector2.UnitY * base.Projectile.gfxOffY;
                Vector2 origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
                CEUtils.DrawGlow(position + Main.screenPosition, color, Projectile.scale * 0.6f);
                Main.EntitySpriteDraw(texture, position, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            }
            return false;
        }
        public int tofs;
    }
    public class AshesSpiritFireball : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            Main.projFrames[Projectile.type] = 1;
        }
        public override void PostAI()
        {
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 7)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
        }
        public Color TrailColor(float completionRatio, Vector2 vertex)
        {
            Color result = new Color(255, 255, 255) * completionRatio;
            return result;
        }

        public float TrailWidth(float completionRatio, Vector2 vertex)
        {
            return MathHelper.Lerp(0, 14 * Projectile.scale, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float scale = 60 / 40f;
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red * 0.8f, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White * 0.8f, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.timeLeft = 400;
        }
        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Color color = Color.OrangeRed;
            var mp = this;
            if (mp.odp.Count > 1)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = color * 0.66f;
                b.A = 255;
                float a = 0;
                float lr = 0;
                ve.Add(new ColoredVertex(mp.odp[0] - Main.screenPosition + (mp.odp[1] - mp.odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18 * Projectile.scale,
                          new Vector3(0, 1, 1),
                        b * (1f / (float)mp.odp.Count)));
                ve.Add(new ColoredVertex(mp.odp[0] - Main.screenPosition + (mp.odp[1] - mp.odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18 * Projectile.scale,
                      new Vector3(0, 0, 1),
                      b * (1f / (float)mp.odp.Count)));
                for (int i = 1; i < mp.odp.Count; i++)
                {
                    a += 1f / (float)mp.odp.Count;

                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                          b * a));
                    lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                }
                a = 1;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/wohslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                    ve = new List<ColoredVertex>();
                    b = color;

                    a = 0;
                    lr = 0;
                    for (int i = 1; i < mp.odp.Count; i++)
                    {
                        a += 1f / (float)mp.odp.Count;

                        ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 12 * Projectile.scale,
                              new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 12 * Projectile.scale,
                              new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                              b * a));
                        lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                    }
                    tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/wohslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            tofs++;

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor, (_, _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            Main.spriteBatch.ExitShaderRegion();
            if (odp.Count > 1)
            {
                Vector2 position = odp[odp.Count - 1] - Main.screenPosition + Vector2.UnitY * base.Projectile.gfxOffY;
                CEUtils.DrawGlow(position + Main.screenPosition, color, Projectile.scale * 0.75f);
                CEUtils.DrawGlow(position + Main.screenPosition, color * 4, Projectile.scale * 0.5f);
            }
            return false;
        }
        public int tofs;
    }
    #endregion
}
