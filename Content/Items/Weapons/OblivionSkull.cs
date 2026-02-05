using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles.LuminarisShoots;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class OblivionSkull : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Summon;
            Item.width = 30;
            Item.height = 46;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 6;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<OblivionSkullMinion>();
            Item.shootSpeed = 2f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.noMelee = true;
            Item.mana = 8;
            Item.buffType = ModContent.BuffType<OblivionSkullBuff>();
            Item.rare = ItemRarityID.Orange;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 3);
            int projectile = Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, Item.damage, knockback, player.whoAmI, 0, 1, 0);
            Main.projectile[projectile].originalDamage = Item.damage;
            return false;
        }
    }
    public class OblivionSkullBuff : BaseMinionBuff
    {
        public override int ProjType => ModContent.ProjectileType<OblivionSkullMinion>();
    }
    public class OblivionSkullMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.minionSlots = 2;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            Projectile.MinionCheck<OblivionSkullBuff>();
            if (Projectile.localAI[0]++ < 3)
            {
                Projectile.timeLeft++;
                return;
            }
            if (Projectile.Distance(player.Center) > 4000)
            {
                Projectile.Center = player.Center + CEUtils.randomPointInCircle(128);
            }
            Projectile.pushByOther(0.36f);
            player.Calamity().mouseWorldListener = true;
            float speedMult = 0.95f;
            NPC target = Projectile.FindMinionTarget();
            float targetRot = Projectile.rotation;
            maxProj = 6;
            if (target != null)
            {
                maxProj = int.Min(6, (int)((target.life + target.defense * 6f) / (1 - target.Calamity().DR) / Projectile.damage) + 1);
                targetRot = (target.Center - Projectile.Center).ToRotation();
                if(delay-- < 0)
                {
                    speedMult = 0.85f;
                    ChargeCounter++;
                    if (projCharge < maxProj && ChargeCounter % chargeTime == 0)
                    {
                        projCharge++;
                        CEUtils.PlaySound("YharonFireball1", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
                        Vector2 pos = ((projCharge - 1) * (MathHelper.TwoPi / maxProj)).ToRotationVector2() * 60 + Projectile.Center;
                        for(int i = 0; i < 9; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(pos + CEUtils.randomPointInCircle(3), CEUtils.randomPointInCircle(6), Color.MediumPurple, 18, Main.rand.NextFloat(0.26f, 0.36f), 0.6f, Main.rand.NextFloat(-0.1f, 0.1f), true));
                        }
                    }
                    foreach (Vector2 vec in GetProjOffsets())
                    {
                        Vector2 pos = Projectile.Center + vec;
                        CEUtils.AddLight(pos, new Color(230, 255, 255), 0.4f);
                    }
                    if (ChargeCounter > (maxProj * 1.35f) * chargeTime)
                    {
                        ChargeCounter = 0;
                        Fire(target.Center);
                        projCharge = 0;
                        delay = 30;
                    }
                }
            }
            else
            {
                targetRot = (player.Calamity().mouseWorld - Projectile.Center).ToRotation();
                ChargeCounter = 0;
                projCharge = 0;
            }
            Vector2 tpos = player.Center + new Vector2(0, -120);
            float dist = CEUtils.getDistance(Projectile.Center, tpos);
            float num = Utils.Remap(dist, 0, 1000, 0, 10) * speedMult;
            if(CEUtils.getDistance(Projectile.Center, tpos) > 100)
                Projectile.velocity *= 0.97f;
            Projectile.velocity += (tpos - Projectile.Center).normalize() * num;
            Projectile.velocity *= speedMult;
            Projectile.rotation = CEUtils.RotateTowardsAngle(Projectile.rotation, targetRot, 0.08f, false);
        }
        public void Fire(Vector2 targetPos)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath25, Projectile.Center);
            int projType = ModContent.ProjectileType<SkullProj>();
            foreach(Vector2 vec in GetProjOffsets())
            {
                Vector2 pos = Projectile.Center + vec;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, (targetPos - pos).normalize() * 8, projType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                for (int i = 0; i < 9; i++)
                {
                    GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(pos + CEUtils.randomPointInCircle(3), CEUtils.randomPointInCircle(6), Color.MediumPurple, 18, Main.rand.NextFloat(0.26f, 0.36f), 0.6f, Main.rand.NextFloat(-0.1f, 0.1f), true));
                }
            }
        }
        public IEnumerable<Vector2> GetProjOffsets()
        {
            for (int i = 0; i < projCharge; i++)
            {
                yield return (i * (MathHelper.TwoPi / maxProj)).ToRotationVector2() * 60;
            }
        }
        public int projCharge = 0;
        public int ChargeCounter = 0;
        public int delay = 30;
        public int maxProj = 6;
        public int chargeTime = 12;
        public void DrawSkull(Vector2 pos)
        {
            Texture2D tex = CEUtils.RequestTex("CalamityEntropy/Content/Items/Weapons/SkullProj");
            Main.EntitySpriteDraw(tex, pos - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2, Projectile.scale, Projectile.rotation.ToRotationVector2().X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D projTex = CEUtils.RequestTex("CalamityEntropy/Content/Items/Weapons/SkullProj");

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, Projectile.rotation.ToRotationVector2().X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            foreach (Vector2 vec in GetProjOffsets())
            {
                Vector2 pos = Projectile.Center + vec;
                DrawSkull(pos);
            }
            return false;
        }
    }
    public class SkullProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, 1);
            Projectile.width = Projectile.height = 26;
            Projectile.MaxUpdates = 4;
            Projectile.light = 0.42f;
            Projectile.timeLeft = 600;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 16;
        }
        public NPC target;
        public override void AI()
        {
            if (!Main.dedServ)
            {
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Projectile.Center + CEUtils.randomPointInCircle(3), Vector2.Zero, Color.MediumPurple * 0.36f, 18, Main.rand.NextFloat(0.35f, 0.4f), 0.6f, Main.rand.NextFloat(-0.1f, 0.1f), true));
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(target == null || !target.active)
            {
                target = Projectile.FindMinionTarget();
            }
            if(target != null)
            {
                Projectile.velocity = CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 0.02f).ToRotationVector2() * Projectile.velocity.Length();
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath3, Projectile.Center);
            for (int i = 0; i < 12; i++)
            {
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Projectile.Center + CEUtils.randomPointInCircle(6), CEUtils.randomPointInCircle(12), Color.MediumPurple, 32, Main.rand.NextFloat(0.4f, 0.6f), 0.6f, Main.rand.NextFloat(-0.05f, 0.05f), true));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, Projectile.rotation.ToRotationVector2().X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            return false;
        }
    }
}
