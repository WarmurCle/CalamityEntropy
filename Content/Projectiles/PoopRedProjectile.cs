using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod;
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
    public class PoopRedProjectile : PoopProj
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Util.Util.PlaySound("firedeath hiss", 1, Projectile.Center);
        }
        public override bool BreakWhenHitNPC => false;
        public override bool damageNPCAfterLand => true;
    }

}