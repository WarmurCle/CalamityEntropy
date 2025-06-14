using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkGemini : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().stroke = true;
            Item.Entropy().NameColor = Color.LightBlue;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().tooltipStyle = 4;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Gemini");
        public override Color tooltipColor => Color.LightBlue;
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Homing += 1.2f;
            modifer.HomingRange += 0.5f;
        }
    }
}