using CalamityEntropy.Util;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.ILEditing
{
    public static class EModILEdit
    {

        public static void load()
        {
            //IL_Player.getDPS += HookGetDPS;
        }

        /*private static void HookGetDPS(ILContext il)
        {
            var c = new ILCursor(il);
            if(!c.TryGotoNext(i => i.MatchConvI4()))
            {
                return;
            }
            if (!c.TryGotoNext(i => i.MatchConvI4()))
            {
                return;
            }
            c.Index++;
            c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Player, int>>((returnV, player) =>
            {
                if (player.Entropy().Godhead)
                {
                    return player.Entropy().effectCount;
                }
                return returnV;
            });

        }*/
    }
}