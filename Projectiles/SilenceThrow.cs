using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using CalamityEntropy.Projectiles;
using CalamityMod.Projectiles.Rogue;
using CalamityEntropy.Util;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;
namespace CalamityEntropy.Projectiles
{
    public class SilenceThrow : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        float angle;
        float speed = 30;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rougeDC;
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 260;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.ArmorPenetration = 36;
        }
        NPC target = null;
        int hook = -1;
        bool thrownHook = false;
        public override void AI(){
            if (Projectile.ai[0] == 0)
            {
                angle = Projectile.velocity.ToRotation();
            }
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 8)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
            Projectile.ai[0]++;
            Projectile.rotation += 0.3f;
            if (target == null || !thrownHook)
            {
                target = Projectile.FindTargetWithinRange(800, false);
            }
            if (target != null && !thrownHook && Projectile.ai[0] > 10)
            {
                thrownHook = true;
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Sounds/chain1"), Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                {
                    int speed = 14;
                    if (Projectile.Calamity().stealthStrike)
                    {
                        speed = 22;
                    }
                    hook = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.One) * speed, ModContent.ProjectileType<SilenceHook>(), speed / 4, 0, Projectile.owner, 0, Projectile.whoAmI);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 30, 3.6f, 180, 12);
            
            for (int i = 0; i < 16; i++)
            {
                Particle p = new Particle();
                p.position = target.Center;
                p.alpha = 0.6f;

                var rd = Main.rand;
                p.velocity = new Vector2((float)((rd.NextDouble() - 0.5) * 16), (float)((rd.NextDouble() - 0.5) * 16));
                VoidParticles.particles.Add(p);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/SilenceThrow").Value;
            float c = 0;
            for (int i = 0; i < odp.Count; i++) {
                Main.spriteBatch.Draw(tx, odp[i] - Main.screenPosition, null, Color.White * c* 0.6f, odr[i], new Vector2(tx.Width / 2, tx.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0);
                c += 1f / odp.Count;
            }
            return true;
        }

    }

}