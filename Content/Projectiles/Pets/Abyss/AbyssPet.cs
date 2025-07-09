using CalamityEntropy.Content.Buffs.Pets;
using CalamityEntropy.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Pets.Abyss
{
    public class AbyssPet : ModProjectile
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

            if (Main.gameMenu)
            {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/AbyssPet").Value;
                Main.EntitySpriteDraw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            Player player = Main.player[Projectile.owner];
            List<Texture2D> list = new List<Texture2D>();
            if (counter > 36)
            {
                counter -= 36;
            }
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.velocity.Length() < 1)
                {
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Afk1").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Afk2").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Afk3").Value);
                }
                else
                {
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Walk1").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Walk2").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Walk3").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Abyss/Walk4").Value);
                }
                Texture2D tx = list[(((int)counter / 6) % list.Count)];
                if (Projectile.velocity.X > -2 && Projectile.velocity.X < 2f)
                {
                    if (player.Center.X > Projectile.Center.X)
                    {
                        Projectile.direction = 1;
                    }
                    else
                    {
                        Projectile.direction = -1;
                    }
                }
                if (Projectile.direction == -1)
                {
                    Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                }
                else
                {
                    Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            else
            {
                Main.EntitySpriteDraw(head, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(head.Width, head.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(body, bodyP - Main.screenPosition, null, lightColor, (Projectile.Center - bodyP).ToRotation(), new Vector2(body.Width, body.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tail, tailP - Main.screenPosition, null, lightColor, (bodyP - tailP).ToRotation(), new Vector2(body.Width, body.Height) / 2, Projectile.scale, SpriteEffects.None, 0);


            }

            return false;

        }
        void MoveToTarget(Vector2 targetPos)
        {
            Lighting.AddLight(Projectile.Center, 1.2f, 1.2f, 1.2f);
            bodyP = Projectile.Center + (bodyP - Projectile.Center).SafeNormalize(Vector2.Zero) * 32;
            tailP = bodyP + (tailP - bodyP).SafeNormalize(Vector2.Zero) * 32;
            float br = (Projectile.Center - bodyP).ToRotation();
            float tr = (bodyP - tailP).ToRotation();
            br = CEUtils.RotateTowardsAngle(br, Projectile.rotation, 0.1f, false);
            tr = CEUtils.RotateTowardsAngle(tr, br, 0.1f, false);
            bodyP = Projectile.Center - br.ToRotationVector2() * 32;
            tailP = bodyP - tr.ToRotationVector2() * 32;
            if (CEUtils.getDistance(Projectile.Center, targetPos) > 1400)
            {
                Projectile.Center = Main.player[Projectile.owner].Center - new Vector2(0, 50);
            }
            if (Projectile.ai[1] == 1)
            {
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

                if (CEUtils.getDistance(Projectile.Center, targetPos) > 140)
                {
                    Vector2 px = targetPos - Projectile.Center;
                    px.Normalize();
                    Projectile.velocity += px * 1f;

                    Projectile.velocity *= 0.98f;

                }
                if (Projectile.Center.Y < targetPos.Y - 16 && CEUtils.getDistance(Projectile.Center, targetPos) < 100 && !(CEUtils.isAir(Projectile.owner.ToPlayer().Center + new Vector2(0, Projectile.owner.ToPlayer().height / 2 + 2), true)) && CEUtils.isAir(Projectile.Center))
                {
                    Projectile.ai[1] = 0;
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
            else
            {
                if (Projectile.velocity.Length() < 1)
                {
                    counter += 1;
                }
                else
                {
                    counter += Math.Abs(Projectile.velocity.X / 1.5f);
                }
                Projectile.tileCollide = true;
                Projectile.rotation = 0;
                Projectile.velocity.Y += 0.5f;
                if (CEUtils.getDistance(targetPos, Projectile.Center) > 340 || (Math.Abs(targetPos.Y - Projectile.Center.Y) > 60 && Projectile.owner.ToPlayer().velocity.Y == 0))
                {
                    Projectile.ai[1] = 1;
                }
                else if (CEUtils.getDistance(targetPos * new Vector2(1, 0), Projectile.Center * new Vector2(1, 0)) > 120)
                {
                    if (targetPos.X > Projectile.Center.X)
                    {
                        Projectile.velocity.X += 0.6f;
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.6f;
                    }
                    Projectile.velocity.X *= 0.98f;
                }
                else
                {
                    Projectile.velocity.X *= 0.93f;
                }
                if (targetPos.X > Projectile.Center.X)
                {
                    Projectile.direction = 1;
                }
                else
                {
                    Projectile.direction = -1;
                }

                if (Math.Abs(Projectile.velocity.X) > 0.5f && !CEUtils.isAir(Projectile.Center + (Projectile.velocity * new Vector2(1, 0)).SafeNormalize(Vector2.Zero) * 13 + new Vector2(0, 18)))
                {
                    Projectile.velocity.Y -= 1.5f;
                }
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
            if (!player.dead && player.HasBuff(ModContent.BuffType<AbyssBuff>()))
            {
                Projectile.timeLeft = 2;
            }

        }


    }
}
