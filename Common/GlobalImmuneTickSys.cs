using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Utilities;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class GlobalImmuneTickSysGNPC : GlobalNPC
    {
        public static List<int> NPCHasGlobalImmuneTick;
        public override void SetStaticDefaults()
        {
            NPCHasGlobalImmuneTick = new List<int>() { ModContent.NPCType<CruiserHead>(), ModContent.NPCType<CruiserBody>(), ModContent.NPCType<CruiserTail>() };
        }
        public override void Unload()
        {
            NPCHasGlobalImmuneTick = null;
        }
        public override bool InstancePerEntity => true;
        public int immune = 0;
        public void SyncImmuneTick(NPC NPC, Projectile proj, int player)
        {
            NPC hitted = NPC;
            if (NPC.realLife == -1)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.realLife == NPC.whoAmI)
                    {
                        SyncImmuneTickWithNPC(proj, hitted, n, player);
                    }
                }
            }
            else
            {
                SyncImmuneTick(NPC.realLife.ToNPC(), proj, player);
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPCHasGlobalImmuneTick.Contains(npc.type))
            {
                SyncImmuneTick(npc, projectile, projectile.owner);
            }
        }
        public void SyncShieldDashImmune(NPC NPC, int plr)
        {
            if (NPCHasGlobalImmuneTick.Contains(NPC.type))
            {
                if (NPC.realLife == -1)
                {
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.realLife == NPC.whoAmI && NPC.Calamity().dashImmunityTime[plr] > n.Calamity().dashImmunityTime[plr])
                        {
                            n.Calamity().dashImmunityTime[plr] = NPC.Calamity().dashImmunityTime[plr];
                        }
                    }
                }
                else
                {
                    if (NPC.realLife.ToNPC().Calamity().dashImmunityTime[plr] < NPC.Calamity().dashImmunityTime[plr])
                    {
                        NPC.realLife.ToNPC().Calamity().dashImmunityTime[plr] = NPC.Calamity().dashImmunityTime[plr];
                    }
                    SyncShieldDashImmune(NPC.realLife.ToNPC(), plr);
                }
            }
        }
        public override void OnHitByItem(NPC NPC, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPCHasGlobalImmuneTick.Contains(NPC.type))
            {
                NPC.gimmune().immune = player.itemTime;
                if (NPC.realLife == -1)
                {
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.realLife == NPC.whoAmI)
                        {
                            n.gimmune().immune = player.itemTime;
                        }
                    }
                }
                else
                {
                    OnHitByItem(NPC.realLife.ToNPC(), player, item, hit, damageDone);
                }
            }
        }
        public void SyncImmuneTickWithNPC(Projectile proj, NPC NPC, NPC sync, int player)
        {
            if (proj.usesLocalNPCImmunity)
            {
                if (proj.localNPCHitCooldown == -1)
                {
                    proj.gimmune().NPCImmune[NPC.whoAmI] = proj.gimmune().NPCImmune[sync.whoAmI] = -1;
                }
                else
                {
                    if (proj.localNPCHitCooldown > 0)
                    {
                        proj.gimmune().NPCImmune[NPC.whoAmI] = proj.gimmune().NPCImmune[sync.whoAmI] = proj.localNPCHitCooldown;
                    }
                }
            }
            else if (proj.usesIDStaticNPCImmunity)
            {
                GlobalImmuneTickSysGProj.IDStaticImmune[proj.type, NPC.whoAmI] = GlobalImmuneTickSysGProj.IDStaticImmune[proj.type, sync.whoAmI] = proj.idStaticNPCHitCooldown;
            }
            else
            {
                if (proj.penetrate != 0)
                {
                    NPC.gimmune().immune = sync.gimmune().immune = 10;
                }
            }
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (projectile.penetrate == 1 && !projectile.usesIDStaticNPCImmunity) { return null; }

            if (immune != 0 && !(projectile.usesLocalNPCImmunity || projectile.usesIDStaticNPCImmunity))
            {
                return false;
            }
            if (GlobalImmuneTickSysGProj.IDStaticImmune[projectile.whoAmI, npc.whoAmI] != 0)
            {
                return false;
            }
            if (projectile.gimmune().NPCImmune[npc.whoAmI] != 0)
            {
                return false;
            }

            return null;
        }
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (immune != 0)
            {
                return false;
            }
            return base.CanBeHitByItem(npc, player, item);
        }
        public override void AI(NPC npc)
        {
            if (immune > 0)
            {
                immune--;
            }
            if (readySyncDashImmune)
            {
                readySyncDashImmune = false;
                SyncShieldDashImmune(npc, sdPlayer.whoAmI);
            }
        }
        public bool readySyncDashImmune = false;
        public Player sdPlayer = null;

    }

    public class GlobalImmuneTickSysGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int[] NPCImmune = new int[Main.npc.Length];
        public static int[,] IDStaticImmune = new int[ProjectileLoader.ProjectileCount, Main.npc.Length];
        public override bool PreAI(Projectile projectile)
        {
            for(int i = 0; i < NPCImmune.Length; i++)
            {
                if (NPCImmune[i] > 0)
                    NPCImmune[i]--;
            }
            return true;
        }
    }
    public class GlobalImmuneTickSys : ModSystem
    {
        public override void PostUpdateProjectiles()
        {
            for(int i = 0; i < ProjectileLoader.ProjectileCount; i++)
            {
                for(int j = 0; j < Main.npc.Length; j++)
                {
                    if (GlobalImmuneTickSysGProj.IDStaticImmune[i, j] > 0)
                    {
                        GlobalImmuneTickSysGProj.IDStaticImmune[i, j] --;
                    }
                }
            }
        }
    }
    public static class GUtil
    {
        public static GlobalImmuneTickSysGNPC gimmune(this NPC n) => n.GetGlobalNPC<GlobalImmuneTickSysGNPC>();
        public static GlobalImmuneTickSysGProj gimmune(this Projectile n) => n.GetGlobalProjectile<GlobalImmuneTickSysGProj>();

    }
}
