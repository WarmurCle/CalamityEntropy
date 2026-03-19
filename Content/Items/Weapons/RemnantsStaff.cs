using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class RemnantsStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.width = 60;
            Item.height = 78;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 10;
            Item.UseSound = CEUtils.GetSound("beast_lavaball_rise1");
            Item.shoot = ModContent.ProjectileType<TectinicShardHoming>();
            Item.shootSpeed = 18f;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }
        public int attackCount = 0;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlowerofFire)
                .AddIngredient<TectonicShard>(6)
                .AddTile(TileID.Hellforge)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity = velocity.RotatedBy(player.direction * -0.16f);
            position = position + velocity.normalize() * 80;
            if (attackCount % 2 == 0)
            {
                type = ModContent.ProjectileType<RemnantsLaser>();
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI).ToProj();
            }
            else
            {
                type = ModContent.ProjectileType<ScorchingFireballMagic>();
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.75f, 1f), type, damage / 4, knockback / 4, player.whoAmI);
                }
            }
            attackCount++;
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedBy(player.direction * 0.16f);
        }
        public override bool MagicPrefix()
        {
            return true;
        }
    }
    public class ScorchingFireballMagic : ModProjectile
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
            if (odp.Count > 8)
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
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 400;
            Projectile.MaxUpdates = 2;
        }
        public override void AI()
        {
            if (Projectile.localAI[2]++ > 12)
                Projectile.HomingToNPCNearby(3f, 0.92f);
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
    public class RemnantsLaser : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 5 * 60);
            float scale = 100 / 40f;
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, Color.Red * 0.8f, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, Color.White * 0.8f, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            GeneralParticleHandler.SpawnParticle(new CustomPulse(target.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 26));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(target.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(target.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 22));
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 8000;
        }
        public int counter = 0;
        public int length = 2000;
        NPC ownern = null;
        public float width = 0;
        public int aicounter = 0;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;
        }
        public bool st = true;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 16;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (st)
            {
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(255, 255, 20), 0.6f, 1, true, BlendState.Additive, 0, 14);
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(255, 255, 160), 0.29f, 1, true, BlendState.Additive, 0, 14);
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(255, 255, 160), 0.29f, 1, true, BlendState.Additive, 0, 14);
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(255, 255, 160), 0.29f, 1, true, BlendState.Additive, 0, 14);
                CEUtils.PlaySound("CrystalBallActive", 0.6f + Main.rand.NextFloat(-0.2f, 0.2f), Projectile.Center, 10, 0.4f);
                st = false;
            }
            if (Projectile.timeLeft < 6)
            {
                width -= 1f / 16f;
            }
            else
            {
                width += 1f / 16f;

            }
            aicounter++;
            float maxlength = 2000;
            for (float i = 0; i < maxlength; i += 8)
            {
                Vector2 v = Projectile.Center + Projectile.rotation.ToRotationVector2() * i;
                length = (int)i;
                if (!CEUtils.inWorld(v) || (Main.tile[(int)(v.X / 16), (int)(v.Y / 16)].IsTileSolid()))
                {
                    break;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * length, targetHitbox, 30);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            counter++;
            Texture2D tex = CEUtils.getExtraTex("DeathRay2");
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied, SamplerState.LinearWrap);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(-counter * 18, 0, length, tex.Height), Color.Yellow, Projectile.rotation, new Vector2(0, tex.Height / 2), new Vector2(1, width * 0.8f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(-counter * 26, 0, length, tex.Height), new Color(255, 255, 90), Projectile.rotation, new Vector2(0, tex.Height / 2), new Vector2(1, width * 0.6f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(-counter * 40, 0, length, tex.Height), Color.White, Projectile.rotation, new Vector2(0, tex.Height / 2), new Vector2(1, width * 0.5f), SpriteEffects.None, 0);
            Main.spriteBatch.UseBlendState(BlendState.Additive, SamplerState.LinearWrap);

            Texture2D star = CEUtils.getExtraTex("StarTexture");
            float num = 0.5f * (float)Math.Sin(Main.GameUpdateCount / 10f);
            float num2 = 0.5f * (float)Math.Sin(Main.GameUpdateCount / 10f + MathHelper.PiOver4);
            Vector2 pos = Projectile.Center;
            Color color = new Color(225, 190, 40);
            Main.spriteBatch.Draw(star, pos - Main.screenPosition, null, color * Projectile.Opacity, 0, star.Size() / 2f, new Vector2(1 + num, 1 - num) * Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos - Main.screenPosition, null, color * Projectile.Opacity, 0, star.Size() / 2f, new Vector2(1 - num2, 1 + num2) * Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos - Main.screenPosition, null, color * Projectile.Opacity, MathHelper.PiOver4, star.Size() / 2f, new Vector2(1 + num, 1 - num) * Projectile.scale * 0.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos - Main.screenPosition, null, color * Projectile.Opacity, MathHelper.PiOver4, star.Size() / 2f, new Vector2(1 - num2, 1 + num2) * Projectile.scale * 0.2f, SpriteEffects.None, 0);
            Vector2 epos = Projectile.Center + Projectile.rotation.ToRotationVector2() * length;
            Main.spriteBatch.Draw(star, epos - Main.screenPosition, null, color * Projectile.Opacity, 0, star.Size() / 2f, new Vector2(1 + num, 1 - num) * Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, epos - Main.screenPosition, null, color * Projectile.Opacity, 0, star.Size() / 2f, new Vector2(1 - num2, 1 + num2) * Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, epos - Main.screenPosition, null, color * Projectile.Opacity, MathHelper.PiOver4, star.Size() / 2f, new Vector2(1 + num, 1 - num) * Projectile.scale * 0.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, epos - Main.screenPosition, null, color * Projectile.Opacity, MathHelper.PiOver4, star.Size() / 2f, new Vector2(1 - num2, 1 + num2) * Projectile.scale * 0.2f, SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();

            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
