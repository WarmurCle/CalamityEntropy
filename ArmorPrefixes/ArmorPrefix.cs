using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.ArmorPrefixes
{

    public abstract class ArmorPrefix : ModType
    {
        public static List<ArmorPrefix> instances = new List<ArmorPrefix>();
        protected sealed override void Register()
        {
            instances.Add(this);
        }

        public virtual int getRollChance()
        {
            return 5;
        }

        public virtual bool? canApplyTo(Item item)
        {
            return null;
        }

        public virtual void ApplyTo(Item item) {
            if (Util.Util.IsArmor(item) && canApplyTo(item) != false)
            {
                item.Entropy().armorPrefixName = this.FullName;
            }
        }

        public static ArmorPrefix RollPrefixToItem(Item item) {
            if (!Util.Util.IsArmor(item))
            {
                return null;
            }
            int WeightAll = 0;
            List<ArmorPrefix> canApplyList = new List<ArmorPrefix>();

            foreach (ArmorPrefix ap in instances) {
                if (ap.canApplyTo(item) != false) {
                    WeightAll += ap.getRollChance();
                    canApplyList.Add(ap);
                }
            }

            if (WeightAll == 0 || canApplyList.Count == 0)
            {
                return null;
            }

            int chosenWeight = Main.rand.Next(0, WeightAll);
            int check = 0;
            foreach (ArmorPrefix ap in canApplyList)
            {
                check += ap.getRollChance();
                if (chosenWeight < check)
                {
                    return ap;
                }
            }
            return null;
        }

        public virtual float AddDefense()
        {
            return 0;
        }

        public virtual string getDesc()
        {
            return Language.GetOrRegister("Mods." + this.Mod.Name + ".ArmorPrefix" + this.Name + "Description").Value;
        }
        public virtual TooltipLine getDescTooltipLine()
        {
            return new TooltipLine(CalamityEntropy.Instance, "Armor Prefix Description", this.getDesc()) { OverrideColor = Color.Green };
        }
        public string GivenName { get { return Language.GetOrRegister("Mods." + this.Mod.Name + ".ArmorPrefix" + this.Name + "Name").Value; } }

        public virtual void updateEquip(Player player, Item item)
        {

        }

        public virtual Color getColor()
        {
            return Color.Blue;
        }

        public virtual bool Dramatic()
        {
            return false;
        }

        public static ArmorPrefix findByName(string name)
        {
            foreach (ArmorPrefix prefix in instances)
            {
                if (prefix.RegisterName() == name)
                {
                    return prefix;
                }
            }
            return null;
        }
        public virtual string RegisterName()
        {
            return this.Name;
        }
        public virtual string getName()
        {
            return GivenName;
        }
        
    }
}