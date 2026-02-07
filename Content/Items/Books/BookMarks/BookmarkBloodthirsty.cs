using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
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
        public override void OnCreated(ItemCreationContext context)
        {
            if (context is RecipeItemCreationContext)
            {
                Main.LocalPlayer.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral(Main.LocalPlayer.name + " " + Mod.GetLocalization("BloodthirstyKilled").Value)), 99, 0, false, false, -1, false, 99999);
                if (Main.LocalPlayer.statLife <= 0 || Main.LocalPlayer.dead)
                    Item.TurnToAir();
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "TooltipBt", Mod.GetLocalization("BloodthirstyRequirement").Value) { OverrideColor = Color.Yellow});
        }
    }
    public class BloodthirstBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            var plr = projectile.GetOwner();
            if (plr.Calamity().RageEnabled && CECooldowns.CheckCD("BloodthirstyRage", 4) && !plr.HasBuff<RageMode>())
            {
                plr.Calamity().rage += projectile.GetOwner().Calamity().rageMax / 34;    
            }
        }
    }
}