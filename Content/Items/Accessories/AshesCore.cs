using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.Hungry
{
    public class AshesCore : ModItem
    {
        public static int BaseDamage = 24;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 58;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.maxMinions += 1;
        }
        private static int projType = -1;
        public static int ProjType { get { if (projType == -1) { projType = ModContent.ProjectileType<AshesSpirit>(); } return projType; } }

    }
    public class AshesSpirit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2500;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.width = Projectile.height = 26;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public NPC target = null;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
        
        }
    }
}
