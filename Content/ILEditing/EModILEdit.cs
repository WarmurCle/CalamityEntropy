using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.LoreItems;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.ILEditing
{
    public static class EModILEdit
    {
        private delegate float UpdateStealthGenDelegate(Func<CalamityPlayer, float> orig, CalamityPlayer self);
        private delegate float CALEOCAI_Delegate(Func<NPC, Mod, bool> orig, NPC npc, Mod mod);
        public static void load()
        {
            var originalMethod = typeof(CalamityPlayer)
            .GetMethod("UpdateStealthGenStats",
                      System.Reflection.BindingFlags.NonPublic |
                      System.Reflection.BindingFlags.Instance,
                      null,
            Type.EmptyTypes,
            null);

            var _hook = EModHooks.Add(originalMethod, UpdateStealthGenHook);


            originalMethod = typeof(CalamityPlayer)
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

            if(ModLoader.TryGetMod("AlchemistNPCLite", out var anpc))
            {
                ANPCSupport.ANPCShopAdd.LoadHook();
            }

            CalamityEntropy.Instance.Logger.Info("CalamityEntropy's Hook Loaded");
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
            throw new NotImplementedException();
        }
    }
}