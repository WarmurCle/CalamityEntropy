using CalamityMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.StatBloats
{
    internal partial class StatBloatToWeapons :GlobalItem
    {
        #region 传奇武器使用
        private const float PostMLWeaponsBoost = 1.3f; // 月后
        private const float PostProfanedWeaponsBoost = 2.2f; // 亵渎后||使徒 不是哥们，你们dps怎么这么低
        private const float PostPolterghastWeaponsBoost = 2.4f; // 幽花后
        private const float PostOldDukeWeaponsBoost = 2.5f; // 老猪后
        private const float PostDOGWeaponsBoost = 3f; // 神后
        private const float PostYharonWeaponsBoost = 5f; // 龙后
        private const float PostExoAndScalWeaponsBoost = 7f; // 巨械終灾后
        private const float PostShadowspecWeaponsBoost = 10f; // 巨械終灾后 
        #endregion

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (CrossModStatBloats.LegendaryList.Contains(item.type))
                damage *= SelectedBloat();
        }
        //到底是谁他妈在做全流程数值膨胀的传奇武器？
        private float SelectedBloat()
        {
            float boost = 1f;
            //击倒月总. 1.3
            if (Condition.DownedMoonLord.IsMet())
                boost += PostMLWeaponsBoost - 1f;
            if (DownedBossSystem.downedProvidence)
                boost += PostProfanedWeaponsBoost - PostMLWeaponsBoost;
            if (DownedBossSystem.downedPolterghast)
                boost += PostPolterghastWeaponsBoost - PostProfanedWeaponsBoost;
            if (DownedBossSystem.downedBoomerDuke)
                boost += PostOldDukeWeaponsBoost - PostPolterghastWeaponsBoost;
            if (DownedBossSystem.downedDoG)
                boost += PostDOGWeaponsBoost - PostOldDukeWeaponsBoost;
            if (DownedBossSystem.downedYharon)
                boost += PostYharonWeaponsBoost - PostDOGWeaponsBoost;
            if (DownedBossSystem.downedExoMechs || DownedBossSystem.downedCalamitas)
                boost += PostExoAndScalWeaponsBoost - PostYharonWeaponsBoost;
            if (DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas)
                boost += PostShadowspecWeaponsBoost - PostExoAndScalWeaponsBoost;
            //clamp一下避免因为过度计算出事
            boost = MathHelper.Clamp(boost, 1f,PostShadowspecWeaponsBoost);
            return boost;        
        }

        internal void HandleTlipocaz()
        {

        }
        
    }
}
