using System.IO;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.MainHammer;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.Phantasmal
{
    public class PhantasmalHammer: ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public Player Owner => Main.player[Projectile.owner];
        public EGlobalProjectile ModProj => Projectile.Entropy();
        public override string Texture => BaseHammerClass.TexturePathBase + "GodsHammer";
        public int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public ref float AttackTimer => ref Projectile.ai[1];
        public ref float TotalArcAngle => ref Projectile.ai[2];
        public bool IsFlip
        {
            get => ModProj.ExtraProjAI[0] is 1f;
            set => ModProj.ExtraProjAI[0] = value ? 1f : 0f;
        }
        public ref float SpriteRotation => ref ModProj.ExtraProjAI[1];
        public ref float ArcRotation => ref ModProj.ExtraProjAI[1];
        const int SetUpdate = 3;
        //是否画圆
        private bool _isArcRotating = false; 
        //旋转起始角
        private float _arcStartRotation;
        private bool ShouldDrawVertex = true;
        //总转角
        //持续帧数
        private const int ArcDuration = 15 * SetUpdate;
        //发起旋转前的原始速度
        private float _originalSpeed;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            //生成的一瞬确定这个锤子的“旋转朝向”
            IsFlip = TotalArcAngle < 0;
            SpriteRotation = Projectile.velocity.ToRotation();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = SetUpdate;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0f;
            Projectile.DamageType = CEUtils.RogueDC;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(IsFlip);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            IsFlip = reader.ReadBoolean();
        }
        public override bool? CanDamage() => AttackTimer > ArcDuration;
        public override void AI()
        {
            if (Projectile.timeLeft < 50)
                Projectile.Kill();

            //生成，渐变
            if (!ShouldDrawVertex)
                Projectile.rotation += 0.2f * IsFlip.ToDirectionInt();
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Projectile.Opacity += 0.1f;
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity, 0f, 1f);
            AttackTimer += 1f;
            //绘制圆弧运动
            if (AttackTimer < ArcDuration)
            {
                DrawArc();
                return;
            }
        
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.HomingNPCBetter(target, 1800f, 20f, 20f, ignoreDist: true);
            else
            {
                Projectile.AccelerateToTarget(Owner.Center, 20f, 1.8f, 4800);
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                {
                    Projectile.Kill();
                    Projectile.netUpdate = true;
                }
            }
        }

        private void DrawArc()
        {
            if (!_isArcRotating)
            {
                _isArcRotating = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                Projectile.velocity *= 0.40f;
            }

            if (_isArcRotating)
            {
                float arcProgress = (float)AttackTimer / ArcDuration;
                //计算当前的角度
                ArcRotation = _arcStartRotation + TotalArcAngle * arcProgress;
                //同步旋转角度与速度
                Projectile.velocity = ArcRotation.ToRotationVector2() * Projectile.velocity.Length();
                //?
                if (AttackTimer >= ArcDuration)
                {
                    //重置速度
                    Projectile.velocity = ArcRotation.ToRotationVector2() * _originalSpeed;
                    //跳出循环
                    _isArcRotating = false;
                }
                return;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new(251, 184, 255, 100);
        private SpriteBatch SB { get => Main.spriteBatch; }
        #region  Draw
        public float SetProjWidth(float ratio)
        {
            float width = Projectile.width;
            width *= MathHelper.SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(1f, 5f, Projectile.velocity.Length(), true);
            Color c = GodsHammerProj.TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.1f, ratio, true) * velocityOpacityFadeout;
        }
        //DrawOffset
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            return Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.5f * 0.5f;
        }

        public void DrawVertex()
        {
            float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
            GameShaders.Misc["CalamityMod:SideStreakTrail"].UseImage1("Images/Misc/Perlin");
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(SetProjWidth, SetTrailColor, PrimitiveOffsetFunction, shader: GameShaders.Misc["CalamityMod:SideStreakTrail"]), 51);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (ShouldDrawVertex)
            {
                SB.EnterShaderRegion(BlendState.Additive);
                DrawVertex();
                SB.ExitShaderRegion();
            }
            Projectile.QuickDrawBloomEdge(Color.LightPink, rotOffset: -MathHelper.PiOver4);
            Projectile.QuickDrawWithTrailing(0.7f, Color.GhostWhite, -MathHelper.PiOver4);
            return false;
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShouldDrawVertex = false;
            //从灾厄上抄下来的, 由于有一些特殊效果所以粒子会少一点
            float numberOfDusts = 4f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                int dType = Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.WitherLightning;
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Vector2 velOffset = new Vector2(4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, dType, new Vector2(velOffset.X, velOffset.Y), 0, default, 0.3f);
                dust.noGravity = true;
                dust.velocity = velOffset;
                dust.scale = 1.2f;
            }
            SoundEngine.PlaySound(GodsHammerProj.HitSound, Projectile.Center);
            TargetIndex = target.whoAmI;
            if (Projectile.numHits % 2 is 0)
                SpawnNebulaShot(Projectile, target);
            
        }

        public void SpawnNebulaShot(Projectile projectile,NPC target)
        {
            //灾厄抄写下来的
            projectile.netUpdate = true;
            Vector2 targetPos = target.Center;
            int laserID = ModContent.ProjectileType<NebulaEnegry>();
            //每轮生成两个。
            for (int i = 0; i < 2; ++i)
            {
                //确定位置
                Vector2 spawnPosBase = (target.Center - Owner.Center).SafeNormalize(Vector2.UnitX);
                float warpDistance = 12f * 16f;
                float warpRadians = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                Vector2 warpOffset = -warpDistance * spawnPosBase.RotatedBy(warpRadians);
                Vector2 spawnPos = Owner.MountedCenter + warpOffset;
                //确定初始速度，精准一些。
                Vector2 velDir = (targetPos - spawnPos).SafeNormalize(Vector2.UnitX);
                Vector2 vel = velDir * Main.rand.NextFloat(15f, 19f);
                SpawnDust(spawnPos, vel);
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), spawnPos, vel, laserID, projectile.damage / 2, 2.5f, projectile.owner, target.whoAmI);
                    proj.DamageType = ModContent.GetInstance<RogueDamageClass>();
                    proj.Entropy().ExtraProjAI[0] = (Main.rand.NextBool() ? (float)NebulaEnegry.Flip.DoFlip : (float)NebulaEnegry.Flip.None);
                }
            }
        }
        private void SpawnDust(Vector2 spawnPos, Vector2 dir)
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
                ShinyOrbParticle orbs = new ShinyOrbParticle(toEdge, Vector2.Zero, GodsHammerProj.TrailColor, 40, 0.7f, BlendState.Additive);
                orbs.Spawn();
            }
            int yetAnotherParticlesCounts = 12;
            for (int k = 0; k < 18; k++)
            {
                float shortAxis = 6f - k;
                float longAxis = 16f - k;
                for (int j = 0; j < yetAnotherParticlesCounts; j++)
                {
                    float angleStep = (float)(MathHelper.TwoPi / yetAnotherParticlesCounts);
                    float angle = j * angleStep;
                    Vector2 edge = spawnPos + angle.ToEllipseVector2Edge(shortAxis, longAxis, baseRot);
                    ShinyOrbParticle orbs = new ShinyOrbParticle(edge, Vector2.Zero, GodsHammerProj.TrailColor, 40, 0.6f, BlendState.Additive);
                    orbs.Spawn();
                }
            }
        }
        public override bool PreKill(int timeLeft)
        {
            //即将死亡的时候，生成一个克隆锤子。
            int projID = ModContent.ProjectileType<PhantasmalHammerClone>();
            //获取当前锤子到玩家的向量，归一化后转90°
            Vector2 dir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2 * IsFlip.ToDirectionInt());
            //转化为实际速度
            Vector2 vel = dir * 18f;
            //直接追加这个射弹。
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, projID, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            proj.ai[2] = TargetIndex;
            proj.localAI[0] = IsFlip.ToDirectionInt();
            return true;
        }
        
    }
}