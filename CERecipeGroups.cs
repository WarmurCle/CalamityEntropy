using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public class CERecipeGroups : ModSystem
    {
        public static string gems = "Gems";
        public static string AnyOrichalcumBar = "OrichalcumBars";
        public static string butterflies = "Butterflies";
        public static string evilBar = "EvilBars";
        public static RecipeGroup gems_;
        public static RecipeGroup AnyOrichalcumBar_;
        public static RecipeGroup butterflies_;
        public static RecipeGroup evilBar_;
        public override void AddRecipeGroups()
        {
            gems = $"{Mod.Name}:" + gems;
            AnyOrichalcumBar = $"{Mod.Name}:" + AnyOrichalcumBar;
            butterflies = $"{Mod.Name}:" + butterflies;
            evilBar = $"{Mod.Name}:" + evilBar;

            AnyOrichalcumBar_ = new RecipeGroup(() => CalamityEntropy.Instance.GetLocalization("AnyOrichalcumBar").Value, ItemID.OrichalcumBar, ItemID.MythrilBar);
            gems_ = new RecipeGroup(() => "Gems", ItemID.Ruby, ItemID.Sapphire, ItemID.Diamond, ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst);
            butterflies_ = new RecipeGroup(() => "Butterflies", ItemID.EmpressButterfly, ItemID.GoldButterfly, ItemID.HellButterfly, ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly, ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly);
            evilBar_ = new RecipeGroup(() => CalamityEntropy.Instance.GetLocalization("AnyEvilBar").Value, ItemID.CrimtaneBar, ItemID.DemoniteBar);
            
            RecipeGroup.RegisterGroup(AnyOrichalcumBar, AnyOrichalcumBar_);
            RecipeGroup.RegisterGroup(gems, gems_);
            RecipeGroup.RegisterGroup(butterflies, butterflies_);
            RecipeGroup.RegisterGroup(evilBar, evilBar_);
        }


        public static void unload()
        {
            gems = null;
            butterflies = null;
            AnyOrichalcumBar = null;
            evilBar = null;
        }
    }
}
