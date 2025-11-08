using System;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer.Beam;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer.ExtraProj;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Biomes;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer
{
    public partial class PunishmentHammerProj: BaseHammerClass
    {
        
        #region 攻击逻辑的枚举
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
        #endregion

        #region 一些其他的东西，如果你真的很需要调整平衡，就修改这里
        protected override BaseProjSD ProjStat => new(
            HitCooldown: 20,
            LifeTime: 300,
            Width: 66,
            Height: 66,
            rotation: 0.2f
        );
        protected override BoomerangDefault BoomerangStat => new(
            returnTime: 30,
            acceleration: 0.7f,
            returnSpeed: 12f,
            killDistance: 1800
        );
        //总的挂载时间
        private const int TotalSpinTime = 1800;
        private int MountedIndex = -1;
        #endregion
        #region Typedef
        //没啥必要，我写这个纯因为觉得长单词麻烦
        internal ref bool Update => ref Projectile.netUpdate;
        public static readonly SoundStyle AdditionHitSigSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { MaxInstances = 0, Volume = 0.80f };
        public override string Texture => TexturePathBase + "PunishmentHammer";
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            //气笑了
            Projectile.scale *= 1.1f;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.White);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits % 2 == 0 && Projectile.numHits < 6)
            {
                NormalShootPunishmentStar(target);
                SoundStyle pickSound = Utils.SelectRandom(Main.rand, HammerSoundID.HammerStrike.ToArray());
                SoundEngine.PlaySound(pickSound with { Pitch = Main.rand.NextFloat(0.6f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
            }
            if (!Stealth)
                return;
            
            TargetIndex = target.whoAmI;
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HammerSoundID.HammerStrike.ToArray());
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.6f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
            //处于挂载条件下，持续发射神圣新星
            if (AttackType == DoType.IsStealth)
            {
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 360);
                int type = ModContent.ProjectileType<PunishmentStarMounted>();
                //生成神圣新星用的挂载弹
                if (Owner.ownedProjectileCounts[type] < 1)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, type, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    proj.localAI[0] = Projectile.Center.X;
                    proj.localAI[1] = Projectile.Center.Y;
                    proj.ai[2] = Projectile.whoAmI;
                    proj.Calamity().stealthStrike = true;
                    MountedIndex = proj.whoAmI;
                }
            }
            else
            {
                //其余状态下，存入敌对单位
                TargetIndex = target.whoAmI;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Stealth && AttackType is DoType.IsStealth && _isHanging && Projectile.timeLeft < 30)
            {
                Projectile.Center.CirclrDust(36, 3f, DustID.HallowedWeapons, 12);
                //原版粒子总体够用，但我还是决定用这个光球。
                float rotArg = 360f / 36;
                for (int i = 0; i < 36; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotArg);
                    Vector2 offsetPos = new Vector2(4f, 0f).RotatedBy(rot);
                    Vector2 dVel = new Vector2(4f, 0f).RotatedBy(rot);
                    GlowOrbParticle glowOrbParticle = new(Projectile.Center + offsetPos, dVel, false, 80, 1.2f, Main.rand.NextBool() ? Color.Gold : Color.White);
                    GeneralParticleHandler.SpawnParticle(glowOrbParticle);
                }

                SoundEngine.PlaySound(AdditionHitSigSound, Projectile.Center);
                //生成准备进行十字裁决的挂载射弹
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HolyJudgementMounted>(), Projectile.damage, 0f, Owner.whoAmI);
                proj.ai[0] = TargetIndex;
            }
        }
        //手动绘制这个射弹，我不想用你灾的绘制方式
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge();
            Projectile.QuickDrawWithTrailing(0.4f, Color.White);
            
            return false;
        }

        #region AI方法合集
        private void DoGeneric()
        {
            //挂载单位存在的时候，时刻更新他
            if(MountedIndex != -1)
            {
                Projectile proj = Main.projectile[MountedIndex];
                proj.localAI[0] = Projectile.Center.X;
                proj.localAI[1] = Projectile.Center.Y;
            }
            if (Stealth && AttackType == DoType.IsStealth)
            {
                Projectile.rotation += MathHelper.ToRadians(5f);
                return;
            }
            Projectile.rotation += 0.2f;
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(10, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemDiamond, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.HallowedWeapons, new Vector2(Projectile.velocity.X * 0.15f + velOffset.X, Projectile.velocity.Y * 0.15f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
        }
        private void NormalShootPunishmentStar(NPC target)
        {
            int dCounts = 36;
            Vector2 center = target.Center;
            float randsRad = MathHelper.Pi;
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            int div = 3;
            if (_isHanging)
            {
                center = Owner.Center;
                randsRad = MathHelper.PiOver2;
                dir = -(target.Center - center).SafeNormalize(Vector2.UnitX);
                div = 4;
            }
            else
                center.CirclrDust(dCounts, Main.rand.NextFloat(1.2f, 1.4f), Main.rand.NextBool() ? DustID.GemDiamond : DustID.HallowedWeapons, 3);
            for (int i = 1 ; i < 3; i++)
            {
                float beginAngle = dir.ToRotation();
                Vector2 velocity = beginAngle.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-randsRad / div, randsRad / div)) * 8f;
                Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, velocity, ModContent.ProjectileType<PunishmentStar>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                star.timeLeft = 100;
                star.penetrate = 1;
            }
        }
        //返程AI
        private void DoReturning()
        {
            //返程时执行类似回旋镖的AI
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed, BoomerangStat.Acceleration, BoomerangStat.KillDistance);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //无潜伏属性，处死射弹
                if (!Stealth)
                {
                    Projectile.Kill();
                    Update = true;
                }
                //其余情况下，根据情况进行潜伏攻击
                else
                {
                    //音效
                    SoundEngine.PlaySound(AdditionHitSigSound, Projectile.Center);
                    //当前没有任何挂载锤，则正常进入挂载状态
                    if (!_isHanging)
                    {
                        Projectile.Center.CirclrDust(24, 3f, DustID.HallowedWeapons, 10);
                        AttackType = DoType.IsStealth;
                        Update = true;

                        //以防万一干掉局部无敌帧，改用静态无敌帧
                        Projectile.usesLocalNPCImmunity = false;
                        Projectile.timeLeft = TotalSpinTime;
                        Projectile.usesIDStaticNPCImmunity = true;
                        //仍然使用10静态无敌帧
                        Projectile.idStaticNPCHitCooldown = 25;
         
                    }
                    //否则，执行其他AI
                    else
                    {
                        Owner.Center.CirclrDust(24, 3f, DustID.GemRuby, 10);
                        //锤子本身会在进入这个AI逻辑后处死
                        AttackType = DoType.IsAddition;
                        Update = true;
                        Projectile.timeLeft = ProjStat.LifeTime;
                    }
                }
            }
        }
        private void DoStealth()
        {
            //此处需首次获得射弹并处理，仅第一帧
            bool canHomingToTarget = Projectile.GetTargetSafe(out NPC target, TargetIndex, true, 600f);
            //第一帧刷新攻击时间。
            if (AttackTimer is 0)
            {
                Projectile.timeLeft = TotalSpinTime - 1;
                AttackTimer = 1;
                
            }
            //没有合理的目标，处死射弹
            if (!canHomingToTarget)
            {
                Projectile.Kill();
                return;
            }
            //正式执行潜伏AI逻辑，注意这里实际上直接无视最低距离
            Projectile.HomingNPCBetter(target, 1800f, 20f, 20f, ignoreDist: true);
            //标记挂载状态
            _isHanging = true;
        }
        private void DoAddition()
        {
            //其他情况下投掷出来的潜伏锤子只会正常回击并于玩家位置生成echo锤
            bool hasValidTarget = Projectile.GetTargetSafe(out NPC target, TargetIndex, true);
            if (hasValidTarget)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(Main.rand.NextFloat(MathHelper.PiOver4)));
                    Vector2 spawnSpeed = dir * 12f;
                    float ai1 = target.whoAmI;
                    Projectile hammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, spawnSpeed, ModContent.ProjectileType<PunishmentStar>(), Projectile.damage, Projectile.knockBack * 1.5f, Projectile.owner, 0f, ai1);
                    hammer.DamageType = CEUtils.RogueDC;
                    hammer.ai[2] = 1f;
                    Update = true;

                }
            }
            //无论如何，都直接处死这个锤子
            Projectile.Kill();
        }
        private void DoShooted()
        {
            AttackTimer += 1;
            //满足返程时间，返回
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                //重置计时器
                AttackTimer = 0;
                //切换攻击模组
                AttackType = DoType.IsReturning;
                //网络同步
                Update = true;
            }
        }
        #endregion
    }
}