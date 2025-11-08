using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.MainHammer;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.Phantasmal
{
    public class PhantasmalHammerClone : ModProjectile, ILocalizedModType
    {
        //攻击枚举
        private enum AttackStyle
        {
            DoShooted,
            DoReturning,
            DoArcRotating,
            DoReverse
        }
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => BaseHammerClass.TexturePathBase + "GodsHammer";
        //切换至返程玩家的AI时间
        private const float SwitchToReturning = 12f;
        //进行圆弧运动的总时间
        private const float TotalArcDuration = 30;
        //允许的旋转弧度。这里取180
        private const float TotalArcAngle = MathHelper.Pi;
        //是否进行了圆弧运动的初始化
        private bool _initArcRotation = false;
        //进入圆弧前的初始角度
        private float _arcStartRotation;
        //进入圆弧前的初始速度
        private float _originalSpeed;
        private float CurArcRotation = 0f;
        private AttackStyle AttackType
        {
            get => (AttackStyle)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private int TargetIndex => (int)Projectile.ai[2];
        private float InitCenterX => Projectile.localAI[0];
        private float InitCenterY => Projectile.localAI[1];
        private Player Owner => Projectile.GetOwner();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
            Projectile.DamageType = CEUtils.RogueDC;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackType)
            {
                case AttackStyle.DoShooted:
                    IsShooted();
                    break;
                case AttackStyle.DoReturning:
                    IsReturning();
                    break;
                case AttackStyle.DoArcRotating:
                    IsArcRotating();
                    break;
                case AttackStyle.DoReverse:
                    IsReverse();
                    break;
            }
        }
        //制作一个小的圆弧运动
        private void IsArcRotating()
        {
            bool isFlip = Projectile.localAI[0] == 1f;
            float realArcAngle = TotalArcAngle * isFlip.ToDirectionInt();
            //初始化这个圆弧运动
            if (!_initArcRotation)
            {
                _initArcRotation = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                //根据原始速度的大小提供不同的减速
                float decayFactor = 1.2f;
                //结果在 0f ~ 1f 之间
                float normalized = 1f / (1f + _originalSpeed * decayFactor); 
                //映射到 0.4f ~ 0.8f
                float arcSpeed = MathHelper.Lerp(0.6f, 0.9f, normalized);
                arcSpeed = MathHelper.Clamp(arcSpeed, 0.65f, 0.9f);
                //降低初始速度以减小旋转半径
                Projectile.velocity *= arcSpeed;
            }
            if (_initArcRotation)
            {
                //当前进程
                float arcProgress = AttackTimer / (TotalArcDuration /1.5f);
                //物品转角
                CurArcRotation = _arcStartRotation + realArcAngle * arcProgress;
                Projectile.velocity = CurArcRotation.ToRotationVector2() * Projectile.velocity.Length();
                if (arcProgress >= 1f)
                {
                    //恢复初始速度。
                    Projectile.velocity = CurArcRotation.ToRotationVector2() * _originalSpeed;
                    Projectile.netUpdate = true;
                    AttackTimer = 0;
                    AttackType = AttackStyle.DoReverse;
                }
            }
            AttackTimer += 1f;
        }

        private void IsReverse()
        {
            AttackTimer += 1f;
            //在这个过程中持续检查target合法性。
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.HomingNPCBetter(target, 20f + AttackTimer * 0.8f, 10f, 1);
        }

        private void IsReturning()
        {
            AttackTimer += 1f;
            Projectile.AccelerateToTarget(Owner.Center, 20f + AttackTimer / 2f, 1.2f, 3600);
            if(Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //否则切换至第三个状态。
                AttackType = AttackStyle.DoArcRotating;
                Projectile.netUpdate = true;
                Projectile.penetrate = 1;
                AttackTimer = 0f;
            }
        }

        private void IsShooted()
        {
            //初始发射。
            AttackTimer += 1f;
            if(AttackTimer >  SwitchToReturning)
            {
                AttackType = AttackStyle.DoReturning;
                AttackTimer = 0f;
                Projectile.netUpdate = true;
                //收回并拐弯时播报落星的声音
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 1f, Pitch = 0.8f }, Owner.Center);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //从灾厄抄写的锤子特效
            SoundStyle select = Utils.SelectRandom(Main.rand, HammerSoundID.HammerStrike.ToArray());
            SoundEngine.PlaySound(select, Projectile.Center);
            float damageInterpolant = Utils.GetLerpValue(950f, 1600f, hit.Damage, true);
            Vector2 splatterDirection = Projectile.velocity;
            for (int i = 0; i < 8; i++)
            {
                int sparkLifetime = Main.rand.Next(55, 70);
                float sparkScale = Main.rand.NextFloat(0.7f, Main.rand.NextFloat(3.3f, 5.5f)) + damageInterpolant * 0.85f;
                Color sparkColor = Color.Lerp(Color.Purple, Color.GhostWhite, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.HotPink, Main.rand.NextFloat());

                Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.7f) * Main.rand.NextFloat(1f, 1.2f);
                sparkVelocity.Y -= 7f;
                SparkParticle spark = new(Projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (AttackType != AttackStyle.DoReverse)
                return;
            //命中时的圆环
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 0.5f;
            for (int i = 0; i < 36; i++)
            {
                Vector2 dir2 = MathHelper.ToRadians(i * 10f).ToRotationVector2() * 0.5f;
                dir2.X /= 3.6f;
                dir2 = dir2.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 pos = Projectile.Center + dir * 8f + dir2 * 10f;
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(pos, dir2 * 5f, Main.rand.NextBool() ? Color.White : Color.HotPink, 40, (3.5f - Math.Abs(18f - i) / 6f), BlendState.Additive);
                shinyOrbParticle.Spawn();
            }
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HammerSoundID.HammerStrike.ToArray());
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.6f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
        }
        private SpriteBatch SB { get => Main.spriteBatch; }
        #region DrawMethod
        //DrawProjWidth
        public float SetProjWidth(float ratio)
        {
            float width = 50;
            width *= MathHelper.SmoothStep(1.4f, 1.0f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(2f, 5f, Projectile.velocity.Length(), true);
            Color c = GodsHammerProj.TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.08f, ratio, true) * velocityOpacityFadeout;
        }
        //DrawOffset
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            Vector2 pos = Projectile.Size * 0.4f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.5f;
            return pos;
        }
        #endregion

        //TODO：下面那个轨迹把归元漩涡的轨迹改成另外一种，现在这个纯纯占位符
        public override bool PreDraw(ref Color lightColor)
        {
            SB.EnterShaderRegion(BlendState.Additive);
            float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
            GameShaders.Misc["CalamityMod:SideStreakTrail"].UseImage1("Images/Misc/Perlin");
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(SetProjWidth, SetTrailColor, PrimitiveOffsetFunction, shader: GameShaders.Misc["CalamityMod:SideStreakTrail"]), 51);
            SB.ExitShaderRegion();

            Projectile.QuickDrawBloomEdge(Color.HotPink, rotOffset: -MathHelper.PiOver4);
            Projectile.QuickDrawWithTrailing(0.7f, Color.GhostWhite, 4, -MathHelper.PiOver4);
            
            return false;
        }
    }
}