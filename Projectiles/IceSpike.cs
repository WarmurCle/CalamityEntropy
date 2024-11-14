using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using CalamityEntropy.Projectiles;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.StatBuffs;
using CalamityEntropy.Dusts;
namespace CalamityEntropy.Projectiles
{
    public class IceSpike: ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 0;
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.Center, 8, 8, ModContent.DustType<IcePiece1>());
            }
            SoundStyle sd = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
            sd.Volume = 0.5f;
            SoundEngine.PlaySound(sd, Projectile.Center);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 600);
            target.AddBuff(BuffID.Frostburn, 1080);
            Main.player[Projectile.owner].AddBuff(ModContent.BuffType<CosmicFreeze>(), 600);
        }
        public override void AI(){

            Projectile.velocity.Y += 0f;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }


        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/IceSpike").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(24, 24), 1, SpriteEffects.None, 0);
            return false;
        }
    }
    

}