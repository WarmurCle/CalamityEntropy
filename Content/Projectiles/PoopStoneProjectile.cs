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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopStoneProjectile : PoopProj
    {
        public override bool BreakWhenHitNPC => false;
        public override int damageChance => 35;
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.damage *= 3;
        }
        public override int dustType => DustID.Stone;
    }

}