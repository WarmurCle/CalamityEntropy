using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Buffs.Pets;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Pets.DarkFissure
{
    public class DarkFissure : ModProjectile
    {
        public float counter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 24;
            Projectile.height = 40;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            
            if (Main.gameMenu) {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/DarkFissure").Value;
                Main.spriteBatch.Draw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            Player player = Main.player[Projectile.owner];
            List<Texture2D> list = new List<Texture2D>();
            if (counter > 36)
            {
                counter -= 36;
            }
            if (Projectile.ai[1] == 1)
            {
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/DarkFissure").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/DarkFissure2").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/DarkFissure3").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/DarkFissure4").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/DarkFissure5").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/DarkFissure6").Value);
            }
            else
            {
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/walk1").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/walk2").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/walk3").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/walk4").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/walk5").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/DarkFissure/walk6").Value);
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
                Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            }
            else
            {
                Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            }


            return false;

        }
        void MoveToTarget(Vector2 targetPos)
        {
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 1400)
            {
                Projectile.Center = Main.player[Projectile.owner].Center - new Vector2(0, 50);
            }
            if (Projectile.ai[1] == 1)
            {
                counter++;
                Projectile.tileCollide = false;
                Projectile.rotation = MathHelper.ToRadians((Projectile.velocity.X * 1.4f));
                if (Util.Util.getDistance(Projectile.Center, targetPos) > 90)
                {
                    Vector2 px = targetPos - Projectile.Center;
                    px.Normalize();
                    Projectile.velocity += px * 0.66f;

                    Projectile.velocity *= 0.988f;

                }
                if (Util.Util.getDistance(Projectile.Center, targetPos) < 100 && !(Util.Util.isAir(Projectile.owner.ToPlayer().Center + new Vector2(0, Projectile.owner.ToPlayer().height / 2 + 2), true)) && Util.Util.isAir(Projectile.Center))
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
                counter += Math.Abs(Projectile.velocity.X / 1.2f);
                Projectile.tileCollide = true;
                Projectile.rotation = 0;
                Projectile.velocity.Y += 0.5f;
                if (Util.Util.getDistance(targetPos, Projectile.Center) > 500) {
                    Projectile.ai[1] = 1;
                }
                else if (Util.Util.getDistance(targetPos * new Vector2(1, 0), Projectile.Center * new Vector2(1, 0)) > 140)
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
                
                if (Math.Abs(Projectile.velocity.X) > 0.5f && !Util.Util.isAir(Projectile.Center + (Projectile.velocity * new Vector2(1, 0)).SafeNormalize(Vector2.Zero) * 13 + new Vector2(0, 18)))
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
            if (!player.dead && (player.HasBuff(ModContent.BuffType<DevourerAndTheApostles>()) || player.HasBuff(ModContent.BuffType<WeakGravity>())))
            {
                Projectile.timeLeft = 2;
            }

        }


    }
}
