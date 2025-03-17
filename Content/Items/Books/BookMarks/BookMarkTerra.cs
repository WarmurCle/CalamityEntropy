
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Util;
using CalamityMod.Items;
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
    public class BookMarkTerra : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Terra");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Damage += 0.2f;
            modifer.PenetrateAddition += 3;
        }
        public override int modifyBaseProjectile()
        {
            return ModContent.ProjectileType<TerraBoulder>();
        }
        public override Color tooltipColor => Color.YellowGreen;
    }

    public class TerraBoulder : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = true;
            this.gravity = 0.6f;
            Projectile.extraUpdates = 1;
            Projectile.width = Projectile.height = 32;
        }
        
        public override void AI()
        {
            base.AI();
            if(Projectile.velocity.X > 0)
            {
                Projectile.rotation += 0.1f;
            }
            else
            {
                Projectile.rotation -= 0.1f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(oldVelocity.X != 0 && Projectile.velocity.X == 0)
            {
                Projectile.velocity.X = oldVelocity.X * -1;
            }
            if (oldVelocity.Y != 0 && Projectile.velocity.Y == 0)
            {
                Projectile.velocity.Y = oldVelocity.Y * -1f;
            }
            if (Main.rand.NextBool(5))
            {
                Projectile.penetrate -= 1;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = this.color;
            Util.Util.DrawAfterimage(Projectile.getTexture(), Projectile.Entropy().odp, Projectile.Entropy().odr, Projectile.scale);
            return base.PreDraw(ref lightColor);
        }
    }

}