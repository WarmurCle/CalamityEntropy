using CalamityEntropy.Content.Items.Books;
using CalamityMod.Items;
using CalamityMod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class ExquisiteCrown : ModItem
    {
        public int AdditionalPrice => 200;
        public static float MAGEDAMAGE = 0.05f;
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 16;
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions += 1;
            player.Entropy().exquisiteCrown = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[D]", MAGEDAMAGE.ToPercent().ToString());
        }
    }

    public class RubyCrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 10;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.OwnerEntropy().exquisiteCrown)
            {
                Projectile.timeLeft = 3;
            }
            else
            {
                Projectile.Kill();
                return;
            }
            Player player = Projectile.GetOwner();

            Projectile.Center = player.MountedCenter + player.gfxOffY * Vector2.UnitY + Vector2.UnitY * -24;
            if (Projectile.ai[0]-- <= -60)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    NPC target = Projectile.FindMinionTarget(1400);
                    if (target != null)
                    {
                        Projectile.ai[0] = 0;
                        int dmg = ((int)(player.GetTotalDamage(DamageClass.Summon).ApplyTo(32))).ApplyOldFashionedDmg();
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (target.Center - Projectile.Center).normalize() * 12, ModContent.ProjectileType<CrownRubyProj>(), dmg, 6, player.whoAmI);
                    }
                }
            }
            Projectile.velocity *= 0;
            Projectile.rotation = 0;

        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }
    public class CrownRubyProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, true, 1);
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = 4;
            Projectile.light = 0.42f;
        }
        public override void AI()
        {
            if (Projectile.localAI[2]++ == 0)
            {
                GeneralParticleHandler.SpawnParticle(new ImpactParticle(Projectile.Center, 0, 12, 0.6f, new Color(255, 80, 80)));
            }
            Projectile.rotation += (Projectile.velocity.X > 0 ? 1 : -1) * 0.2f;
            /* for (float i = 0; i < 1; i += 0.1f)
            {
                int ruby = Dust.NewDust(Projectile.position - Projectile.velocity * i, Projectile.width, Projectile.height, DustID.GemRuby, Projectile.velocity.X, Projectile.velocity.Y, 90, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }*/
            for (float i = 0; i <= 1; i += 0.25f)
            {
                Vector2 velocity1 = CEUtils.randomPointInCircle(3);
                Particle sparkle1 = new CritSpark(Projectile.Center - Projectile.velocity * i + Projectile.velocity * 1, velocity1, Color.White * 0.6f, Color.Crimson, 0.5f, 12, 0.1f, 3f, Main.rand.NextFloat(0f, 0.01f));
                GeneralParticleHandler.SpawnParticle(sparkle1);
            }
        }
        public override void OnKill(int timeLeft)
        {
            GeneralParticleHandler.SpawnParticle(new ImpactParticle(Projectile.Center, 0, 12, 0.6f, new Color(255, 80, 80)));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(Color.White));
            return false;
        }
    }
}
