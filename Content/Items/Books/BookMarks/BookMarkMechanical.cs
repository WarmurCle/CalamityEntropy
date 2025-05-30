﻿using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkMechanical : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Mechanical");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.armorPenetration += 6;
            modifer.Homing += 0.4f;
        }
        public override Color tooltipColor => Color.LightGray;
        public override EBookProjectileEffect getEffect()
        {
            return new MechanicalBMEffect();
        }
    }
    public class MechanicalBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            if (ownerClient && Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 2 : 3))
            {
                Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.UnitY * -8, ModContent.ProjectileType<Detector>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
    }
}