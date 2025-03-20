using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkAbyss : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Abyss");
        public override EBookProjectileEffect getEffect()
        {
            return new AbyssBMEffect();
        }
        public override Color tooltipColor => new Color(93, 134, 196);
    }

    public class AbyssBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(4))
            {
                int damage = projectile.damage / 8;
                Vector2 p = target.Center + Util.Util.randomRot().ToRotationVector2() * 300;
                Projectile.NewProjectile(projectile.GetSource_FromThis(), p, (target.Center - p).SafeNormalize(Vector2.One),  ModContent.ProjectileType<AbyssBookmarkCrack>(), damage, projectile.knockBack, projectile.owner);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;
                Util.Util.PlaySound("crack", 1, projectile.Center, 3);
            }
        }
    }
}