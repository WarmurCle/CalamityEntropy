using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class MalignLightning : ModProjectile
    {
        List<Vector2> points = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 128;

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Vector2 vc;
                vc = Projectile.Center;

                Vector2 avc = Projectile.velocity;
                avc.Normalize();
                for (int i = 0; i < 64; i++)
                {
                    points.Add(vc);
                    vc += avc * 14;
                    avc = avc.RotatedByRandom(0.2f);
                }
            }
            Projectile.ai[0] += 1;
            if (Projectile.ai[0] > 10)
            {
                Projectile.Kill();
            }
        }
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (points.Count < 1)
            {
                return false;
            }
            for (int i = 1; i < points.Count; i++)
            {
                if (CEUtils.LineThroughRect(points[i - 1], points[i], targetHitbox, 4))
                {
                    return true;
                }
            }
            return false;
        }
        public float PrimitiveWidthFunction(float completionRatio) => (1 - completionRatio) * Projectile.scale * 6 * ((10f - Projectile.ai[0]) / 10f);

        public Color PrimitiveColorFunction(float completionRatio)
        {
            float colorInterpolant = (float)Math.Sin(Projectile.identity / 3f + completionRatio * 20f + Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f + 0.5f;
            Color color = new Color(255, 190, 255);
            return color;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (points.Count < 1)
            {
                return false;
            }
            Color color = new Color(255, 190, 255);
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].UseImage1("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].Apply();

            PrimitiveRenderer.RenderTrail(points, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => Projectile.Size * 0.2f, false,
                shader: GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"]), 10);

            return false;
        }
    }

}