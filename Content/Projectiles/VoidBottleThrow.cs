using CalamityEntropy.Content.Dusts;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class VoidBottleThrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.light = 2f;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.checkProj.Add(Projectile);
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X * 0.5f);
            Projectile.velocity *= 0.99f;
            if (Projectile.ai[0] == 120)
            {
                Projectile.ai[1] = 1;
                SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDust(Projectile.Center, 8, 8, ModContent.DustType<GlassBreak>());
                }
            }
            if (Projectile.ai[0] == 200)
            {
                Projectile.ai[1] = 2;
                SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDust(Projectile.Center, 8, 8, ModContent.DustType<GlassBreak>());
                }
            }
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(Projectile.Center), 1800f, 1000f, 0f, 4.5f) * Projectile.ai[0] / 60f;
            if (Projectile.ai[0] > 200)
            {
                for (int i = 0; i < 4; i++)
                {
                    Particle p = new Particle();
                    p.position = Projectile.Center;
                    p.alpha = 1.5f;
                    var r = Main.rand;
                    p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * 8), (float)((r.NextDouble() - 0.5) * 8));
                    VoidParticles.particles.Add(p);
                }

            }
            else if (Projectile.ai[0] > 120)
            {
                Particle p = new Particle();
                p.position = Projectile.Center;
                p.alpha = 1.5f;
                var r = Main.rand;
                p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * 8), (float)((r.NextDouble() - 0.5) * 8));
                VoidParticles.particles.Add(p);
            }
            if (Projectile.ai[0] == 280)
            {
                Projectile.Kill();
                SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(Projectile.Center, 8, 8, ModContent.DustType<GlassBreak>());
                }
                /*if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(Projectile.owner, ModContent.NPCType<CruiserHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, Projectile.owner, ModContent.NPCType<CruiserHead>());
*/
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D tx1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidBottleThrow").Value;
            Texture2D tx2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidBottleThrow1").Value;
            Texture2D tx3 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidBottleThrow2").Value;
            Texture2D tx = tx1;
            if (Projectile.ai[1] == 1)
            {
                tx = tx2;
            }
            if (Projectile.ai[1] == 2)
            {
                tx = tx3;
            }

            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, 1, SpriteEffects.None, 0);

            return false;
        }


    }


}