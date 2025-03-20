
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
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkSulphurousSea : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("SulphurousSea");
        public override EBookProjectileEffect getEffect()
        {
            return new SulphurousSeaBMEffect();
        }

        public override Color tooltipColor => Color.SkyBlue;
    }

    public class SulphurousSeaBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            Vector2 shotDir = Util.Util.randomRot().ToRotationVector2();
            Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center + shotDir * 32, shotDir * 6, ModContent.ProjectileType<AquashardSplit>(), damageDone / 6, projectile.knockBack / 3, projectile.owner).ToProj().DamageType = projectile.DamageType;
            SoundEngine.PlaySound(in SoundID.Item27, projectile.Center);
        }
    }
}