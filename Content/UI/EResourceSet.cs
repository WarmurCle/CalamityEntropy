using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.UI
{
    public class EResourceSet : ModResourceDisplaySet
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
    }
}
