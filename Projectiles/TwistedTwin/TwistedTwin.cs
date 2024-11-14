using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using ReLogic.Content;
using CalamityEntropy.Buffs;
using CalamityEntropy.Util;
namespace CalamityEntropy.Projectiles.TwistedTwin
{
    public class TwistedTwin : ModProjectile
    {
        public static float damageMul = 0.08f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {   
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.light = 0.5f;
            Projectile.minion = true;
            Projectile.minionSlots = 2;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI(){
            Player player = Main.player[Projectile.owner];
            if (player.HasBuff(ModContent.BuffType<TwistedTwinBuff>())){
                Projectile.timeLeft = 3;
            }
            int index = -1;
            int pos = 1;
            foreach (Projectile p in Main.projectile)
            {
                if (p.whoAmI == Projectile.whoAmI)
                {
                    break;
                }
                if (p.type == Projectile.type && p.ai[1] == Projectile.ai[1] && p.whoAmI != Projectile.whoAmI && p.active && p.owner == Projectile.owner)
                {
                    index = p.whoAmI;
                    pos++;
                }
            }
            Vector2 targetPos;
            float rot;
            if (Projectile.owner == Main.myPlayer)
            {
                rot = (Main.MouseWorld - Main.myPlayer.ToPlayer().Center).ToRotation();
                Projectile.ai[2] = rot;
                
            }
            else
            {
                rot = Projectile.ai[2];
            }
            rot += (float)(Math.PI * 0.5);
            float spacing = 56;
            if (index == -1)
            {
                targetPos = player.Center + (rot.ToRotationVector2() * Projectile.ai[1]) * spacing;
            }
            else
            {
                targetPos = index.ToProj().Center + (rot.ToRotationVector2() * Projectile.ai[1]) * spacing;
            }
            Projectile.netUpdate = true;
            Projectile.Center = Projectile.Center + (targetPos - Projectile.Center) * 0.24f;
            //if (Util.Util.getDistance(Projectile.Center, targetPos) < 64) {
            //    Projectile.Center = Projectile.Center + (targetPos - Projectile.Center) * 0.3f;
            //}
            Projectile.ai[0]--;
            if (Projectile.owner.ToPlayer().channel)
            {
                Projectile.ai[0] = 6;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects ef = SpriteEffects.None;
            if (Projectile.owner.ToPlayer().direction < 0)
            {
                ef = SpriteEffects.FlipHorizontally;
            }
            Texture2D tx;
            if (Projectile.ai[1] > 0)
            {
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/TwistedTwin/TwistedTwin").Value;
                if (Projectile.ai[0] > 0)
                {
                    tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/TwistedTwin/a1").Value;
                }
            }
            else
            {
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/TwistedTwin/b1").Value;
                if (Projectile.ai[0] > 0)
                {
                    tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/TwistedTwin/b2").Value;
                }
            }
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White, 0, tx.Size() / 2, Projectile.scale, ef, 0);
            return false;
        }
    }
}

