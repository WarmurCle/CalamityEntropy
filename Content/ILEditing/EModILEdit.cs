using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
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
        public static void load()
        {
            var originalMethod = typeof(CalamityPlayer)
            .GetMethod("UpdateStealthGenStats",
                      System.Reflection.BindingFlags.NonPublic |
                      System.Reflection.BindingFlags.Instance, 
                      null,
            Type.EmptyTypes ,
            null);

            UpdateStealthGenDelegate dlg = UpdateStealthGenHook;
            var _hook = EModHooks.Add(originalMethod, dlg);
            CalamityEntropy.Instance.Logger.Info("CalEntropy's Hook Loaded");
        }
        private static float UpdateStealthGenHook(Func<CalamityPlayer, float> orig, CalamityPlayer self)
        {
            return orig(self) * self.Player.Entropy().RogueStealthRegenMult;
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