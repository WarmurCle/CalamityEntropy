using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BMDirtProj : EBookBaseProjectile
    {
        public int itemType => (int)Projectile.ai[0];
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 5;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 26;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] > 0)
            {
                NPC targeth = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 600);
                if (targeth != null)
                {
                    Projectile.localAI[2] = 3;
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy((target.Center - Projectile.Center).ToRotation());
                }
                else
                {
                    float cw = Math.Abs(Projectile.Center.X - target.Center.X);
                    float ch = Math.Abs(Projectile.Center.Y - target.Center.Y);
                    if (cw < ch)
                    {
                        Projectile.velocity.Y *= -1;
                    }
                    else
                    {
                        Projectile.velocity.X *= -1;
                    }
                }
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            }
            if (Projectile.ai[1] == -1)
            {
                target.velocity *= 0.4f;
                Projectile.Kill();
            }
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.localAI[2]++ > 6)
            {
                Projectile.velocity.Y += 0.4f;
                Projectile.velocity *= 0.998f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.01f;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Projectile.ai[0] != ItemID.StoneBlock ? DustID.Dirt : DustID.Stone);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int type = itemType;
            if (type > 0)
            {
                Main.instance.LoadItem(type);
                Texture2D tex = TextureAssets.Item[type].Value;
                Main.EntitySpriteDraw(Projectile.getDrawData(lightColor, tex));
            }
            {

            }
            return false;
        }
    }
}