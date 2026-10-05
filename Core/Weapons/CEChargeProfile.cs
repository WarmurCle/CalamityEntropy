namespace CalamityEntropy.Core.Weapons
{
    public enum CEChargeTrigger
    {
        /// <summary>ChargeBar 在持有期间按帧蓄势</summary>
        ChargeBar,

        HitCount,

        /// <summary>Periodic 按帧冷却,只在手持时上涨</summary>
        Periodic
    }

    public readonly struct CEChargeProfile
    {
        public readonly CEChargeTrigger Trigger;

        /// <summary>Max 是充满量,蓄力条和周期按帧计,命中按次数计</summary>
        public readonly float Max;

        /// <summary>原 StealthDamageMultiplier</summary>
        public readonly float DamageMult;

        /// <summary>原 StealthVelocityMultiplier</summary>
        public readonly float VelocityMult;

        /// <summary>原 StealthKnockbackMultiplier</summary>
        public readonly float KnockbackMult;

        private CEChargeProfile(CEChargeTrigger trigger, float max, float damageMult, float velocityMult, float knockbackMult) {
            Trigger = trigger;
            Max = max;
            DamageMult = damageMult;
            VelocityMult = velocityMult;
            KnockbackMult = knockbackMult;
        }

        public static CEChargeProfile ChargeBar(float seconds, float damageMult = 1f, float velocityMult = 1f, float knockbackMult = 1f)
            => new(CEChargeTrigger.ChargeBar, seconds * 60f, damageMult, velocityMult, knockbackMult);

        public static CEChargeProfile HitCount(int hits, float damageMult = 1f, float velocityMult = 1f, float knockbackMult = 1f)
            => new(CEChargeTrigger.HitCount, hits, damageMult, velocityMult, knockbackMult);

        public static CEChargeProfile Periodic(float seconds, float damageMult = 1f, float velocityMult = 1f, float knockbackMult = 1f)
            => new(CEChargeTrigger.Periodic, seconds * 60f, damageMult, velocityMult, knockbackMult);
    }

    /// <summary>武器在 Shoot 里调 TryConsume,而且 maxStack 只能是 1</summary>
    public interface ICEChargeWeapon
    {
        CEChargeProfile ChargeProfile { get; }
    }
}
