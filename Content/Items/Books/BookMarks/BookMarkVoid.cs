using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkVoid : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Void");
        public override EBookProjectileEffect getEffect()
        {
            return new VoidBMEffect();
        }
        public override Color tooltipColor => Color.Lerp(Color.MediumPurple, Color.Gold, (0.5f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 6)));
    }

    public class VoidBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            (projectile.ModProjectile as EBookBaseProjectile).color = Color.MediumPurple;
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 80, 1.5f, 800, 18);
            if (Main.rand.NextBool(projectile.hasEffect<APlusBMEffect>() ? 4 : 5))
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<VoidBurst>(), projectile.damage * 8, 1, projectile.owner);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), 0, 1, projectile.owner);

                for (int i = 0; i < 74; i++)
                {
                    EParticle.NewParticle(new Smoke() { Lifetime = 26, timeleftmax = 26 }, target.Center, Utilities.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 16), new Color(140, 140, 255), 0.3f, 1, true, BlendState.Additive);
                    EParticle.NewParticle(new Smoke() { Lifetime = 26, timeleftmax = 26 }, target.Center, Utilities.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 16), Color.LightGoldenrodYellow, 0.3f, 1, true, BlendState.Additive);
                }
            }
        }
    }
}