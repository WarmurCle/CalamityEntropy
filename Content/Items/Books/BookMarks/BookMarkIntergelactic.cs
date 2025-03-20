using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkIntergelactic : BookMark 
    {
        public override Texture2D UITexture => BookMark.GetUITexture("Intergelactic");
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            if(ModLoader.TryGetMod("CatalystMod", out Mod caly))
            {
                if (caly.TryFind<ModRarity>("SuperbossRarity", out ModRarity rare))
                {
                    Item.rare = rare.Type;
                }
            }
        }
        public override Color tooltipColor => Color.LightSkyBlue;
        public override EBookProjectileEffect getEffect()
        {
            return new IGBMEffect();
        }
    }
    public class IGBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            if(ownerClient && Main.rand.NextBool(6))
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity * 0.6f, ModContent.ProjectileType<NovaSlimerProj>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
    }
}