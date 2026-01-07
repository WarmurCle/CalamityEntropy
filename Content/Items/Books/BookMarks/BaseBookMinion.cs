using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityEntropy.ScreenShaker;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public abstract class BaseBookMinion : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public virtual float DamageMult => 1;
        public int itemType = -1;

        public override void AI()
        {
            base.AI();
            if (itemType == -1)
                itemType = Projectile.GetOwner().HeldItem.type;
            if (BookMarkLoader.GetPlayerHeldEntropyBook(Projectile.GetOwner(), out var eb))
            {
                Projectile.damage = eb.CauculateProjectileDamage(DamageMult);
                Projectile.CritChance = (int)eb.GetProjectileModifer().Crit;
            }
            if (Projectile.GetOwner().HeldItem.type != itemType)
                Projectile.Kill();
        }
        public override void ApplyHoming()
        {
        }
    }
}