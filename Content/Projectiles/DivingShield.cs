using System.Collections.Generic;
using CalamityEntropy.Util;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class DivingShield : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public float addLs = 0;
        public float lsj = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.scale = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
        }
        public int frame = 0;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public bool pld = false;
        public override void AI(){
            Projectile.velocity *= 0.99f;
            if (!pld)
            {
                SoundStyle sd = new SoundStyle("CalamityEntropy/Assets/Sounds/divine_intervention");
                sd.Volume = 0.36f;
                SoundEngine.PlaySound(sd, Projectile.Center);
                pld = true;
            }           
            if (Projectile.timeLeft > 14)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.hostile && p.Colliding(p.Hitbox, Projectile.Hitbox) && p.active)
                    {
                        p.Entropy().DI = true;
                        p.hostile = false;
                        p.friendly = true;
                        p.velocity *= -1.4f;
                        p.rotation += 3.1415f;
                        p.owner = Projectile.owner;
                        p.damage *= 12;
                        if (p.type == ModContent.ProjectileType<AresGaussNukeProjectile>())
                        {
                            p.damage /= 4;
                        }
                        lsj = 0.36f;
                        SoundStyle sd = new SoundStyle("CalamityEntropy/Assets/Sounds/shield");
                        sd.Volume = 0.4f;
                        SoundEngine.PlaySound(sd, Projectile.Center);
                        if (Projectile.timeLeft < 150)
                        {
                            Projectile.timeLeft += 40;
                        }
                    }
                }
                foreach (NPC n in Main.npc)
                {
                    if (n.friendly || !n.active)
                    {
                        continue;
                    }
                    if (Projectile.getRect().Intersects(n.getRect()))
                    {
                        if (n.Entropy().dscd <= 0)
                        {
                            n.Entropy().dscd = 26;
                            n.velocity *= -1.6f;
                            lsj = 0.36f;
                            SoundStyle sd = new SoundStyle("CalamityEntropy/Assets/Sounds/shield");
                            sd.Volume = 0.4f;
                            if (n.rotation != 0)
                            {
                                n.rotation += 3.1415f;
                            }
                            SoundEngine.PlaySound(sd, Projectile.Center);
                            if (Projectile.timeLeft < 150)
                            {
                                Projectile.timeLeft += 40;
                            }
                        }
                    }
                    
                }
            }
            Projectile.ai[0]++;
            if(frame < 6)
            {
                if (Projectile.ai[0] % 2 == 0)
                {
                    frame++;
                }
            }

            if (Projectile.timeLeft < 15)
            {
                alpha -= 0.08f;
                if (Projectile.ai[0] % 2 == 0)
                {
                    frame++;
                }
            }
            else
            {
                if (alpha < 1)
                {
                    alpha += 0.05f;
                }
            }
            lsj -= 0.09f;
            addLs += lsj;
            if (addLs < 0)
            {
                addLs = 0;
                lsj = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public float alpha = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/lightball").Value;
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, new Color(229, 299, 147) * alpha * 0.5f, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, 1.4f + addLs, SpriteEffects.None, 0);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/DivingShield").Value;
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, new Rectangle(48 * frame, 0, 48, 48), Color.White, Projectile.rotation, new Vector2(tx.Height, tx.Height) / 2, 2, SpriteEffects.None, 0);
            
            return false;
        }
    }
    

}