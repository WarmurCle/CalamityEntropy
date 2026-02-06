using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookmarkBloodthirsty : BookMark, IPriceFromRecipe
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Bloodthirsty");
        public override Color tooltipColor => new Color(255, 6, 6);
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.attackSpeed += Main.LocalPlayer.Entropy().BloodthirstyEffect;
            modifer.lifeSteal += 0.1f;
        }
        public override EBookProjectileEffect getEffect()
        {
            return new BloodthirstBMEffect();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientBoneDust>(2)
                .AddIngredient<BloodOrb>()
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
    public class BloodthirstBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if(CECooldowns.CheckCD("BloodthirstyRage", 4) && !projectile.GetOwner().HasBuff<RageMode>())
                projectile.GetOwner().Calamity().rage += projectile.GetOwner().Calamity().rageMax / 34;
        }
    }
}