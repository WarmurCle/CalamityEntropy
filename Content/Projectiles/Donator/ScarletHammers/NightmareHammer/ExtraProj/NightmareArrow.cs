using CalamityEntropy.Assets.Register;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles;
using CalamityEntropy.Core.Construction;
using CalamityEntropy.Utilities;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.ExtraProj
{
    public class NightmareArrow : BaseScarletProj
    {
        private enum DoType
        {
            IsSpawned,
            IsChasing,
            IsHit
        }
        public override string Texture => CEUtils.InvisAsset;
        //新建一个
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private int TargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 360;
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.friendly = true;
        }
        public override bool ShouldUpdatePosition() => AttackType != DoType.IsSpawned;
        public override bool? CanDamage() => AttackType != DoType.IsSpawned; 
        public override void AI()
        {
            //直接在AI里写死刷新，下方会手动控制这个射弹的处死逻辑
            Lighting.AddLight(Projectile.Center, TorchID.White);
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackType)
            {
                case DoType.IsSpawned:
                    DoSpawned();
                    break;
                case DoType.IsChasing:
                    DoChasing();
                    break;
                case DoType.IsHit:
                    DoHit();
                    break;
            }
            if (AttackType != DoType.IsChasing)
                Projectile.timeLeft = 150;

            if ((Projectile.Center - Owner.Center).Length() > 2400f)
                Projectile.Kill();

            DrawTrailingDust();

        }
        float amp2 = 35f;
        private void DrawTrailingDust()
        {
            //正弦波频率
            float freq = 0.2f;
            //振幅
            if(AttackType is DoType.IsHit)
            {
                amp2 -= 1f;
                if (amp2 < 5f)
                    amp2 = 5f;
            }
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f;
            for (int i = 0; i < 2; i++)
            {
                int side = i == 0 ? 1 : -1;
                //基础横向偏移，用于控制射弹与路径的距离。
                float baseOffset = 5f;
                //让相位差不变，使他们在零点上同步
                float angle = AttackTimer * freq;
                //曲线1使用Sin，曲线2使用-Sin确保反向运动
                float wave = (float)Math.Sin(angle);
                if (i == 1) wave = -wave;
                //计算垂直方向向量。
                Vector2 perpendDir = direction.RotatedBy(MathHelper.PiOver2);
                //最终确定生成位置的偏差
                Vector2 waveOffset = perpendDir * wave * amp2 + perpendDir * baseOffset;
                //修改粒子生成位置。
                Vector2 spawnPosition = Projectile.Center + waveOffset;
                //计算例子速度，粒子需要在零点反向运动。因为总体上，他们是在原点位置被“推开”的
                //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
                float verticleVel = (float)Math.Cos(angle) * 1.2f;
                if (i == 1) verticleVel = -verticleVel;
                Vector2 realVel = speedValue + perpendDir * verticleVel;
                //最终生成粒子。
                
                //跳过屏幕外绘制
                if (CEUtils.OutOffScreen(spawnPosition))
                    continue;
                Color drawColor = i == 0 ? Color.Black : new(75, 0, 130);
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, drawColor, 140, 1.2f, i != 0 ? BlendState.Additive : BlendState.AlphaBlend);
                shinyOrbParticle.Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType != DoType.IsChasing)
                return;

            Projectile.netUpdate = true;
            AttackTimer = 0f;
            AttackType = DoType.IsHit;
        }
        #region AI方法合集
        private void DoChasing()
        {
            if(Projectile.GetTargetSafe(out NPC target, TargetIndex,true))
                Projectile.HomingNPCBetter(target, 24f, 10f, 2);

            AttackTimer += 1;
        }
        private void DoHit()
        {
            AttackTimer += 1;
            float progress = AttackTimer / 45f;
            Projectile.velocity *= 0.987f;
            Projectile.scale = MathHelper.Lerp(1f, 0f, progress);
            if (Projectile.scale is 0f)
                Projectile.Kill();
        }
        private void DoSpawned()
        {
            //获取相对差
            float xOffset = Projectile.localAI[0];
            float yOffset = Projectile.localAI[1];
            //转化为实际点位
            Vector2 offsetPos = new(-xOffset, -yOffset);
            //获取目标位置。
            if(Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                //更新射弹位置
                Projectile.Center = target.Center + offsetPos;
            }
            AttackTimer += 1;
            float progress = AttackTimer / 5f;
            Projectile.scale = MathHelper.Clamp(EasingHandler.EaseInCubic(progress), 0f, 1f);
            //给予一个向下的初速度。
            Projectile.velocity = new Vector2(0f, 4f);
            //速度。
            if (AttackTimer < 25f)
                return;

            CEUtils.PlaySound("SwordHit0", 1.4f, Owner.Center, 4, 0.8f);
            AttackType = DoType.IsChasing;
            AttackTimer = 0;
            Projectile.netUpdate = true;
            //重新给予射弹向上的初速度。
        }
        #endregion
        private void DrawTrail(Color color, int height, int width = 0)
        {
            //干掉可能存在零向量
            Projectile.ClearInvalidPoint(out List<Vector2> validPositions, out List<float> validRot);
            int drawPointTime = 4;
            //自定义尝试获取贝塞尔曲线
            BezierCurveHandler.GetValidBeizerCurvePow(validPositions, validRot, out List<Vector2> smoothPositions, out List<float> smoothRots, drawPointTime);
            List<VertexPosition2DColorTexture> vertexList = [];
            for (int i = 0; i < smoothPositions.Count; i++)
            {
                Vector2 worldCenter = smoothPositions[i] + Projectile.Size / 2;
                float progress = (float)i / (smoothPositions.Count - 1);
                Vector2 posOffset = new Vector2(0, height).RotatedBy(smoothRots[i]);
                Vector2 oldCenter = worldCenter - Main.screenPosition;
                //添加顶点
                VertexPosition2DColorTexture UpClass = new(oldCenter + posOffset, color, new Vector2(progress, 0), 0);
                VertexPosition2DColorTexture DownClass = new(oldCenter - posOffset, color, new Vector2(progress, 1), 0);
                vertexList.Add(UpClass);
                vertexList.Add(DownClass);
            }
            if (vertexList.Count >= 3)
            {
                GraphicsDevice GD = Main.graphics.GraphicsDevice;
                //我不会用Shader，我只能先这样搞
                Texture2D value = TextureRegister.Trail_RvSlash.Value;
                //贴图
                GD.Textures[0] = value;
                //绘制
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertexList.ToArray(), 0, vertexList.Count - 2);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch SB = Main.spriteBatch;
            Texture2D projTex =  ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/StarTrail").Value;
            SB.Draw(projTex, Projectile.Center - Main.screenPosition, null, Color.Black, Projectile.rotation, projTex.Size() / 2, new Vector2(0.7f, 0.5f), SpriteEffects.None, 0f);
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //需要绘制第二个，作为辉光效果
            SB.Draw(projTex, Projectile.Center - Main.screenPosition, null, Color.DarkViolet, Projectile.rotation, projTex.Size() / 2, new Vector2(0.5f, 0.3f), SpriteEffects.None, 0f);

            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawTrail(Color.Black, 10);

            SB.End();
            SB.BeginDefault();

            return false;
        }
    }
}