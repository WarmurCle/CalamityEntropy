using CalamityEntropy.Common;
using CalamityEntropy.Core.Construction;
using CalamityEntropy.Utilities;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using InfernumMode.Content.BehaviorOverrides.BossAIs.Dragonfolly;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.MainHammer
{
    public class NebulaEnegry : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private enum DoType
        {
            IsHomingToTarget,
            IsArcRotating
        };
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public enum Flip
        {
            None,
            DoFlip
        }
        private ref float TargetIndex => ref Projectile.ai[0];
        private ref float Accele => ref Projectile.ai[1];
        private bool IsHit = false; 
        private ref float CurRotation => ref Projectile.localAI[0];
        private ref float Progress => ref Projectile.localAI[2];
        private ref float SpriteRotation => ref Projectile.localAI[1];
        private EGlobalProjectile ModProj => Projectile.Entropy();
        private bool Differ
        {
            get => ModProj.ExtraProjAI[0] == 1f;
            set => ModProj.ExtraProjAI[0] = value ? 1f : 0f;
        }
        private float _arcStartRotation;
        private float _originalSpeed;
        float Length = 16;
        public override string Texture => CEUtils.InvisAsset;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.height = 8;
            Projectile.width = 8;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 500;
            Projectile.friendly = true;
            if (Projectile.velocity != Vector2.Zero)
                Projectile.velocity /= Projectile.extraUpdates;
        }
        private float DrawScale = 1f;
        public override void AI()
        {
            //初始化
            if (Accele is 0)
            {
                InitDust(Projectile.Center, Projectile.velocity);
                SpawnFlyingDust();
                Accele += 1;
                Progress = 45f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            SpriteRotation += MathHelper.ToRadians(1f);
            SpriteRotation %= MathHelper.TwoPi;
            if (SpriteRotation < 0)
                SpriteRotation += MathHelper.TwoPi;
            switch (AttackType)
            {
                case DoType.IsArcRotating:
                    DoArcRotating();
                    break;
                case DoType.IsHomingToTarget:
                    DoHomingToTarget();
                    break;
            }
        }

        private void DoArcRotating()
        {
            float progress = (Progress / 45f);
            DrawScale = EasingHandler.EaseOutCubic(progress).ToClamp();
            Projectile.velocity *= 0.923f;
            Progress--;
            if (progress <- 15f)
                Projectile.Kill();
        }

        private void DoHomingToTarget()
        {
            //获取敌对单位
            //重新搜索一次单位
            if (!Projectile.GetTargetSafe(out NPC target, (int)TargetIndex))
                return;
            
            Projectile.HomingNPCBetter(target, 24f + Accele / 2f, 20f, 2);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //首次命中时开始转角拐弯
            if (!IsHit)
            {
                IsHit = true;
                AttackType = DoType.IsArcRotating;
                Projectile.netUpdate = true;
            }
        }
        private void SpawnFlyingDust()
        {
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(14, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                float dFlyVelX = Projectile.velocity.X * 0.4f + velOffset.X;
                float dFlyVelY = Projectile.velocity.Y * 0.4f + velOffset.Y;
                float dScale = 0.8f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.WitherLightning, new Vector2(dFlyVelX, dFlyVelY), 100, default, dScale);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.TwoPi);
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                float dFlyVelX = Projectile.velocity.X * 0.5f + velOffset.X;
                float dFlyVelY = Projectile.velocity.Y * 0.5f + velOffset.Y;
                float dScale = 0.8f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.GemDiamond, new Vector2(dFlyVelX, dFlyVelY), 100, default, dScale);
                dust.noGravity = true;
            }
        }
        private void InitDust(Vector2 spawnPos, Vector2 dir)
        {
            int particlesCounts = 24;
            float baseRot = dir.ToRotation() + MathHelper.PiOver2;
            for (int i = 0; i < particlesCounts; i++)
            {
                //角度步长
                float angleStep = (float)(MathHelper.TwoPi / particlesCounts);
                //粒子角度
                float angle = i * angleStep;
                //转化为椭圆点
                Vector2 toEdge = spawnPos + angle.ToEllipseVector2Edge(10f, 30f, baseRot);
                //设置速度，略微朝内
                GlowOrbParticle orb = new(toEdge, Vector2.Zero, false, 80, 0.7f, GodsHammerProj.TrailColor);
                GeneralParticleHandler.SpawnParticle(orb);
            }
            int maxK = 16;
            for (int k = 0; k < maxK; k++)
            {
                Vector2 randOffset = Main.rand.NextVector2Circular(4f, 2f);
                Vector2 yetPos = Projectile.Center + randOffset;
                float speed = MathHelper.Clamp(2f, 0f, 2f);
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * speed;
                float scale = MathHelper.Lerp(0.7f, 0f, k / 16);
                scale = MathHelper.Clamp(scale, 0f, 0.7f);
                GlowOrbParticle orb2 = new(yetPos, vel, false, 60, scale, GodsHammerProj.TrailColor);
                GeneralParticleHandler.SpawnParticle(orb2);
            }
        }
        private SpriteBatch SB { get => Main.spriteBatch; }
        private GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Trail = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1w").Value;
            Texture2D StarLine = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/StarTexture").Value;
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //做掉可能存在的零向量。
            Projectile.ClearInvalidPoint(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);

            //直接获取需要的贝塞尔曲线。
            List<VertexPosition2DColorTexture> list = [];
            //创建顶点列表
            for (int i = 0; i < validPosition.Count; i++)
            {
                Vector2 worldCenter = validPosition[i] + Projectile.Size / 2;
                Vector2 oldCenter = worldCenter - Main.screenPosition;
                float progress = (float)i / (validPosition.Count - 1);
                Vector2 posOffset = new Vector2(0, 6 *DrawScale).RotatedBy(validRot[i]);
                Color color = GodsHammerProj.TrailColor;
                VertexPosition2DColorTexture upClass = new(oldCenter + posOffset, color, new Vector2(progress, 0), 0f);
                VertexPosition2DColorTexture downClass = new(oldCenter - posOffset, color, new Vector2(progress, 1), 0f);
                list.Add(upClass);
                list.Add(downClass);    
            }
            if (list.Count >= 3)
            {
                GD.Textures[0] = Trail;
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
                GD.Textures[0] = null;
            }
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D sharpTears = StarLine;
            Color projColor = Color.White;
            projColor.A = 0;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SB.Draw(sharpTears, drawPos, null, Color.White, Projectile.rotation, sharpTears.Size() / 2, 0.36f * Projectile.scale * new Vector2(1.4f, 0.5f) * DrawScale, SpriteEffects.None, 0);

            SB.End();
            SB.BeginDefault();
            return false;
        }
    }
}