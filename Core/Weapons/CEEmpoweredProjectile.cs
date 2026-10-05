using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Core.Weapons
{
    /// <summary>原 stealthStrike,来源标记同挂这里</summary>
    public class CEEmpowerGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool Empowered;

        /// <summary>不过网,主人端命中钩子读</summary>
        internal bool FromWeaponUse;

        /// <summary>所有者端,命中回充</summary>
        internal Item sourceItem;

        /// <summary>大招衍生不回充</summary>
        internal bool creditBlocked;

        /// <summary>GetSource_Accessory 也是 EntitySource_ItemUse</summary>
        private static bool IsWeaponUse(EntitySource_ItemUse itemUse)
            => itemUse.Item != null && !itemUse.Item.accessory && itemUse.Item.damage > 0;

        public override void OnSpawn(Projectile projectile, IEntitySource source) {
            //EntitySource_ItemUse 继承 Parent,须先判
            if (source is EntitySource_ItemUse itemUse) {
                FromWeaponUse = IsWeaponUse(itemUse);

                if (itemUse.Item?.ModItem is not ICEChargeWeapon)
                    return;

                sourceItem = itemUse.Item;

                //OnSpawn 早于首包,标志随包走
                if (projectile.owner >= 0 && projectile.owner < Main.maxPlayers
                    && Main.player[projectile.owner].GetModPlayer<CEChargePlayer>().EmpowerWindowActive) {
                    Empowered = true;
                }
                return;
            }

            //父弹一跳继承,不向上爬
            if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProj) {
                CEEmpowerGlobalProjectile parentGlobal = parentProj.GetGlobalProjectile<CEEmpowerGlobalProjectile>();
                FromWeaponUse = parentGlobal.FromWeaponUse;
                if (parentGlobal.sourceItem != null) {
                    sourceItem = parentGlobal.sourceItem;
                    creditBlocked = parentGlobal.Empowered || parentGlobal.creditBlocked;
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
            if (Empowered || creditBlocked || sourceItem == null)
                return;
            CEChargeWeapon.CreditHit(Main.player[projectile.owner], sourceItem);
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
            bitWriter.WriteBit(Empowered);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
            Empowered = bitReader.ReadBit();
        }
    }

    public static class CEEmpowerExtensions
    {
        public static bool IsEmpowered(this Projectile projectile)
            => projectile.GetGlobalProjectile<CEEmpowerGlobalProjectile>().Empowered;

        public static bool IsFromWeaponUse(this Projectile projectile)
            => projectile.GetGlobalProjectile<CEEmpowerGlobalProjectile>().FromWeaponUse;

        /// <summary>sync 时补包,窗口打标不用</summary>
        public static void SetEmpowered(this Projectile projectile, bool sync = true) {
            projectile.GetGlobalProjectile<CEEmpowerGlobalProjectile>().Empowered = true;
            if (sync)
                CEUtils.SyncProj(projectile);
        }
    }
}
