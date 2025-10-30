﻿using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.LoreItems;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Schematics;
using CalamityMod.UI;
using CalamityMod.World;
using InnoVault;
using InnoVault.GameSystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

using ReLogic.Content;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using static InnoVault.GameSystem.ItemRebuildLoader;

namespace CalamityEntropy.Content.ILEditing
{
    public static class EModILEdit
    {
        [VaultLoaden("CalamityEntropy/Assets/UI/ExtraStealth")]
        public static Asset<Texture2D> extraStealthBar;
        [VaultLoaden("CalamityEntropy/Assets/UI/ExtraStealthFull")]
        public static Asset<Texture2D> extraStealthBarFull;
        [VaultLoaden("CalamityEntropy/Assets/UI/Shadowbar")]
        public static Asset<Texture2D> shadowBarTex;
        [VaultLoaden("CalamityEntropy/Assets/UI/ShadowBar1")]
        public static Asset<Texture2D> shadowProg;
        [VaultLoaden("CalamityEntropy/Assets/UI/ShadowBar2")]
        public static Asset<Texture2D> shadowProgFull;
        [VaultLoaden("CalamityEntropy/Assets/UI/SolarBar")]
        public static Asset<Texture2D> solarBarTex;
        [VaultLoaden("@CalamityMod/UI/MiscTextures/StealthMeter")]
        public static Asset<Texture2D> edgeTex;
        public static MethodBase updateStealthGenMethod;
        private delegate float UpdateStealthGenDelegate(Func<CalamityPlayer, float> orig, CalamityPlayer self);
        private delegate float CALEOCAI_Delegate(Func<NPC, Mod, bool> orig, NPC npc, Mod mod);
        public static void load()
        {
            updateStealthGenMethod = typeof(CalamityPlayer)
            .GetMethod("UpdateStealthGenStats",
                      System.Reflection.BindingFlags.NonPublic |
                      System.Reflection.BindingFlags.Instance,
                      null,
            Type.EmptyTypes,
            null);

            var _hook = EModHooks.Add(updateStealthGenMethod, UpdateStealthGenHook);


            var originalMethod = typeof(CalamityPlayer)
            .GetMethod("ConsumeStealthByAttacking",
                      System.Reflection.BindingFlags.Public |
                      System.Reflection.BindingFlags.Instance,
                      null,
            Type.EmptyTypes,
            null);
            _hook = EModHooks.Add(originalMethod, ConsumeStealthByAttackingHook);

            originalMethod = typeof(EyeOfCthulhuAI)
            .GetMethod("BuffedEyeofCthulhuAI",
                      System.Reflection.BindingFlags.Public |
                      System.Reflection.BindingFlags.Static,
                      null,
            new Type[] { typeof(NPC), typeof(Mod) },
            null);
            _hook = EModHooks.Add(originalMethod, EOCAIHook);

            originalMethod = typeof(LoreItem)
            .GetMethod("CanUseItem",
                      System.Reflection.BindingFlags.Public |
                      System.Reflection.BindingFlags.Instance,
                      null,
            new Type[] { typeof(Player) },
            null);
            _hook = EModHooks.Add(originalMethod, canuseitem_hook);

            //public static CooldownInstance AddCooldown(this Player p, string id, int duration, bool overwrite = true)
            originalMethod = typeof(CalamityUtils)
                .GetMethod("AddCooldown", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Player), typeof(string), typeof(int), typeof(bool) }, null);

            _hook = EModHooks.Add(originalMethod, addCdHook);

            originalMethod = typeof(StealthUI)
                .GetMethod("DrawStealthBar", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(SpriteBatch), typeof(CalamityPlayer), typeof(Vector2) }, null);

            _hook = EModHooks.Add(originalMethod, drawStealthBarHook);

            originalMethod = typeof(CalamityPlayer)
                .GetMethod("ProcessTriggers", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(TriggersSet) }, null);

            _hook = EModHooks.Add(originalMethod, processTriggersHook);

            if (ModLoader.TryGetMod("AlchemistNPCLite", out var anpc))
            {
                ANPCSupport.ANPCShopAdd.LoadHook();
            }
            var Item_Name_Get_Method = typeof(Item).GetProperty("Name", BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
            if(Item_Name_Get_Method != null) {
                EModHooks.Add(Item_Name_Get_Method, On_Name_Get_Hook);
            }

            StoreForbiddenArchivePositionHook.LoadHook();

            CalamityEntropy.Instance.Logger.Info("CalamityEntropy's Hook Loaded");
        }
        public static string On_Name_Get_Hook(On_GetItemName_get_Delegate orig, Item item)
        {
            if (Main.gameMenu)
                return orig(item);
            string orgName = orig.Invoke(item);
            if (item.Entropy().GetOverrideName(item, orgName, out string NameNew))
            {
                return NameNew;
            }
            return orgName;
        }
        public static FieldInfo mouseTextCacheField = null;

        public static void processTriggersHook(Action<CalamityPlayer, TriggersSet> orig, CalamityPlayer calPlayer, TriggersSet ts)
        {
            bool flag = false;
            if (CalamityKeybinds.AngelicAllianceHotKey.JustPressed && calPlayer.angelicAlliance && Main.myPlayer == calPlayer.Player.whoAmI && !calPlayer.divineBless && !calPlayer.Player.HasCooldown(CalamityMod.Cooldowns.DivineBless.ID))
            {
                flag = true;
                var Player = calPlayer.Player;

                int seconds = CalamityUtils.SecondsToFrames(15f);
                Player.AddBuff(ModContent.BuffType<CalamityMod.Buffs.StatBuffs.DivineBless>(), seconds, false);
                SoundEngine.PlaySound(AngelicAlliance.ActivationSound, Player.Center);

                // Spawn an archangel for every minion you have
                List<int> angelAmtList = new();
                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    if (proj.minionSlots <= 0f || !proj.CountsAsClass<SummonDamageClass>())
                        continue;

                    if (proj.active && proj.owner == Player.whoAmI)
                        angelAmtList.Add(projIndex);
                }

                var source = Player.GetSource_Accessory(calPlayer.FindAccessory(ModContent.ItemType<AngelicAlliance>()));
                for (int projIndex = 0; projIndex < angelAmtList.Count; projIndex++)
                {
                    Projectile proj = Main.projectile[angelAmtList[projIndex]];
                    float start = 360f / angelAmtList.Count;
                    int damage = Player.ApplyArmorAccDamageBonusesTo(proj.damage / 10);

                    Projectile.NewProjectile(source, new Vector2((int)(Player.Center.X + (Math.Sin(projIndex * start) * 300)), (int)(Player.Center.Y + (Math.Cos(projIndex * start) * 300))), Vector2.Zero, ModContent.ProjectileType<AngelicAllianceArchangel>(), damage, proj.knockBack / 10f, Player.whoAmI, Main.rand.Next(180), projIndex * start);
                    Player.statLife += 2;
                    Player.HealEffect(2);
                    if (Player.statLife > Player.statLifeMax2)
                        Player.statLife = Player.statLifeMax2;
                }
                calPlayer.angelicAlliance = false;
            }
            orig(calPlayer, ts);
            if (flag)
            {
                calPlayer.angelicAlliance = true;
            }
        }
        public static void drawStealthBarHook(Action<SpriteBatch, CalamityPlayer, Vector2> orig, SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            var edgeTexField = typeof(StealthUI).GetField("edgeTexture", BindingFlags.Static | BindingFlags.NonPublic);
            var barTexField = typeof(StealthUI).GetField("barTexture", BindingFlags.Static | BindingFlags.NonPublic);
            var barFullTexField = typeof(StealthUI).GetField("fullBarTexture", BindingFlags.Static | BindingFlags.NonPublic);
            bool resetBarTex = false;
            Texture2D origTex = CEUtils.RequestTex("CalamityMod/UI/MiscTextures/StealthMeter");
            Texture2D origBar = null;
            Texture2D origBarFull = null;
            if (modPlayer.Player.Entropy().worshipRelic)
            {
                resetBarTex = true;
                origBar = (Texture2D)barTexField.GetValue(null);
                origBarFull = (Texture2D)barFullTexField.GetValue(null);
                edgeTexField.SetValue(null, solarBarTex.Value);
            }
            float num = modPlayer.rogueStealth;
            if (modPlayer.Player.Entropy().shadowPact)
            {
                resetBarTex = true;
                origBar = (Texture2D)barTexField.GetValue(null);
                origBarFull = (Texture2D)barFullTexField.GetValue(null);
                edgeTexField.SetValue(null, shadowBarTex.Value);
                barTexField.SetValue(null, shadowProg.Value);
                barFullTexField.SetValue(null, shadowProgFull.Value);
                modPlayer.rogueStealth = modPlayer.Player.Entropy().shadowStealth * modPlayer.rogueStealthMax;
            }
            orig(spriteBatch, modPlayer, screenPos);
            float uiScale = Main.UIScale;
            Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle stealthBar = Utils.CenteredRectangle(screenPos, origTex.Size() * uiScale);

            if (modPlayer.Player.Entropy().ExtraStealth > 0)
            {
                if (stealthBar.Intersects(mouseHitbox))
                {
                    if (!CalamityConfig.Instance.MeterPosLock)
                        Main.LocalPlayer.mouseInterface = true;

                    if (modPlayer.rogueStealthMax > 0f && modPlayer.stealthUIAlpha >= 0.5f)
                    {
                        string stealthStr = (100f * modPlayer.rogueStealth).ToString("n2") + "+" + (100f * modPlayer.Player.Entropy().ExtraStealth).ToString("n2");
                        string maxStealthStr = (100f * modPlayer.rogueStealthMax).ToString("n2");
                        string textToDisplay = $"{CalamityUtils.GetTextValue("UI.Stealth")}: {stealthStr}/{maxStealthStr}\n";

                        if (!Main.keyState.IsKeyDown(Keys.LeftShift))
                        {
                            textToDisplay += CalamityUtils.GetTextValue("UI.StealthShiftText");
                        }
                        else
                        {
                            textToDisplay += CalamityUtils.GetTextValue("UI.StealthInfoText");
                        }

                        Main.instance.MouseText(textToDisplay, null, 0, 0, -1, -1, -1, -1, noOverride: true);
                        modPlayer.stealthUIAlpha = MathHelper.Lerp(modPlayer.stealthUIAlpha, 0.25f, 0.035f);
                    }
                }
            }
            if (modPlayer.Player.Entropy().shadowPact)
            {
                modPlayer.rogueStealth = num;
                if (stealthBar.Intersects(mouseHitbox))
                {
                    if (modPlayer.Player.Entropy().shadowStealth > 0)
                    {
                        string stealthStr = (100f * modPlayer.Player.Entropy().shadowStealth).ToString("n2");
                        string maxStealthStr = 100.ToString("n2");
                        string textToDisplay = $"{CalamityEntropy.Instance.GetLocalization("ShadowBar")}: {stealthStr}/{maxStealthStr}\n";

                        if (!Main.keyState.IsKeyDown(Keys.LeftShift))
                        {
                            textToDisplay += CalamityEntropy.Instance.GetLocalization("ShiftMoreInfo");
                        }
                        else
                        {
                            textToDisplay += CalamityEntropy.Instance.GetLocalization("ShadowBarInfo");
                        }

                        Main.instance.MouseText(textToDisplay, null, 0, 0, -1, -1, -1, -1, noOverride: true);
                    }
                }
            }
            if (modPlayer.Player.Entropy().worshipRelic)
            {
                if (stealthBar.Intersects(mouseHitbox))
                {
                    if (modPlayer.rogueStealthMax > 0f && modPlayer.stealthUIAlpha >= 0.5f)
                    {
                        string stealthStr = (100f * modPlayer.rogueStealth).ToString("n2");
                        string maxStealthStr = (100f * modPlayer.rogueStealthMax).ToString("n2");
                        string textToDisplay = $"{CalamityEntropy.Instance.GetLocalization("SolarBar")}: {stealthStr}/{maxStealthStr}\n";

                        if (!Main.keyState.IsKeyDown(Keys.LeftShift))
                        {
                            textToDisplay += CalamityEntropy.Instance.GetLocalization("ShiftMoreInfo");
                        }
                        else
                        {
                            textToDisplay += CalamityEntropy.Instance.GetLocalization("SolarBarInfo");
                        }

                        Main.instance.MouseText(textToDisplay, null, 0, 0, -1, -1, -1, -1, noOverride: true);
                    }
                }
            }
            if (resetBarTex)
            {
                edgeTexField.SetValue(null, origTex);
                barTexField.SetValue(null, origBar);
                barFullTexField.SetValue(null, origBarFull);
            }
            var emp = modPlayer.Player.Entropy();
            if (emp.ExtraStealth > 0)
            {
                float offset = (edgeTex.Value.Width - extraStealthBar.Value.Width) * 0.5f;
                float completionRatio = emp.ExtraStealth / modPlayer.rogueStealthMax;
                Rectangle barRectangle = new Rectangle(0, 0, (int)(extraStealthBar.Value.Width * completionRatio), extraStealthBar.Value.Width);
                bool full = emp.ExtraStealth > 0 && emp.ExtraStealth >= modPlayer.rogueStealthMax;
                spriteBatch.Draw(full ? extraStealthBarFull.Value : extraStealthBar.Value, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White * modPlayer.stealthUIAlpha, 0f, CEUtils.RequestTex("CalamityMod/UI/MiscTextures/StealthMeterStrikeIndicator").Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            }
        }


        public static CooldownInstance addCdHook(Func<Player, string, int, bool, CooldownInstance> orig, Player player, string id, int duration, bool overwrite)
        {
            return orig.Invoke(player, id, (int)(duration * player.Entropy().CooldownTimeMult), overwrite);
        }
        private static bool EOCAIHook(Func<NPC, Mod, bool> orig, NPC npc, Mod mod)
        {
            if (CalamityEntropy.EntropyMode)
            {
                return EOCEntropyModeAI.BuffedEyeofCthulhuAI(npc, mod);
            }
            else
            {
                return orig(npc, mod);
            }
        }
        public static void ConsumeStealthByAttackingHook(Action<CalamityPlayer> orig, CalamityPlayer self)
        {
            if (self.Player.TryGetModPlayer<EModPlayer>(out var emp) && emp.WeaponsNoCostRogueStealth)
            {
                return;
            }
            orig(self);
        }
        private static float UpdateStealthGenHook(Func<CalamityPlayer, float> orig, CalamityPlayer self)
        {
            if (self.Player.TryGetModPlayer<EModPlayer>(out var mp) && mp.NoNaturalStealthRegen)
            {
                return 0;
            }
            return (orig(self) + self.Player.Entropy().RogueStealthRegen) * self.Player.Entropy().RogueStealthRegenMult;
        }
        private static bool canuseitem_hook(Func<ModItem, Player, bool> orig, ModItem self, Player player)
        {
            if (ModContent.GetInstance<ServerConfig>().LoreSpecialEffect && LoreReworkSystem.loreEffects.ContainsKey(self.Type))
            {
                return true;
            }
            return orig(self, player);
        }
    }
    public static class StoreForbiddenArchivePositionHook
    {
        public static void LoadHook()
        {
            var method = typeof(DungeonArchive).GetMethod("PlaceArchive", BindingFlags.Static | BindingFlags.Public);
            MonoModHooks.Modify(method, sfahook);
        }

        private static void sfahook(ILContext il)
        {
            ILCursor cursor = new(il);

            int xLocalIndex = 0;
            int yLocalIndex = 0;
            ConstructorInfo pointConstructor = typeof(Point).GetConstructor([typeof(int), typeof(int)]);
            MethodInfo placementMethod = typeof(SchematicManager).GetMethods().First(m => m.Name == "PlaceSchematic");

            // Find the first instance of the schematic placement call. There are three, but they all take the same information so it doesn't matter which one is used as a reference.
            cursor.GotoNext(i => i.MatchLdstr(SchematicKeys.BlueArchiveKey));

            // Find the part of the method call where the placement Point type is made, and read off the IL indices for the X and Y coordinates with intent to store them elsewhere.
            cursor.GotoNext(i => i.MatchNewobj(pointConstructor));
            cursor.GotoPrev(i => i.MatchLdloc(out yLocalIndex));
            cursor.GotoPrev(i => i.MatchLdloc(out xLocalIndex));

            // Go back to the beginning of the method and store the placement position so that it isn't immediately discarded after world generation- Ceaseless Void's natural spawning needs it.
            // This needs to be done at each of hte three schematic placement variants since sometimes post-compilation optimizations can scatter about return instructions.
            cursor.Index = 0;
            for (int i = 0; i < 3; i++)
            {
                cursor.GotoNext(i => i.MatchLdftn(out _));
                cursor.GotoNext(MoveType.After, i => i.MatchCallOrCallvirt(out _));
                cursor.Emit(OpCodes.Ldloc, xLocalIndex);
                cursor.Emit(OpCodes.Ldloc, yLocalIndex);
                cursor.EmitDelegate((int x, int y) =>
                {
                    EDownedBosses.ForbiddenArchiveCenter = new(x, y);
                });
            }
        }
    }
    public static class EModHooks
    {
        private static ConcurrentDictionary<(MethodBase, Delegate), Hook> _hooks = new ConcurrentDictionary<(MethodBase, Delegate), Hook>();
        public static ConcurrentDictionary<(MethodBase, Delegate), Hook> Hooks => _hooks;
        public static Hook Add(MethodBase method, Delegate hookDelegate)
        {
            if (method == null)
            {
                CalamityEntropy.Instance.Logger.Warn($"CalamityEntropy: Error when add hook to {method.Name}: The MethodBase passed in is Null");
                return null;
                //throw new ArgumentException("The MethodBase passed in is Null");
            }
            if (hookDelegate == null)
            {
                CalamityEntropy.Instance.Logger.Warn($"CalamityEntropy: Error when add hook to {method.Name}: The HookDelegate passed in is Null");
                return null;
                //throw new ArgumentException("The HookDelegate passed in is Null");
            }

            Hook hook = new Hook(method, hookDelegate);

            if (!hook.IsApplied)
            {
                hook.Apply();
            }
            _hooks.TryAdd((method, hookDelegate), hook);
            return hook;
        }

        public static bool CheckHookStatus()
        {
            int hookDownNum = 0;
            foreach (var hook in _hooks.Values)
            {
                if (!hook.IsApplied)
                {
                    hookDownNum++;
                }
            }
            if (hookDownNum > 0)
            {
                return false;
            }
            return true;
        }

        public static void UnLoadData()
        {
            foreach (var hook in _hooks.Values)
            {
                if (hook == null)
                {
                    continue;
                }
                if (hook.IsApplied)
                {
                    hook.Undo();
                }
                hook.Dispose();
            }
            _hooks.Clear();
        }

        internal static void Add(Action<SpriteBatch> exitShaderRegion, object esrHook)
        {
        }
    }
}