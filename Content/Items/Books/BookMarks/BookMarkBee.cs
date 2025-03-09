
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
    public class BookMarkBee : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Bee");
        public override EBookProjectileEffect getEffect()
        {
            return new BeeBMEffect();
        }

        public override Color tooltipColor => Color.Orange;
    }

    public class BeeBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 shotDir = Util.Util.randomRot().ToRotationVector2();
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center + shotDir * 32, shotDir * 6, ProjectileID.Bee, damageDone / 4, projectile.knockBack / 3, projectile.owner).ToProj().DamageType = projectile.DamageType;
            }
        }
    }
}