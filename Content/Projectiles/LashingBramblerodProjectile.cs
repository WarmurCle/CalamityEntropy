using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace CalamityEntropy.Content.Projectiles
{
    public class LashingBramblerodProjectile : BaseWhip
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.MaxUpdates = 6;
            this.segments = 30;
            this.rangeMult = 3.2f;
        }
        public override string getTagEffectName => "LashingBramblerod";
        public override SoundStyle? WhipSound => SoundID.Grass;
        public override Color StringColor => Color.DarkGreen;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Utilities.Util.PlaySound("beeSting", 1, target.Center);
        }
        public override bool PreAI()
        {
            var points = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);
            EParticle.NewParticle(new LifeLeaf(), points[points.Count - 1], Util.randomPointInCircle(4), Color.White, Main.rand.NextFloat(0.8f, 1.2f), 1, false, BlendState.AlphaBlend, Util.randomRot());

            return base.PreAI();
        }
        public override int handleHeight => 30;
        public override int segHeight => 20;
        public override int endHeight => 30;
        public override int segTypes => 2;
    }
}