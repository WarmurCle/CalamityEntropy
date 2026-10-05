using CalamityEntropy.Common;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Lores
{
    /// <summary>
    /// 接替灾厄 LoreItem;无重力、满亮度,按住 Shift 显示 Lore 全文
    /// 挂了 LoreEffect 才能用,背包右键开关,开关在 LoreReworkSystem
    /// </summary>
    public abstract class CELoreItem : ModItem
    {
        // 3.33 继承灾厄 LoreItem 时分类就是 Items.Lore，三语文案至今仍挂在该分类下；
        // 换成自有基类后若落回 ModItem 默认的 Items，六件传记的名字与正文会全部变成孤儿键
        public override string LocalizationCategory => "Items.Lore";

        /// <summary>本物品是否挂有 LoreEffect（即走 LoreReworkSystem 开关通道）。</summary>
        public bool HasLoreEffect => LoreReworkSystem.loreEffects != null && LoreReworkSystem.loreEffects.ContainsKey(Type);

        public override void SetStaticDefaults() {
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        // 原灾厄基类恒返回 false，本模组靠已删除的 CanUseItem 钩子放行；
        // 此处以等价原生逻辑接管：效果系统开启且本物品注册了 LoreEffect 时可用，
        // 使用后的开关切换由 LoreReworkItem.UseItem 完成
        public override bool CanUseItem(Player player) => LoreEffect.Enabled && HasLoreEffect;

        // 背包右键同样可开关效果，且不消耗物品
        public override bool CanRightClick() => LoreEffect.Enabled && HasLoreEffect;

        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player) {
            // 与 LoreReworkItem.UseItem 保持同一条开关路径
            LoreReworkSystem.ToggleLore(Item);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                Main.LocalPlayer.Entropy().SyncPlayer(-1, Main.myPlayer, false);

            LoreEffect effect = LoreReworkSystem.loreEffects[Type];
            if (effect.useSound.HasValue)
                SoundEngine.PlaySound(LoreReworkSystem.Enabled(Type) ? effect.useSound.Value : CEUtils.GetSound("AscendantOff"), player.Center);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (Main.keyState.IsKeyDown(Keys.LeftShift)) {
                tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
                tooltips.Add(new TooltipLine(Mod, "CalamityEntropy:Lore", this.GetLocalizedValue("Lore")));
            }
            else {
                tooltips.Add(new TooltipLine(Mod, "CalamityEntropy:LoreHint", Language.GetTextValue("Mods.CalamityEntropy.LoreHoldShift")));
            }
        }
    }
}
