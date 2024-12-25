using System;
using System.Collections.Generic;
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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopBlackProjectile : PoopProj
    {
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (Projectile.owner == Main.myPlayer)
            {
                CalamityEntropy.blackMaskTime = 4 * 60;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (Util.Util.getDistance(npc.Center, Projectile.Center) < 4000)
                    {
                        NPC.HitInfo hit = npc.CalculateHitInfo(Projectile.damage / 8, 0, false, 0, Projectile.DamageType);
                        npc.StrikeNPC(hit);
                        npc.AddBuff(31, 4 * 60);
                        npc.AddBuff(ModContent.BuffType<TemporalSadness>(), 4 * 60);
                    }
                }
            }
        }
    }

}