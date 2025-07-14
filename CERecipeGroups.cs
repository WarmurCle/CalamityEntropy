using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy
{
    public static class CERecipeGroups
    {
        public static
            RecipeGroup gems;
        public static void init()
        {
            gems = new RecipeGroup(() => "Gems", ItemID.Ruby, ItemID.Sapphire, ItemID.Diamond, ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst);
        }

        public static void unload()
        {
            gems = null;
        }
    }
}
