using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs
{
    public class TopazJewel : ModNPC
    {
        private const int BoltShootGateValue = 30;
        private const int BoltShootGateValue_Death = 24;
        private const int BoltShootGateValue_BossRush = 18;
        private const float LightTelegraphDuration = 45f;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 8;
            NPC.width = 22;
            NPC.height = 22;
            NPC.defense = 10;
            NPC.DR_NERD(0.1f);

            NPC.lifeMax = 120;
            double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);

            NPC.knockBackResist = 0.8f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.Calamity().VulnerableToSickness = false;
        }
        public int shootingTime = 0;
        public override void AI()
        {
            shootingTime--;
            if (shootingTime < -120)
            {
                shootingTime = 200;
            }
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Lighting.AddLight(NPC.Center, 0.8f, 0f, 0f);

            NPC.rotation = NPC.velocity.X / 15f;

            NPC.TargetClosest();

            float velocity = 0.2f;
            float acceleration = 0.03f;

            Vector2 targetPos = NPC.target.ToPlayer().Center + new Vector2(0, -280);
            if (NPC.Distance(targetPos) > 200)
            {
                NPC.velocity += (targetPos - NPC.Center).normalize() * velocity;
                NPC.velocity *= (1 - acceleration);
            }
            NPC.velocity += (targetPos - NPC.Center).normalize() * velocity * 0.1f;
            if (shootingTime >= 0 || !(NPC.ai[0] == 0f))
            {
                NPC.ai[0] += 1.6f;
            }
            if (NPC.ai[0] >= (BossRushEvent.BossRushActive ? BoltShootGateValue_BossRush : CalamityWorld.death ? BoltShootGateValue_Death : BoltShootGateValue))
            {
                NPC.ai[0] = 0f;

                Vector2 npcPos = NPC.Center;
                float xDist = Main.player[NPC.target].Center.X - npcPos.X;
                float yDist = Main.player[NPC.target].Center.Y - npcPos.Y;
                Vector2 projVector = CEUtils.randomPointInCircle(2) + new Vector2(float.Min((targetPos.X - NPC.Center.X), 400) * 0.01f, -8);

                float speed = Main.masterMode ? 6f : 4;
                int type = ModContent.ProjectileType<TopazJewelProjectile>();


                for (int dusty = 0; dusty < 10; dusty++)
                {
                    Vector2 dustVel = projVector;
                    dustVel.Normalize();
                    int ruby = Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.GemRuby, dustVel.X, dustVel.Y, 100, default, 2f);
                    Main.dust[ruby].velocity *= 1.5f;
                    Main.dust[ruby].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[ruby].scale = 0.5f;
                        Main.dust[ruby].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = NPC.damage / 2;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), npcPos, projVector, type, damage, 0f, Main.myPlayer);
                }

                NPC.netUpdate = true;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color finalColor = new Color(255, 255, 125);

            return finalColor;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemRuby, hit.HitDirection, -1f, 0, default, 1f);
            Main.dust[dust].noGravity = true;

            if (NPC.life <= 0)
            {
                NPC.position = NPC.Center;
                NPC.width = NPC.height = 45;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 2; i++)
                {
                    int rubyDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemRuby, 0f, 0f, 100, default, 2f);
                    Main.dust[rubyDust].velocity *= 3f;
                    Main.dust[rubyDust].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[rubyDust].scale = 0.5f;
                        Main.dust[rubyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 10; j++)
                {
                    int rubyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemRuby, 0f, 0f, 100, default, 3f);
                    Main.dust[rubyDust2].noGravity = true;
                    Main.dust[rubyDust2].velocity *= 5f;
                    rubyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemRuby, 0f, 0f, 100, default, 2f);
                    Main.dust[rubyDust2].noGravity = true;
                    Main.dust[rubyDust2].velocity *= 2f;
                }
            }
        }

    }


    public class TopazJewelProjectile : ModProjectile
    {
        public override string Texture => new JewelProjectile().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[base.Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[base.Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            base.Projectile.width = 14;
            base.Projectile.height = 14;
            base.Projectile.penetrate = -1;
            base.Projectile.hostile = true;
            Projectile.MaxUpdates = 2;
        }
        public float grv = 0;
        public override void AI()
        {
            base.Projectile.rotation += 0.3f * (float)base.Projectile.direction;
            for (int i = 0; i < 2; i++)
            {
                int num = Dust.NewDust(base.Projectile.position, base.Projectile.width, base.Projectile.height, DustID.GemTopaz, base.Projectile.velocity.X, base.Projectile.velocity.Y, 255, default(Color), 1.2f);
                Dust obj = Main.dust[num];
                obj.noGravity = true;
                obj.velocity *= 0.3f;
            }
            Projectile.velocity.Y += grv;
            grv += 0.003f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(in SoundID.Dig, base.Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                int num = Dust.NewDust(base.Projectile.position, base.Projectile.width, base.Projectile.height, 90, base.Projectile.oldVelocity.X, base.Projectile.oldVelocity.Y, 50, default(Color), 1.2f);
                Dust obj = Main.dust[num];
                obj.noGravity = true;
                obj.scale *= 1.25f;
                obj.velocity *= 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
