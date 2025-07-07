using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AntiVoid : ModItem, IDevItem
    {
        public string DevName => "ChaLost";

        public override void SetStaticDefaults()
        {

            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 5));
        }
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 144;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 8;
            Item.useAnimation = 16;
            Item.autoReuse = true;
            Item.scale = 2f;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.damage = 100;
            Item.knockBack = 4;
            Item.crit = 6;
            Item.shoot = ModContent.ProjectileType<AntivoidSlash>();
            Item.shootSpeed = 12;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
        }
        public override bool AltFunctionUse(Player player)
        {
            return !player.HasCooldown(AntivoidDashCooldown.ID);
        }
        public override bool CanShoot(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
            }
            else
            {
                Item.useTime = 8;
                Item.useAnimation = 16;
            }
            return base.CanShoot(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<AntivoidDash>();
                damage *= 4;
                player.AddCooldown(AntivoidDashCooldown.ID, 15);
                player.RemoveAllGrapplingHooks();
                if(player.mount.Active) 
                    player.mount.Dismount(player);
            }
            else
            {
                velocity = CEUtils.randomRot().ToRotationVector2() * velocity.Length();
            }
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.rand.NextBool() ? -1 : 1);
            return false;
        }


        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
        }
    }

    public class AntivoidSlash : ModProjectile
    {
        List<float> odr = new List<float>();
        List<float> odl = new List<float>();
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
            Projectile.MaxUpdates = 12;
        }
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.SetShake(target.Center, 6, 3000);
            CEUtils.PlaySound("antivoidhit", Main.rand.NextFloat(0.8f, 1.2f), target.Center);
            Color impactColor = Color.LightBlue;
            float impactParticleScale = Main.rand.NextFloat(1.4f, 1.6f);

            SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.Blue, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);


            float sparkCount = 32;
            for (int i = 0; i < sparkCount; i++)
            {
                float p = Main.rand.NextFloat();
                Vector2 sparkVelocity2 = (target.Center - Projectile.Center).normalize().RotatedByRandom(p * 0.4f) * Main.rand.NextFloat(6, 34 * (2 - p));
                int sparkLifetime2 = (int)((2 - p) * 7);
                float sparkScale2 = 0.6f + (1 - p);
                Color sparkColor2 = Color.Lerp(Color.DeepSkyBlue, Color.Purple, p);
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (Projectile.frame == 7 ? 1f : 0.65f), false, (int)(sparkLifetime2 * (Projectile.frame == 7 ? 1.2f : 1f)), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : Color.Firebrick);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
        public float RotF = 0;
        public float rc = 0;
        public float yc = 0;
        public override void AI()
        {
            if(yc == 0)
            {
                yc = Main.rand.NextFloat(0.3f, 0.8f);
            }
            Player owner = Projectile.GetOwner();
            float MaxUpdateTimes = 14 * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            counter++;
            if (init)
            {
                CEUtils.PlaySound("antivoiduse", Main.rand.NextFloat(0.7f, 1.4f), Projectile.Center);
                Projectile.scale *= owner.HeldItem.scale;
                init = false;
            }
            Projectile.timeLeft = 3;
            if(RotF == 0)
            {
                RotF = Main.rand.NextFloat(MathHelper.ToRadians(340), MathHelper.ToRadians(400));
                rc = Main.rand.NextFloat(0.2f, 0.4f);
            }
            alpha = 1;
            scale = 1f;
            float cr = MathHelper.ToRadians(140);
            Vector2 jw = ((RotF * -rc + CEUtils.Parabola(progress * 0.5f, RotF))).ToRotationVector2() * new Vector2(1, yc);
            lg = (jw).Length();

            Projectile.rotation = Projectile.velocity.ToRotation() + jw.ToRotation() * Projectile.ai[0];

            Projectile.Center = Projectile.GetOwner().MountedCenter;

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
            if (counter > MaxUpdateTimes)
            {
                Projectile.Kill();
            }
            odr.Add(Projectile.rotation);
            odl.Add(lg);
            if (odr.Count > 110)
            {
                odl.RemoveAt(0);
                odr.RemoveAt(0);
            }
        }
        public float lg = 0;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public Texture2D tex => Projectile.GetTexture();
        public float rofs = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            rofs += 0.01f;
            Texture2D trail = CEUtils.getExtraTex("RuneRibbon");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = 14 * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            float ofs = 0;
            for (int i = 0; i < odr.Count; i++)
            {
                Color b = new Color(220, 200, 255);
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(170 * Projectile.scale * odl[i], 0).RotatedBy(odr[i])),
                      new Vector3(ofs, 1, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(Projectile.scale * odl[i], 0).RotatedBy(odr[i])),
                      new Vector3(ofs, 0, 1),
                      b));
                if (i < odr.Count - 1) {
                    ofs += 1f / odr.Count;
                }
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/AntivoidTrail", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["alpha"].SetValue(1f - progress);
                shader.Parameters["offset"].SetValue(rofs);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();
                gd.Textures[1] = CEUtils.getExtraTex("MotionTrail3");
                gd.Textures[0] = trail;
                gd.Textures[2] = CEUtils.getExtraTex("Extra_202");
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }


            int dir = (int)(Projectile.ai[0]);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.ToRadians(70 - 24) : Projectile.rotation + +MathHelper.ToRadians(110 + 24);


            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha * (1 - progress), rot, origin, (Projectile.scale) * scale * lg, effect);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (160 * lg) * Projectile.scale * scale, targetHitbox, 64);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (160 * lg) * Projectile.scale * scale, 54, DelegateMethods.CutTiles);
        }
    }

    public class AntivoidDash : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public AntivoidTrail trail;
        public StarTrailParticle trail2;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, true, -1);
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
                CEUtils.PlaySound("AntivoidDash", 1, Projectile.Center);
            }
            var player = Projectile.GetOwner();
            player.Entropy().immune = 5;
            if(trail == null)
            {
                trail = new AntivoidTrail();
                trail2 = new StarTrailParticle() { addPoint = false, maxLength = 60 };
                EParticle.spawnNew(trail, Projectile.Center, Vector2.Zero, new Color(40, 10, 80, 255), 1, 1, true, BlendState.NonPremultiplied);
                EParticle.spawnNew(trail2, Projectile.Center, Vector2.UnitX * 0.1f, new Color(255, 20, 20, 255), 1, 1, true, BlendState.Additive);
            }
            trail.AddPoint(Projectile.Center + Projectile.velocity);
            trail2.Position = (Projectile.Center + new Vector2(0, -10));
            trail2.AddPoint(trail2.Position);
            trail.Lifetime = trail2.Lifetime = 30;
            if (Projectile.ai[1]++ > 20)
            {
                Projectile.velocity *= 0.98f;
            }
            player.Center = Projectile.Center;
            player.velocity = Projectile.velocity;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2]++ == 0)
            {
                CEUtils.PlaySound("AntivoidDashSlash", 1, target.Center);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.GetOwner().Center -= Projectile.velocity;
            return base.OnTileCollide(oldVelocity);
        }
    }
}
