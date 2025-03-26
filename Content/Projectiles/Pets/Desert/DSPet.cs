using CalamityEntropy.Content.Buffs.Pets;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Pets.Desert
{
    public class DSPet : ModProjectile
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
            Projectile.height = 54;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.gameMenu)
            {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/fly1").Value;
                if (Projectile.owner.ToPlayer().Entropy().PetsHat)
                {
                    txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/fly1").Value;
                }
                Main.EntitySpriteDraw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

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
                if (Projectile.owner.ToPlayer().Entropy().PetsHat)
                {
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/fly1").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/fly2").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/fly3").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/fly4").Value);
                }
                else
                {
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/fly1").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/fly2").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/fly3").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/fly4").Value);
                }

            }
            else
            {
                if (Projectile.owner.ToPlayer().Entropy().PetsHat)
                {
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/walk1").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/walk2").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/walk3").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/s/walk4").Value);
                }
                else
                {
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/walk1").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/walk2").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/walk3").Value);
                    list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Desert/walk4").Value);
                }
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
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += px * 0.8f;
                }
                if (Projectile.Center.Y < targetPos.Y - 16 && Util.Util.getDistance(Projectile.Center, targetPos) < 100 && !(Util.Util.isAir(Projectile.owner.ToPlayer().Center + new Vector2(0, Projectile.owner.ToPlayer().height / 2 + 2), true)))
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
                if (Projectile.velocity.Y == 0)
                {
                    counter += Math.Abs(Projectile.velocity.X / 4);
                }
                Projectile.tileCollide = true;
                Projectile.rotation = 0;
                Projectile.velocity.Y += 0.5f;
                if (Util.Util.getDistance(targetPos, Projectile.Center) > 340 || (Math.Abs(targetPos.Y - Projectile.Center.Y) > 60 && Projectile.owner.ToPlayer().velocity.Y == 0))
                {
                    Projectile.ai[1] = 1;
                }
                else if (Util.Util.getDistance(targetPos * new Vector2(1, 0), Projectile.Center * new Vector2(1, 0)) > 140)
                {
                    if (targetPos.X > Projectile.Center.X)
                    {
                        Projectile.velocity.X += 1f;
                    }
                    else
                    {
                        Projectile.velocity.X -= 1f;
                    }
                    Projectile.velocity.X *= 0.95f;
                }
                else
                {
                    Projectile.velocity.X *= 0.9f;
                }
                if (targetPos.X > Projectile.Center.X)
                {
                    Projectile.direction = 1;
                }
                else
                {
                    Projectile.direction = -1;
                }
                if (Util.Util.getDistance(Projectile.Center, targetPos) < 150 && Math.Abs(Projectile.owner.ToPlayer().velocity.X) > 2f)
                {
                    if (Math.Abs(Projectile.velocity.X) > Math.Abs(Projectile.owner.ToPlayer().velocity.X))
                    {
                        Projectile.velocity.X = Math.Abs(Projectile.owner.ToPlayer().velocity.X) * (Projectile.velocity.X > 0 ? 1 : -1);
                    }
                }
                if (Math.Abs(Projectile.velocity.X) > 0.3f && !Util.Util.isAir(Projectile.Center + (Projectile.velocity * new Vector2(1, 0)).SafeNormalize(Vector2.Zero) * 14 + new Vector2(0, 23)))
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
            if (Projectile.velocity.Length() < 1.2f && Projectile.ai[1] == 0)
            {
                counter = 7;
            }
            if (!player.dead && (player.HasBuff(ModContent.BuffType<DustyWhistleBuff>()) || player.HasBuff(ModContent.BuffType<AquaticAmuletBuff>())))
            {
                Projectile.timeLeft = 2;
            }

        }


    }
}
