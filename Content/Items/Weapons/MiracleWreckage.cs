using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class MiracleWreckage : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 2068;
            Item.DamageType = ModContent.GetInstance<MeleeDamageClass>();
            Item.width = 48;
            Item.height = 60;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MiracleWreckageHeld>();
            Item.shootSpeed = 12f;
        }
        public int atkType = 1;
        public int useCount = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            useCount++;
            Projectile.NewProjectile(source, position, velocity, type, damage * (useCount % 4 == 2 ? 2 : 1), knockback, player.whoAmI, atkType == 0 ? -1 : atkType, useCount % 4 == 2 ? 0.5f : 0);
            atkType *= -1;
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
    public class MiracleWreckageHeld : ModProjectile
    {
        public class MWParticle
        {
            public Vector2 offset = Vector2.Zero;
            public Vector2 velocity;
            public Color color;
            public float scale = Main.rand.NextFloat(1f, 1.6f);
            public MWParticle(Vector2 vel)
            {
                velocity = vel;
                int rcl = Main.rand.Next(2);
                if (rcl == 0)
                {
                    color = Main.rand.NextBool() ? Color.Purple : Color.Pink;
                }
                else
                {
                    color = Main.rand.NextBool() ? Color.MediumPurple : Color.Red;
                }
                color *= 0.6f;
            }
            public int counter = 0;
            public float alpha = 1;
            public void update()
            {
                counter++;
                offset += velocity;
                if(counter > 4)
                {
                    alpha *= 0.7f;
                }
                
            }
        }
        public List<MWParticle> particles = new List<MWParticle>();
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/MiracleWreckage";
        List<float> odr = new List<float>();
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 100000;
            Projectile.MaxUpdates = 14;
            Projectile.light = 1;
        }
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("HalleysInfernoHit", Main.rand.NextFloat(1f, 1.12f), target.Center, 4, 1f * CEUtils.WeapSound, path:"CalamityMod/Sounds/Item/");
            for(int i = 0; i < 32; i++)
            {
                Color clr = Main.rand.NextBool() ? new Color(180, 160, 250) : new Color(210, 160, 255);
                EParticle.NewParticle(new ShadeDashParticle() { c1 = clr, c2 = clr, TL = 12 }, target.Center + CEUtils.randomPointInCircle(26),
                    (target.Center - Projectile.Center).normalize().RotatedByRandom(0.2f) * Main.rand.NextFloat(10, 64), Color.White, 1, 1, true, BlendState.NonPremultiplied, 0, 16);
                ;

            }
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(220, 220, 255), 3, 1, true, BlendState.Additive, 0, 6);
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 255, 255), 1.6f, 1, true, BlendState.Additive, 0, 6);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= Main.rand.NextFloat(1, 1.25f);
        }
        public override void AI()
        {
            Player owner = Projectile.GetOwner();
            float MaxUpdateTimes = owner.itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            counter++;
            if (init)
            {
                CEUtils.PlaySound("HalleysInfernoShoot", Projectile.ai[1] == 0 ? 1.1f : 1.22f, Projectile.Center, volume:0.46f * CEUtils.WeapSound, path: "CalamityMod/Sounds/Item/");
                Projectile.scale *= owner.HeldItem.scale;
                init = false;
                
            }
            Projectile.timeLeft = 3;
            float RotF = 5.4f;
            alpha = 1;
            scale = 1f;

            Projectile.rotation = Projectile.velocity.ToRotation() + (RotF * -0.5f + RotF * CEUtils.GetRepeatedCosFromZeroToOne(progress, 3)) * Projectile.ai[0] * (Projectile.velocity.X > 0 ? -1 : 1);
            Projectile.Center = Projectile.GetOwner().MountedCenter;
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].update();
                if (particles[i].counter > 60 * (Projectile.ai[1] + 0.5f))
                {
                    particles.RemoveAt(i);
                }
                
            }
            for (int i = 0; i < 1; i++)
            {
                particles.Add(new MWParticle(new Vector2(Main.rand.NextFloat(11, 12) * CEUtils.Parabola(progress, 1), 0).RotatedByRandom(0.02f)) { offset = CEUtils.randomPointInCircle(10) });
                particles[particles.Count - 1].color *= CEUtils.Parabola(progress, 1);
            }
            if (progress > 0.36f && progress < 0.74f)
            {
                for (int i = 0; i < 1; i++)
                {
                    float dist = Main.rand.NextFloat();
                    var smokeGlow = new HeavySmokeParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * (80 + dist * 740) * scale * (Projectile.ai[1] + 0.5f), Projectile.rotation.ToRotationVector2().RotatedBy(Projectile.ai[0] * Projectile.GetOwner().direction * -MathHelper.PiOver2) * dist * 30, new Color(60, 30, 80), 10, Main.rand.NextFloat(0.6f, 2f) * (Projectile.ai[1] + 0.5f), 0.8f, 0.008f, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
            if (progress > 0.4f && shoot && Projectile.ai[1] == 0)
            {
                shoot = false;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 3.4f, ModContent.ProjectileType<MiracleShoot>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner);
                }
            }
            if (odr.Count > 2600)
            {
                odr.RemoveAt(0);
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
            if (counter > MaxUpdateTimes)
            {
                owner.itemTime = 1;
                owner.itemAnimation = 1;
                Projectile.Kill();
            }
            odr.Add(Projectile.rotation);
            if (odr.Count > 44)
            {
                odr.RemoveAt(0);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D trail = CEUtils.getExtraTex("StreakGoop");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);

            {
                for (int i = 0; i < odr.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(220 * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(140 * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color2"].SetValue((Color.LightBlue).ToVector4());
                    shader.Parameters["color1"].SetValue((Color.MediumPurple).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(1 - progress);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }
            {
                ve.Clear();
                for (int i = 0; i < odr.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(1000 * (0.5f + Projectile.ai[1]) * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(800 * (0.5f + Projectile.ai[1]) * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color2"].SetValue((new Color(240, 200, 255)).ToVector4());
                    shader.Parameters["color1"].SetValue((Color.Red).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(1 - progress);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }


            int dir = (int)(Projectile.ai[0]);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            float MaxUpdateTime = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;

            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * scale, effect);

            Texture2D g = CEUtils.getExtraTex("Glow");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Texture2D c = CEUtils.getExtraTex("SemiCircularSmear");
            float alphac = (1 + ((float)Math.Cos((progress - 0.5f) * MathHelper.TwoPi))) * 0.5f;
            alphac *= alphac * 0.8f;
            Main.spriteBatch.Draw(c, Projectile.Center - Main.screenPosition, null, Color.MediumPurple * alphac, CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), Projectile.rotation, 0.2f, false), c.Size() / 2f, 4.6f * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(c, Projectile.Center - Main.screenPosition, null, Color.Red * alphac * 0.5f, CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), Projectile.rotation, 0.2f, false), c.Size() / 2f, 7.5f * (Projectile.ai[1] + 0.5f), SpriteEffects.None, 0);

            Main.spriteBatch.Draw(c, Projectile.Center - Main.screenPosition, null, Color.Pink * alphac * 0.6f, CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), Projectile.rotation, 0.2f, false), c.Size() / 2f, 13 * (Projectile.ai[1] + 0.5f), SpriteEffects.None, 0);
            foreach (var p in particles)
            {
                Main.spriteBatch.Draw(g, Projectile.Center + p.offset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, p.color, Projectile.rotation + p.velocity.ToRotation(), new Vector2(40, 128), new Vector2(1f, 0.16f) * p.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * scale * 860 * (Projectile.ai[1] + 0.5f), targetHitbox, 128);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * scale * 860 * (Projectile.ai[1] + 0.5f), 128, DelegateMethods.CutTiles);
        }
    }
    public class MiracleShoot : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.MaxUpdates = 4;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft < 16)
                return false;
            return base.CanHitNPC(target);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = Projectile.timeLeft / 30f;

            Projectile.velocity *= 0.96f;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trail = CEUtils.getExtraTex("MotionTrail5");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<Vector2> p1 = new List<Vector2>();
            List<Vector2> p2 = new List<Vector2>();
            for (float i = -1; i <= 1; i += 0.01f)
            {
                p2.Add(((i * 1f).ToRotationVector2() * new Vector2(1.2f, 1)).RotatedBy(Projectile.rotation) * 10);
                p1.Add(((i * 1.6f).ToRotationVector2() * new Vector2(1.2f, 1)).RotatedBy(Projectile.rotation) * 320);
            }
            for (int i = 0; i < p1.Count; i++)
            {
                Color b = new Color(230, 220, 255);
                ve.Add(new ColoredVertex(Projectile.Center + Projectile.rotation.ToRotationVector2() * -180 - Main.screenPosition + p1[i],
                      new Vector3((i) / ((float)p1.Count - 1), 1, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center + Projectile.rotation.ToRotationVector2() * -180 - Main.screenPosition + p2[i],
                      new Vector3((i) / ((float)p1.Count - 1), 0, 1),
                      b));
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail4", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue(Color.AliceBlue.ToVector4());
                shader.Parameters["color1"].SetValue((Color.MediumPurple).ToVector4());
                shader.Parameters["alpha"].SetValue(Projectile.Opacity);
                shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 200);
                gd.Textures[1] = CEUtils.getExtraTex("PatchyTallNoise");
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;

                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

            }
            return false;
        }
    }

}
