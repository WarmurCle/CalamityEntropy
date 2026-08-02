using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.LoreItems;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public class Fooveria : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 48;
            Item.height = 60;
            Item.useTime = 24;
            Item.crit = 12;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FooveriaHeld>();
            Item.shootSpeed = 12f;
            Item.Entropy().Legend = true;
            Item.Calamity().CannotBeEnchanted = true;
            LastLevel = -1;
            UpdateInventory(Main.LocalPlayer);
        }
        public int useCounter = 0;
        public int atkType = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            damage *= useCounter % 4 == 3 ? 2 : 1;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, useCounter % 4 == 3 ? 1 : Main.rand.NextBool() ? 1 : -1, useCounter % 4 == 3 ? 1 : 0);
            atkType *= -1;
            useCounter++;
            return false;
        }
        public override bool MeleePrefix()
        {
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Item.QuickDrawItemWithBloomToWorld(spriteBatch, Color.SkyBlue, ref scale, rotation);
            return false;
        }
        public override void AddRecipes()
        {

        }
        public static int GetLevel()
        {
            int Level = 0;
            bool flag = true;
            void Check(bool f)
            {
                if (f && flag)
                {
                    Level++;
                }
                else
                {
                    flag = false;
                }
            }

            Check(NPC.downedSlimeKing);
            Check(NPC.downedBoss1);
            Check(DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator);
            Check(DownedBossSystem.downedSlimeGod);
            Check(Main.hardMode);
            Check(NPC.downedMechBossAny);
            Check(NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3);
            Check(NPC.downedPlantBoss);
            Check(NPC.downedGolemBoss);
            Check(NPC.downedMoonlord);
            Check(DownedBossSystem.downedProvidence);
            Check(DownedBossSystem.downedDoG);
            Check(DownedBossSystem.downedYharon);
            Check(DownedBossSystem.downedExoMechs || DownedBossSystem.downedCalamitas);
            Check(DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs);

            return Level;

        }
        public int LastLevel = -1;
        public override void UpdateInventory(Player player)
        {
            int level = GetLevel();
            if (LastLevel != level)
            {
                int dmg = Item.damage;
                switch (level)
                {
                    case 0: dmg = 12; break;
                    case 1: dmg = 18; break;
                    case 2: dmg = 24; break;
                    case 3: dmg = 28; break;
                    case 4: dmg = 36; break;
                    case 5: dmg = 80; break;
                    case 6: dmg = 100; break;
                    case 7: dmg = 120; break;
                    case 8: dmg = 140; break;
                    case 9: dmg = 150; break;
                    case 10: dmg = 480; break;
                    case 11: dmg = 600; break;
                    case 12: dmg = 1250; break;
                    case 13: dmg = 1400; break;
                    case 14: dmg = 1600; break;
                    case 15: dmg = 3600; break;
                }
                Item.damage = dmg;
                Item.useTime = Item.useAnimation = int.Max(10, 16 - GetLevel() / 4);
                LastLevel = level;
                Item.crit = level * 3;
                Item.knockBack = level / 2;
                Item.scale = 1;
                Item.Prefix(Item.prefix);
            }
            if (player.HeldItem == Item)
            {
                player.Entropy().BBarNoDecrease = 120;
                player.Calamity().mouseWorldListener = true;
            }
        }
        public override bool AllowPrefix(int pre)
        {
            return true;
        }
    }
    public class FooveriaHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/Fooveria";
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 100000;
            Projectile.MaxUpdates = 16;
        }
        public float rotRP = 0;
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;
        public bool RightHold = true;
        public bool LeftClicked = false;
        public bool Spin = false;
        public bool snd = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (snd)
            {
                CEUtils.PlaySound("GrassSwordHitMetal", Main.rand.NextFloat(0.7f, 1.3f), target.Center, 10, CEUtils.WeapSound * 0.7f);
                if (target.Organic())
                {
                }
                else
                {
                    CEUtils.PlaySound("metalhit", Main.rand.NextFloat(0.8f, 1.2f), target.Center, 6, CEUtils.WeapSound * 0.7f);
                }
                CEUtils.PlaySound("GrassSwordHit" + Main.rand.Next(4).ToString(), 1, target.Center, 16, CEUtils.WeapSound * 0.7f);
                snd = false;
            }
            Color impactColor = Color.LightBlue;
            float impactParticleScale = Main.rand.NextFloat(1.4f, 1.6f);

            SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.SkyBlue, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);

            float sparkCount = 16 + Fooveria.GetLevel() / 2;
            for (int i = 0; i < sparkCount; i++)
            {
                float p = Main.rand.NextFloat();
                Vector2 sparkVelocity2 = (target.Center - Projectile.Center).normalize().RotatedByRandom(p * 0.4f) * Main.rand.NextFloat(6, 20 * (2 - p)) * (1 + Fooveria.GetLevel() * 0.1f) * 0.7f;
                int sparkLifetime2 = (int)((2 - p) * 16);
                float sparkScale2 = 0.6f + (1 - p);
                sparkScale2 *= (1 + Fooveria.GetLevel() * 0.06f);
                Color sparkColor2 = Color.Lerp(Color.LightSkyBlue, Color.AliceBlue, p);
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.LightBlue : Color.SkyBlue);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            if (Projectile.ai[2] > 0)
            {
                Projectile.Kill();
                Projectile.GetOwner().Entropy().noItemTime = 40;
            }
        }
        public float rScale = 1;
        public float slashP = Main.rand.NextFloat(0.2f, 0.3f);
        public override void AI()
        {
            CEUtils.AddLight(Projectile.Center + Projectile.velocity.normalize() * 20 * Projectile.scale, Color.LightBlue, Projectile.scale);
            Player owner = Projectile.GetOwner();
            if (owner.dead)
            {
                Projectile.Kill();
                return;
            }
            float MaxUpdateTimes = owner.itemTimeMax * Projectile.MaxUpdates;
            if (Projectile.ai[2] == 1)
            {
                MaxUpdateTimes *= 2;
                slashP = 1;
            }
            float progress = (counter / MaxUpdateTimes);
            counter++;
            if (init)
            {
                Projectile.ai[1] *= Projectile.velocity.X > 0 ? 1 : -1;
                CEUtils.PlaySound("powerwhip", Projectile.ai[2] == 1 ? 1f : 1.4f, Projectile.Center, 12, 0.6f);
                Projectile.scale = 1f + 0.05f * Fooveria.GetLevel();
                float scale_ = owner.HeldItem.scale;
                owner.ApplyMeleeScale(ref scale_);
                Projectile.scale *= scale_;
                init = false;
                if (Main.myPlayer == Projectile.owner)
                {

                }
            }
            float p = Main.rand.NextFloat();
            rScale = 1;
            Projectile.timeLeft = 3;
            alpha = 1;
            float cr = MathHelper.ToRadians(10);
            float RotF = 4.5f; 
            float rot = progress <= 0.5f ? ((RotF * -0.5f + CEUtils.Parabola(progress, RotF + cr)) * Projectile.ai[1]) : ((RotF * 0.5f + cr - CEUtils.GetRepeatedCosFromZeroToOne(2 * (progress - 0.5f), 1) * cr) * Projectile.ai[1]);
            Vector2 v = rot.ToRotationVector2() * new Vector2(1, slashP);
            Projectile.rotation = v.ToRotation() + Projectile.velocity.ToRotation();
            Projectile.Center = Projectile.GetOwner().GetDrawCenter();
            rScale = v.Length();
            scale = 1.4f;

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
            if (progress < 0.45f)
            {
                SpawnParticle();
                odr.Add(Projectile.rotation);
                ods.Add(rScale);
                if (odr.Count > 120)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
            }
            else
            {
                if (odr.Count > 0)
                {
                    ods.RemoveAt(0);
                    odr.RemoveAt(0);
                }
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public bool NoDraw = false;
        public Vector2 lPos = Vector2.Zero;
        public void SpawnParticle()
        {
            Vector2 vpos = Projectile.rotation.ToRotationVector2() * rScale * scale * Projectile.scale * 116;
            if (lPos == Vector2.Zero)
            {
                lPos = vpos;
                return;
            }

            Vector2 sparkVelocity2 = (vpos - lPos).normalize() * 4 * Projectile.ai[1];
            int sparkLifetime2 = (int)(Main.rand.NextFloat() * 16);
            float sparkScale2 = 0.6f * Main.rand.NextFloat();
            sparkScale2 *= (1 + Fooveria.GetLevel() * 0.06f);
            Color sparkColor2 = Color.Lerp(Color.AliceBlue, Color.LightBlue, Main.rand.NextFloat());
            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 116 * scale * Projectile.scale * rScale;
            if (Main.rand.NextBool())
            {
                AltSparkParticle spark = new AltSparkParticle(pos, sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            else
            {
                LineParticle spark = new LineParticle(pos, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.LightBlue : Color.SkyBlue);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            EParticle.spawnNew(new GlowLightParticle() { lightColor = Color.LightBlue * 0.5f, HideTime = 16 }, Projectile.Center + Projectile.rotation.ToRotationVector2() * 100 * scale * Projectile.scale * rScale * Main.rand.NextFloat(0.25f, 1), sparkVelocity2 * 0.2f, Color.LawnGreen, Main.rand.NextFloat(0.1f, 0.2f) * scale * Projectile.scale, 1, true, BlendState.Additive, 0, 20);
            lPos = vpos;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (NoDraw)
            {
                return false;
            }
            Texture2D tex = Projectile.GetTexture();
            Texture2D trail = CEUtils.getExtraTex("MotionTrail2");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(220, 255, 200);
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(116 * Projectile.scale * scale * ods[i], 0).RotatedBy(odr[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(32 * Projectile.scale * scale * ods[i], 0).RotatedBy(odr[i])),
                      new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                      b));
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue((new Color(90, 120, 255)).ToVector4());
                shader.Parameters["color1"].SetValue((new Color(60, 200, 255)).ToVector4());
                shader.Parameters["alpha"].SetValue(1 - progress);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                trail = CEUtils.getExtraTex("SplitTrail");
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                Main.spriteBatch.ExitShaderRegion();
            }


            int dir = (int)(Projectile.ai[1]);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            float MaxUpdateTime = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            Main.spriteBatch.UseAdditiveClamp();
            if (Projectile.ai[2] > 0)
            {
                for(float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver4)
                {
                    Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition + i.ToRotationVector2() * 4, null, lightColor * alpha, rot, origin, Projectile.scale * scale * rScale * 0.6f, effect);
                }
            }
            Main.spriteBatch.ExitShaderRegion();
            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * scale * rScale * 0.6f, effect);
            Main.spriteBatch.UseAdditiveClamp();
            if (Projectile.ai[2] > 0)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.White * alpha, rot, origin, Projectile.scale * scale * rScale * 0.6f, effect);
                Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.White * alpha, rot, origin, Projectile.scale * scale * rScale * 0.6f, effect);
            }
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 120 * Projectile.scale * scale * rScale, targetHitbox, 64);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 120 * Projectile.scale * scale * rScale, 54, DelegateMethods.CutTiles);
        }
    }

}
