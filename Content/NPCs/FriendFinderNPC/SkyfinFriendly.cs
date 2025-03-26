using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.FriendFinderNPC
{
    public class SkyfinFriendly : FriendFindNPC
    {
        public ref float AttackState => ref NPC.ai[0];
        public ref float AttackTimer => ref NPC.ai[1];
        public Entity _target = null;
        public Entity Target => (_target == null ? this.FindTarget() : _target);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 22;
            NPC.aiStyle = AIType = -1;

            NPC.damage = 80;
            NPC.lifeMax = 500;
            NPC.defense = 8;
            NPC.knockBackResist = 0.3f;


            NPC.value = Item.buyPrice(0, 0, 3, 65);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            NPC.friendly = true;
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            _target = this.FindTarget();
            int idealDirection = (NPC.velocity.X > 0).ToDirectionInt();
            NPC.spriteDirection = idealDirection;
            this.applyCollisionDamage();
            switch ((int)AttackState)
            {
                case 0:
                    Vector2 flyDestination = Target.Center + new Vector2((Target.Center.X < NPC.Center.X).ToDirectionInt() * 400f, -240f);
                    Vector2 idealVelocity = NPC.SafeDirectionTo(flyDestination) * 10f;
                    NPC.velocity = (NPC.velocity * 29f + idealVelocity) / 29f;
                    NPC.velocity = NPC.velocity.MoveTowards(idealVelocity, 1.5f);

                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection > 0).ToInt() * MathHelper.Pi;

                    if (NPC.WithinRange(flyDestination, 40f) || AttackTimer > 150f)
                    {
                        AttackState = 1f;
                        NPC.velocity *= 0.65f;
                        NPC.netUpdate = true;
                    }
                    break;

                case 1:
                    NPC.spriteDirection = (Target.Center.X > NPC.Center.X).ToDirectionInt();
                    NPC.velocity *= 0.97f;
                    NPC.velocity = NPC.velocity.MoveTowards(Vector2.Zero, 0.25f);
                    NPC.rotation = NPC.rotation.AngleTowards(NPC.AngleTo(Target.Center) + (NPC.spriteDirection > 0).ToInt() * MathHelper.Pi, 0.2f);

                    float chargeSpeed = 11.5f;
                    if (DownedBossSystem.downedAquaticScourge)
                        chargeSpeed += 4f;
                    if (DownedBossSystem.downedPolterghast)
                        chargeSpeed += 3.5f;
                    if (NPC.velocity.Length() < 1.25f)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.Center);
                        for (int i = 0; i < 36; i++)
                        {
                            Dust acid = Dust.NewDustPerfect(NPC.Center, (int)CalamityDusts.SulphurousSeaAcid);
                            acid.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 6f;
                            acid.scale = 1.1f;
                            acid.noGravity = true;
                        }

                        AttackState = 2f;
                        AttackTimer = 0f;
                        NPC.velocity = NPC.SafeDirectionTo(Target.Center) * chargeSpeed;
                        NPC.netUpdate = true;
                    }
                    break;

                case 2:
                    float angularTurnSpeed = MathHelper.Pi / 300f;
                    idealVelocity = NPC.SafeDirectionTo(Target.Center);
                    Vector2 leftVelocity = NPC.velocity.RotatedBy(-angularTurnSpeed);
                    Vector2 rightVelocity = NPC.velocity.RotatedBy(angularTurnSpeed);
                    if (leftVelocity.AngleBetween(idealVelocity) < rightVelocity.AngleBetween(idealVelocity))
                        NPC.velocity = leftVelocity;
                    else
                        NPC.velocity = rightVelocity;

                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.spriteDirection > 0).ToInt() * MathHelper.Pi;

                    if (AttackTimer > 50f)
                    {
                        AttackState = 0f;
                        AttackTimer = 0f;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY * 8f, 0.14f);
                        NPC.netUpdate = true;
                    }
                    break;
            }
            AttackTimer++;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulphuricScale>(), 2, 1, 3);
            LeadingConditionRule postAS = npcLoot.DefineConditionalDropSet(DropHelper.PostAS());
            postAS.Add(ModContent.ItemType<SkyfinBombers>(), 20);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulphurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulphurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
