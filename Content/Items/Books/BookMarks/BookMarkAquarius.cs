using CalamityEntropy.Utilities;
using CalamityMod.Items;
using CalamityMod.Projectiles.Turret;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkAquarius : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().stroke = true;
            Item.Entropy().NameColor = Color.LightBlue;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().tooltipStyle = 4;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Aquarius");
        public override Color tooltipColor => Color.LightBlue;
        public override EBookProjectileEffect getEffect()
        {
            return new AquariusBMEffect();
        }
    }

    public class AquariusBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            int shootCount = 5;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                float rot = 0;
                if (npc != target && npc.Distance(target.Center) < 800 && !npc.friendly && !npc.dontTakeDamage)
                {
                    rot = (target.Center - npc.Center).ToRotation();
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, rot.ToRotationVector2() * 8, ModContent.ProjectileType<WaterShot>(), projectile.damage / 16, projectile.knockBack + 0.5f, projectile.owner);
                    shootCount--;
                }
            }
            for (; shootCount > 0; shootCount--)
            {
                float rot = Utilities.Util.randomRot();
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, rot.ToRotationVector2() * 8, ModContent.ProjectileType<WaterShot>(), projectile.damage / 16, projectile.knockBack + 0.5f, projectile.owner);
            }
        }
    }
}