using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Items.Weapons.GrassSword;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public class TlipocasScythe : RogueWeapon, IDevItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public string DevName => "Kino";
        public override float StealthDamageMultiplier => 1.6f;
        public override float StealthVelocityMultiplier => 1f;
        public override float StealthKnockbackMultiplier => 2f;

        public int SpeedUpTime = 0;
        public static int GetLevel()
        {
            //return Main.LocalPlayer.inventory[9].stack;
            int Level = 0;
            bool flag = true;
            void Check(bool f)
            {   
                if (f && flag)
                {
                    Level++;
                }
                else
                {
                    flag = false;
                }
            }
            //16
            Check(NPC.downedBoss1);
            Check(NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind);
            Check(DownedBossSystem.downedSlimeGod);
            Check(Main.hardMode);
            Check(DownedBossSystem.downedBrimstoneElemental);
            Check(DownedBossSystem.downedCalamitasClone);
            Check(EDownedBosses.downedProphet);
            Check(DownedBossSystem.downedRavager);
            Check(NPC.downedAncientCultist);
            Check(NPC.downedMoonlord);
            Check(DownedBossSystem.downedSignus);
            Check(DownedBossSystem.downedPolterghast);
            Check(DownedBossSystem.downedDoG);
            Check(EDownedBosses.downedCruiser);
            Check(DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs);
            Check(DownedBossSystem.downedPrimordialWyrm);
            return Level;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
            tooltips.Add(new TooltipLine(Mod, "Description", Mod.GetLocalization(Main.zenithWorld ? "TScytheZenithDesc" : "TScytheDesc").Value) { OverrideColor = Color.Crimson });
            bool holdAlt = Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
            bool holdShift = Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            if (holdShift && holdAlt)
                holdAlt = false;
            //这里的tooltip染色处理都是用[c/16进制颜色：]的
            #region 路径
            string pathPrefix = $"{CEUtils.LocalPrefix}.LegendaryAbility.";
            string lockedPath = pathPrefix + "General.Locked";
            //武器路径
            string pathBase = pathPrefix + $"{GetType().Name}Legend.";
            string pathLore = pathBase + "Dialog.TScytheDia";
            string pathAbility = pathBase + "Ability.TScytheA";
            string pathCondition = pathBase + "Downed.TScytheU";
            #endregion
            //从方法列表中获取被手动分类过的能力Tooltip
            string throwTooltip = AbilityThrowDesc(pathAbility, pathCondition, lockedPath);
            string teleportTooltip = AbilityTeleportDesc(pathAbility, pathCondition, lockedPath);
            string dashTooltip = AbilityDashDesc(pathAbility, pathCondition, lockedPath);
            string statTooltip = AbilityStat(pathAbility, pathCondition, lockedPath);

            bool isNeither = !holdShift && !holdAlt;
            bool shouldDrawHoldshift = !holdAlt && holdShift;
            bool shouldDrawPages = !holdShift && holdAlt;

            bool shouldDrawPagesTips = isNeither || shouldDrawHoldshift;
            bool shouldDrawHoldShiftTips = isNeither || shouldDrawPages;
            bool any = holdAlt || holdShift;
            
            //显示按住左shift的能力文本
            //我没有动原本的逻辑，只是为了处理翻页我才大幅度重写了这里的tooltip。如果有需求的话也可以直接把上方的能力列表全部组合起来变成按住左shift的文本
            if (shouldDrawHoldshift)
            {
                HandleHoldShift(tooltips);
            }
            //显示按住左alt的能力文本
            if (shouldDrawPages)
                HandleSwapAbility(tooltips, throwTooltip, teleportTooltip, dashTooltip, statTooltip);
            //显示按住左alt的提醒文本
            if (shouldDrawPagesTips && !any)
                tooltips.QuickAddTooltip($"{pathPrefix}General.PagesTips", Color.Yellow, LineName: "PagesMoreInfo");
            //显示按住左shift的提醒文本
            if (shouldDrawHoldShiftTips && !any)
                tooltips.QuickAddTooltipDirect(Mod.GetLocalization("PressShiftForMoreInfo").Value, Color.Yellow, LineName: "ShiftMoreInfo");

            HandleLoreAndLevel(tooltips, pathLore);
        }

        private void HandleHoldShift(List<TooltipLine> tooltips)
        {
            string Get(string key)
            {
                return Mod.GetLocalization(key).Value;
            }
            bool flag = NPC.downedBoss1;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA1") + (flag ? "" : Get("LOCKED") + " " + Get("TSU1")))
            { OverrideColor = flag ? Color.Yellow : Color.Gray });

            flag = DownedBossSystem.downedSlimeGod;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc", Get("TSA1B"))
            { OverrideColor = flag ? Color.Yellow : Color.Gray });

            flag = AllowThrow();
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA2") + (flag ? "" : Get("LOCKED") + " " + Get("TSU2")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = DownedBossSystem.downedBrimstoneElemental;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc", Get("TSA2B"))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = EDownedBosses.downedProphet;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA3") + (flag ? "" : Get("LOCKED") + " " + Get("TSU3")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = EDownedBosses.downedProphet;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA4") + (flag ? "" : Get("LOCKED") + " " + Get("TSU4")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = EDownedBosses.downedNihilityTwin;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA5") + (flag ? "" : Get("LOCKED") + " " + Get("TSU5")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = DownedBossSystem.downedDoG;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA6") + (flag ? "" : Get("LOCKED") + " " + Get("TSU6")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = DownedBossSystem.downedYharon;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA7") + (flag ? "" : Get("LOCKED") + " " + Get("TSU7")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = EDownedBosses.downedCruiser;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA8") + (flag ? "" : Get("LOCKED") + " " + Get("TSU8")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = DownedBossSystem.downedCalamitas;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA9") + (flag ? "" : Get("LOCKED") + " " + Get("TSU9")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });
        }
        #region Lore，与等级
        private void HandleLoreAndLevel(List<TooltipLine> tooltips, string pathLore)
        {
            int LoreLevel = Math.Clamp(GetLevel() + 1, 1, 17);
            string curLore = pathLore + LoreLevel.ToString();
            tooltips.QuickAddTooltip(curLore, Color.HotPink);
            //等级信息。
            string curLevel = Mod.GetLocalization("NowLV").Value + " - " + GetLevel() + "/16";
            TooltipLine levelTooltip = new(Mod, "CurLevel", curLevel) { OverrideColor = Color.Yellow };
            tooltips.Add(levelTooltip);
        }
        #endregion
        #region 文本翻页
        private void HandleSwapAbility(List<TooltipLine> tooltips, string throwTooltip, string teleportTooltip, string dashTooltip, string statTooltip)
        {
            //切换选择
            string selectedOne;
            selectedOne = SelectedDesc switch
            {
                AbilityDescSelect.Throw => throwTooltip,
                AbilityDescSelect.Dash => dashTooltip,
                AbilityDescSelect.Teleport => teleportTooltip,
                _ => statTooltip,
            };
            //重写原版的“Tooltip”内容。
            //其实也不用重写，草，加一行就行了
            tooltips.QuickAddTooltipDirect(selectedOne);
        }
        
        //投掷能力
        private string AbilityThrowDesc(string pathAbility, string pathCondition, string lockedPath)
        {
            //获取能力标题，并转化为文本值
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.ThrowTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();
            //任意邪恶boss后 - 基本投掷
            string baseThrowText = $"{pathAbility}2".ToLangValue();
            string downedEvilText = $"{lockedValue} {$"{pathCondition}2".ToLangValue()}";
            bool downedAnyEvil = NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind;
            //先知后 - 投掷按左键
            string pressThrowText = $"{pathAbility}3".ToLangValue();
            string downedProphetText = $"{lockedValue} {$"{pathCondition}3".ToLangValue()}";
            //进入判定区，全部组合完毕后，再使用标准符进行染色
            //能力锁定时，组合(锁定文本 +  能力文本)后染色并返回，实际文本将会为：[c/16进制颜色：总文本内容]，即进行了格式化字符串而非常规的复写颜色，下同
            baseThrowText = downedAnyEvil ? DyeText(baseThrowText, Color.Yellow) : DyeText(downedEvilText + "\n" + baseThrowText, Color.Gray);
            pressThrowText = EDownedBosses.downedProphet ? DyeText(pressThrowText, Color.Yellow) : DyeText(downedProphetText + "\n" + pressThrowText, Color.Gray);
            //处理封存 - 邪恶boss
            //最后组合： 标题 + 上述已经组合起来的能力文本 + 多个换行符
            string combination = DyeText(titleText, Color.Crimson)
                               + "\n" + baseThrowText
                               + "\n" + pressThrowText;
            return combination;
        }
        
        //传送能力
        private string AbilityTeleportDesc(string pathAbility, string pathCondition, string lockedPath)
        {
            //获取能力标题，并转化为文本值
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.TeleportTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();
            //传送斩击启用
            int cd = EDownedBosses.downedCruiser ? 10 : 15;
            string allowTeleportSlice = $"{pathAbility}2B".ToLangValue();
            //格式化
            allowTeleportSlice = allowTeleportSlice.ToFormatValue(cd);
            string downedBrimmyText = $"{lockedValue} {$"{pathCondition}2B".ToLangValue()})";
            
            //传送斩击附魔
            int voidBuffTime = EDownedBosses.downedCruiser ? 30 : 15;
            string enchanted = $"{pathAbility}5".ToLangValue();
            enchanted = enchanted.ToFormatValue(voidBuffTime.ToString(), "25%");
            string downedPolterText = $"{lockedValue} {$"{pathCondition}5".ToLangValue()})";

            string dogText = DyeText(DownedBossSystem.downedDoG ? $"{pathAbility}6".ToLangValue() : $"{lockedValue} {$"{pathCondition}6".ToLangValue()})", DownedBossSystem.downedDoG ? Color.Yellow : Color.Gray);

            //进入判定区，全部组合完毕后，再使用标准符进行染色
            //能力锁定时，组合(锁定文本 +  能力文本)后染色并返回，实际文本将会为：[c/16进制颜色：总文本内容]，即进行了格式化字符串而非常规的复写颜色，下同
            allowTeleportSlice = NPC.downedBoss1 ? DyeText(allowTeleportSlice, Color.Yellow) : DyeText(downedBrimmyText + "\n" + allowTeleportSlice, Color.Gray);
            enchanted = DownedBossSystem.downedPolterghast ? DyeText(enchanted, Color.Yellow) : DyeText(downedPolterText + "\n" + enchanted, Color.Gray);

            //最后组合： 标题 + 上述已经组合起来的能力文本 + 多个换行符
            string combination = DyeText(titleText, Color.Crimson)
                               + "\n" + allowTeleportSlice
                               + "\n" + enchanted
                               + "\n" + dogText;
            return combination;
        }
        //突刺能力
        private string AbilityDashDesc(string pathAbility, string pathCondition, string lockedPath)
        {
            //获取能力标题，并转化为文本值
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.DashTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();
            string allowDashText = $"{pathAbility}1".ToLangValue();
            //启用冲刺
            string downedEoCText = $"{lockedValue} {$"{pathCondition}1".ToLangValue()})";
            //冲刺滞留裂缝
            string tearDashText = $"{pathAbility}5".ToLangValue();
            string downedDoGText = $"{lockedValue} {$"{pathCondition}6".ToLangValue()})";

            //进入判定区，全部组合完毕后，再使用标准符进行染色
            //能力锁定时，组合(锁定文本 +  能力文本)后染色并返回，实际文本将会为：[c/16进制颜色：总文本内容]，即进行了格式化字符串而非常规的复写颜色，下同
            allowDashText = NPC.downedBoss1 ? DyeText(allowDashText, Color.Yellow) : DyeText(downedEoCText + "\n" + allowDashText, Color.Gray);
            tearDashText = DownedBossSystem.downedDoG ? DyeText(tearDashText, Color.Yellow) : DyeText(downedDoGText + "\n" + tearDashText, Color.Gray);

            //最后组合： 标题 + 上述已经组合起来的能力文本 + 多个换行符
            string combination = DyeText(titleText, Color.Crimson)
                               + "\n" + allowDashText
                               + "\n" + tearDashText;
            return combination;
        }

        //数值能力
        private string AbilityStat(string pathAbility, string pathCondition, string lockedPath)
        {
            //获取能力标题，并转化为文本值，染色。
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.StatTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();

            //突刺时给予无敌，造成伤害 - 史莱姆神
            string invinciDashText = $"{pathAbility}1B".ToLangValue();
            string downedSGLocked = $"{lockedValue} {$"{pathCondition}1B".ToLangValue()}";
            //自活 - 龙
            string selfReviveText = $"{pathAbility}7".ToLangValue();
            string dowendYharonLocked= $"{lockedValue} {$"{pathCondition}7".ToLangValue()}";
            //虚空触 - 巡游
            string voidTouchText = $"{pathAbility}8".ToLangValue();
            string downedPurpleWormLocked = $"{lockedValue} {$"{pathCondition}8".ToLangValue()}";
            //近程伤害 - 终灾
            string closeDamageText = $"{pathAbility}9".ToLangValue();
            string dowendScalLocked = $"{lockedValue} {$"{pathCondition}9".ToLangValue()}";
            
            //进入判定区，全部组合完毕后，再使用标准符进行染色
            //能力锁定时，组合(锁定文本 +  能力文本)后染色并返回，实际文本将会为：[c/16进制颜色：总文本内容]，即进行了格式化字符串而非常规的复写颜色，下同
            invinciDashText = DownedBossSystem.downedSlimeGod ? DyeText(invinciDashText, Color.Yellow) : DyeText(downedSGLocked + "\n" + invinciDashText, Color.Gray);
            selfReviveText = DownedBossSystem.downedYharon ? DyeText(selfReviveText, Color.Yellow) : DyeText(dowendYharonLocked + "\n" + selfReviveText, Color.Gray);
            voidTouchText = EDownedBosses.downedCruiser ? DyeText(voidTouchText, Color.Yellow) : DyeText(downedPurpleWormLocked + "\n" + voidTouchText, Color.Gray);
            closeDamageText = DownedBossSystem.downedCalamitas ? DyeText(closeDamageText, Color.Yellow) : DyeText(dowendScalLocked + "\n" + closeDamageText, Color.Gray);

            //最后组合： 标题 + 上述已经组合起来的能力文本 + 多个换行符
            string combination = DyeText(titleText, Color.Crimson)
                               + "\n" + invinciDashText
                               + "\n" + selfReviveText
                               + "\n" + voidTouchText
                               + "\n" + closeDamageText;
            return combination;
        }
        private static string DyeText(string textValue, Color color)
        {
            string colorValue =  $"{color.R:X2}{color.G:X2}{color.B:X2}";
            //处理染色换行
            string[] lines = textValue.Split(['\n'], StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = $"[c/{colorValue}:{lines[i]}]";
            }
            string realValue = string.Join("\n", lines);
            return realValue;
        }
        //枚举
        private enum AbilityDescSelect
        {
            Stat,
            Throw,
            Dash,
            Teleport
        }
        private AbilityDescSelect SelectedDesc = AbilityDescSelect.Stat;
        //右键切换物品描述需要的东西
        public override bool CanRightClick() => Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
        public override bool ConsumeItem(Player player) => false;
        public override void RightClick(Player player)
        {
            SelectedDesc++;
            if ((int)SelectedDesc % 4 is 0)
                SelectedDesc = AbilityDescSelect.Stat;
        }
        #endregion
        public override void UpdateInventory(Player player)
        {
            int dmg = 20;
            int lv = GetLevel();
            switch(lv)
            {
                case 0: dmg = 20; break;
                case 1: dmg = 36; break;
                case 2: dmg = 40; break;
                case 3: dmg = 60; break;
                case 4: dmg = 80; break;
                case 5: dmg = 140; break;
                case 6: dmg = 180; break;
                case 7: dmg = 220; break;
                case 8: dmg = 300;  break;
                case 9: dmg = 400;  break;
                case 10: dmg = 500;  break;
                case 11: dmg = 600;  break;
                case 12: dmg = 800;  break;
                case 13: dmg = 1200;  break;
                case 14: dmg = 1800;  break;
                case 15: dmg = 2200;  break;
                case 16: dmg = 3600;  break;
            }
            Item.damage = dmg;
            if(player.name.ToLower() is "tlipoca")
            {
                Item.SetNameOverride(Mod.GetLocalization("TScytheSpecialName").Value);
            }
            else if(Main.zenithWorld)
            {
                Item.SetNameOverride(Mod.GetLocalization("TScytheZenithName").Value);
            }
            if (throwType == -1)
                throwType = ModContent.ProjectileType<TlipocasScytheThrow>();
            Item.useTime = Item.useAnimation = (44 - GetLevel()) / (SpeedUpTime > 0 ? 6 : 1);
        }
        public override void HoldItem(Player player)
        {
            UpdateInventory(player);
            if(SpeedUpTime > 0)
                SpeedUpTime--;
            if (SpeedUpTime < 0)
                SpeedUpTime = 0;
        }
        public override void SetDefaults()
        {
            Item.width = 132;
            Item.height = 116;
            Item.damage = 22;
            Item.DamageType = CEUtils.RogueDC;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.ArmorPenetration = 32;
            Item.shoot = ModContent.ProjectileType<TlipocasScytheHeld>();
            Item.Entropy().tooltipStyle = 4;
            Item.Entropy().NameColor = new Color(160, 0, 0);
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(90, 0, 0);
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
            Item.Entropy().Legend = true;
        }
        public int swing = 0;
        public static int throwType = -1;
        public override bool AllowPrefix(int pre) => false;

        public static bool AllowDash() => NPC.downedBoss1;
        public static bool DashImmune() => DownedBossSystem.downedSlimeGod;
        public static bool AllowThrow() => NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind;
        public static bool AllowSpin() => EDownedBosses.downedProphet;
        public static bool DashUpgrade() => DownedBossSystem.downedSignus;
        public static bool AllowRevive() => DownedBossSystem.downedYharon;
        public static bool AllowVoidEmpowerment() => EDownedBosses.downedNihilityTwin;
        public override bool AltFunctionUse(Player player) => AllowThrow();
        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasBuff<VoidEmpowerment>())
            {
                damage = (int)(damage * 1.2f);
            }
            
            if (player.altFunctionUse == 2)
            {
                velocity *= 0.46f;
                type = throwType;
                damage = (int)(damage / 1.5f);
            }
            if (AllowDash() && player.controlUp && !player.HasCooldown(TlipocasScytheSlashCooldown.ID))
            {
                player.AddCooldown(TlipocasScytheSlashCooldown.ID, 7 * 60);
                int p = Projectile.NewProjectile(source, position, velocity.normalize() * 1000 * (DashUpgrade() ? 1.33f : 1), ModContent.ProjectileType<TSSlash>(), damage * 2, knockback, player.whoAmI);
                if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                if (DownedBossSystem.downedPolterghast)
                {
                    Projectile.NewProjectile(source, position + velocity.normalize() * 400 * (DashUpgrade() ? 1.33f : 1), velocity.normalize() * 1000 * (DashUpgrade() ? 1.33f : 1), ModContent.ProjectileType<TSSlash>(), damage * 2, knockback, player.whoAmI, 0, 1);
                }
            }
            else
            {
                if (player.ownedProjectileCounts[throwType] > 0)
                    return false;
                int p = Projectile.NewProjectile(source, position, velocity, type, damage * 1, knockback, player.whoAmI, swing == 0 ? 1 : -1);
                if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    int ut = 44 - GetLevel();
                    SpeedUpTime += ut + (int)(ut * 0.35f);
                }
                CostStealthForPlr(player);

                swing = 1 - swing;
            }
            return false;
        }
        public void CostStealthForPlr(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                float cost = 1;
                if (player.Calamity().stealthStrike90Cost)
                    cost = 0.9f;
                if (player.Calamity().stealthStrike75Cost)
                    cost = 0.75f;
                if (player.Calamity().stealthStrikeHalfCost)
                    cost = 0.5f;
                player.Calamity().rogueStealth -= player.Calamity().rogueStealthMax * cost;
            }
            else
            {
                player.Calamity().rogueStealth = 0;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Voidstone>(10)
                .AddIngredient<BloodOrb>(10)
                .AddIngredient(ItemID.Deathweed)
                .AddIngredient<LoreAwakening>()
                .AddTile(TileID.Anvils).Register();
        }
    }
    
    public class TSSlash : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC);
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if(!TlipocasScythe.DashImmune())
                return false;
            return null;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("slice", 1, target.Center);
            EParticle.NewParticle(new MultiSlash() { xadd = 1f, lx = 1f, endColor = Color.Red, spawnColor = Color.IndianRed }, target.Center, Vector2.Zero, Color.IndianRed, 1, 1, true, BlendState.Additive, 0);
            if(TlipocasScythe.DashUpgrade())
            {
                target.AddBuff<MarkedforDeath>(20 * 60);
                target.AddBuff<WhisperingDeath>(20 * 60);
            }
        }
        public bool MovePlayer = true;
        public override void AI()
        {
            if(TlipocasScythe.DashImmune())
            {
                Projectile.GetOwner().Entropy().immune = 4;
            }
            if (Projectile.localAI[1]++ == 0)
            {
                CEUtils.PlaySound("AbyssalBladeLaunch", 1, Projectile.Center);
            }
            Player player = Projectile.GetOwner();
            if (Projectile.ai[1] == 0 && MovePlayer)
            {
                Vector2 odp = player.Center;
                player.Center = Vector2.Lerp(Projectile.Center + Projectile.velocity, Projectile.Center, Projectile.timeLeft / 10f);
                if (CEUtils.IsPlayerStuck(player))
                {
                    MovePlayer = false;
                    player.Center = odp;
                }
            }
            if (Projectile.timeLeft == 10)
            {
                if (Projectile.ai[1] == 0)
                {
                    if(DownedBossSystem.downedDoG)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity / 16f, ModContent.ProjectileType<BloodCrack>(), Projectile.damage / 6, 0, Projectile.owner);
                    }
                    player.Entropy().screenShift = 1;
                    player.Entropy().screenPos = player.Center;
                    Vector2 top = Projectile.Center;
                    Vector2 sparkVelocity2 = Projectile.velocity * 0.08f;
                    Vector2 rd = Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2);
                    int sparkLifetime2 = 24;
                    float sparkScale2 = 1.5f;
                    for (float i = 0; i < 1; i += 0.01f)
                    {
                        Color sparkColor2 = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0, 1));
                        var spark = new AltSparkParticle(top + CEUtils.randomPointInCircle(32), sparkVelocity2 * Main.rand.NextFloat(), false, (int)(sparkLifetime2), sparkScale2 * Main.rand.NextFloat(0.6f, 1), sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
                else
                {
                    if (DownedBossSystem.downedDoG)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.PiOver2) / 16f / 2, ModContent.ProjectileType<BloodCrack>(), Projectile.damage / 6, 0, Projectile.owner);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedBy(-MathHelper.PiOver2) / 16f / 2, ModContent.ProjectileType<BloodCrack>(), Projectile.damage / 6, 0, Projectile.owner);
                    }
                    Vector2 top = Projectile.Center;
                    Vector2 sparkVelocity2 = Projectile.velocity * 0.04f;
                    Vector2 rd = Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2);
                    int sparkLifetime2 = 24;
                    float sparkScale2 = 1.5f;
                    for (float i = 0; i < 1; i += 0.01f)
                    {
                        Color sparkColor2 = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0, 1));
                        var spark = new AltSparkParticle(top + CEUtils.randomPointInCircle(32), sparkVelocity2.RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool() ? 1 : -1)) * Main.rand.NextFloat(), false, (int)(sparkLifetime2), sparkScale2 * Main.rand.NextFloat(0.6f, 1), sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
        }

        public override bool ShouldUpdatePosition() { return false; }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] != 0)
            {
                return CEUtils.LineThroughRect(Projectile.Center - Projectile.velocity.RotatedBy(MathHelper.PiOver2 * 0.5f), Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * 0.5f), targetHitbox, 32);
            }
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity, targetHitbox, 32);
        }
    }

    public class TlipocasScytheHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/TlipocasScythe";
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 2;
            if(DownedBossSystem.downedCalamitas)
            {
                float dmgMult = Utils.Remap(CEUtils.getDistance(target.Center, Projectile.Center), 160, 300, 1.35f, 1);
                modifiers.FinalDamage *= dmgMult;
            }
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.light = 0.2f;
            Projectile.MaxUpdates = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public List<float> oldRots = new List<float>();
        public List<float> oldScale = new List<float>();
        public float counter = 0;
        public bool flagS = true;
        public bool flag = true;
        public bool Canhit = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.SetShake(target.Center, Projectile.Calamity().stealthStrike ? 8 : 4);
            if (Projectile.ai[1] == 1 && TlipocasScythe.AllowVoidEmpowerment())
            {
                int VETime = EDownedBosses.downedCruiser ? 30 : 15;
                VETime *= 60;
                Projectile.GetOwner().AddBuff(ModContent.BuffType<VoidEmpowerment>(), VETime);
            }
            if (target.townNPC && target.life <= 0)
            {
                SoundEngine.PlaySound(PerforatorHive.DeathSound, target.Center);
                var player = Projectile.GetOwner();
                player.QuickSpawnItem(target.GetSource_Death(), new Item(ItemID.SilverCoin, 10), 10);
                player.QuickSpawnItem(target.GetSource_Death(), new Item(ModContent.ItemType<BloodOrb>(), 2), 2);
                if(NPC.downedPlantBoss)
                {
                    player.QuickSpawnItem(target.GetSource_Death(), new Item(1508, 2), 2);
                }
                if(NPC.downedMoonlord)
                {
                    player.QuickSpawnItem(target.GetSource_Death(), new Item(ModContent.ItemType<Necroplasm>(), 2), 2);
                }
            }
            if (flagS)
            {
                if(TlipocasScythe.AllowSpin() && Projectile.Calamity().stealthStrike)
                {
                    Projectile.GetOwner().Heal(DownedBossSystem.downedPolterghast ? 30 : 15);
                }
                CEUtils.PlaySound("voidseekershort", 1, target.Center, 6, CEUtils.WeapSound);
                flagS = false;
                if (!target.Organic())
                {
                    CEUtils.PlaySound("metalhit", Main.rand.NextFloat(0.8f, 1.2f) / Projectile.ai[1], target.Center, 6);
                }
            }
            if(EDownedBosses.downedCruiser)
            {
                EGlobalNPC.AddVoidTouch(target, 60, 5, 1000, 12);
            }
            if(DownedBossSystem.downedCalamitas)
            {
                target.AddBuff<VulnerabilityHex>(60 * 5);
            }
            else
            if (DownedBossSystem.downedRavager)
            {
                if (DownedBossSystem.downedProvidence)
                {
                    target.AddBuff<Shred>(60 * 5);
                }
                else
                {
                    target.AddBuff<Laceration>(60 * 5);
                }
            }
            else
            {
                target.AddBuff<HeavyBleeding>(60 * 5);
            }
            Color impactColor = Color.Red;
            float impactParticleScale = Main.rand.NextFloat(1.4f, 1.6f);

            SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.LawnGreen, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);


            float sparkCount = 8 + TlipocasScythe.GetLevel();
            for (int i = 0; i < sparkCount; i++)
            {
                float p = Main.rand.NextFloat();
                Vector2 sparkVelocity2 = (target.Center - Projectile.Center).normalize().RotatedByRandom(p * 0.4f) * Main.rand.NextFloat(6, 20 * (2 - p)) * (1 + TlipocasScythe.GetLevel() * 0.1f);
                int sparkLifetime2 = (int)((2 - p) * 16);
                float sparkScale2 = 0.6f + (1 - p);
                sparkScale2 *= (1 + TlipocasScythe.GetLevel() * 0.06f);
                Color sparkColor2 = Color.Lerp(Color.DarkRed, Color.IndianRed, p);
                if (Projectile.GetOwner().HasBuff<VoidEmpowerment>())
                {
                    sparkColor2 = Color.Lerp(Color.DeepSkyBlue, Color.Purple, p);
                    if (Main.rand.NextBool())
                    {
                        AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (Projectile.frame == 7 ? 1f : 0.65f), false, (int)(sparkLifetime2 * (Projectile.frame == 7 ? 1.2f : 1f)), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Color.Purple);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
                else
                {
                    if (Main.rand.NextBool())
                    {
                        AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : Color.DarkRed);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 120, 120), 1, 1, true, BlendState.Additive, 0, 6);

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Canhit;
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            player.itemTime = player.itemAnimation = 2;
            float progress = (counter / (player.itemAnimationMax * Projectile.MaxUpdates));
            counter++;
            if (Projectile.ai[2] == 0)
                player.heldProj = Projectile.whoAmI;
            
            int dir = (int)Projectile.ai[0] * Math.Sign(Projectile.velocity.X);

            float ySc = 0.6f;
            ProjScale = 1.4f + TlipocasScythe.GetLevel() * 0.04f;
            ProjScale *= (1 + Projectile.ai[1] * 0.5f);
            if (Projectile.ai[1] == 1)
            {
                if (progress < 0.1f)
                {
                    counter = (int)(0.1f * player.itemAnimationMax * Projectile.MaxUpdates + 1);
                }
            }
            if(Projectile.Calamity().stealthStrike)
            {
                ySc = 0.5f;
                ProjScale *= 2f;
            }
            float r = 3.6f;
            float r1 = 0.5f;
            float r2 = 0.8f;
            float pn = 0.36f;
            if(progress >= pn && flag)
            {
                Canhit = true;
                flag = false;
                CEUtils.PlaySound("scytheswing", Main.rand.NextFloat(1.6f, 1.8f), Projectile.Center, 4, CEUtils.WeapSound);
            }
            if(progress < pn)
            {
                Projectile.rotation = (-r/2f - CEUtils.GetRepeatedCosFromZeroToOne(progress / pn, 2) * r1) * dir;
            }
            else 
            {
                Projectile.rotation = (-r/2f - r1 + (CEUtils.GetRepeatedParaFromZeroToOne((progress - pn) / (1 - pn), 3) * 0.6f + 0.4f * CEUtils.GetRepeatedParaFromZeroToOne((progress - pn) / (1 - pn), 2)) * (r + r2)) * dir;
            }
            scale = (Projectile.rotation.ToRotationVector2() * new Vector2(1, ySc)).Length();
            if(progress > 0.7f)
            {
                ProjScale *= (1 - (progress - 0.7f) / 0.3f) * 0.2f + 0.8f;
            }
            Projectile.rotation = (Projectile.rotation.ToRotationVector2() * new Vector2(1, ySc)).ToRotation() + Projectile.velocity.ToRotation();
            if(progress > 1)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[2] == 0)
                player.SetHandRotWithDir(Projectile.rotation, Math.Sign(Projectile.velocity.X));
            if (progress > pn)
            {
                oldScale.Add(scale);
                oldRots.Add(Projectile.rotation);
                if (oldRots.Count > 120)
                {
                    oldRots.RemoveAt(0);
                    oldScale.RemoveAt(0);
                }
            }
            if (Projectile.ai[2] == 0)
                Projectile.Center = player.MountedCenter + new Vector2(player.direction * -6, 0);

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 146 * scale * ProjScale, targetHitbox, (int)(36 * ProjScale));
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 150 * Projectile.scale * scale * ProjScale, 54, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D trail = CEUtils.getExtraTex("StreakGoop");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);

            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(168 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(90 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? Color.Purple : Color.Firebrick).ToVector4());
                    shader.Parameters["color2"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? new Color(160, 160, 255) : Color.OrangeRed).ToVector4());

                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            int dir = (int)(Projectile.ai[0]) * Math.Sign(Projectile.velocity.X);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * ProjScale * scale, effect);

   
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        
        public float alpha = 1;
        public float ProjScale = 1;
        public float scale = 1;
    }
    public class TlipocasScytheThrow : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/TlipocasScythe";
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (DownedBossSystem.downedCalamitas)
            {
                float dmgMult = Utils.Remap(CEUtils.getDistance(target.Center, Projectile.Center), 160, 300, 1.35f, 1);
                modifiers.FinalDamage *= dmgMult;
            }
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.light = 0.2f;
            Projectile.MaxUpdates = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 64;
        }

        public List<Vector2> oldPos = new List<Vector2>();
        public List<float> oldRots = new List<float>();
        public List<float> oldScale = new List<float>();
        public float counter = 0;
        public bool flagS = true;
        public bool flag = true;
        public bool StickOnMouse = false;
        public bool RightLast = true;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.SetShake(target.Center, Projectile.Calamity().stealthStrike ? 8 : 4);
            if (Projectile.numHits == 1)
            {
                if (TlipocasScythe.AllowSpin() && Projectile.Calamity().stealthStrike)
                {
                    Projectile.GetOwner().Heal(DownedBossSystem.downedPolterghast ? 30 : 15);
                }
            }
            if(counter < 16 * 10)
            {
                counter = 16 * 10;
            }
            CEUtils.PlaySound("voidseekershort", 1, target.Center, 6, CEUtils.WeapSound * 0.4f);
            if (!target.Organic())
            {
                CEUtils.PlaySound("metalhit", Main.rand.NextFloat(0.8f, 1.2f) / Projectile.ai[1], target.Center, 6, CEUtils.WeapSound * 0.4f);
            }
            if (EDownedBosses.downedCruiser)
            {
                EGlobalNPC.AddVoidTouch(target, 60, 5, 1000, 12);
            }
            if (DownedBossSystem.downedCalamitas)
            {
                target.AddBuff<VulnerabilityHex>(60 * 5);
            }
            else
            if (DownedBossSystem.downedRavager)
            {
                if (DownedBossSystem.downedProvidence)
                {
                    target.AddBuff<Shred>(60 * 5);
                }
                else
                {
                    target.AddBuff<Laceration>(60 * 5);
                }
            }
            else
            {
                target.AddBuff<HeavyBleeding>(60 * 5);
            }
            Color impactColor = Color.Red;
            float impactParticleScale = Main.rand.NextFloat(1.4f, 1.6f);

            SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.LawnGreen, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);


            float sparkCount = 14 + TlipocasScythe.GetLevel();
            for (int i = 0; i < sparkCount; i++)
            {
                float p = Main.rand.NextFloat();
                Vector2 sparkVelocity2 = CEUtils.randomRot().ToRotationVector2().RotatedByRandom(p * 0.4f) * Main.rand.NextFloat(6, 20 * (2 - p)) * (1 + TlipocasScythe.GetLevel() * 0.1f);
                int sparkLifetime2 = (int)((2 - p) * 16);
                float sparkScale2 = 0.6f + (1 - p);
                sparkScale2 *= (1 + TlipocasScythe.GetLevel() * 0.06f);
                Color sparkColor2 = Color.Lerp(Color.DarkRed, Color.IndianRed, p);
                if (Projectile.GetOwner().HasBuff<VoidEmpowerment>())
                {
                    sparkColor2 = Color.Lerp(Color.DeepSkyBlue, Color.Purple, p);
                    if (Main.rand.NextBool())
                    {
                        AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (Projectile.frame == 7 ? 1f : 0.65f), false, (int)(sparkLifetime2 * (Projectile.frame == 7 ? 1.2f : 1f)), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Color.Purple);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
                else
                {
                    if (Main.rand.NextBool())
                    {
                        AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : Color.DarkRed);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 120, 120), 1, 1, true, BlendState.Additive, 0, 6);

        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StickOnMouse);
            writer.Write(counter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StickOnMouse = reader.ReadBoolean();
            counter = reader.ReadSingle();
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            ProjScale = 1.4f + TlipocasScythe.GetLevel() * 0.02f;
            counter++;
            player.itemTime = player.itemAnimation = 2;
            if (counter > 16 * (StickOnMouse ? 66 : 16))
            {
                Projectile.velocity *= 0.996f;
                Projectile.velocity += (player.Center - Projectile.Center).normalize() * 0.05f;
                if(CEUtils.getDistance(Projectile.Center, player.Center) < Projectile.velocity.Length() + 128)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (StickOnMouse)
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += (player.Calamity().mouseWorld - Projectile.Center).normalize() * 0.1f;
                }
            }
            player.Calamity().mouseWorldListener = true;

            Projectile.rotation += 0.04f;
            oldScale.Add(1);
            oldPos.Add(Projectile.Center);
            oldRots.Add(Projectile.rotation);
            if(oldRots.Count > 16 * 6)
            {
                oldRots.RemoveAt(0);
                oldScale.RemoveAt(0);
                oldPos.RemoveAt(0);
            }
            if(Main.myPlayer == Projectile.owner)
            {
                if(Main.mouseLeft && !player.HasCooldown(TeleportSlashCooldown.ID) && DownedBossSystem.downedBrimstoneElemental)
                {
                    player.AddCooldown(TeleportSlashCooldown.ID, (EDownedBosses.downedCruiser ? 10 : 15) * 60);
                    player.Entropy().screenShift = 1f;
                    player.Entropy().screenPos = player.Center;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, (Projectile.Center - player.Center).SafeNormalize((Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX)) * 16, ModContent.ProjectileType<TlipocasScytheHeld>(), Projectile.damage * 6, Projectile.knockBack, player.whoAmI, 1, 1);
                    if(DownedBossSystem.downedPolterghast)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, (Projectile.Center - player.Center).SafeNormalize((Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX)) * 16, ModContent.ProjectileType<TlipocasScytheHeld>(), Projectile.damage * 6, Projectile.knockBack, player.whoAmI, 1, 1, 1);
                        EParticle.spawnNew(new PlayerShadowBlack() { plr = player }, player.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.AlphaBlend, 0, 60);
                    }
                    Projectile.Kill();
                    player.Center = Projectile.Center;
                }
                if(!StickOnMouse && Main.mouseRight && TlipocasScythe.AllowSpin() && !RightLast)
                {
                    counter = 0;
                    StickOnMouse = true;
                    CEUtils.SyncProj(Projectile.whoAmI);
                    Projectile.damage = (int)(Projectile.damage / 2.5f);
                }
                RightLast = Main.mouseRight;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Center.getRectCentered((int)(142 * ProjScale), (int)(142 * ProjScale)).Intersects(targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D trail = CEUtils.getExtraTex("StreakGoop");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);

            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(84 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(50 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? Color.Purple : Color.Firebrick).ToVector4());
                    shader.Parameters["color2"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? new Color(160, 160, 255) : Color.OrangeRed).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            Vector2 origin = tex.Size() / 2f;
            float rot = Projectile.rotation;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * ProjScale * scale, SpriteEffects.None);


            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public float alpha = 1;
        public float ProjScale = 1;
        public float scale = 1;
    }
    public class BloodCrack : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ArmorPenetration = 128;
            Projectile.timeLeft = 48;
        }
        public List<Vector2> points = new List<Vector2>();
        int d = 0;
        public override void AI()
        {
            d++;
            if (d > 16)
            {
                Projectile.velocity *= 0;
            }
            else
            {
                Vector2 o = (points.Count > 0 ? points[points.Count - 1] : Projectile.Center - Projectile.velocity);
                Vector2 nv = Projectile.Center + CEUtils.randomVec(4);
                for (float i = 0.1f; i <= 1; i += 0.1f)
                {
                    points.Add(Vector2.Lerp(o, nv, i));
                }
            }
        }
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (points.Count < 1)
            {
                return false;
            }
            for (int i = 1; i < points.Count; i++)
            {
                if (CEUtils.LineThroughRect(points[i - 1], points[i], targetHitbox, 30))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public void draw()
        {
            if (points.Count < 1)
            {
                return;
            }
            Texture2D px = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
            float jd = 1;
            float lw = Projectile.timeLeft / 30f;
            Color color = Color.White;
            for (int i = 1; i < points.Count; i++)
            {
                Vector2 jv = Vector2.Zero;
                CEUtils.drawLine(Main.spriteBatch, px, points[i - 1], points[i] + jv, color * jd, 1f * lw * (new Vector2(-16, 0).RotatedBy(MathHelper.ToRadians(180 * ((float)i / points.Count)))).Y, 3);
            }
        }
    }
}
