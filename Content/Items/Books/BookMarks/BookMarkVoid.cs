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
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 80, 1.5f, 800, 18);
            if (Main.rand.NextBool(5))
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<VoidBurst>(), projectile.damage * 8, 1, projectile.owner);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), 0, 1, projectile.owner);
                
                for(int i = 0; i < 74; i++)
                {
                    EParticle.spawnNew(new Smoke() { timeLeft = 26, timeleftmax = 26 }, projectile.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 16), new Color(140, 140, 255), 0.3f, 1, true, BlendState.Additive);
                    EParticle.spawnNew(new Smoke() { timeLeft = 26, timeleftmax = 26 }, projectile.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 16), Color.LightGoldenrodYellow, 0.3f, 1, true, BlendState.Additive);
                }
            }
        }
    }
}