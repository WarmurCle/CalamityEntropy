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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopGoldenProjectile : PoopProj
    {
        public static int ShootCd = 24;
        public int myCD = 0;
        public override void AI()
        {
            base.AI();
            NPC target = Util.Util.findTarget(Projectile.owner.ToPlayer(), Projectile, 1000, false);
            if (target != null)
            {
                if(myCD <= 0)
                {
                    myCD = ShootCd;
                    int type = Main.rand.Next(3);
                    if (type == 2)
                    {
                        type = Main.rand.Next(3);
                    }
                    if (type == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(new Vector2(0, -1)) * 14, ProjectileID.GoldCoin, Projectile.damage / 2, 1, Projectile.owner);
                    }
                    if (type == 1)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(new Vector2(0, -1)) * 14, ProjectileID.GoldCoin, Projectile.damage, 1, Projectile.owner);
                    }
                    if (type == 2)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(new Vector2(0, -1)) * 14, ProjectileID.GoldCoin, Projectile.damage * 2, 1, Projectile.owner);
                    }
                }
            }
            myCD--;
        }
        public override int dustType => DustID.GoldCoin;
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Item.NewItem(Projectile.GetSource_FromThis(), Projectile.getRect(), new Item(ItemID.SilverCoin, Main.rand.Next(1, 40)));
        }
    }

}