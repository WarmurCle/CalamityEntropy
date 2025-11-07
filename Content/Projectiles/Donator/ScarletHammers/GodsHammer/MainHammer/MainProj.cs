using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.MainHammer
{
    public partial class GodsHammerProj: BaseHammerClass, ILocalizedModType
    {
        protected override BoomerangDefault BoomerangStat => new(
            returnTime: 34,
            returnSpeed: 28f,
            acceleration: 0.4f,
            killDistance: 1800
        );
        protected override BaseProjSD ProjStat => new(
            //这里的无敌帧有意整成10
            HitCooldown: 10,
            //这里存续时间无所谓，因为会在AI里时刻被更新
            LifeTime: 200,
            Width: 86,
            Height: 72,
            rotation: 0.2f
        );
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public static Color TrailColor => new(255, 142, 246);
        public static readonly SoundStyle HitSound = SoundID.Item88 with { Volume = 0.85f }; //Item88:使用流星法杖的音效
        #region Typedef
        public ref bool Update => ref Projectile.netUpdate;
        public ref bool IsHanging => ref ModPlayer._anyHammerAttacking;
        public int CahceEU
        {
            get => (int)ModProj.ExtraProjAI[0];
            set => ModProj.ExtraProjAI[0] = value;
        }
        public ref float SpriteRotation => ref ModProj.ExtraProjAI[1];
        #endregion
        public override string Texture => TexturePathBase + "GodsHammer";
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 4;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SpriteRotation = Projectile.velocity.ToRotation();
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;
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
            }
        }
        public override bool PreKill(int timeLeft)
        {
            if (AttackType is DoType.IsStealth && _drawArcTime > 0)
            {
                SoundStyle select = Utils.SelectRandom(Main.rand, HammerSoundID.HammerStrike.ToArray());
                SoundEngine.PlaySound(select, Projectile.Center);
                //震屏
                Owner.Calamity().GeneralScreenShakePower = 8;
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HitSound with {MaxInstances = 2}, Projectile.Center);
            if (Stealth && AttackType is DoType.IsStealth && _drawArcTime > 0)
                StealthHit(target, hit.Damage, target.whoAmI);
            if (!Stealth)
            {
                NormalHit(target);
            }
        }
        private SpriteBatch SB { get => Main.spriteBatch; }
        #region DrawMethod
        //DrawProjWidth
        public float SetProjWidth(float ratio)
        {
            float width = Projectile.width;
            width *= MathHelper.SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        //DrawTrailColor
        float Hue = 1.7f;
        public Color SetTrailColor(float ratio)
        {
            float hue = Hue % 1f + 0.2f;
            if (hue >= 0.99f)
                hue = 0.99f;

            float velocityOpacityFadeout = Utils.GetLerpValue(2f, 5f, Projectile.velocity.Length(), true);
            Color c = TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.2f, ratio, true) * velocityOpacityFadeout;
        }
        //DrawOffset
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            return Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.5f * Vector2.UnitX;
        }
        #endregion
        //TODO：下面那个轨迹把归元漩涡的轨迹改成另外一种，现在这个纯纯占位符
        public override bool PreDraw(ref Color lightColor)
        {
            if (Stealth)
            {
                SB.EnterShaderRegion(BlendState.Additive);
                float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
                GameShaders.Misc["CalamityMod:SideStreakTrail"].UseImage1("Images/Misc/Perlin");
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(SetProjWidth, SetTrailColor, PrimitiveOffsetFunction, shader: GameShaders.Misc["CalamityMod:SideStreakTrail"]), 51);
                SB.ExitShaderRegion();
            }
            Projectile.QuickDrawBloomEdge();
            Projectile.QuickDrawWithTrailing(0.7f, Color.White, 4);
            return false;
        }
          private void DoStealth()
        {
            NPC target = Projectile.FindClosestTarget(4800f);
            //初始化上一锚点缓存位为玩家中心
            if (_lastAnchorPosition == Vector2.Zero)
                _lastAnchorPosition = Owner.Center;

            //如果玩家突然死亡，处死自己
            if (Owner.dead)
                Projectile.Kill();
    
            //小于特定次数前ban掉下方所有AI
            if (_drawArcTime < 1)
            {
                DrawDynamicArc();
                return;
            }
            if (target != null)
            {
                //以超高的速度冲向你的敌怪
                Projectile.HomingNPCBetter(target, 1f, 24f, 18f, 2, ignoreDist: true);
            }
        }
        private void DoGeneric() 
        {
            if (!Stealth)
            {
                Projectile.rotation += ProjStat.RotationSpeed;
                if (Main.rand.NextBool(8))
                {
                    Vector2 offset = new Vector2(10, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                    Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                    Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.WitherLightning, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                    dust.noGravity = true;
                }

                if (Main.rand.NextBool(10))
                {
                    Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                    Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                    Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemDiamond, new Vector2(Projectile.velocity.X * 0.15f + velOffset.X, Projectile.velocity.Y * 0.15f + velOffset.Y), 100, default, 0.8f);
                    dust.noGravity = true;
                }

            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                IsHanging = true;
            }
            
        }
        private void DoShooted()
        {
            //潜伏+初始状态下，执行特殊ACT音效，并获得超高EU
            bool ShouldACT = AttackTimer is 0 && Stealth;
            if (ShouldACT)
            {
                CahceEU = Projectile.extraUpdates;
                Projectile.extraUpdates = Projectile.extraUpdates + 2;
                SoundStyle selectOne = Utils.SelectRandom(Main.rand, HammerSoundID.HammerTypes.ToArray()) with { Volume = 0.8f, MaxInstances = 0 };
                SoundEngine.PlaySound(selectOne, Projectile.Center);
            }

            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackTimer = 0;
                AttackType = DoType.IsReturning;
                //恢复普通EU
                if (ShouldACT)
                    Projectile.extraUpdates = CahceEU;
                Update = true;
            }
        }
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
                    Projectile.localNPCHitCooldown = 45;
                    Update = true;
                }
            }
        }
    }
}