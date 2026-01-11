using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using InnoVault;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public interface IPriceFromRecipe
    {
        public virtual int AdditionalPrice => 0;
    }
    public class PriceSetGItem : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (PriceSetSys.Inited && entity.ModItem != null && entity.ModItem is IPriceFromRecipe pfr)
            {
                entity.value = entity.ModItem.GetPriceFromRecipe(CEUtils.FindRecipe(entity.type)) + pfr.AdditionalPrice;
            }
        }
    }
    public class PriceSetSys : ModSystem
    {
        public static bool Inited = false;
        public override void AddRecipes()
        {
        }
        public override void Load()
        {
            VaultLoad.EndLoadenEvent += endLoad;
        }

        public override void Unload()
        {
            VaultLoad.EndLoadenEvent -= endLoad;
        }

        private void endLoad()
        {
            if (MaliciousCode.CALAMITY__OVERHAUL)
            {
                CWRWeakRef.CWRRef.SetupFishSkillBM();
            }
            Inited = true;
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                if (item.ModItem != null && item.ModItem is IPriceFromRecipe pfr)
                {
                    item.value = item.ModItem.GetPriceFromRecipe(CEUtils.FindRecipe(item.type)) + pfr.AdditionalPrice;
                }
            }
        }
    }
}
