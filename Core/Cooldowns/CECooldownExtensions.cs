using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Core.Cooldowns
{
    /// <summary>CECooldown 和 Common/CECooldowns 是两套,那边没有 UI</summary>
    public static class CECooldown
    {
        public static CECooldownInstance Add(Player player, string id, int duration, bool overwrite = true)
            => player.GetModPlayer<CECooldownPlayer>().Add(id, duration, overwrite);

        public static bool Has(Player player, string id)
            => player.GetModPlayer<CECooldownPlayer>().Has(id);

        public static bool TryGet(Player player, string id, out CECooldownInstance instance)
            => player.GetModPlayer<CECooldownPlayer>().TryGet(id, out instance);

        public static bool Remove(Player player, string id)
            => player.GetModPlayer<CECooldownPlayer>().Remove(id);

        public static void Clear(Player player)
            => player.GetModPlayer<CECooldownPlayer>().Clear();
    }

    /// <summary>
    /// 形状对齐原 CalamityUtils 冷却扩展
    /// 不要和灾厄的 using 同文件,AddCooldown / HasCooldown 会二义
    /// </summary>
    public static class CECooldownExtensions
    {
        /// <summary>EntropyCooldowns 替代 .Entropy() 取冷却,查找失败会抛异常</summary>
        public static CECooldownPlayer EntropyCooldowns(this Player player)
            => player.GetModPlayer<CECooldownPlayer>();

        public static CECooldownInstance AddCooldown(this Player player, string id, int duration, bool overwrite = true)
            => player.GetModPlayer<CECooldownPlayer>().Add(id, duration, overwrite);

        public static bool HasCooldown(this Player player, string id)
            => player.GetModPlayer<CECooldownPlayer>().Has(id);

        public static bool TryGetCooldown(this Player player, string id, out CECooldownInstance instance)
            => player.GetModPlayer<CECooldownPlayer>().TryGet(id, out instance);

        public static bool RemoveCooldown(this Player player, string id)
            => player.GetModPlayer<CECooldownPlayer>().Remove(id);

        public static void ClearCooldowns(this Player player)
            => player.GetModPlayer<CECooldownPlayer>().Clear();

        public static IList<CECooldownInstance> GetDisplayedCooldowns(this Player player)
            => player.GetModPlayer<CECooldownPlayer>().GetDisplayed();

        public static CEChargeMeter GetChargeMeter(this Player player, string key, float max)
            => player.GetModPlayer<CECooldownPlayer>().GetCharge(key, max);

        public static CEChargeMeter GetChargeMeter(this Item item, float max) {
            var global = item.GetGlobalItem<CEChargeGlobalItem>();
            if (global.meter == null)
                global.meter = new CEChargeMeter(max);
            else
                global.meter.Max = max;
            return global.meter;
        }
    }
}
