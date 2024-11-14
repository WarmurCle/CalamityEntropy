using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using CalamityMod.World;
using CalamityMod.Particles;
using CalamityMod.Items.Potions;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.GameContent;
using CalamityEntropy.Projectiles.Pets.Signus;
namespace CalamityEntropy.NPCs.AbyssalWraith
{
    [AutoloadBossHead]
    public class AbyssalWraith : ModNPC
    {

        public int animation = 0;
        public int escape = 0;
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.checkNPC.Add(NPC);
        }
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.65f,
                PortraitScale = 0.7f,
                CustomTexturePath = "CalamityEntropy/Extra/CruiserBes",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.AbyssalWraithBestiary")
            });
        }
        public override void SetDefaults()
        {

            NPC.boss = true;
            NPC.width = 140;
            NPC.height = 140;
            NPC.damage = 136;
            if (Main.expertMode){
                NPC.damage += 20;   
            }if (Main.masterMode){
                NPC.damage += 20;
            }
            NPC.defense = 44;
            NPC.lifeMax = 1100000;
            if (CalamityWorld.death)
            {
                NPC.damage += 20;
            }
            else if (CalamityWorld.revenge)
            {
                NPC.damage += 20;
            }
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCHit4;
            NPC.value = 200000f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.Entropy().VoidTouchDR = 0.55f;
            NPC.dontCountMe = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/boss2");
            }
        }

        
        public override void SendExtraAI(BinaryWriter writer)
        {
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
        }
        public override void AI()
        {
            spawnParticle();
            NPC.target = NPC.FindClosestPlayer();
            if (NPC.HasValidTarget)
            {
                escape = 0;
                stayAtPlayerUp();
            }
            else
            {
                escape++;
                NPC.velocity.Y -= 1;
                NPC.velocity *= 0.98f;
                animation = 0;
                if (escape >= 160)
                {
                    NPC.active = false;
                }
            }

            NPC.netUpdate = true;
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public void stayAtPlayerUp()
        {
            Player target = NPC.target.ToPlayer();
            Vector2 pos = target.Center - new Vector2(0, 120);
            if (Util.Util.getDistance(NPC.Center, pos) > 240 || NPC.velocity.Length() < 4)
            {
                NPC.velocity += (pos - NPC.Center).SafeNormalize(Vector2.Zero) * 0.4f;
                NPC.rotation = MathHelper.ToRadians(NPC.velocity.X * 1.4f);
                NPC.velocity *= 0.99f;
            }
        }
        public void spawnParticle()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 direction = new Vector2(0, 1).RotatedBy(NPC.rotation);
                Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.1f) * Main.rand.NextFloat(10f, 30f) * 0.9f;
                CalamityMod.Particles.Particle smoke = new HeavySmokeParticle(NPC.Center + direction * 46f, smokeSpeed + NPC.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.NextBool(3))
                {
                    CalamityMod.Particles.Particle smokeGlow = new HeavySmokeParticle(NPC.Center + direction * 46f, smokeSpeed + NPC.velocity, Main.hslToRgb(0.85f, 1, 0.8f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0.01f, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }

        }
        public override void OnKill()
        {

            NPC.SetEventFlagCleared(ref EDownedBosses.downedAbyssalWraith, -1);
        }

        public override bool CheckActive()
        {
            return false;
        }
        
        public void Draw()
        {
            Vector2 drawCenter = NPC.Center - new Vector2(0, 30);
            Texture2D body = TextureAssets.Npc[NPC.type].Value;
            SpriteBatch sb = Main.spriteBatch;
            sb.Draw(body, drawCenter - Main.screenPosition, null, Color.White, NPC.rotation, body.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPosition, Color drawColor)
        {

            if (NPC.IsABestiaryIconDummy)
                return true;
            return false;
        }

        
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }


    }
}
