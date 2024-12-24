using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
    public class PoopGiantProjectile : PoopProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 64;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.damage *= 3;
        }
        public override bool BreakWhenHitNPC => false;

        public override int damageChance => 18;
    }

}