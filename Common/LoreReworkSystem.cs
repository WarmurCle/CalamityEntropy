using CalamityMod;
using CalamityMod.Items.LoreItems;
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
        public virtual LocalizedText Decription
        {
            get
            {
                var mi = ContentSamples.ItemsByType[ItemType].ModItem;
                Mod mod = mi.Mod;
                if (mod is CalamityMod.CalamityMod)
                {
                    mod = CalamityEntropy.Instance;
                }
                return Language.GetOrRegister(mod.GetLocalizationKey(mi.Name + "Desc"));
            }
        }
        public abstract int ItemType { get; }
        public virtual SoundStyle? useSound => CEUtils.GetSound("loreEnabled");

        public virtual void ModifyTooltip(TooltipLine tooltip)
        {
        }

        public virtual void UpdateEffects(Player player)
        {
        }
    }
    public class LoreReworkSystem : ICELoader
    {
        public static Dictionary<int, LoreEffect> loreEffects;
        public void LoadData()
        {
            loreEffects = new Dictionary<int, LoreEffect>();
        }
        public void UnLoadData()
        {
            loreEffects = null;
        }
        public static void ToggleLore(Item item)
        {
            if (!ModContent.GetInstance<ServerConfig>().LoreSpecialEffect)
                return;
            if (loreEffects.ContainsKey(item.type))
            {
                if (Main.LocalPlayer.Entropy().enabledLoreItems.Contains(item.type))
                {
                    Main.LocalPlayer.Entropy().enabledLoreItems.Remove(item.type);
                }
                else
                {
                    Main.LocalPlayer.Entropy().enabledLoreItems.Add(item.type);
                }

            }
        }
        public static bool Enabled<T>() where T : LoreItem
        {
            return Main.LocalPlayer.Entropy().enabledLoreItems.Contains(ModContent.ItemType<T>());
        }
        public static bool Enabled(int type)
        {
            return Main.LocalPlayer.Entropy().enabledLoreItems.Contains(type);
        }
    }
    public class LoreReworkItem : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (!ModContent.GetInstance<ServerConfig>().LoreSpecialEffect)
                return;
            if (LoreReworkSystem.loreEffects.ContainsKey(entity.type))
            {
                entity.useTurn = true;
                entity.useTime = entity.useAnimation = 20;
                entity.useStyle = ItemUseStyleID.HoldUp;
            }
        }
        public override bool? UseItem(Item item, Player player)
        {
            if (!LoreReworkSystem.loreEffects.ContainsKey(item.type) || !ModContent.GetInstance<ServerConfig>().LoreSpecialEffect)
            {
                return null;
            }
            LoreReworkSystem.ToggleLore(item);
            if (LoreReworkSystem.loreEffects[item.type].useSound.HasValue)
            {
                SoundEngine.PlaySound(LoreReworkSystem.Enabled(item.type) ? LoreReworkSystem.loreEffects[item.type].useSound.Value : CEUtils.GetSound("AscendantOff"), player.Center);
            }
            return true;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ModContent.GetInstance<ServerConfig>().LoreSpecialEffect)
                return;

            if (!LoreReworkSystem.loreEffects.ContainsKey(item.type))
            {
                return;
            }
            Color? LoreColor = null;
            if (item.ModItem != null && item.ModItem is LoreItem li)
            {
                LoreColor = li.LoreColor;
            }
            TooltipLine tooltipLineEF = new TooltipLine(base.Mod, "Entropy:Effect", Language.GetTextValue("Mods.CalamityEntropy.UseToggle"));
            if (LoreColor.HasValue)
            {
                tooltipLineEF.OverrideColor = LoreColor.Value;
            }
            tooltips.Add(tooltipLineEF);
            var dsc = LoreReworkSystem.loreEffects[item.type].Decription;

            TooltipLine tooltipLineA = new TooltipLine(base.Mod, "Entropy:Effect", dsc.Value);
            if (LoreColor.HasValue)
            {
                tooltipLineA.OverrideColor = LoreColor.Value;
            }
            LoreReworkSystem.loreEffects[item.type].ModifyTooltip(tooltipLineA);
            tooltips.Add(tooltipLineA);

            TooltipLine tooltipLineE = new TooltipLine(base.Mod, "Entropy:Effect", Language.GetTextValue("Mods.CalamityEntropy." + (LoreReworkSystem.Enabled(item.type) ? "Enabled" : "Disabled")));
            tooltipLineE.OverrideColor = LoreReworkSystem.Enabled(item.type) ? Color.Yellow : Color.Gray;
            tooltips.Add(tooltipLineE);



            TooltipLine tooltipLine = new TooltipLine(base.Mod, "CalamityMod:Lore", Language.GetTextValue("Mods.CalamityEntropy.loreCruiser"));
            if (LoreColor.HasValue)
            {
                tooltipLine.OverrideColor = LoreColor.Value;
            }

            CalamityUtils.HoldShiftTooltip(tooltips, new TooltipLine[1] { tooltipLine }, hideNormalTooltip: true);
        }
    }
}
