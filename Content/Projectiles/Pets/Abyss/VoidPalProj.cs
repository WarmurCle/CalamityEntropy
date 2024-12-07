using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Buffs.Pets;
using CalamityEntropy.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Pets.Abyss
{
    public class VoidPalProj : ModProjectile
    {
        public float counter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            base.SetStaticDefaults();

        }
        public Texture2D head = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Head").Value;
        public Texture2D body = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Body").Value;
        public Texture2D tail = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Tail").Value;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 24;
            Projectile.height = 44;
        }
        public Vector2 bodyP = Vector2.Zero;
        public Vector2 tailP = Vector2.Zero;
        public override bool PreDraw(ref Color lightColor)
        {
            
            if (Main.gameMenu) {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/AbyssPet").Value;
                Main.spriteBatch.Draw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            Player player = Main.player[Projectile.owner];
            List<Texture2D> list = new List<Texture2D>();
            if (counter > 36)
            {
                counter -= 36;
            }
            Main.spriteBatch.Draw(head, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(head.Width, head.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(body, bodyP - Main.screenPosition, null, lightColor, (Projectile.Center - bodyP).ToRotation(), new Vector2(body.Width, body.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tail, tailP - Main.screenPosition, null, lightColor, (bodyP - tailP).ToRotation(), new Vector2(body.Width, body.Height) / 2, Projectile.scale, SpriteEffects.None, 0);


            

            return false;

        }
        void MoveToTarget(Vector2 targetPos)
        {
            Lighting.AddLight(Projectile.Center, 1.2f, 1.2f, 1.2f);
            bodyP = Projectile.Center + (bodyP - Projectile.Center).SafeNormalize(Vector2.Zero) * 32;
            tailP = bodyP + (tailP - bodyP).SafeNormalize(Vector2.Zero) * 32;
            float br = (Projectile.Center - bodyP).ToRotation();
            float tr = (bodyP - tailP).ToRotation();
            br = Util.Util.rotatedToAngle(br, Projectile.rotation, 0.1f, false);
            tr = Util.Util.rotatedToAngle(tr, br, 0.1f, false);
            bodyP = Projectile.Center - br.ToRotationVector2() * 32;
            tailP = bodyP - tr.ToRotationVector2() * 32;
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 1800)
            {
                Projectile.Center = Main.player[Projectile.owner].Center - new Vector2(0, 50);
            }
            counter++;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 2; i++)
            {
                Particle p = new Particle();
                p.alpha = 0.4f;
                p.position = Projectile.Center;
                p.velocity = new Vector2(0.3f, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2);
                VoidParticles.particles.Add(p);
            }
            for (int i = 0; i < 2; i++)
            {
                Particle p = new Particle();
                p.alpha = 0.4f;
                p.position = Projectile.Center - Projectile.velocity / 2;
                p.velocity = new Vector2(0.3f, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2);
                VoidParticles.particles.Add(p);
            }

            if (Util.Util.getDistance(Projectile.Center, targetPos) > 140)
            {
                Vector2 px = targetPos - Projectile.Center;
                px.Normalize();
                Projectile.velocity += px * 0.36f;

                Projectile.velocity *= 0.996f;

            }
            if (Projectile.velocity.X > 0)
            {
                Projectile.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
            }
        }
            
            
        
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false;
            return true;
        }

        public override void AI()
        {
            
            Player player = Main.player[Projectile.owner];
            MoveToTarget(player.Center + new Vector2(0, 0));
            if (!player.dead && player.HasBuff(ModContent.BuffType<VoidPal>()))
            {
                Projectile.timeLeft = 2;
            }

        }


    }
}
