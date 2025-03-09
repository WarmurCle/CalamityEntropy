
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
    public class BookMarkCorrupt : BookMark
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BookMarkCrimson>();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        
        public override Texture2D UITexture => BookMark.GetUITexture("Corrupt");
        public override EBookProjectileEffect getEffect()
        {
            return new CorruptBMEffect();
        }
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.attackSpeed += 0.12f;
            modifer.armorPenetration += 6;
        }
        public override Color tooltipColor => Color.Purple;
    }

    public class CorruptBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            (projectile.ModProjectile as EBookBaseProjectile).color = Color.MediumPurple;
        }
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 shotDir = projectile.velocity.RotateRandom(1).normalize() * 14;
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, shotDir, 496, damageDone, projectile.knockBack / 3, projectile.owner, 0, Main.rand.NextFloat(-0.1f, 0.1f)).ToProj().DamageType = projectile.DamageType;
            }

        }
    }
}