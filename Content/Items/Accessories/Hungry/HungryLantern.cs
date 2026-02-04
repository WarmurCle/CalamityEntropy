using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.Hungry
{
    public class HungryLantern : ModItem
    {
        public static int Damage = 25;
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 62;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }
        public static string ID => "HungryLantern";
        public static float TagDamage = 0.12f;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip(ID);
            player.maxMinions += 1;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", TagDamage.ToPercent());
        }
        private static int projType = -1;
        public static int ProjType { get { if (projType == -1) { projType = ModContent.ProjectileType<TheHungry>(); } return projType; } }

    }
    public class TheHungry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public NPC target = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D vein = this.getTextureAlt("Vein");
            CEUtils.drawChain(Projectile.Center, Projectile.GetOwner().Center, vein.Width, vein);
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3) {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.OwnerEntropy().hasAcc(HungryLantern.ID))
                Projectile.timeLeft = 5;

            var t = Projectile.FindMinionTarget(1000);
            if (target != null && (!target.active || target.Distance(Projectile.Center) > 1200))
                target = null;
            if (target == null || (t != null && target.whoAmI != t.whoAmI))
                target = t;
            Projectile.position += Projectile.GetOwner().velocity;
            if (target == null)
            {
                Projectile.velocity *= 0.92f;
                Projectile.velocity += (target.Center - Projectile.Center).normalize() * 1.8f;
                if (CEUtils.getDistance(Projectile.Center, target.Center) < Projectile.velocity.Length() + 30)
                {
                    Projectile.Center = target.Center;
                    target.Entropy().HungryTagged = 3;
                    Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                }
            }
            else
            {
                Player player = Projectile.GetOwner();
                if (CEUtils.getDistance(Projectile.Center, player.Center) > 280)
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += (player.Center - Projectile.Center).normalize().RotatedBy(Projectile.ai[2]) * 0.6f;
                }
                else
                {
                    Projectile.ai[2] = Main.rand.NextFloat(-0.08f, 0.08f);
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One) * float.Lerp(Projectile.velocity.Length(), 12, 0.05f);
                }
            }
            Projectile.ai[1]--;
        }
    }
}
