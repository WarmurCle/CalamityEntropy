using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class WelkingShield : ModProjectile
    {
        public SoundStyle sound = new SoundStyle("CalamityEntropy/Assets/Sounds/flashback");
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.4f;

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public bool flag = true;
        public int btime = 16;
        public float rp = 2;
        
        public override void AI()
        {
            if (flag)
            {
                flag = false;
                Util.Util.PlaySound("vshield", 1, Projectile.Center);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            rp *= 0.73f;
            btime--;
            Player plr = Projectile.getOwner();
            Projectile.Center = plr.Center;
            if(btime < 0)
            {
                Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity <= 0)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Vector2 p1 = Projectile.Center + new Vector2(50, -70).RotatedBy(Projectile.rotation);
                Vector2 p2 = Projectile.Center + new Vector2(50, 70).RotatedBy(Projectile.rotation);
                foreach(NPC n in Main.ActiveNPCs)
                {
                    if(!n.friendly && Util.Util.LineThroughRect(p1, p2, n.Hitbox, 56))
                    {
                        Projectile.getOwner().Entropy().immune = 20;
                        n.SimpleStrikeNPC(56, Projectile.velocity.X > 0 ? 1 : -1, true, 20, DamageClass.Melee);
                        n.velocity = (n.Center - Projectile.getOwner().Center).normalize() * (n.velocity.Length() * 2 + n.velocity.Length() > 0.01f ? 12 : 0);
                        Block();
                        break;
                    }
                }
            }
        }
        public void Block()
        {
            btime = 0;
            rp = 0;
            Projectile.getOwner().velocity = Projectile.rotation.ToRotationVector2() * -4;
            Projectile.getOwner().Entropy().immune = 46;
            CalamityEntropy.Instance.screenShakeAmp = 4;
            Projectile.getOwner().Calamity().GeneralScreenShakePower = 4;
            Projectile.getOwner().Entropy().vShieldCD = 100;
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(sound, Projectile.Center);
            }
            EParticle.spawnNew(new Sn(), Projectile.Center, Projectile.rotation.ToRotationVector2() * 16, Color.SkyBlue, 0.66f, 1, true, BlendState.Additive, Projectile.rotation);
            EParticle.spawnNew(new AbyssalLine() { xadd = 0.44f, lx = 0.44f }, Projectile.Center + Projectile.rotation.ToRotationVector2() * 66, Vector2.Zero, Color.SkyBlue, 1, 1, true, BlendState.Additive, Projectile.rotation + MathHelper.PiOver2);
        }
        public override string Texture => "CalamityEntropy/Assets/Extra/WelkinShield";
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.getTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation + (Projectile.velocity.X > 0 ? -1 : 1) * rp, tex.Size() * 0.5f, Projectile.Opacity, SpriteEffects.None);
            return false;
        }
    }
    public class WelkingShieldGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool friendly = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if(source is EntitySource_Parent ep && ep.Entity is Projectile pj)
            {
                if (pj.GetGlobalProjectile<WelkingShieldGProj>().friendly)
                {
                    friendly = true;
                    projectile.friendly = true;
                    projectile.hostile = false;
                    projectile.owner = pj.owner;
                }
            }
        }
        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if(projectile.damage > 0 && projectile.hostile && projectile.Colliding(projectile.getRect(), target.getRect()) && target.ownedProjectileCounts[ModContent.ProjectileType<WelkingShield>()] > 0)
            {
                foreach(Projectile proj in Main.ActiveProjectiles)
                {
                    if(proj.ModProjectile is WelkingShield ws && ws.btime > 0)
                    {
                        if(Util.Util.GetAngleBetweenVectors(proj.velocity, projectile.Center - proj.Center) < MathHelper.ToRadians(65))
                        {
                            projectile.velocity = proj.velocity.normalize() * projectile.velocity.Length();
                            ws.Block();
                            /*if(int.Max(projectile.width, projectile.height) < 90)
                            {
                                projectile.hostile = false;
                                projectile.friendly = true;
                                projectile.owner = proj.owner;
                                friendly = true;
                                projectile.damage *= 10;
                            }*/
                            return false;
                        }
                    }
                }
            }
            return base.CanHitPlayer(projectile, target);
        }

        public override bool PreAI(Projectile projectile)
        {
            foreach (var target in Main.ActivePlayers)
            {
                if (projectile.damage > 0 && projectile.hostile && projectile.Colliding(projectile.getRect(), target.getRect().Center.ToVector2().getRectCentered(136, 136)) && target.ownedProjectileCounts[ModContent.ProjectileType<WelkingShield>()] > 0)
                {
                    foreach (Projectile proj in Main.ActiveProjectiles)
                    {
                        if (proj.ModProjectile is WelkingShield ws && ws.btime > 0)
                        {
                            if (Util.Util.GetAngleBetweenVectors(proj.velocity, projectile.Center - proj.Center) < MathHelper.ToRadians(65))
                            {
                                projectile.velocity = proj.velocity.normalize() * projectile.velocity.Length();
                                ws.Block();
                                /*if (int.Max(projectile.width, projectile.height) < 90)
                                {
                                    projectile.hostile = false;
                                    projectile.friendly = true;
                                    projectile.owner = proj.owner;
                                    friendly = true;
                                    projectile.damage *= 10;
                                }*/
                                break;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }

}