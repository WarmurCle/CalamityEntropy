using InnoVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if(PriceSetSys.Inited && entity.ModItem != null && entity.ModItem is IPriceFromRecipe pfr)
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
