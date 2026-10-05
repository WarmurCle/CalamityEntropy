using CalamityEntropy.Core.Cooldowns;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Weapons
{
    /// <summary>Shoot 开头用 IsReady 查询,用 TryConsume 释放</summary>
    public static class CEChargeWeapon
    {
        public static CEChargeMeter GetMeter(Item item) {
            if (item?.ModItem is not ICEChargeWeapon chargeWeapon)
                return null;
            return item.GetChargeMeter(chargeWeapon.ChargeProfile.Max);
        }

        public static bool IsReady(Item item) {
            CEChargeMeter meter = GetMeter(item);
            return meter != null && meter.Ready;
        }

        /// <summary>同一组射击只调一次 TryConsume,当帧窗口会自动打标</summary>
        public static bool TryConsume(Player player, Item item) {
            CEChargeMeter meter = GetMeter(item);
            if (meter == null || !meter.Consume())
                return false;
            player.GetModPlayer<CEChargePlayer>().OpenEmpowerWindow();
            return true;
        }

        public static void Empower(int projIndex) {
            if (projIndex >= 0 && projIndex < Main.maxProjectiles)
                Main.projectile[projIndex].SetEmpowered();
        }

        public static void Empower(Projectile projectile) => projectile.SetEmpowered();

        /// <summary>CreditHit 是命中计数入口,框架已经调过,父链之外才手调</summary>
        public static void CreditHit(Player player, Item item) {
            if (item?.ModItem is not ICEChargeWeapon chargeWeapon)
                return;
            if (chargeWeapon.ChargeProfile.Trigger != CEChargeTrigger.HitCount)
                return;
            Gain(player, item, chargeWeapon.ChargeProfile, 1f);
        }

        internal static void Gain(Player player, Item item, in CEChargeProfile profile, float amount) {
            CEChargeMeter meter = item.GetChargeMeter(profile.Max);
            float rate = player.GetModPlayer<CEChargePlayer>().ChargeRateMult;
            if (meter.Gain(amount * rate))
                PlayReadyFeedback(player);
        }

        public static void PlayReadyFeedback(Player player) {
            if (Main.dedServ || player.whoAmI != Main.myPlayer)
                return;
            CEChargeMeter.PlayReadyCue(player);
            string text = CalamityEntropy.Instance.GetLocalization("ChargeReady", () => "蓄势就绪").Value;
            CombatText.NewText(player.getRect(), new Color(255, 224, 120), text);
        }
    }

    public class CEChargePlayer : ModPlayer
    {
        /// <summary>饰品在 UpdateAccessory 里乘 ChargeRateMult</summary>
        public float ChargeRateMult = 1f;

        private uint empowerWindowFrame = uint.MaxValue;

        public override void ResetEffects() {
            ChargeRateMult = 1f;
        }

        internal void OpenEmpowerWindow() => empowerWindowFrame = Main.GameUpdateCount;

        internal bool EmpowerWindowActive => empowerWindowFrame == Main.GameUpdateCount;
    }
}
