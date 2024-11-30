using CalamityEntropy.Items;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Biomes;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Projectiles.SamsaraCasket
{
    public class Dirge : SamsaraSword
    {
        public override void AI()
        {
            base.AI();
            
        }
        public override void attackAI(NPC t)
        {
            Vector2 targetPos = t.Center + new Vector2(0, 100 + t.height);
            Projectile.velocity = (targetPos - Projectile.Center) * 0.1f;
            Projectile.rotation = (t.Center - Projectile.Center).ToRotation();
            setDamage(5);
            if (Projectile.ai[1]++ % 40 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        float rot = Util.Util.randomRot();
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), t.Center + rot.ToRotationVector2() * 400, rot.ToRotationVector2() * -16, 645, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        p.ToProj().DamageType = Projectile.DamageType;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return base.PreDraw(ref lightColor);
        }
    }
}
