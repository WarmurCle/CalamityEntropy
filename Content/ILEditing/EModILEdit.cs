namespace CalamityEntropy.Content.ILEditing
{
    public static class EModILEdit
    {

        public static void load()
        {
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