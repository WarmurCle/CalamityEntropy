using CalamityEntropy.Core;
using InnoVault;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Content.Items.Weapons.Nemesis
{
    internal class EndSkillOrbOnSpan : ModProjectile
    {
        public override string Texture => EffectLoaderAlt.AssetPath + "placeholder";
        private List<Vector2> PosLists;
        private float orbNinmsWeith;
        private float maxOrbNinmsWeith;
        private bool onSpan = true;
        private bool onOrb = true;

        public override void SetDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 7000;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 5;
            Projectile.timeLeft = 100 * Projectile.MaxUpdates;
            maxOrbNinmsWeith = Main.rand.NextFloat(3, 53.3f);
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (onSpan)
            {
                Projectile.ai[0] = Projectile.Center.X;
                Projectile.ai[1] = Projectile.Center.Y;
                Projectile.rotation = Projectile.velocity.ToRotation();
                PosLists = [];
                Vector2 rot = Projectile.velocity.UnitVector();
                for (int i = 0; i < 100; i++)
                {
                    PosLists.Add(Projectile.Center + rot * 50 * i);
                }
                Vector2 toOwner = Projectile.Center.To(Main.player[Projectile.owner].Center).UnitVector();
                Projectile.position += toOwner * 1000;
                onSpan = false;
            }

            if (Projectile.timeLeft > 25)
            {
                //Projectile.ai[0] += 0.001f;
                Projectile.localAI[0] += 0.001f;
                orbNinmsWeith += 0.05f;
                if (orbNinmsWeith > maxOrbNinmsWeith)
                {
                    orbNinmsWeith = maxOrbNinmsWeith;
                }
            }
            else
            {
                //Projectile.ai[0] -= 0.001f;
                Projectile.localAI[0] -= 0.001f;
                orbNinmsWeith -= 0.05f;
                if (orbNinmsWeith < 0)
                {
                    orbNinmsWeith = 0;
                }
            }

            if (Projectile.timeLeft == 50 && onOrb && Projectile.IsOwnedByLocalPlayer())
            {
                SoundEngine.PlaySound(SoundID.Item69 with { Pitch = 1.24f }, Projectile.position);

                onOrb = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
