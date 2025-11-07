using CalamityEntropy.Common;
using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers
{
    public struct BaseProjSD(int HitCooldown, int LifeTime, int Width, int Height, float rotation,bool UseLocalHit = true)
    {
        //所有的锤子逻辑实际上只有潜伏有区别
        public int HitCooldown = HitCooldown;
        public int LifeTime = LifeTime;
        public int Width = Width;
        public int Height = Height;
        public bool UseLocalHit = UseLocalHit;
        public float RotationSpeed = rotation;
    }
    /// <summary>
    /// 基础回旋镖的相关数据
    /// </summary>
    /// <param name="returnTime">返程时间m/param>
    /// <param name="returnSpeed">返程基础速度</param>
    /// <param name="acceleration">返程加速度</param>
    /// <param name="killDistance">超出距离处死</param>
    public struct BoomerangDefault(int returnTime, float returnSpeed, float acceleration, int killDistance)
    {
        public int ReturnTime = returnTime;
        public float Acceleration = acceleration;
        public float ReturnSpeed = returnSpeed;
        public int KillDistance = killDistance;
    }
    public abstract class BaseHammerClass : ModProjectile, ILocalizedModType
    {
        public static string TexturePathBase => "CalamityEntropy/Content/Items/Donator/Scarlet/";
        public Player Owner => Main.player[Projectile.owner];
        public EModPlayer ModPlayer => Owner.Entropy();
        public EGlobalProjectile ModProj => Projectile.Entropy();
        public bool Stealth => Projectile.Calamity().stealthStrike;
        public ref bool _isHanging => ref ModPlayer._anyHammerAttacking;
        public int AttackTimer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public int TargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public new string LocalizationCategory => "Projectiles.Rogue";
        /// <summary>
        /// 基础射弹数据
        /// </summary>
        protected abstract BaseProjSD ProjStat { get; }
        /// <summary>
        /// 基础回旋镖类模组数据。
        /// returnTime：返程时间
        /// returnSpeed：返程基础速度
        /// acceleration：返程加速度
        /// killDistance：处死距离
        /// </summary>
        protected abstract BoomerangDefault BoomerangStat{ get; }
        public override void SetDefaults()
        {
            Projectile.width = ProjStat.Width;
            Projectile.height = ProjStat.Height;
            Projectile.localNPCHitCooldown = ProjStat.HitCooldown;
            Projectile.usesLocalNPCImmunity = ProjStat.UseLocalHit;
            Projectile.penetrate = -1;
            Projectile.timeLeft = ProjStat.LifeTime;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            ExSD();
        }
        public virtual void ExSD() { }
    }
}