using Terraria;
using Terraria.ID;

namespace CalamityEntropy
{
    public static class CERecipeGroups
    {
        public static RecipeGroup gems;
        public static RecipeGroup butterflies;
        public static void init()
        {
            gems = new RecipeGroup(() => "Gems", ItemID.Ruby, ItemID.Sapphire, ItemID.Diamond, ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst);
            butterflies = new RecipeGroup(() => "Butterflies", ItemID.EmpressButterfly, ItemID.GoldButterfly, ItemID.HellButterfly, ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly, ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly);
        }

        public static void unload()
        {
            gems = null;
            butterflies = null;
        }
    }
}
