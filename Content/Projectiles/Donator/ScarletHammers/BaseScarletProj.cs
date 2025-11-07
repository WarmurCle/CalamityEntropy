using CalamityEntropy.Common;
using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers
{
    public abstract class BaseScarletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        internal Player Owner => Main.player[Projectile.owner];
        internal EModPlayer EPlayer => Owner.Entropy();
        internal EGlobalProjectile EModProj => Projectile.Entropy();
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            ExSD();
        }
        public virtual void ExSD() { }
    }

}