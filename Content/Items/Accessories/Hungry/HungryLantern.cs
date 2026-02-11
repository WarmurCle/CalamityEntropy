using CalamityMod.Items;
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
        public static float TagDamage = 0.09f;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip(ID);
            player.maxMinions += 1; TagDamage = 0.1f;
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
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2500;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 50;
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
            if (CEUtils.getDistance(Projectile.Center, Projectile.GetOwner().Center) > 3000)
                Projectile.Kill();
            if (Projectile.GetOwner().dead)
                Projectile.Kill();
            if (!Projectile.active)
                return;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                if (Projectile.frame > 2)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            if (Projectile.OwnerEntropy().hasAcc(HungryLantern.ID))
                Projectile.timeLeft = 5;

            var t = Projectile.FindMinionTarget(1000);
            if (target == null || (t != null && target.whoAmI != t.whoAmI))
                target = t;
            if (target != null && (!target.active || target.Distance(Projectile.GetOwner().Center) > 760))
                target = null;
            if (target != null)
            {
                Vector2 targetPos = (target.Center + (Projectile.GetOwner().Center - target.Center).normalize() * 18);
                Projectile.velocity *= 0.75f;
                Projectile.velocity += (targetPos - Projectile.Center).normalize() * 6.5f;
                if (CEUtils.getDistance(Projectile.Center, targetPos) < Projectile.velocity.Length() + target.velocity.Length() + 6)
                {
                    Projectile.Center = targetPos;
                    target.Entropy().HungryTagged = 3;
                    Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                    Projectile.velocity *= 0;
                }
            }
            else
            {
                Player player = Projectile.GetOwner();
                Projectile.position += player.velocity;
                if (CEUtils.getDistance(Projectile.Center, player.Center) > 180)
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += (player.Center - Projectile.Center).normalize().RotatedBy(Projectile.ai[2]) * 0.6f;
                }
                else
                {
                    Projectile.ai[2] = Main.rand.NextFloat(-0.2f, 0.2f);
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One) * float.Lerp(Projectile.velocity.Length(), 12, 0.05f);
                }
            }
            Projectile.ai[1]--;
            Projectile.rotation = (Projectile.Center - Projectile.GetOwner().Center).ToRotation();
        }
    }
}
