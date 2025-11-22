using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.DustCarverBow
{
    public class CarverSpirit : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.width = Projectile.height = 32;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 5;
        }

        public enum Mode
        {
            Penetrate,
            Shooting,
            Defending
        }
        public override bool? CanHitNPC(NPC target)
        {
            return mode == Mode.Penetrate ? null : false;
        }
        public Mode mode { get { return (Mode)Projectile.ai[0]; } set { Projectile.ai[0] = (int)value; } }
        public int Delay = 0;

        public List<Vector2> OldPos = new List<Vector2>();
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            if(player.HeldItem.ModItem is DustCarver && !player.dead)
            {
                Projectile.timeLeft = 4;
            }
            Projectile.damage = player.GetWeaponDamage(player.HeldItem);
            Projectile.MaxUpdates = 1;
            if(CEUtils.getDistance(Projectile.Center, player.Center) > 4000)
            {
                Projectile.Center = player.Center + CEUtils.randomPointInCircle(100);
                Projectile.velocity *= 0;
            }
            int count = 0;
            int id = 0;
            foreach(Projectile proj in Main.ActiveProjectiles)
            {
                if(proj.type == Projectile.type && proj.owner == Projectile.owner)
                {
                    if (proj.whoAmI == Projectile.whoAmI)
                    {
                        id = count;
                    }
                    count++;
                }
            }
            Delay--;
            NPC target = CEUtils.FindTarget_HomingProj(Projectile, Vector2.Lerp(player.Center, Projectile.Center, 0.32f), 3400);
            switch(mode)
            {
                case Mode.Penetrate:
                    {
                        if (target == null)
                        {
                            Projectile.pushByOther(0.6f);
                            if (CEUtils.getDistance(Projectile.Center, player.Center) > 120)
                            {
                                Projectile.velocity += (player.Center - Projectile.Center).normalize();
                                Projectile.velocity *= 0.98f;
                            }
                            Projectile.rotation += Projectile.velocity.X * 0.02f;
                        }
                        else
                        {
                            Projectile.pushByOther(3f);
                            if (Delay == 1)
                                Projectile.velocity *= 0.1f;
                            if (CEUtils.getDistance(Projectile.Center, target.Center) < 300)
                            {
                                if (Delay <= 0)
                                {
                                    EParticle.NewParticle(new DOracleSlash() { centerColor = Color.White }, target.Center + (Projectile.Center - target.Center).normalize() * 80, Vector2.Zero, Color.Crimson, 140, 1, true, BlendState.Additive, (target.Center - Projectile.Center).ToRotation(), 16);
                                    Projectile.ResetLocalNPCHitImmunity();
                                    Delay = 8;
                                    Projectile.velocity = (target.Center + (target.Center - Projectile.Center).normalize() * 250 - Projectile.Center) / 8f;
                                }
                            }
                            else
                            {
                                Projectile.velocity += (target.Center - Projectile.Center).normalize() * 2;
                                Projectile.velocity *= 0.97f;
                            }
                            Projectile.rotation = (target.Center - Projectile.Center).ToRotation() + MathHelper.PiOver4;
                        }
                        break;
                    }
                case Mode.Shooting:
                    {
                        player.Calamity().mouseWorldListener = true;
                        Vector2 tpos = player.Center + (player.heldProj.ToProj().rotation.ToRotationVector2() * -200).RotatedBy((id - count / 2f) * 0.22f);

                        Projectile.velocity += (tpos - Projectile.Center) * float.Min(0.1f, CEUtils.getDistance(Projectile.Center, tpos) / 300f);
                        Projectile.velocity *= 0.6f;
                        Projectile.pushByOther(0.6f);

                        Projectile.rotation = ((target == null ? player.Calamity().mouseWorld : target.Center) - Projectile.Center).ToRotation();
                        if (target != null && Projectile.owner == Main.myPlayer)
                        {
                            if (Delay <= 0)
                            {
                                Delay = 40;
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (target.Center - Projectile.Center).normalize() * 36, ModContent.ProjectileType<CarverBolt>(), Projectile.damage * 4, 4, Projectile.owner).ToProj().scale *= 0.7f;
                                Projectile.velocity -= Projectile.rotation.ToRotationVector2() * 12;
                            }
                        }
                        break;
                    }
                case Mode.Defending:
                    {
                        bool hasproj = false;
                        Projectile tar = null;
                        float distance = 1200;
                        Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver2;
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.hostile && !p.friendly && p.damage > 0 && Math.Max(p.width, p.height) < 150 && CEUtils.getDistance(Projectile.Center, p.Center) < distance && CEUtils.getDistance(player.Center, p.Center) < 256)
                            {
                                if (p.Colliding(p.getRect(), p.Center.getRectCentered(16, 16)) && !p.Colliding(p.getRect(), (p.Center + p.rotation.ToRotationVector2() * 320).getRectCentered(36, 36)) && !p.Colliding(p.getRect(), (p.Center + p.velocity.normalize() * 320).getRectCentered(36, 36)))
                                {
                                    if (Delay <= 0 && p.Colliding(p.getRect(), Projectile.getRect()))
                                    {
                                        p.Kill();
                                        Delay = 300;
                                    }
                                    else
                                    {
                                        tar = p;
                                        distance = CEUtils.getDistance(Projectile.Center, p.Center);
                                        hasproj = true;
                                    }
                                }
                            }
                        }
                        if (hasproj && Delay <= 12)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.velocity += (tar.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 4f;
                                Projectile.velocity *= 0.94f;
                            }
                        }
                        else
                        {
                            Vector2 targetPosp;
                            int index = id;
                            int maxFlies = count;
                            
                            targetPosp = player.Center + MathHelper.ToRadians(((float)(index) / (float)(maxFlies)) * 360).ToRotationVector2().RotatedBy(player.Entropy().CasketSwordRot * 0.4f) * 140;
                            Projectile.velocity += (targetPosp - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f;
                            Projectile.velocity *= 0.9f;
                            if (CEUtils.getDistance(targetPosp, Projectile.Center) < Projectile.velocity.Length() + 32)
                            {
                                Projectile.Center = targetPosp;
                                Projectile.velocity *= 0f;
                            }

                        }
                        break;
                    }
            }
            OldPos.Add(Projectile.Center);
            if (OldPos.Count > 8)
                OldPos.RemoveAt(0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = (mode == Mode.Penetrate ? Projectile.GetTexture() : (mode == Mode.Shooting ? this.getTextureAlt() : this.getTextureAlt("AltB" + (((int)Main.GameUpdateCount / 4) % 3).ToString())));
            if (mode == Mode.Defending && Delay > 5)
                lightColor *= 0.4f;
            else
            {
                OldPos.Add(Projectile.Center);
                for (int i = 1; i < OldPos.Count; i++)
                {
                    for (float j = 0; j < 1; j += 0.1f)
                    {
                        Main.EntitySpriteDraw(tex, Vector2.Lerp(OldPos[i - 1], OldPos[i], j) - Main.screenPosition, null, lightColor * 0.12f * ((float)i / OldPos.Count), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
                    }
                }
                OldPos.RemoveAt(OldPos.Count - 1);
            }
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor, tex));
            return false;
        }
    }
}
