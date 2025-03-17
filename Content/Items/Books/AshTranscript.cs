
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Util;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class AshTranscript : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 160;
            Item.useAnimation = Item.useTime = 16;
            Item.crit = 10;
            Item.mana = 8;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark5").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<AshTranscriptHeld>();
        public override int SlotCount => 4;

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<NightEpic>()
                .AddIngredient<DivineGeode>(6)
                .AddIngredient(ItemID.Ectoplasm, 6)
                .AddTile<ProfanedCrucible>()
                .Register();
        }
    }

    public class AshTranscriptHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/AshTranscript/AshTranscriptOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/AshTranscript/AshTranscriptPage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/AshTranscript/AshTranscriptUI";

        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.PenetrateAddition += 2;
            return m;
        }
        public override float randomShootRotMax => 0.02f;
        public override int baseProjectileType => ModContent.ProjectileType<ATLaser>();

        public override int frameChange => 3;
        public override EBookProjectileEffect getEffect()
        {
            return new HolyFireDebuffEffect();
        }

    }

    public class HolyFireDebuffEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 600);
        }
    }
}