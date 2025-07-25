﻿using CalamityEntropy.Content.Projectiles.Pets;
using CalamityMod.Prefixes;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs.Pets
{
    public class ConsecratedRefuge : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<Pooney>());
        }
    }
}
