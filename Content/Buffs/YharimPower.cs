使用 CalamityMod;
使用泰拉瑞亚;
使用 Terraria.ID;
使用 Terraria.ModLoader;

命名空间 CalamityEntropy.内容.增益
{
    公共 类 YharimPower : ModBuff
    {
        公共 覆盖 无返回值 SetStaticDefaults()
        {
            Main.debuff[Type] = 假;
            Main.pvpBuff[Type] = 真;
            Main.buffNoSave[Type] =   假;
            BuffID.Sets LongExpertDebuff[Type] = 假;
        }

        公共 覆盖 无返回值 更新(玩家 player, 引用 整数 buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.06f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.pickSpeed -= 0.3f;
            player.GetAttackSpeed(DamageClass.Melee) *= 1.1f;
            player.GetKnockback(DamageClass.Summon) *= 2;
            player.moveSpeed *= 1.1f;
            player.statDefense += 10;
            player.lifeRegen += 6;
        }
    }
输入：}
