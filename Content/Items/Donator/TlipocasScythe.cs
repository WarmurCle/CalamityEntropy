using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Items.Vanity;
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
using Terraria.GameContent;
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
        public override float StealthDamageMultiplier => 1.2f;
        public override float StealthVelocityMultiplier => 1f;
        public override float StealthKnockbackMultiplier => 2f;

        public int SpeedUpTime = 0;
        public static int GetLevel()
        {
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
        public int NowLevel = 0;
        public bool RecheckStats = true;
        private static float UpdatePos
        {
            get
            {
                return ((float)(MathF.Sin(Main.GlobalTimeWrappedHourly * 1f) * 1.2f + 1.4f)).ToClamp(1.0f, 2.4f);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * 2f, null, Color.DarkRed with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);

            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.Red);
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Description", Mod.GetLocalization(Main.zenithWorld ? "TScytheZenithDesc" : "TScytheDesc").Value) { OverrideColor = Color.Crimson });
            bool holdAlt = Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
            bool holdShift = Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            if (holdShift && holdAlt)
                holdAlt = false;
            #region 路径
            string pathPrefix = $"{CEUtils.LocalPrefix}.LegendaryAbility.";
            string lockedPath = pathPrefix + "General.Locked";
            string pathBase = pathPrefix + $"{GetType().Name}Legend.";
            string pathLore = pathBase + "Dialog.TScytheDia";
            string pathAbility = pathBase + "Ability.TScytheA";
            string pathCondition = pathBase + "Downed.TScytheU";
            #endregion
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

            if (shouldDrawHoldshift)
            {
                HandleHoldShift(tooltips);
            }
            if (shouldDrawPages)
                HandleSwapAbility(tooltips, throwTooltip, teleportTooltip, dashTooltip, statTooltip);
            if (shouldDrawPagesTips && !any)
                tooltips.QuickAddTooltip($"{pathPrefix}General.PagesTips", Color.Yellow, LineName: "PagesMoreInfo");
            if (shouldDrawHoldShiftTips && !any)
                tooltips.QuickAddTooltipDirect(Mod.GetLocalization("PressShiftForMoreInfo").Value, Color.Yellow, LineName: "ShiftMoreInfo");
            bool isKilledNPC = Main.LocalPlayer.GetModPlayer<TlipocasSingleHit>().isPressed;
            if (isKilledNPC)
                tooltips.QuickAddTooltip($"{pathBase}TlipocazKilledNPC", Color.Red, LineName: "CanKilledNPC");
            else
                tooltips.QuickAddTooltip($"{pathBase}TlipocazKilledNPCNot", Color.Gray, LineName: "CanNotKileldNPC");
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

            flag = EDownedBosses.downedNihilityTwin;
            tooltips.Add(new TooltipLine(Mod, "Ability Desc",
                Get("TSA4") + (flag ? "" : Get("LOCKED") + " " + Get("TSU4")))
            { OverrideColor = (flag ? Color.Yellow : Color.Gray) });

            flag = DownedBossSystem.downedPolterghast;
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
            string curLevel = Mod.GetLocalization("NowLV").Value + " - " + GetLevel() + "/16";
            TooltipLine levelTooltip = new(Mod, "CurLevel", curLevel) { OverrideColor = Color.Yellow };
            tooltips.Add(levelTooltip);
        }
        #endregion
        #region 文本翻页
        private void HandleSwapAbility(List<TooltipLine> tooltips, string throwTooltip, string teleportTooltip, string dashTooltip, string statTooltip)
        {
            string selectedOne;
            selectedOne = SelectedDesc switch
            {
                AbilityDescSelect.Throw => throwTooltip,
                AbilityDescSelect.Dash => dashTooltip,
                AbilityDescSelect.Teleport => teleportTooltip,
                _ => statTooltip,
            };
            tooltips.QuickAddTooltipDirect(selectedOne);
        }

        private string AbilityThrowDesc(string pathAbility, string pathCondition, string lockedPath)
        {
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.ThrowTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();
            string baseThrowText = $"{pathAbility}2".ToLangValue();
            string downedEvilText = $"{lockedValue} {$"{pathCondition}2".ToLangValue()}";
            bool downedAnyEvil = NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind;
            string pressThrowText = $"{pathAbility}3".ToLangValue();
            string downedProphetText = $"{lockedValue} {$"{pathCondition}3".ToLangValue()}";
            baseThrowText = downedAnyEvil ? DyeText(baseThrowText, Color.Yellow) : DyeText(downedEvilText + "\n" + baseThrowText, Color.Gray);
            pressThrowText = EDownedBosses.downedProphet ? DyeText(pressThrowText, Color.Yellow) : DyeText(downedProphetText + "\n" + pressThrowText, Color.Gray);
            string combination = DyeText(titleText, Color.Crimson)
       + "\n" + baseThrowText
       + "\n" + pressThrowText;
            return combination;
        }

        private string AbilityTeleportDesc(string pathAbility, string pathCondition, string lockedPath)
        {
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.TeleportTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();
            int cd = EDownedBosses.downedCruiser ? 10 : 15;
            string allowTeleportSlice = $"{pathAbility}2B".ToLangValue();
            allowTeleportSlice = allowTeleportSlice.ToFormatValue(cd);
            string downedBrimmyText = $"{lockedValue} {$"{pathCondition}2B".ToLangValue()})";

            int voidBuffTime = EDownedBosses.downedCruiser ? 30 : 15;
            string enchanted = $"{pathAbility}5".ToLangValue();
            enchanted = enchanted.ToFormatValue(voidBuffTime.ToString(), "25%");
            string downedPolterText = $"{lockedValue} {$"{pathCondition}5".ToLangValue()})";

            string dogText = DyeText(DownedBossSystem.downedDoG ? $"{pathAbility}6".ToLangValue() : $"{lockedValue} {$"{pathCondition}6".ToLangValue()})", DownedBossSystem.downedDoG ? Color.Yellow : Color.Gray);

            allowTeleportSlice = NPC.downedBoss1 ? DyeText(allowTeleportSlice, Color.Yellow) : DyeText(downedBrimmyText + "\n" + allowTeleportSlice, Color.Gray);
            enchanted = DownedBossSystem.downedPolterghast ? DyeText(enchanted, Color.Yellow) : DyeText(downedPolterText + "\n" + enchanted, Color.Gray);

            string combination = DyeText(titleText, Color.Crimson)
                   + "\n" + allowTeleportSlice
                   + "\n" + enchanted
                   + "\n" + dogText;
            return combination;
        }
        private string AbilityDashDesc(string pathAbility, string pathCondition, string lockedPath)
        {
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.DashTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();
            string allowDashText = $"{pathAbility}1".ToLangValue();
            string downedEoCText = $"{lockedValue} {$"{pathCondition}1".ToLangValue()})";
            string tearDashText = $"{pathAbility}5".ToLangValue();
            string downedDoGText = $"{lockedValue} {$"{pathCondition}6".ToLangValue()})";

            allowDashText = NPC.downedBoss1 ? DyeText(allowDashText, Color.Yellow) : DyeText(downedEoCText + "\n" + allowDashText, Color.Gray);
            tearDashText = DownedBossSystem.downedDoG ? DyeText(tearDashText, Color.Yellow) : DyeText(downedDoGText + "\n" + tearDashText, Color.Gray);

            string combination = DyeText(titleText, Color.Crimson)
                   + "\n" + allowDashText
                   + "\n" + tearDashText;
            return combination;
        }

        private string AbilityStat(string pathAbility, string pathCondition, string lockedPath)
        {
            string titleText = $"{CEUtils.LocalPrefix}.LegendaryAbility.{GetType().Name}Legend.Conditions.StatTitle".ToLangValue();
            string lockedValue = lockedPath.ToLangValue();

            string invinciDashText = $"{pathAbility}1B".ToLangValue();
            string downedSGLocked = $"{lockedValue} {$"{pathCondition}1B".ToLangValue()}";
            string selfReviveText = $"{pathAbility}7".ToLangValue();
            string dowendYharonLocked = $"{lockedValue} {$"{pathCondition}7".ToLangValue()}";
            string voidTouchText = $"{pathAbility}8".ToLangValue();
            string downedPurpleWormLocked = $"{lockedValue} {$"{pathCondition}8".ToLangValue()}";
            string closeDamageText = $"{pathAbility}9".ToLangValue();
            string dowendScalLocked = $"{lockedValue} {$"{pathCondition}9".ToLangValue()}";

            invinciDashText = DownedBossSystem.downedSlimeGod ? DyeText(invinciDashText, Color.Yellow) : DyeText(downedSGLocked + "\n" + invinciDashText, Color.Gray);
            selfReviveText = DownedBossSystem.downedYharon ? DyeText(selfReviveText, Color.Yellow) : DyeText(dowendYharonLocked + "\n" + selfReviveText, Color.Gray);
            voidTouchText = EDownedBosses.downedCruiser ? DyeText(voidTouchText, Color.Yellow) : DyeText(downedPurpleWormLocked + "\n" + voidTouchText, Color.Gray);
            closeDamageText = DownedBossSystem.downedCalamitas ? DyeText(closeDamageText, Color.Yellow) : DyeText(dowendScalLocked + "\n" + closeDamageText, Color.Gray);

            string combination = DyeText(titleText, Color.Crimson)
                   + "\n" + invinciDashText
                   + "\n" + selfReviveText
                   + "\n" + voidTouchText
                   + "\n" + closeDamageText;
            return combination;
        }
        private static string DyeText(string textValue, Color color)
        {
            string colorValue = $"{color.R:X2}{color.G:X2}{color.B:X2}";
            string[] lines = textValue.Split(['\n'], StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = $"[c/{colorValue}:{lines[i]}]";
            }
            string realValue = string.Join("\n", lines);
            return realValue;
        }
        private enum AbilityDescSelect
        {
            Stat,
            Throw,
            Dash,
            Teleport
        }
        private AbilityDescSelect SelectedDesc = AbilityDescSelect.Stat;
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
            int lv = GetLevel();
            if (NowLevel != lv || RecheckStats)
            {
                RecheckStats = false;
                NowLevel = lv;
                int dmg = 20;
                switch (lv)
                {
                    case 0: dmg = 20; break;
                    case 1: dmg = 30; break;
                    case 2: dmg = 35; break;
                    case 3: dmg = 50; break;
                    case 4: dmg = 72; break;
                    case 5: dmg = 130; break;
                    case 6: dmg = 180; break;
                    case 7: dmg = 250; break;
                    case 8: dmg = 280; break;
                    case 9: dmg = 330; break;
                    case 10: dmg = 480; break;
                    case 11: dmg = 580; break;
                    case 12: dmg = 750; break;
                    case 13: dmg = 1000; break;
                    case 14: dmg = 1300; break;
                    case 15: dmg = 1800; break;
                    case 16: dmg = 2700; break;
                }
                Item.damage = dmg;
                Item.crit = lv;
                Item.knockBack = lv / 2;
                Item.scale = 1;
                Item.Prefix(Item.prefix);
            }
            else
            {
                Item.ClearNameOverride();
                if (player.name.ToLower() is "tlipoca" or "Kino" || (player.TryGetModPlayer<VanityModPlayer>(out var mp) && mp.vanityEquippedLast == "BlackFlower"))
                {
                    Item.SetNameOverride(Mod.GetLocalization("TScytheSpecialName").Value);
                }
                else if (Main.zenithWorld)
                {
                    Item.SetNameOverride(Mod.GetLocalization("TScytheZenithName").Value);
                }
                if (throwType == -1)
                    throwType = ModContent.ProjectileType<TlipocasScytheThrow>();
                FuncKilledTownNPC(player);
            }
            Item.useTime = Item.useAnimation = (40 - GetLevel()) / (SpeedUpTime > 0 ? 6 : 1);
        }
        private void FuncKilledTownNPC(Player player)
        {
            if (Item.favorited && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                player.Entropy().CanSlainTownNPC = true;
        }
        public override void HoldItem(Player player)
        {
            UpdateInventory(player);
            if (SpeedUpTime > 0)
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
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.ArmorPenetration = 15;
            Item.shoot = ModContent.ProjectileType<TlipocasScytheHeld>();
            Item.Entropy().tooltipStyle = 4;
            Item.Entropy().NameColor = new Color(160, 0, 0);
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(90, 0, 0);
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
            Item.Entropy().Legend = true;
            RecheckStats = true;
            UpdateInventory(Main.LocalPlayer);
        }
        public int swing = 0;
        public static int throwType = -1;
        public override bool AllowPrefix(int pre) => true;

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
                damage = (int)(damage / 2.0f);
            }
            if (AllowDash() && player.controlUp && !player.HasCooldown(TlipocasScytheSlashCooldown.ID))
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TlipocasScytheHeld>(), DashUpgrade() ? damage : damage / 4, knockback, player.whoAmI, swing == 0 ? 1 : -1, -1);
                player.AddCooldown(TlipocasScytheSlashCooldown.ID, 15 * 60);
                CalamityEntropy.FlashEffectStrength = 0.2f;
                int p = Projectile.NewProjectile(source, position, velocity.normalize() * 1000 * (DashUpgrade() ? 1.33f : 1), ModContent.ProjectileType<TSSlash>(), damage * 2 , knockback, player.whoAmI);
                if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                if (DownedBossSystem.downedPolterghast)
                {
                    Projectile.NewProjectile(source, position + velocity.normalize() * 400 * (DashUpgrade() ? 1.33f : 1), velocity.normalize() * 1000 * (DashUpgrade() ? 1.33f : 1), ModContent.ProjectileType<TSSlash>() , damage * 2 , knockback, player.whoAmI, 0, 1);
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
                    int ut = 40 - GetLevel();
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
            if (!TlipocasScythe.DashImmune())
                return false;
            return null;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("slice", 1, target.Center);
            EParticle.NewParticle(new MultiSlash() { xadd = 1f, lx = 1f, endColor = Color.Red, spawnColor = Color.IndianRed }, target.Center, Vector2.Zero, Color.IndianRed, 1, 1, true, BlendState.Additive, 0);
            if (TlipocasScythe.DashUpgrade())
            {
                target.AddBuff<MarkedforDeath>(8 * 60);
                target.AddBuff<WhisperingDeath>(8 * 60);
            }
        }
        public bool MovePlayer = true;
        public override void AI()
        {
            if (TlipocasScythe.DashImmune())
            {
                Projectile.GetOwner().Entropy().immune = 4;
            }
            if (Projectile.localAI[1]++ == 0)
            {
                CEUtils.PlaySound("AbyssalBladeLaunch", 1, Projectile.Center);
            }
            Player player = Projectile.GetOwner();
            CalamityEntropy.FlashEffectStrength = 0.3f;
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
                    if (DownedBossSystem.downedDoG)
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
            modifiers.SourceDamage *= 1.5f;
            if (DownedBossSystem.downedCalamitas)
            {
                float dmgMult = Utils.Remap(CEUtils.getDistance(target.Center, Projectile.Center), 160, 300, 1.25f, 1);
                modifiers.FinalDamage *= dmgMult;
            }
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.light = 0.2f;
            Projectile.MaxUpdates = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public List<float> oldRots = new List<float>();
        public List<float> oldScale = new List<float>();
        public float counter = 0;
        public bool flagS = true;
        public bool flag = true;
        public bool Canhit = false;
        public bool shake = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (shake)
                CEUtils.SetShake(target.Center, Projectile.Calamity().stealthStrike ? 8 : 5);
            shake = false;
            if (Projectile.ai[1] == 1 && TlipocasScythe.AllowVoidEmpowerment())
            {
                int VETime = EDownedBosses.downedCruiser ? 15 : 8;
                VETime *= 60;
                Projectile.GetOwner().AddBuff(ModContent.BuffType<VoidEmpowerment>(), VETime);
            }
            if (target.townNPC && target.life <= 0)
            {
                SoundEngine.PlaySound(PerforatorHive.DeathSound, target.Center);
                var player = Projectile.GetOwner();
                player.QuickSpawnItem(target.GetSource_Death(), new Item(73, 20), 20);
                player.QuickSpawnItem(target.GetSource_Death(), new Item(ModContent.ItemType<BloodOrb>(), 30), 30);
                if (NPC.downedPlantBoss)
                {
                    player.QuickSpawnItem(target.GetSource_Death(), new Item(1508, 20), 20);
                    player.QuickSpawnItem(target.GetSource_Death(), new Item(74, 514), 20);
                }
                if (NPC.downedMoonlord)
                {
                    player.QuickSpawnItem(target.GetSource_Death(), new Item(ModContent.ItemType<Necroplasm>(), 20), 20);
                }
            }
            if (flagS)
            {
                if (TlipocasScythe.AllowSpin() && Projectile.Calamity().stealthStrike)
                {
                    Projectile.GetOwner().Heal(DownedBossSystem.downedPolterghast ? 10 : 7);
                }
                CEUtils.PlaySound("voidseekershort", 1, target.Center, 6, CEUtils.WeapSound);
                flagS = false;
                if (!target.Organic())
                {
                    CEUtils.PlaySound("metalhit", Main.rand.NextFloat(1.4f, 1.6f) / Projectile.ai[1], target.Center, 6);
                }
            }
            if (EDownedBosses.downedCruiser)
            {
                EGlobalNPC.AddVoidTouch(target, 60, 3, 1000, 12);
            }
            if (DownedBossSystem.downedCalamitas)
            {
                target.AddBuff<VulnerabilityHex>(60 * 3);
            }
            if (DownedBossSystem.downedRavager)
            {
                if (DownedBossSystem.downedProvidence)
                {
                    if (DownedBossSystem.downedPrimordialWyrm)
                    {
                        target.AddBuff<LifeOppress>(60 * 3);
                    }
                    else
                    {
                        target.AddBuff<ArmorCrunch>(60 * 3);
                    }
                }
                else
                {
                    target.AddBuff<Laceration>(60 * 3);
                }
            }
            else
            {
                target.AddBuff<HeavyBleeding>(60 * 3);
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
        public override bool? CanDamage() => Canhit;
        public override bool? CanHitNPC(NPC target)
        {
            var togglePlayer = Projectile.GetOwner().GetModPlayer<TlipocasSingleHit>();
            bool canDealDamageToAll = (target.type != NPCID.DD2EterniaCrystal && !togglePlayer.isPressed);
            bool canDealDamageToNotFridly = !target.friendly && togglePlayer.isPressed;
            return canDealDamageToAll || canDealDamageToNotFridly;
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
            ProjScale = 1.56f + TlipocasScythe.GetLevel() * 0.04f;
            ProjScale *= (1 + Projectile.ai[1] * 0.5f);
            if (Projectile.ai[1] == 1)
            {
                if (progress < 0.1f)
                {
                    counter = (int)(0.1f * player.itemAnimationMax * Projectile.MaxUpdates + 1);
                }
            }
            if (Projectile.ai[1] == -1)
            {
                if (progress < 0.3f)
                {
                    counter = (int)(0.3f * player.itemAnimationMax * Projectile.MaxUpdates + 1);
                }
            }
            if (Projectile.Calamity().stealthStrike)
            {
                ySc = 0.5f;
                ProjScale *= 2f;
            }
            if (Projectile.ai[1] == -1)
            {
                ySc = 0.12f;
                ProjScale = 4.4f;
            }
            float r = 3.6f;
            float r1 = 0.5f;
            float r2 = 0.8f;
            float pn = 0.36f;
            if (progress >= pn && flag)
            {
                Canhit = true;
                flag = false;
                CEUtils.PlaySound("scytheswing", Main.rand.NextFloat(1.6f, 1.8f), Projectile.Center, 4, CEUtils.WeapSound);
            }
            if (progress < pn)
            {
                Projectile.rotation = (-r / 2f - CEUtils.GetRepeatedCosFromZeroToOne(progress / pn, 2) * r1) * dir;
            }
            else
            {
                Projectile.rotation = (-r / 2f - r1 + (CEUtils.GetRepeatedParaFromZeroToOne((progress - pn) / (1 - pn), 3) * 0.6f + 0.4f * CEUtils.GetRepeatedParaFromZeroToOne((progress - pn) / (1 - pn), 2)) * (r + r2)) * dir;
            }
            scale = (Projectile.rotation.ToRotationVector2() * new Vector2(1, ySc)).Length();
            if (progress > 0.7f)
            {
                ProjScale *= (1 - (progress - 0.7f) / 0.3f) * 0.2f + 0.8f;
            }
            Projectile.rotation = (Projectile.rotation.ToRotationVector2() * new Vector2(1, ySc)).ToRotation() + Projectile.velocity.ToRotation();
            if (progress > 1)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[2] == 0)
                player.SetHandRotWithDir(Projectile.rotation, Math.Sign(Projectile.velocity.X));
            if (progress > pn)
            {
                oldScale.Add(scale);
                oldRots.Add(Projectile.rotation);
                if (oldRots.Count > 170)
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
            Effect _shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

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

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, _shader, Main.GameViewMatrix.TransformationMatrix);
                    _shader.Parameters["color1"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? Color.Purple : Color.Firebrick).ToVector4());
                    _shader.Parameters["color2"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? new Color(190, 190, 255) : new Color(255, 60, 60)).ToVector4());

                    _shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 2.4f);
                    _shader.Parameters["alpha"].SetValue(1);
                    _shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }
            ve.Clear();
            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(158 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(100 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, _shader, Main.GameViewMatrix.TransformationMatrix);
                    _shader.Parameters["color1"].SetValue(new Vector4(1, 1, 1, 0));
                    _shader.Parameters["color2"].SetValue((Color.White).ToVector4() * 0.82f);

                    _shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 3f);
                    _shader.Parameters["alpha"].SetValue(1);
                    _shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak1");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }
            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
            int dir = (int)(Projectile.ai[0]) * Math.Sign(Projectile.velocity.X);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            TlipocasScytheHeld.shader ??= ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/RedTrans", AssetRequestMode.ImmediateLoad).Value;
            if (Projectile.GetOwner().HasBuff<VoidEmpowerment>())
            {
                Main.spriteBatch.EnterShaderRegion(BlendState.AlphaBlend, TlipocasScytheHeld.shader);
                TlipocasScytheHeld.shader.CurrentTechnique.Passes[0].Apply();
            }

            Main.spriteBatch.Draw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * ProjScale * scale, effect, 0);


            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public static Effect shader = null;
        public float alpha = 1;
        public float ProjScale = 1;
        public float scale = 1;
    }
    internal class TlipocasSingleHit : ModPlayer
    {
        public bool isToggle = false;
        public bool isPressed = false;
        public override void PostUpdate()
        {
            if (Main.mouseMiddle && Main.HoverItem.type == ModContent.ItemType<TlipocasScythe>() && Main.playerInventory)
            {
                if (Main.mouseMiddleRelease)
                {
                    if (isPressed)
                    {
                        isPressed = false;
                        Player.Center.CirclrDust(36, 1.26f, DustID.GemRuby, 6, 3f);
                        SoundEngine.PlaySound(SoundID.Item103 with { MaxInstances = 4, Pitch = 0.4f });
                    }
                    else
                    {
                        isPressed = true;
                        Player.Center.CirclrDust(36, 1.26f, DustID.GemRuby, -6, 3f);
                        SoundEngine.PlaySound(SoundID.Item103 with { MaxInstances = 4, Pitch = 0.4f });
                    }

                }
            }
        }
    }

    public class TlipocasScytheThrow : ModProjectile
    {
        public float TeleportSlashDamageMult = 4f;
        public override string Texture => "CalamityEntropy/Content/Items/Donator/TlipocasScythe";
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (StickOnMouse)
                modifiers.SourceDamage /= 2.0f;

            if (DownedBossSystem.downedCalamitas)
            {
                float dmgMult = Utils.Remap(CEUtils.getDistance(target.Center, Projectile.Center), 160, 300, 1.25f, 1);
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
        public bool shake = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (shake || StickOnMouse)
                CEUtils.SetShake(target.Center, (Projectile.Calamity().stealthStrike ? 8 : 4) * (StickOnMouse ? 0.5f : 1));
            shake = false;
            if (Projectile.numHits == 1)
            {
                if (TlipocasScythe.AllowSpin() && Projectile.Calamity().stealthStrike)
                {
                    Projectile.GetOwner().Heal(DownedBossSystem.downedPolterghast ? 10 : 7);
                }
            }
            if (counter < 16 * 10)
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
                EGlobalNPC.AddVoidTouch(target, 60, 3, 1000, 12);
            }
            if (DownedBossSystem.downedCalamitas)
            {
                target.AddBuff<VulnerabilityHex>(60 * 3);
            }
            if (DownedBossSystem.downedRavager)
            {
                if (DownedBossSystem.downedProvidence)
                {
                    if (DownedBossSystem.downedPrimordialWyrm)
                    {
                        target.AddBuff<LifeOppress>(60 * 3);
                    }
                    else
                    {
                        target.AddBuff<ArmorCrunch>(60 * 3);
                    }
                }
                else
                {
                    target.AddBuff<Laceration>(60 * 3);
                }
            }
            else
            {
                target.AddBuff<HeavyBleeding>(60 * 3);
            }
            Color impactColor = Color.Red;
            float impactParticleScale = Main.rand.NextFloat(1.4f, 1.6f);

            SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.LawnGreen, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);


            float sparkCount = 6 + TlipocasScythe.GetLevel();
            for (int i = 0; i < sparkCount; i++)
            {
                float p = Main.rand.NextFloat();
                Vector2 sparkVelocity2 = CEUtils.randomRot().ToRotationVector2().RotatedByRandom(p * 0.4f) * Main.rand.NextFloat(6, 20 * (2 - p)) * (1 + TlipocasScythe.GetLevel() * 0.1f);
                int sparkLifetime2 = (int)((2 - p) * 9);
                float sparkScale2 = 0.4f + (1 - p);
                sparkScale2 *= (1 + TlipocasScythe.GetLevel() * 0.03f);
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
                if (CEUtils.getDistance(Projectile.Center, player.Center) < Projectile.velocity.Length() + 128)
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
            if (oldRots.Count > 16 * 6)
            {
                oldRots.RemoveAt(0);
                oldScale.RemoveAt(0);
                oldPos.RemoveAt(0);
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (Main.mouseLeft && !player.HasCooldown(TeleportSlashCooldown.ID) && DownedBossSystem.downedBrimstoneElemental)
                {
                    player.AddCooldown(TeleportSlashCooldown.ID, (EDownedBosses.downedCruiser ? 20 : 30) * 60);
                    player.Entropy().screenShift = 1f;
                    player.Entropy().screenPos = player.Center;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, (Projectile.Center - player.Center).SafeNormalize((Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX)) * 16, ModContent.ProjectileType<TlipocasScytheHeld>(), (int)(Projectile.damage * TeleportSlashDamageMult * 1.25f), Projectile.knockBack, player.whoAmI, 1, 1);
                    if (DownedBossSystem.downedPolterghast)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, (Projectile.Center - player.Center).SafeNormalize((Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX)) * 16, ModContent.ProjectileType<TlipocasScytheHeld>(), (int)(Projectile.damage * TeleportSlashDamageMult * 0.5f), Projectile.knockBack, player.whoAmI, 1, 1, 1);
                        EParticle.spawnNew(new PlayerShadowBlack() { plr = player }, player.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.AlphaBlend, 0, 60);
                    }
                    Projectile.Kill();
                    Vector2 v1 = player.position;
                    Vector2 v2 = Projectile.Center - new Vector2(player.width, player.height) / 2;
                    for (float i = 0; i <= 1; i += 0.05f)
                    {
                        EParticle.NewParticle(new PlayerShadowBlack() { plr = player }, Vector2.Lerp(v1, v2, i), Vector2.Zero, Color.White, 1, 1, true, BlendState.AlphaBlend, 0, 12);
                    }
                    player.Center = Projectile.Center;
                    player.Entropy().immune = 32;
                }
                if (!StickOnMouse && Main.mouseRight && TlipocasScythe.AllowSpin() && !RightLast)
                {
                    counter = -80;
                    StickOnMouse = true;
                    CEUtils.SyncProj(Projectile.whoAmI);
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

            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(74 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(30 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? Color.Purple : Color.Firebrick).ToVector4());
                    shader.Parameters["color2"].SetValue((Projectile.GetOwner().HasBuff<VoidEmpowerment>() ? new Color(190, 190, 255) : new Color(255, 60, 60)).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 2.4f);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            ve.Clear();
            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(69 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(35 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue(new Vector4(1, 1, 1, 0));
                    shader.Parameters["color2"].SetValue(Color.White.ToVector4() * 0.82f);
                    shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 3f);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak1");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);

            Vector2 origin = tex.Size() / 2f;
            float rot = Projectile.rotation;

            TlipocasScytheHeld.shader ??= ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/RedTrans", AssetRequestMode.ImmediateLoad).Value;
            if (Projectile.GetOwner().HasBuff<VoidEmpowerment>())
            {
                Main.spriteBatch.EnterShaderRegion(BlendState.AlphaBlend, TlipocasScytheHeld.shader);
                TlipocasScytheHeld.shader.CurrentTechnique.Passes[0].Apply();
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * ProjScale * scale, SpriteEffects.None, 0);

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
