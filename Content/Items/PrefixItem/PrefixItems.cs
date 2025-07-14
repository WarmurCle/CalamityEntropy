using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.PrefixItem
{
    // Blessing Prefixes
    public class BlessingVoid : BasePrefixItem
    {
        public override string PrefixName => "Void";
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VoidScales>(1).
                AddIngredient<NightmareFuel>(2)
                .Register();
        }
    }
    public class BlessingVoidTouched : BasePrefixItem
    {
        public override string PrefixName => "VoidTouched";
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VoidScales>(2).
                AddIngredient<AscendantSpiritEssence>(1)
                .Register();
        }
    }
    public class BlessingLastStand : BasePrefixItem
    {
        public override string PrefixName => "LastStand";
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<YharonSoulFragment>(10).
                AddIngredient<EffulgentFeather>(10).
                AddIngredient<AshesofAnnihilation>(2)
                .AddIngredient<ExoPrism>(2)
                .Register();
        }
    }
    public class BlessingEnd : BasePrefixItem
    {
        public override string PrefixName => "End";
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<YharonSoulFragment>(1)
                .AddIngredient<AshesofAnnihilation>(1)
                .AddIngredient<ExoPrism>(1)
                .Register();
        }
    }
    public class BlessingHeatDeath : BasePrefixItem
    {
        public override string PrefixName => "HeatDeath";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = Type;
        }
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityOverhaul", out var overhaul))
            {
                CreateRecipe().
                    AddIngredient(overhaul.Find<ModItem>("InfinityCatalyst").Type, 16)
                    .AddIngredient<VoidScales>(99)
                    .AddIngredient<WyrmTooth>(99)
                    .Register();
            }
        }
    }

    // RuneStone Prefixes
    public class RuneStoneShining : BasePrefixItem
    {
        public override string PrefixName => "Shining";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Torch, 5)
                .AddIngredient(ItemID.CopperBar, 3)
                .AddIngredient(ItemID.StoneBlock, 10)
                .Register();
        }
    }
    public class RuneStoneSilence : BasePrefixItem
    {
        public override string PrefixName => "Silence";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 1)
                .AddIngredient<BloodOrb>(5)
                .AddIngredient(ItemID.StoneBlock, 10)
                .Register();
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 1)
                .AddIngredient<BloodOrb>(5)
                .AddIngredient(ItemID.StoneBlock, 10)
                .Register();
        }
    }
    public class RuneStoneHard : BasePrefixItem
    {
        public override string PrefixName => "Hard";
        public override void AddRecipes()
        {
            CreateRecipe().AddRecipeGroup(RecipeGroupID.IronBar, 20)
                .AddIngredient(ItemID.Diamond, 2)
                .Register();
        }
    }
    public class RuneStoneThorny : BasePrefixItem
    {
        public override string PrefixName => "Thorny";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Cactus, 8)
                .AddIngredient(ItemID.StoneBlock, 10)
                .Register();
        }
    }
    public class RuneStoneLight : BasePrefixItem
    {
        public override string PrefixName => "Light";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Feather, 4)
                .AddIngredient(ItemID.Cloud, 10)
                .Register();
        }
    }
    public class RuneStoneBiochemistry : BasePrefixItem
    {
        public override string PrefixName => "Biochemistry";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.StoneBlock, 10).
                AddIngredient<CorrodedFossil>(5)
                .Register();
        }
    }
    public class RuneStoneGuarded : BasePrefixItem
    {
        public override string PrefixName => "Guarded";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.StoneBlock, 10).
                AddRecipeGroup(RecipeGroupID.IronBar, 5)
                .AddIngredient(ItemID.TurtleShell)
                .Register();
        }
    }
    public class RuneStoneRegen : BasePrefixItem
    {
        public override string PrefixName => "Regen";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.StoneBlock, 10).
                   AddIngredient(ItemID.LifeCrystal, 1)
                   .AddIngredient(ItemID.CopperBar, 5)
                   .Register();
        }
    }

    // EnchantedScroll Prefixes
    public class EnchantedScrollMassive : BasePrefixItem
    {
        public override string PrefixName => "Massive";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.Ectoplasm)
                .AddIngredient(ItemID.LunarTabletFragment)
                .Register();
        }
    }
    public class EnchantedScrollEvoker : BasePrefixItem
    {
        public override string PrefixName => "Evoker";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.Ectoplasm)
                .AddIngredient<LivingShard>(10)
                .Register();
        }
    }
    public class EnchantedScrollReckless : BasePrefixItem
    {
        public override string PrefixName => "Reckless";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.Ectoplasm)
                .AddIngredient<EssenceofHavoc>(2)
                .Register();
        }
    }
    public class EnchantedScrollMiracle : BasePrefixItem
    {
        public override string PrefixName => "Miracle";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.Ectoplasm)
                .AddIngredient(ItemID.HallowedBar, 5)
                .Register();
        }
    }
    public class EnchantedScrollMagical : BasePrefixItem
    {
        public override string PrefixName => "Magical";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.Ectoplasm)
                .AddIngredient(ItemID.FallenStar, 10)
                .Register();
        }
    }

    // OriginGem Prefixes
    public class OriginGemGreat : BasePrefixItem
    {
        public override string PrefixName => "Great";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ExodiumCluster>(5)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient<UnholyEssence>(4)
                .Register();
        }
    }
    public class OriginGemGodForged : BasePrefixItem
    {
        public override string PrefixName => "GodForged";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ExodiumCluster>(5)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient<CosmiliteBar>(1)
                .Register();
        }
    }
    public class OriginGemWizard : BasePrefixItem
    {
        public override string PrefixName => "Wizard";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ExodiumCluster>(5)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient<RuinousSoul>()
                .Register();
        }
    }
    public class OriginGemSacrifical : BasePrefixItem
    {
        public override string PrefixName => "Sacrifical";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ExodiumCluster>(5)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient<DivineGeode>(4)
                .Register();
        }
    }
    public class OriginGemDestinedGreatness : BasePrefixItem
    {
        public override string PrefixName => "DestinedGreatness";
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ExodiumCluster>(5)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient<Necroplasm>()
                .Register();
        }
    }
}
