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
using CalamityEntropy.Util;
using Terraria.DataStructures;
using System.Security.Permissions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod;
using Terraria.GameContent;
using CalamityEntropy.Items;
namespace CalamityEntropy.Projectiles.SamsaraCasket
{
    public class FireDragonsRoar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = NoneTypeDamageClass.Instance;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public override void AI(){
            Projectile.ArmorPenetration = HorizonssKey.getArmorPen();
            Projectile.ai[0]++;
            for (int i = 0; i < 5; i++)
            {
                Projectile p = Projectile;
                odp.Add(p.Center - p.velocity * (float)i / 5f);
                odr.Add(p.rotation);
                if (odp.Count > 24)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }
            }
            if (Projectile.ai[0] > 10)
            {
                NPC target = Projectile.FindTargetWithinRange(1100, false);
                if (target != null)
                {
                    Projectile.velocity *= 0.945f;
                    Vector2 v = target.Center - Projectile.Center;
                    v.Normalize();

                    Projectile.velocity += v * 2f;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.Next(6, 14) + new Vector2(0, -8), ModContent.ProjectileType<ZeratosBullet0>(), Projectile.damage, Projectile.knockBack * 2, Projectile.owner);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            if (HorizonssKey.getVoidTouchLevel() > 0)
            {
                EGlobalNPC.AddVoidTouch(target, 80, HorizonssKey.getVoidTouchLevel(), 800, 16);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Util.Util.DrawAfterimage(TextureAssets.Projectile[Projectile.type].Value, odp, odr);
            lightColor = Color.White;
            return base.PreDraw(ref lightColor);
        }
    }

}