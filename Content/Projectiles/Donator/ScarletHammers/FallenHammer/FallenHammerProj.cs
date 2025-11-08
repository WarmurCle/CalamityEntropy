using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles;
using CalamityMod;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.FurnitureMonolith;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.PortableExecutable;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.FallenHammer
{
    //Todo：按下鼠标右键后应当刷新一次生命值
    public class FallenHammerProj: BaseHammerClass, ILocalizedModType
    {
        #region 攻击逻辑的枚举        
        internal const short IsAddition = 3;
        #endregion
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth,
            IsAddition
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        #region Typedef
        internal ref bool Update => ref Projectile.netUpdate;
        private bool MouseRight = false;
        public override string Texture => TexturePathBase + "FallenHammer";
        //是否处于旋转状态
        private bool _isArcRotating = false; 
        //旋转起始角
        private float _arcStartRotation;
        //总转角
        private const float TotalArcAngle = MathHelper.Pi;
        //持续帧数
        private const int ArcDuration = 30;
        //转角进度
        private int _arcProgress = 0;
        //发起旋转前的原始速度
        private float _originalSpeed;
        #endregion
        private bool CanEruptionFireBall = false;
        #region 基础数值
        protected override BoomerangDefault BoomerangStat => new
        (
            returnTime: 40,
            returnSpeed: 28f,
            acceleration: 1.5f,
            killDistance: 1000
        );
        protected override BaseProjSD ProjStat => new
        (
            HitCooldown: 10,
            LifeTime: 120,
            Width: 66,
            Height: 66,
            rotation: 0.2f
        );
        //总潜伏攻击时长为五秒
        private int StealthTotalTime => 60 * Projectile.extraUpdates;
        //挂载锤子的攻击频率：5 * 额外更新
        private int HangingHitCooldown => 5 * Projectile.extraUpdates;
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Red);
            DoGeneric();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturning:
                    DoReturning();
                    break;
                case DoType.IsStealth:
                    DoStealth();
                    break;
                case DoType.IsAddition:
                    DoAddition();
                    break;
            }
        }
        

        public override void OnKill(int timeLeft)
        {
            
        }
        //终 极 史 山
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //非右键逻辑下攻击的敌怪传入
            if (AttackType != DoType.IsAddition && !MouseRight)
                TargetIndex = target.whoAmI;

            //如果最终攻击的目标与右键后攻击的目标一致，则处死
            if (AttackType == DoType.IsAddition && TargetIndex == target.whoAmI)
                Projectile.Kill();

            if ((AttackType is DoType.IsStealth || AttackType is DoType.IsAddition) && Projectile.timeLeft < 25 && !CanEruptionFireBall)
            {
                Vector2 center = new Vector2(target.Center.X, target.Center.Y + 30f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, Vector2.Zero, ModContent.ProjectileType<FallenVolcano>(), Projectile.damage * 2, Projectile.knockBack);
                proj.ai[1] = target.whoAmI;
                CanEruptionFireBall = true;
            }
            bool canFuckYou = (AttackType == DoType.IsShooted && !Stealth) || (Stealth && AttackType != DoType.IsReturning);
            bool canFuckSound = (Projectile.numHits < 1 && !Stealth) || Stealth;
            if (canFuckSound)
                SoundEngine.PlaySound(SoundID.Item89 with { MaxInstances = 0, Pitch = 0.8f }, Projectile.Center);
            //爆炸
            if (canFuckYou)
            {
                Projectile fuckYou = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage / 2, Projectile.knockBack, Owner.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                fuckYou.Calamity().stealthStrike = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            Projectile.QuickDrawBloomEdge(targetColor);
            Projectile.QuickDrawWithTrailing(0.5f, Color.White);
            return false;   
        }
        private void PickTagDust(out short Pick)
        {
            switch (Owner.name)
            {
                case "ScarletShelf":
                case "TrueScarlet":
                case "FakeAqua":
                    Pick = DustID.CrimsonTorch;
                    break;
                case "chalost":
                case "查诗络":
                    Pick = DustID.YellowTorch;
                    break;
                case "yinjiu":
                case "银九":
                    Pick = DustID.HallowedTorch;
                    break;
                case "kino":
                    Pick = DustID.BlueTorch;
                    break;
                case "锯角":
                    Pick = DustID.DemonTorch;
                    break;
                case "fr9ezes":
                    Pick = DustID.JungleTorch;
                    break;
                case "kikastorm":
                case "kika":
                    Pick = DustID.WhiteTorch;
                    break;
                default:
                    Pick = DustID.OrangeTorch;
                    break;
            }
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.ToLower())
            {
                case "scarletshelf":
                case "truescarlet":
                case "fakeaqua":

                    baseColor = Color.Red;
                    targetColor = Color.Crimson;
                    break;
                 //查 -- 金
                case "chalost":
                case "查诗络":
                    baseColor = new Color(255, 178, 36);
                    targetColor = Color.Gold;
                    break;
                //银九 - 粉
                case "yinjiu":
                case "银九":
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                //锯角 - 紫
                case "锯角":
                    baseColor = Color.Purple;
                    targetColor = Color.DarkViolet;
                    break;
                //Kino - 蓝
                case "kino":
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                //绿
                case "fr9ezes":
                    baseColor = Color.Green;
                    targetColor = Color.LimeGreen;
                    break;
                //银 - 未指定
                case "kikastorm":
                case "kika":
                    baseColor = Color.Silver;
                    targetColor = Color.White;
                    break;
                default:
                    baseColor = Color.OrangeRed;
                    targetColor = Color.Orange;
                    break;
            }
        }
        private void DoAddition()
        {
            //绘制粒子
            //绘制圆弧转角
            if (!_isArcRotating && AttackTimer is 0)
            {
                _isArcRotating = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _arcProgress = 0;
                _originalSpeed = Projectile.velocity.Length();
                Projectile.velocity *= 0.08f;
            }

            if (_isArcRotating)
            {
                float arcProgress = (float)_arcProgress / ArcDuration;
                //计算当前的角度
                Projectile.rotation = _arcStartRotation + TotalArcAngle * arcProgress;
                //同步旋转角度与速度
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length();
                _arcProgress++;
                if (_arcProgress >= ArcDuration)
                {
                    //AttackTimer标记为1，跳出循环
                    AttackTimer = 1;
                    //重置速度
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * _originalSpeed;
                    _isArcRotating = false;
                }
                return;
            }
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.HomingNPCBetter(target, 1f, 18f, 20f, ignoreDist: true);
            
        }
        public void DrawTrailingDust()
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            //故意不采用循环，因为要稍微处理圆弧状态粒子，但是我技术力不够，先放着了
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 speedValue = direction * 3f;
            Vector2 spawnPosition = Projectile.Center + direction.RotatedBy(MathHelper.PiOver2) * 8f;
            Vector2 realVel = speedValue.RotatedBy(MathHelper.PiOver2);
            ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f, BlendState.Additive);
            shinyOrbParticle.Spawn();

            spawnPosition = Projectile.Center + direction.RotatedBy(-MathHelper.PiOver2) * 8f;
            realVel = speedValue.RotatedBy(-MathHelper.PiOver2);
            ShinyOrbParticle shinyOrbParticle2 = new ShinyOrbParticle(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f, BlendState.Additive);
            shinyOrbParticle2.Spawn();
        }
        
        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackType = DoType.IsReturning;
                AttackTimer = 0;
                Update = true;
            }
        }
        private void DoReturning()
        {
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed, BoomerangStat.Acceleration, BoomerangStat.KillDistance);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                
                //不是潜伏攻击，返回处死射弹
                if (!Stealth)
                {
                    Projectile.Kill();
                    Update = true;
                    return;
                }
                else
                {
                    //二级锤子有极其高频率的攻击方式
                    AttackType = DoType.IsStealth;
                    Update = true;
                    _isHanging = true;
                    Projectile.localNPCHitCooldown = HangingHitCooldown;
                    Projectile.timeLeft = StealthTotalTime;
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 4, MathHelper.PiOver4 / 4)) * Main.rand.NextFloat(14f, 18f);
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir, ModContent.ProjectileType<FallenEruption>(), Projectile.damage, Projectile.knockBack);
                        proj.timeLeft = 300;
                        proj.ai[0] = 25f;
                        proj.extraUpdates = 1;
                    }

                }
            }
        }
        private void DoStealth()
        {
            bool available = Projectile.GetTargetSafe(out NPC target, TargetIndex,true);
            if (Main.mouseRight && MouseRight is false)
            {
                MouseRight = true;
                Update = true;
                Projectile.timeLeft = 300;
            }

            //潜伏状态下，这个锤子会以正常的形式执行五秒内的超高速攻击，锁定敌怪
            if (!MouseRight)
            {
                //神人书架做的神人代码导致这个追踪的东西能秒杀蠕虫
                //你自己看着办
                if (available)
                    Projectile.HomingNPCBetter(target, 1f, 20f, 20f, 1, ignoreDist: true);
            }
            else
            {
                //假定：此时玩家按下了右键，则完全做掉上面的情况，转而冲向玩家的鼠标位置
                if (AttackTimer is 0)
                {
                    SoundEngine.PlaySound(SoundID.Item4 with { MaxInstances = 0, Pitch = 0.5f }, Owner.Center);
                    AttackTimer = 1;
                }
                //冲向鼠标
                Projectile.AccelerateToTarget(Main.MouseWorld, 28f, 1.2f, 1800);
                //除非你没有鼠标，不然这里肯定会在下方赋予成鼠标位置
                Vector2 tar = Main.MouseWorld;
                Rectangle mouseHitBox = new((int)tar.X, (int)tar.Y, Projectile.width, Projectile.height);
                if (Projectile.Hitbox.Intersects(mouseHitBox))
                {
                    //假定，冲向后正常搜索到了敌人，则冲向你的敌人
                    if (available)
                    {
                        TargetIndex = target.whoAmI;
                        //刷新时间
                        Projectile.timeLeft = StealthTotalTime;
                        AttackType = DoType.IsAddition;
                        //临时重置一下
                        AttackTimer = 0;
                        Update = true;
                    }
                    //否则，处死射弹
                    else
                    {
                        SoundEngine.PlaySound(SoundID.Item4 with { MaxInstances = 0, Pitch = 0.5f }, Owner.Center);
                        Projectile.Center.CirclrDust(12, 1.8f, DustID.GemRuby, 12);
                        Projectile.Kill();
                    }
                }
            }
        }
        private void DoGeneric()
        {
            Projectile.rotation += 0.2f;
            if (Stealth)
            {
                DrawTrailingDust();
                return;
            }

            //开潜伏的情况下下面的普攻粒子是全部不生成的
            PickTagDust(out short Pick);
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f;
            //基础横向偏移，用于控制射弹与路径的距离。
            float baseOffset = 1f;
            //让相位差不变，使他们在零点上同步
            float angle = AttackTimer * 0.2f;
            //曲线1使用Sin，曲线2使用-Sin确保反向运动
            float wave = (float)Math.Sin(angle);
            //计算垂直方向向量。
            Vector2 perpendDir = direction.RotatedBy(MathHelper.PiOver2);
            //最终确定生成位置的偏差
            Vector2 waveOffset = perpendDir * wave * 35f + perpendDir * baseOffset;
            //修改粒子生成位置。
            Vector2 spawnPosition = Projectile.Center + waveOffset;
            //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
            float verticleVel = (float)Math.Cos(angle) * 1.2f;
            Vector2 realVel = speedValue + perpendDir * verticleVel;
            //最终生成粒子。
            Dust d = Dust.NewDustPerfect(spawnPosition, Pick, realVel);
            d.scale *= 2.1f;
            d.noGravity = true;
            //全局会一直生成的粒子
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(10, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.SolarFlare, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemRuby, new Vector2(Projectile.velocity.X * 0.15f + velOffset.X, Projectile.velocity.Y * 0.15f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
        }
    }
}