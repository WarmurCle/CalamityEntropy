using System;
using System.Collections.Generic;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopVoidProjectile : PoopProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.light = 1f;
        }
        public override bool BreakWhenHitNPC => false;
        public override int damageChance => 12;
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.damage *= 5;
        }
        public override void AI()
        {
            base.AI();
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if(p.hostile && p.velocity != Vector2.Zero && Util.Util.getDistance(p.Center, Projectile.Center) < 640)
                {
                    p.velocity += (Projectile.Center - p.Center).SafeNormalize(Vector2.Zero) * 1f;
                }
            }
            foreach (NPC p in Main.ActiveNPCs)
            {
                if (!p.friendly && p.velocity != Vector2.Zero && Util.Util.getDistance(p.Center, Projectile.Center) < 640)
                {
                    p.velocity += (Projectile.Center - p.Center).SafeNormalize(Vector2.Zero) * 0.4f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 600, 40);
        }
        public override int dustType => DustID.Corruption;
    }

}