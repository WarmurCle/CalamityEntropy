using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.ExtraProj;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.MainHammer
{
    public partial class NightmareHammerProj: BaseHammerClass, ILocalizedModType
    {
        #region Typedef
        internal ref bool Update => ref Projectile.netUpdate;
        public ref float NotHangingReleaseDarkEnegryTimer => ref Projectile.GetGlobalProjectile<EGlobalProjectile>().ExtraProjAI[0];
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { MaxInstances = 0,Pitch = 0.35f, Volume = 0.35f };
        #endregion
        //攻击枚举
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth,
            IsHanging
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        #region 基础数值
        protected override BoomerangDefault BoomerangStat => new(
            //不准修改这个returnTime低于35
            returnTime: 35,
            returnSpeed: 26f,
            acceleration: 1.5f,
            killDistance: 1800
        );
        protected override BaseProjSD ProjStat => new (
            HitCooldown: 30,
            LifeTime:300,
            Width:66,
            Height:66,
            rotation: 0.2f
        );
        //潜伏攻击的总时间，为了这个锤子所有攻击方式，如果你知道你在做什么，你不应该修改这个值“低于”480
        private const int TotalSpinTime = 480;
        //我也不知道这个是干嘛的，但是我建议别改（
        private int StartSpinTime => 30 * Projectile.extraUpdates;
        #endregion
        #region 其余与平衡无关的杂项。
        public ref float NebulaArrowRotation => ref ModProj.ExtraProjAI[0];
        private int FlaresCounts = 1;
        private float DrawTrailTimer = 0f;
        public override string Texture => TexturePathBase + "NightmareHammer";
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NotHangingReleaseDarkEnegryTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NotHangingReleaseDarkEnegryTimer = reader.ReadSingle();
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        #endregion
        public override void ExSD()
        {
            //夜明后的锤子应该上4eu了
            Projectile.extraUpdates = 4;
        }
        public override void AI()
        {
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
                case DoType.IsHanging:
                    DoHanging();
                    break;
            }
        }
        //全局AI
        private void DoGeneric() 
        {
            Projectile.rotation += ProjStat.RotationSpeed;
            if (!Stealth)
                Projectile.ArmorPenetration = 50;
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            DrawTrailingDust();
        }
        //首次投掷出去时的AI
        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackTimer = 0;
                AttackType = DoType.IsReturning;
                Update = true;
            }
        }
        //返程AI
        private void DoReturning()
        {
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed, BoomerangStat.Acceleration, BoomerangStat.KillDistance);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //当前有任何挂载锤，所有的攻击都会直接在返回后杀掉弹幕
                if (!Stealth)
                {
                    Projectile.Kill();
                    Update = true;
                    return;
                }
                else
                {
                    AttackType = DoType.IsStealth;
                    //重新设定无敌帧
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.timeLeft = TotalSpinTime;
                    Projectile.extraUpdates = 3;
                    Projectile.localNPCHitCooldown = 25 * Projectile.extraUpdates;
                    Update = true;
                }
            }
        }
        //潜伏AI
        private void DoStealth()
        {
            DoGeneric();
            //直接回击敌人
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.HomingNPCBetter(target, 1f, 20f, 20f, 1, ignoreDist: true);
            else
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Debuff
            DebuffsHandler(target);
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
            StealthHitDust();
            //如果没有潜伏攻击，正常生成梦魇之箭
            if (!Stealth && Projectile.numHits < 1)
            {
                for (int i = 0; i < 2; i++)
                    NightmareArrowDrop(target, Projectile.damage);
            }

            //处于潜伏回击时，击中敌人传入这个单位
            if (AttackType is DoType.IsStealth)
            {
                SoundEngine.PlaySound(UseSound, Projectile.Center);
                TargetIndex = target.whoAmI;
                AttackType = DoType.IsHanging;
                Update = true;
                return;
            }
            //此处，处理挂载情况下的射弹操作
            if (AttackType is DoType.IsHanging)
                OnHitHanging(target);
        }

        public override void OnKill(int timeLeft)
        {
            //记得重置玩家类的状态。
            if (AttackType is DoType.IsHanging)
            {
                _isHanging = false;
                Update = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge(Color.DarkMagenta, 6);
            Projectile.QuickDrawWithTrailing(0.7f, Color.White);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        private void DebuffsHandler(NPC target)
        {
            //常态造成元素调谐
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 360);
            //白天，造成破晓 + 双足翼龙诅咒
            if (Main.dayTime)
            {
                target.AddBuff(BuffID.Daybreak, 360);
                target.AddBuff(BuffID.BetsysCurse, 360);
            }
            //夜晚，造成夜魇 + "死亡低语"
            else
            {
                target.AddBuff(ModContent.BuffType<Nightwither>(), 360);
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 360);
            }

        }
        private void OnHitHanging(NPC target)
        {
            bool hasOver2Hammer = false;
            int hammerCounts = 0;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == Type && proj.Calamity().stealthStrike)
                    hammerCounts++;
                if (hammerCounts > 1)
                {
                    hasOver2Hammer = true;
                    break;
                }
            }
            //注意这里的逻辑：
            //假定玩家同时拥有两把以上的挂载锤，下方的射弹生成数量会被很大程度上降低.
            //每次攻击，都会使梦魇箭数量与伤害提升
            Update = true;
            int numFlares = hasOver2Hammer ? 2 : 3;
            if (FlaresCounts < numFlares)
                FlaresCounts += 1;
            //用对数计算控制伤害
            int flareDamage = (int)(Projectile.damage / 2 * Math.Log(1 + FlaresCounts));
            
            for (int i = 0; i < numFlares; i++)
                NightmareArrowDrop(target, flareDamage);
            //每次攻击缩减
            //双锤子以上时，每把锤子最低只有15的攻击频率
            int leastHitCD = hasOver2Hammer ? 12 * Projectile.extraUpdates : 6 * Projectile.extraUpdates;
            Projectile.localNPCHitCooldown -= 5 * Projectile.extraUpdates;
            if (Projectile.localNPCHitCooldown < leastHitCD)
                Projectile.localNPCHitCooldown = leastHitCD;
        }
        private void NightmareArrowDrop(NPC target, int flareDamage)
        {
            //这下面一长串都是为了处理……生成的
            //返程写的挺fuck的
            float xDist = Main.rand.NextFloat(10f, 220f) * Main.rand.NextBool().ToDirectionInt();
            float yDist = Main.rand.NextFloat(800f, 1000f);
            Vector2 srcPos = target.Center + new Vector2(xDist, -yDist);
            if (Projectile.owner != Main.myPlayer)
                return;

            //在滞留所有的射弹
            Projectile sparks = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), srcPos, Vector2.Zero, ModContent.ProjectileType<NightmareArrow>(), flareDamage, 1.1f, Owner.whoAmI);
            sparks.Calamity().stealthStrike = true;
            sparks.ai[2] = target.whoAmI;
            sparks.localAI[0] = xDist;
            sparks.localAI[1] = yDist;
        }

        private void StealthHitDust()
        {
            Vector2 dire = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float rotFactor = MathHelper.Pi / 15;
            float dScaleCap = 1.8f;
            float dSacleBase = 0.5f;
            for (int i = -7; i < 7; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 dVelocity = (Projectile.velocity.Length() / 1.5f * dire).RotatedBy(rot);
                Dust d = Dust.NewDustDirect(Projectile.Center, Projectile.width / 4, Projectile.height / 4, DustID.GemAmethyst, dVelocity.X, dVelocity.Y);
                d.scale = dSacleBase;
                d.noGravity = true;
                d.alpha = 100;
                dSacleBase += 0.2f;
                if (dSacleBase > dScaleCap)
                    dSacleBase = dScaleCap;
            }
        }
        private void DrawTrailingDust()
        {
            DrawTrailTimer++;
            if (DrawTrailTimer < 5f)
                return;
            //正弦波频率
            float freq = 0.2f;
            //振幅
            float amp = 35f;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f;
            for (int i = -1; i < 2; i+= 2)
            {
                //基础横向偏移，用于控制射弹与路径的距离。
                float baseOffset = 5f;
                //让相位差不变，使他们在零点上同步
                float angle = AttackTimer * freq;
                //曲线1使用Sin，曲线2使用-Sin确保反向运动
                float wave = (float)Math.Sin(angle) * i;
                //计算垂直方向向量。
                Vector2 perpendDir = direction.RotatedBy(MathHelper.PiOver2);
                //最终确定生成位置的偏差
                Vector2 waveOffset = perpendDir * wave * amp + perpendDir * baseOffset;
                //修改粒子生成位置。
                Vector2 spawnPosition = Projectile.Center + waveOffset;
                //计算例子速度，粒子需要在零点反向运动。因为总体上，他们是在原点位置被“推开”的
                //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
                float verticleVel = (float)Math.Cos(angle) * 1.2f * i;
                Vector2 realVel = speedValue + perpendDir * verticleVel;
                //跳过屏幕外绘制
                if (CEUtils.OutOffScreen(spawnPosition))
                    continue;
                //最终生成粒子。
                Color drawColor = i > 0 ? Color.Black : new(75, 0, 130);
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, drawColor, 140, 1.2f, i < 0 ? BlendState.Additive : BlendState.AlphaBlend);
                shinyOrbParticle.Spawn();
            }
        }
    }
}