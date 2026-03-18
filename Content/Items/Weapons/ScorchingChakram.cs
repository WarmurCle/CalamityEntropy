using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class ScorchingChakram : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 40;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<ScorchingChakramThrown>();
            Item.shootSpeed = 50f;
            Item.DamageType = CEUtils.RogueDC;
        }

        public override float StealthDamageMultiplier => 1f;
        public override float StealthVelocityMultiplier => 1f;
        public override float StealthKnockbackMultiplier => 2f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 0.5f), knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                CEUtils.SyncProj(p);
                return false;
            }
            return true;
        }
    }
    public class ScorchingChakramThrown : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/ScorchingChakram";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.width = Projectile.height = 80;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }
        public int NoPosUpdate = 0;
        public override bool ShouldUpdatePosition()
        {
            return NoPosUpdate <= 0;
        }
        public List<Vector2> odp = new List<Vector2>();
        public override void AI()
        {
            if (Projectile.localAI[1]++ == 0)
            {
                var snd = SoundID.Item1 with { Volume = 1, MaxInstances = 12 };
                SoundEngine.PlaySound(snd, Projectile.Center); SoundEngine.PlaySound(snd, Projectile.Center); SoundEngine.PlaySound(snd, Projectile.Center);
            }
            odp.Add(Projectile.Center);
            if (odp.Count > 8)
            {
                odp.RemoveAt(0);
            }
            if(NoPosUpdate <= 0)
            {
                Projectile.rotation += 0.016f * Projectile.velocity.Length() * (Projectile.velocity.X > 0 ? 1 : -1);
                Projectile.velocity *= 0.94f;
                Projectile.ai[0]++;
                if (!Projectile.Calamity().stealthStrike)
                {
                    if (Projectile.ai[0] > 50)
                        Projectile.Kill();
                }
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (Projectile.ai[0] >= 50)
                        {
                            NPC target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 1800);
                            if(target != null && Projectile.ai[0] > 100)
                            {
                                if(CEUtils.getDistance(Projectile.Center, target.Center) > 120)
                                    Projectile.velocity += (target.Center - Projectile.Center).normalize() * 3f;
                                else
                                {
                                    Projectile.velocity *= 1.15f;
                                }
                            }
                            if (Projectile.ai[0] <= 100)
                            {
                                int type = ModContent.ProjectileType<ScorchingFireball>();
                                if (target != null && Projectile.ai[0] % 5 == 0)
                                {
                                    CEUtils.PlaySound("YharonFireball1", 1, Projectile.Center);
                                    CEUtils.PlaySound("YharonFireball1", 1, Projectile.Center);
                                    Vector2 shootPos = CEUtils.randomPointInCircle(36) + Projectile.Center;
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPos, (target.Center + target.velocity * 5 - shootPos).normalize() * 54, type, Projectile.damage / 5, 1, Projectile.owner); ;
                                }
                            }
                        }
                    }
                    if (Projectile.ai[0] > 210)
                        Projectile.Kill();
                }
            }
            else
            {
                NoPosUpdate--;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Projectile.localAI[0]++ == 0 || (Projectile.Calamity().stealthStrike && NoPosUpdate <= 0))
            {
                target.AddBuff(BuffID.OnFire3, 100);
                CEUtils.PlaySound("slice", 1, target.Center);
                CEUtils.PlaySound("slice", 1, target.Center);
                CEUtils.PlaySound("slice", 1, target.Center);
                NoPosUpdate = 4;
                for(int i = 0; i < 6; i++)
                {
                    float rot = 2;
                    GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center + Projectile.velocity.normalize() * 20 * Projectile.scale, Projectile.velocity.normalize().RotatedBy(rot).RotatedByRandom(0.3f) * Main.rand.NextFloat(4, 16), false, 16, Projectile.scale * 0.04f, Color.OrangeRed, new Vector2(0.3f, 1), false, false));
                    GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center + Projectile.velocity.normalize() * 20 * Projectile.scale, Projectile.velocity.normalize().RotatedBy(-rot).RotatedByRandom(0.3f) * Main.rand.NextFloat(4, 16), false, 16, Projectile.scale * 0.04f, Color.OrangeRed, new Vector2(0.3f, 1), false, false));
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.PlaySound("RockCrumble", Main.rand.NextFloat(1.2f, 1.5f), Projectile.Center, 8, 0.4f);
            float scale = 120 / 40f;
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red * 0.8f, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White * 0.8f, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 15));
            int num = Projectile.Calamity().stealthStrike ? 10 : 4;
            if (Main.myPlayer == Projectile.owner)
            {
                int type = ModContent.ProjectileType<TectinicShardHoming>();
                float rt = CEUtils.randomRot();
                for (int i = 0; i < num; i++)
                {
                    float tr = (MathHelper.TwoPi / num) * i + rt;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + tr.ToRotationVector2() * 16 * Projectile.scale, tr.ToRotationVector2() * 12, type, Projectile.damage / 4, Projectile.knockBack / 3, Projectile.owner).ToProj().DamageType = Projectile.DamageType;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D tex = Projectile.GetTexture();
            if (odp.Count > 1)
            {
                List<Vector2> poses = new List<Vector2>();
                for (int i = 1; i < odp.Count; i++)
                {
                    for (float j = 0; j < 1; j += 0.2f)
                        poses.Add(Vector2.Lerp(odp[i - 1], odp[i], j));
                }
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                for (int i = 0; i < poses.Count; i ++)
                {
                    float p = ((float)(1 + i) / poses.Count);
                    Color clr = Color.Yellow * 0.8f * p;
                    Main.spriteBatch.Draw(tex, poses[i] - Main.screenPosition, null, clr, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                }
                Main.spriteBatch.ExitShaderRegion();
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class ScorchingFireball : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void PostAI()
        {
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 4)
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
            float scale = 30 / 40f;
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.OrangeRed, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            target.AddBuff(BuffID.OnFire3, 180);
            if (Projectile.timeLeft > 2)
                Projectile.timeLeft = 2;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 25;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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

                        ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 16 * a * Projectile.scale,
                              new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 16 * a * Projectile.scale,
                              new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                              b * a));
                        lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                    }
                    tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MegaStreakBacking2").Value;
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
                CEUtils.DrawGlow(position + Main.screenPosition, Color.White, Projectile.scale * 0.5f);
            }
            return false;
        }
        public int tofs;
    }
}
