using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.LoreItems;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.UI;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

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

        public static Asset<Texture2D> edgeTex;

        public static MethodBase updateStealthGenMethod;
        private delegate float UpdateStealthGenDelegate(Func<CalamityPlayer, float> orig, CalamityPlayer self);
        private delegate float CALEOCAI_Delegate(Func<NPC, Mod, bool> orig, NPC npc, Mod mod);
        public static void load()
        {
            edgeTex = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/StealthMeter");
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

            if (ModLoader.TryGetMod("AlchemistNPCLite", out var anpc))
            {
                ANPCSupport.ANPCShopAdd.LoadHook();
            }

            CalamityEntropy.Instance.Logger.Info("CalamityEntropy's Hook Loaded");
        }
        public static void drawStealthBarHook(Action<SpriteBatch, CalamityPlayer, Vector2> orig, SpriteBatch spriteBatch, CalamityPlayer modPlayer, Vector2 screenPos)
        {
            var edgeTexField = typeof(StealthUI).GetField("edgeTexture", BindingFlags.Static | BindingFlags.NonPublic);
            var barTexField = typeof(StealthUI).GetField("barTexture", BindingFlags.Static | BindingFlags.NonPublic);
            var barFullTexField = typeof(StealthUI).GetField("fullBarTexture", BindingFlags.Static | BindingFlags.NonPublic);
            bool resetBarTex = false;
            Texture2D origTex = null;
            Texture2D origBar = null;
            Texture2D origBarFull = null;
            if (modPlayer.Player.Entropy().worshipRelic)
            {
                resetBarTex = true;
                origTex = (Texture2D)edgeTexField.GetValue(null);
                origBar = (Texture2D)barTexField.GetValue(null);
                origBarFull = (Texture2D)barFullTexField.GetValue(null);
                edgeTexField.SetValue(null, solarBarTex.Value);
            }
            float num = modPlayer.rogueStealth;
            if(modPlayer.Player.Entropy().shadowPact)
            {
                resetBarTex = true;
                origTex = (Texture2D)edgeTexField.GetValue(null);
                origBar = (Texture2D)barTexField.GetValue(null);
                origBarFull = (Texture2D)barFullTexField.GetValue(null);
                edgeTexField.SetValue(null, shadowBarTex.Value);
                barTexField.SetValue(null, shadowProg.Value);
                barFullTexField.SetValue(null, shadowProgFull.Value);
                modPlayer.rogueStealth = modPlayer.Player.Entropy().shadowStealth * modPlayer.rogueStealthMax;
            }
            orig(spriteBatch, modPlayer, screenPos);
            if(modPlayer.Player.Entropy().shadowPact)
            {
                modPlayer.rogueStealth = num;
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
                float uiScale = Main.UIScale;
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
            if(self.Player.TryGetModPlayer<EModPlayer>(out var emp) && emp.WeaponsNoCostRogueStealth)
            {
                return;
            }
            orig(self);
        }
        private static float UpdateStealthGenHook(Func<CalamityPlayer, float> orig, CalamityPlayer self)
        {
            if(self.Player.TryGetModPlayer<EModPlayer>(out var mp) && mp.NoNaturalStealthRegen)
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
    public static class EModHooks
    {
        private static ConcurrentDictionary<(MethodBase, Delegate), Hook> _hooks = new ConcurrentDictionary<(MethodBase, Delegate), Hook>();
        public static ConcurrentDictionary<(MethodBase, Delegate), Hook> Hooks => _hooks;
        public static Hook Add(MethodBase method, Delegate hookDelegate)
        {
            if (method == null)
            {
                throw new ArgumentException("The MethodBase passed in is Null");
            }
            if (hookDelegate == null)
            {
                throw new ArgumentException("The HookDelegate passed in is Null");
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