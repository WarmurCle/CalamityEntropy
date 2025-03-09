using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkGoozma : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Master;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Goozma");
        public override void modifyShootCooldown(ref int shootCooldown)
        {
            shootCooldown = (int)(shootCooldown * 0.64f);
        }
        public override int modifyProjectile(int pNow)
        {
            if(pNow == ModContent.ProjectileType<AstralBullet>())
            {
                return -1;
            }
            return ModContent.ProjectileType<GoozmaStarShot>();
        }
        public override Color tooltipColor => Main.DiscoColor;
        public override EBookProjectileEffect getEffect()
        {
            return new GZMBMEffect();
        }
    }
    public class GZMBMEffect : EBookProjectileEffect
    {

    }

}