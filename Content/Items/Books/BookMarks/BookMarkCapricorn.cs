
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Turret;
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
    public class BookMarkCapricorn : BookMark
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
        public override Texture2D UITexture => BookMark.GetUITexture("Capricorn");
        public override Color tooltipColor => Color.LightBlue;
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.attackSpeed += (float)Math.Min(1, (float)Main.LocalPlayer.GetModPlayer<CapricornBookmarkRecordPlayer>().EBookUsingTime / 800f) * 0.4f;
        }
    }

    public class CapricornBookmarkRecordPlayer : ModPlayer
    {
        public int EBookUsingTime = 0;
        public override void PostUpdate()
        {
            bool isUsing = false;
            Projectile book = null;
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if(p.ModProjectile is EntropyBookHeldProjectile && p.owner == Player.whoAmI)
                {
                    book = p;
                    break;
                }
            }
            if(book != null && book.ModProjectile is EntropyBookHeldProjectile eb)
            {
                if (eb.active)
                {
                    isUsing = true;
                    EBookUsingTime++;
                }
            }
            if (!isUsing)
            {
                EBookUsingTime = 0;
            }
        }
    }
}