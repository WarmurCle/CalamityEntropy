
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Util;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public abstract class BookMark : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.rare = ItemRarityID.Orange;
        }
        public virtual Texture2D UITexture => null;
        public virtual EBookProjectileEffect getEffect()
        {
            return null;
        }
        public static Texture2D GetUITexture(string name)
        {
            return ModContent.Request<Texture2D>("CalamityEntropy/Assets/UI/BookMarks/" + name).Value;
        }
        public virtual void ModiferStat(EBookStatModifer modifer)
        {

        }
        public virtual Color tooltipColor => Color.Green;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine tooltipLine in tooltips)
            {
                if(tooltipLine.Name == "ItemName")
                {
                    continue;
                }
                tooltipLine.OverrideColor = tooltipColor;
            }
            tooltips.Add(new TooltipLine(Mod, "BookMarkTooltip", Mod.GetLocalization("TooltipBookMark").Value) { OverrideColor = Color.Yellow});
        }
    }
}