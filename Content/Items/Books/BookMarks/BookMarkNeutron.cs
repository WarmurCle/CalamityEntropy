using CalamityEntropy.Util;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkNeutron : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Neutron");
        public override EBookProjectileEffect getEffect()
        {
            return new NeutronBMEffect();
        }
        public override Color tooltipColor => Color.Lerp(Color.White, Color.Black, (0.5f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 12) * 0.5f));
    }

    public class NeutronBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            (projectile.ModProjectile as EBookBaseProjectile).color = Color.Black;
        }
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(10))
            {
                Util.Util.PlaySound("blackholeEnd", 1.25f, projectile.Center, 1, 1.2f);
                if (ModLoader.TryGetMod("CalamityOverhaul", out var co))
                {
                    if (co.TryFind<ModProjectile>("EXNeutronExplode", out var mp))
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, mp.Type, projectile.damage * 16, projectile.knockBack, projectile.owner).ToProj().DamageType = projectile.DamageType;
                    }
                }
            }
        }
    }
}