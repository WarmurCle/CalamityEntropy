using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.FriendFinderNPC
{
    public class ScryllarFriendly : FriendFindNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 50;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 18;
            NPC.lifeMax = 800;
            NPC.alpha = 160;
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            NPC.friendly = true;
        }

        public override void AI()
        {
            foreach(NPC npc in Main.ActiveNPCs)
            {
                if(npc.type == NPC.type && npc.whoAmI != NPC.whoAmI && npc.getRect().Intersects(NPC.getRect()))
                {
                    NPC.velocity += (NPC.Center - npc.Center).SafeNormalize(Vector2.UnitX) * 0.2f;
                }
            }
            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            Entity target = this.FindTarget();
            NPC.direction = target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.damage = 50;
            
            if(target is NPC)
            {
                NPC.ai[1]--;
                if (NPC.ai[1] < -160)
                {
                    NPC.ai[1] = 46;
                    NPC.ai[2] = (target.Center - NPC.Center).ToRotation();
                    NPC.netUpdate = true;
                    NPC.velocity -= NPC.ai[2].ToRotationVector2() * 6;
                }
                if (NPC.ai[1] < 0)
                {
                    NPC.velocity += new Vector2((float)MathHelper.Max(0, (target.Center - NPC.Center).Length()) - 100, 0).RotatedBy((target.Center - NPC.Center).ToRotation()) * 0.0014f;
                    NPC.velocity *= 0.94f;
                }
                else
                {
                    NPC.damage = 360;
                    if (NPC.ai[1] == 30)
                    {
                        NPC.ai[2] = (target.Center + target.velocity * 5 - NPC.Center).ToRotation();
                    }
                    if (NPC.ai[1] < 26)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            EParticle.spawnNew(new Smoke() { timeleftmax = 30, timeLeft = 30}, NPC.Center + Util.Util.randomVec(8), Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(0, 0.6f) + new Vector2(0, -1.2f), Color.OrangeRed, 0.16f, 0.14f, true, Microsoft.Xna.Framework.Graphics.BlendState.Additive);
                        }
                    }
                    if (NPC.ai[1] < 30)
                    {
                        NPC.velocity += NPC.ai[2].ToRotationVector2() * 3.6f;
                    }
                    NPC.velocity *= 0.98f;
                }
            }
            else
            {
                NPC.ai[1] = 0;
                if (Util.Util.getDistance(NPC.Center, target.Center) > 160)
                {
                    NPC.velocity += (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.6f;
                }
                NPC.velocity *= 0.98f;
            }
            this.applyCollisionDamage();
            NPC.velocity *= 0.994f;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.GetInstance<CalamityMod.CalamityMod>().Find<ModGore>("Scryllar").Type, NPC.scale);
                }
            }
        }
    }
}
