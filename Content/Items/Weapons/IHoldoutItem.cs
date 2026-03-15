using CalamityEntropy.Content.Buffs;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public interface IHoldoutItem
    {
        public abstract int ProjectileType { get; }
    }
}
