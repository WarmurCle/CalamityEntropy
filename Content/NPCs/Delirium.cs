using CalamityEntropy.Content.NPCs.NihilityTwin;
using CalamityEntropy.Util;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Content.NPCs
{
    public class Delirium : ModNPC
    {
        public static List<int> npcTurns = new List<int>()
        {
            NPCID.KingSlime,
            ModContent.NPCType<GiantClam>(),
            NPCID.EyeofCthulhu,
            ModContent.NPCType<Crabulon>(),
            NPCID.BrainofCthulhu,
            ModContent.NPCType<HiveMind>(),
            ModContent.NPCType<PerforatorHive>(),
            ModContent.NPCType<Crabulon>(),
            NPCID.QueenBee,
            NPCID.SkeletronHead,
            NPCID.Deerclops,
            NPCID.QueenSlimeBoss,
            ModContent.NPCType<Cryogen>(),
            NPCID.Retinazer,
            NPCID.Spazmatism,
            ModContent.NPCType<BrimstoneElemental>(),
            ModContent.NPCType<CalamitasClone>(),
            NPCID.Plantera,
            ModContent.NPCType<AstrumAureus>(),
            NPCID.Golem,
            NPCID.DukeFishron,
            NPCID.HallowBoss,
            NPCID.CultistBoss,
            ModContent.NPCType<RavagerBody>(),
            NPCID.MoonLordCore,
            ModContent.NPCType<Bumblefuck>(),
            ModContent.NPCType<Providence>(),
            ModContent.NPCType<CeaselessVoid>(),
            ModContent.NPCType<Signus>(),
            ModContent.NPCType<Polterghast>(),
            ModContent.NPCType<OldDuke>(),
            ModContent.NPCType<Yharon>(),
            ModContent.NPCType<PrimordialWyrmHead>(),
            ModContent.NPCType<SupremeCalamitas>(),
            ModContent.NPCType<NihilityActeriophage>()
        };
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.damage = 300;
            NPC.lifeMax = 3400000;

        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.netUpdate = true;
            NPC.netSpam = 0;
            int npc = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, npcTurns[Main.rand.Next(npcTurns.Count)]);
            NPC spawn = npc.ToNPC();
            spawn.Center = NPC.Center;
            spawn.lifeMax = NPC.lifeMax;
            spawn.life = NPC.life;
            spawn.damage = NPC.damage;
            spawn.GetGlobalNPC<DeliriumGlobalNPC>().delirium = true;
            spawn.GetGlobalNPC<DeliriumGlobalNPC>().damage = NPC.damage;
            spawn.GetGlobalNPC<DeliriumGlobalNPC>().counter = 180;

            spawn.netUpdate = true;
            spawn.netSpam = 0;
            NPC.active = false;
        }
    }

    public class DeliriumGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool delirium = false;
        public int counter = 0;
        public int damage = 0;
        public override GlobalNPC Clone(NPC from, NPC to)
        {
            var n = to.GetGlobalNPC<DeliriumGlobalNPC>();
            n.delirium = delirium;
            n.counter = counter;
            return n;
        }
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(delirium);
            binaryWriter.Write(counter);
            binaryWriter.Write(npc.lifeMax);
            binaryWriter.Write(npc.life);
            binaryWriter.Write(npc.damage);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            delirium = binaryReader.ReadBoolean();
            counter = binaryReader.ReadInt32();
            npc.lifeMax = binaryReader.ReadInt32();
            npc.life = binaryReader.ReadInt32();
            npc.damage = binaryReader.ReadInt32();
        }

        public override bool CheckActive(NPC npc)
        {
            return !delirium;
        }
    }
}
