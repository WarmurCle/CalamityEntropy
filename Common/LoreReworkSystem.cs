using CalamityEntropy.Core.CalamityRef;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public abstract class LoreEffect
    {
        public static bool Enabled => ModContent.GetInstance<ServerConfig>().LoreSpecialEffect || CalamityEntropy.EntropyMode;
        public virtual LocalizedText Decription {
            get {
                ModItem mi = ContentSamples.ItemsByType[ItemType].ModItem;
                Mod mod = mi.Mod;
                //灾厄 Lore 的效果描述由本模组提供,按实例判定,不引类型、不写模组名
                if (CERef.IsCalamity(mod)) {
                    mod = CalamityEntropy.Instance;
                }
                return Language.GetOrRegister(mod.GetLocalizationKey(mi.Name + "Desc"));
            }
        }
        public abstract int ItemType { get; }
        public virtual SoundStyle? useSound => CEUtils.GetSound("loreEnabled");

        public virtual void ModifyTooltip(TooltipLine tooltip) {
        }

        public virtual void UpdateEffects(Player player) {
        }
    }
    public class LoreReworkSystem : ICELoader
    {
        public static Dictionary<int, LoreEffect> loreEffects;
        public void LoadData() {
            loreEffects = new Dictionary<int, LoreEffect>();
        }
        public void UnLoadData() {
            loreEffects = null;
        }
        public static void ToggleLore(Item item) {
            if (!LoreEffect.Enabled)
                return;

            if (loreEffects.ContainsKey(item.type)) {
                if (Main.LocalPlayer.Entropy().enabledLoreItems.Contains(item.type)) {
                    Main.LocalPlayer.Entropy().enabledLoreItems.Remove(item.type);
                }
                else {
                    Main.LocalPlayer.Entropy().enabledLoreItems.Add(item.type);
                }

            }
        }
        public static bool Enabled<T>() where T : ModItem {
            return Main.LocalPlayer.Entropy().enabledLoreItems.Contains(ModContent.ItemType<T>());
        }
        public static bool Enabled(int type) {
            return Main.LocalPlayer.Entropy().enabledLoreItems.Contains(type);
        }
    }
    public class LoreReworkItem : GlobalItem
    {
        public override void SetDefaults(Item entity) {
            if (!LoreEffect.Enabled)
                return;
            if (LoreReworkSystem.loreEffects.ContainsKey(entity.type)) {
                entity.useTurn = true;
                entity.useTime = entity.useAnimation = 20;
                entity.useStyle = ItemUseStyleID.HoldUp;
            }
        }
        public override bool? UseItem(Item item, Player player) {
            if (!LoreReworkSystem.loreEffects.ContainsKey(item.type) || !LoreEffect.Enabled) {
                return null;
            }
            if (Main.myPlayer == player.whoAmI) {
                LoreReworkSystem.ToggleLore(item);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    Main.LocalPlayer.Entropy().SyncPlayer(-1, Main.myPlayer, false);
            }
            if (LoreReworkSystem.loreEffects[item.type].useSound.HasValue) {
                SoundEngine.PlaySound(LoreReworkSystem.Enabled(item.type) ? LoreReworkSystem.loreEffects[item.type].useSound.Value : CEUtils.GetSound("AscendantOff"), player.Center);
            }
            return true;
        }
        // 外来 Lore(灾厄的 LoreItem 基类)始终让 CanUseItem 返回 false,而 tML 把 CanUseItem 按与合并,
        // GlobalItem 覆盖不了 ModItem 返回的 false;CanRightClick 按或合并,ConsumeItem 按与合并,
        // 所以外来 Lore 的开关改走右键,本模组自己的 CELoreItem 已经在 ModItem 上做了
        // 同一条通道,这里必须避开,否则一次右键会切换两次,结果等于没有切换
        private static bool HandlesRightClick(Item item) {
            return LoreEffect.Enabled
                && item.ModItem is not Content.Items.Lores.CELoreItem
                && LoreReworkSystem.loreEffects != null
                && LoreReworkSystem.loreEffects.ContainsKey(item.type);
        }

        public override bool CanRightClick(Item item) {
            return HandlesRightClick(item);
        }

        public override bool ConsumeItem(Item item, Player player) {
            return !HandlesRightClick(item);
        }

        public override void RightClick(Item item, Player player) {
            if (!HandlesRightClick(item))
                return;
            LoreReworkSystem.ToggleLore(item);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                Main.LocalPlayer.Entropy().SyncPlayer(-1, Main.myPlayer, false);
            LoreEffect effect = LoreReworkSystem.loreEffects[item.type];
            if (effect.useSound.HasValue)
                SoundEngine.PlaySound(LoreReworkSystem.Enabled(item.type) ? effect.useSound.Value : CEUtils.GetSound("AscendantOff"), player.Center);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (!LoreEffect.Enabled || !LoreReworkSystem.loreEffects.ContainsKey(item.type))
                return;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                return;

            TooltipLine tooltipLineEF = new TooltipLine(base.Mod, "CalamityEntropy:LoreEffectInfo", Language.GetTextValue("Mods.CalamityEntropy.UseToggle"));
            tooltips.Add(tooltipLineEF);
            var dsc = LoreReworkSystem.loreEffects[item.type].Decription;

            TooltipLine tooltipLineA = new TooltipLine(base.Mod, "CalamityEntropy:LoreEffectDesc", dsc.Value);
            LoreReworkSystem.loreEffects[item.type].ModifyTooltip(tooltipLineA);
            tooltips.Add(tooltipLineA);

            TooltipLine tooltipLineE = new TooltipLine(base.Mod, "CalamityEntropy:LoreEffectState", Language.GetTextValue("Mods.CalamityEntropy." + (LoreReworkSystem.Enabled(item.type) ? "Enabled" : "Disabled")));
            tooltipLineE.OverrideColor = LoreReworkSystem.Enabled(item.type) ? Color.Yellow : Color.Gray;
            tooltips.Add(tooltipLineE);
        }
    }
}
