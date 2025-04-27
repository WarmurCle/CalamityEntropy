using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkLunar : BookMark
    {
        public static int distance = 340;
        public override Texture2D UITexture => BookMark.GetUITexture("Lunar");
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
        }
        public override Color tooltipColor => Color.YellowGreen;
        public override EBookProjectileEffect getEffect()
        {
            return new LunarBMEffect();
        }
    }
    public class LunarBMEffect : EBookProjectileEffect
    {
        public void applyEffect(NPC n, float p, Projectile projectile)
        {
            n.GetGlobalNPC<LunarBMGlobalNPC>().progress += p;
            n.GetGlobalNPC<LunarBMGlobalNPC>().decreaceCd = 40;
            if (n.GetGlobalNPC<LunarBMGlobalNPC>().progress >= 1)
            {
                int dmg = (int)(projectile.damage * 3f);
                n.GetGlobalNPC<LunarBMGlobalNPC>().progress = 0;
                projectile.getOwner().ApplyDamageToNPC(n, dmg, 0, 0, false, projectile.DamageType);
                Utilities.Util.PlaySound("light_bolt", 1, n.Center);
                for (int i = 0; i < 80; i++)
                {
                    EParticle.spawnNew(new GlowSpark2(), n.Center, Utilities.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 12), Color.Lerp(Color.SpringGreen, new Color(200, 230, 255), Main.rand.NextFloat()), Main.rand.NextFloat(0.1f, 0.2f), 1, true, BlendState.Additive, 0);
                }
                ProjectileLoader.OnHitNPC(projectile, n, n.CalculateHitInfo(dmg, 0, false, 0, projectile.DamageType), dmg);
            }
        }
        public override void UpdateProjectile(Projectile projectile, bool ownerClient)
        {
            (projectile.ModProjectile as EBookBaseProjectile).color = Color.YellowGreen;
            if (ownerClient)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (!n.friendly && !n.dontTakeDamage)
                    {
                        if (projectile.ModProjectile is EBookBaseLaser el)
                        {
                            List<Vector2> points = el.getSamplePoints();
                            foreach (Vector2 point in points)
                            {
                                if (Utilities.Util.getDistance(point, n.Center) < BookMarkLunar.distance)
                                {
                                    applyEffect(n, 1f / 100f, projectile);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (Utilities.Util.getDistance(projectile.Center, n.Center) < BookMarkLunar.distance)
                            {
                                applyEffect(n, 1f / 100f, projectile);
                            }
                        }
                    }
                }
            }
        }
    }
    public class LunarBMGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public float progress = 0;
        public int decreaceCd = 0;
        public override void AI(NPC npc)
        {
            if (decreaceCd > 0)
            {
                decreaceCd--;
            }
            else
            {
                if (progress > 0)
                    progress -= 0.005f;
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            drawColor = Color.Lerp(drawColor, new Color(9, 30, 72), progress);
        }
    }
}