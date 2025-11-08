using System;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.ExtraProj;
using CalamityMod;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.MainHammer
{
    public partial class NightmareHammerProj: BaseHammerClass
    {
        private bool _isArcRotating = false;
        private float _arcStartRotation;
        private float TotalArcAngle;
        private float _originalSpeed;
        private float _prevArcAngle;
        private bool _isReverse = false;
        private void DoHanging()
        {
            //标记进入挂载状态
            _isHanging = true;
            //轨迹粒子
            if (AttackTimer > 8)
                DrawTrailingDust();
            
            DrawArc();
            //Timer应延后自增避免出现执行问题
            AttackTimer += 1;
            //敌对单位非空
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                //只有在特定帧后才允许锤子进行挂载
                if (AttackTimer > StartSpinTime * 2)
                {
                    //直接冲向你的敌人
                    Projectile.HomingNPCBetter(target, 1f, 24f, 20f, 1, ignoreDist: true);
                    DoGeneric();
                    return;
                }
                ReleaseDarkEnegry();
            }
        }
        private void ReleaseDarkEnegry()
        {
            /*
            下方是一段基于主射弹当前速度而做出动态变化的射弹生成代码
            表现来说是，衍生射弹的生成频率将会与主射弹的速度会成正比，并尽可能控制在需要的固定间隔内
            也就是，主射弹速度越快，生成频率越高，速度越慢，则生成频率越慢
            这样一定程度上会抑制盗贼弹幕速度加成对输出的影响
            如果你真的很需要调整平衡，请结合下方的注释好好理解
            */
            //基础生成间隔
            const int BaseSpawnSpeed = 20;
            //射弹的飞行速度
            const float BaseTravelSpeed = 22f;
            //最小生成间隔
            const float MinSpawnSpeed = 15f;
            //最大生成间隔
            const float MaxSpawnSpeed = 24;
            //计算当前速度的模长
            float curSpeed = Projectile.velocity.Length();
            //基于射弹速度间隔进行生成刻计算
            float dynamicSpawnSpeed = (BaseTravelSpeed / curSpeed) * BaseSpawnSpeed;
            //控制在合理范围内
            dynamicSpawnSpeed = MathHelper.Clamp(dynamicSpawnSpeed, MinSpawnSpeed, MaxSpawnSpeed);
            //向下取整
            int spawnRates = (int)Math.Round(dynamicSpawnSpeed);
            Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            //将方向转为实际的角度并临时存储进去
            NebulaArrowRotation = direction.ToRotation();
            if (AttackTimer % spawnRates == 0)
            {
                //这里的暗温魔能会必中
                float baseFlareSpeed = Main.rand.NextFloat(12f, 16f);
                //依据锤子当前的速度，以对数的形式给予伤害加成
                int flareDamage = (int)(Projectile.damage + 2 * (float)Math.Log(1 + Projectile.velocity.Length() / 1.5));
                Vector2 velocity = direction * baseFlareSpeed;
                if (Projectile.owner != Main.myPlayer)
                    return;
                //鬼魂音效
                SoundEngine.PlaySound(SoundID.Item103 with { MaxInstances = 4, Pitch = 0.7f });
                //生成
                Projectile flares = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DarkEnergy>(), flareDamage, 1.1f, Owner.whoAmI, 0f, Main.rand.Next(3));
                flares.DamageType = CEUtils.RogueDC;
                flares.Calamity().stealthStrike = true;
                flares.extraUpdates = 3;
                flares.tileCollide = false;
            }
        }
        //圆弧运动总控
        private void DrawArc()
        {
            //如果已反向，且AttackTimer不会在被置零，返回，不执行下方AI
            if (_isReverse && AttackTimer > StartSpinTime * 2)
                return;
            bool firstArc = !_isArcRotating && AttackTimer is 0 && !_isReverse;
            bool secondArc = !_isArcRotating && AttackTimer > StartSpinTime && _isReverse;
            if (firstArc || secondArc)
            {
                float wtf = MathHelper.TwoPi;
                //随机取用角度
                TotalArcAngle = Main.rand.NextBool() ? wtf : -wtf;
                _isArcRotating = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                //尚未反向，缓存这个角度
                if (!_isReverse)
                    _prevArcAngle = TotalArcAngle;
                else
                    TotalArcAngle = -_prevArcAngle;

                Projectile.velocity /= 3;
            }
            if (_isArcRotating)
            {
                //首次画圆，执行0~StartSpin，第二次画圆，执行StartSpinTime ~ StartSpinTime * 2
                float progress = !_isReverse
                    ? (float)AttackTimer / StartSpinTime
                    : (float)(AttackTimer - StartSpinTime) / StartSpinTime;
                Projectile.rotation = _arcStartRotation + TotalArcAngle * progress;
                //加速
                float speed = Projectile.velocity.Length() + 0.21f * AttackTimer;
                if (speed > _originalSpeed)
                    speed = _originalSpeed;

                Projectile.velocity = Projectile.rotation.ToRotationVector2() * speed;
                //如果进程结束
                if (progress >= 1f)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * _originalSpeed;
                    _isArcRotating = false;
                    //首次反向结束，重置计时器准备反向
                    if (!_isReverse)
                    {
                        AttackTimer = StartSpinTime;
                        //标记启用反向状态
                        _isReverse = true;
                    }
                    else 
                        _isReverse = false;
                }
            }
        }
    }
}