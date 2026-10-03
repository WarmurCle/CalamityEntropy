using CalamityEntropy.Content.NPCs.VoidDestroyer;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Particles.CalamityPorts;
using CalamityEntropy.Content.Projectiles.VoidDestroyer;
using CalamityEntropy.Content.Rarities;
using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.VoidDestroyer
{
    /// <summary>
    /// 物质解离矛:虚空驱逐舰掉落的长矛。
    /// 左键戳击(收-爆-停),命中后在接触点引发暗影爆炸(<see cref="BurstMult"/>)并从伤口弹出 3 发慢速追踪虚空弹(<see cref="BoltMult"/>);
    /// 右键蓄势投掷,飞行途中每隔 <see cref="MatterDissociationSpearThrow.BoltSpacing"/> 像素留下一发虚空弹(最多 7 发),
    /// 命中目标时目标下方立刻显形 4 发瞄准目标的投影三叉戟(<see cref="TridentMult"/>),随即极速贯穿。虚空弹生成 30 帧后才开始追踪
    /// </summary>
    public class MatterDissociationSpear : ModItem
    {
        public const float BoltMult = 0.10f;
        public const float BurstMult = 0.75f;
        public const float TridentMult = 0.25f;
        public const int StabUseTime = 24;
        public const int ThrowUseTime = 32;

        public override void SetDefaults() {
            Item.width = 64;
            Item.height = 64;
            Item.damage = 500;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = StabUseTime;
            Item.useAnimation = StabUseTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 6.5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(platinum: 2, gold: 50);
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.shoot = ModContent.ProjectileType<MatterDissociationSpearStab>();
            Item.shootSpeed = 1f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player) {
            int stab = ModContent.ProjectileType<MatterDissociationSpearStab>();
            int thrown = ModContent.ProjectileType<MatterDissociationSpearThrow>();
            if (player.altFunctionUse == 2) {
                Item.useTime = Item.useAnimation = ThrowUseTime;
                Item.shoot = thrown;
                //同时只允许一支掷出的矛在场,戳击途中也不能起手投掷
                return player.ownedProjectileCounts[thrown] == 0 && player.ownedProjectileCounts[stab] == 0;
            }
            Item.useTime = Item.useAnimation = StabUseTime;
            Item.shoot = stab;
            return player.ownedProjectileCounts[stab] == 0 && !MatterDissociationSpearThrow.WindingUp(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            Vector2 dir = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, player.MountedCenter, dir, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>矛贴图的几何(140×134,矛尖朝右上):沿轴握点、尖端距离与轴角,戳击与投掷共用</summary>
    public static class SpearGeometry
    {
        public const string TexturePath = "CalamityEntropy/Content/Items/Weapons/VoidDestroyer/MatterDissociationSpear";
        public static readonly Vector2 Butt = new Vector2(0f, 129f);
        public static readonly Vector2 Tip = new Vector2(139f, 0f);
        public static readonly float AxisAngle = MathF.Atan2(Tip.Y - Butt.Y, Tip.X - Butt.X);
        public static readonly float Length = Vector2.Distance(Butt, Tip);
        /// <summary>握点离矛尾的距离(下段矛杆,装饰件从 54px 起)</summary>
        public const float GripAlong = 34f;
        public static Vector2 Pivot => Butt + AxisAngle.ToRotationVector2() * GripAlong;
        /// <summary>握点到矛尖</summary>
        public static float TipFromPivot => Length - GripAlong;
        /// <summary>刃根(品红刃最低处)离握点</summary>
        public static float BladeFromPivot => 119f - GripAlong;

        public static readonly Color Blade = new Color(255, 90, 230);
    }

    /// <summary>
    /// 戳击手持弹幕。方向在出手时锁定(速度只作朝向),时间线按物品动画时长缩放:
    /// 收(0 → 25%,后撤 16px)→ 爆(25% → 35%,两帧内前送到 +50px)→ 停(35% → 60%,回落到 +44px 定住)→ 收回。
    /// 判定只在爆与停两段,线段从手一直到穿刺冲击波末端;首次命中顿帧 3 帧,每次戳击只引发一次暗影爆炸与虚空弹
    /// </summary>
    public class MatterDissociationSpearStab : ModProjectile
    {
        public const float GatherEnd = 0.25f;
        public const float BurstEnd = 0.35f;
        public const float RestEnd = 0.6f;
        public const float PullBack = 16f;
        public const float Overshoot = 50f;
        public const float Hold = 44f;
        public const float StreakLength = 80f;
        public const int StreakFrames = 10;
        public const int HitStopFrames = 3;

        public override string Texture => SpearGeometry.TexturePath;

        [VaultLoaden("CalamityEntropy/Content/Items/Weapons/VoidDestroyer/MatterDissociationSpear_Glow")]
        internal static Asset<Texture2D> GlowMask;

        private int total;
        private float timeline;
        private int hitStop;
        private bool triggered;
        private int streakAge = -1;
        private float prevExtension;
        private float extension;

        private Vector2 Dir => Projectile.velocity.SafeNormalize(Vector2.UnitX);
        private float Progress => total > 0 ? MathHelper.Clamp(timeline / total, 0f, 1f) : 0f;
        private bool Live => Progress >= GatherEnd && Progress < RestEnd;

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        /// <summary>伸出量(像素,沿方向):收 → 爆 → 停 → 收回</summary>
        public static float ExtensionAt(float p) {
            if (p < GatherEnd) {
                float k = p / GatherEnd;
                return -PullBack * k * k * (3f - 2f * k);
            }
            if (p < BurstEnd) {
                float k = (p - GatherEnd) / (BurstEnd - GatherEnd);
                return MathHelper.Lerp(-PullBack, Overshoot, 1f - MathF.Pow(1f - k, 4f));
            }
            if (p < RestEnd) {
                float k = (p - BurstEnd) / (RestEnd - BurstEnd);
                return Hold + (Overshoot - Hold) * (1f - k) * (1f - k);
            }
            float r = (p - RestEnd) / (1f - RestEnd);
            return Hold * (1f - r * r * (3f - 2f * r));
        }

        private Player.CompositeArmStretchAmount Stretch {
            get {
                float p = Progress;
                if (p < GatherEnd) {
                    return Player.CompositeArmStretchAmount.Quarter;
                }
                return p < RestEnd ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.ThreeQuarters;
            }
        }

        private Vector2 HandPos(Player owner) {
            float armRot = Dir.ToRotation() - MathHelper.PiOver2;
            return owner.GetFrontHandPosition(Stretch, armRot) + new Vector2(0f, owner.gfxOffY);
        }

        private HeldSprite Sprite(Player owner, float ext) {
            Vector2 world = HandPos(owner) + Dir * ext;
            return HeldSprite.Along(TextureAssets.Projectile[Type].Value, SpearGeometry.Pivot, SpearGeometry.AxisAngle, world, Dir.ToRotation(), Projectile.direction);
        }

        private Vector2 TipPos(Player owner) => HandPos(owner) + Dir * (extension + SpearGeometry.TipFromPivot);

        /// <summary>冲击波当前长度:爆发首 3 帧冲到全长,之后保持到淡出</summary>
        private float StreakNow => streakAge < 0 ? 0f : StreakLength * MathHelper.Clamp((streakAge + 1) / 3f, 0f, 1f);

        public override void AI() {
            Player owner = Projectile.GetOwner();
            if (!owner.active || owner.dead || owner.CCed || owner.HeldItem.type != ModContent.ItemType<MatterDissociationSpear>()) {
                Projectile.Kill();
                return;
            }
            if (total == 0) {
                //各端用同一公式算时长(远端玩家的 itemAnimationMax 不一定可靠)
                total = Math.Max(10, CombinedHooks.TotalAnimationTime(MatterDissociationSpear.StabUseTime, owner, owner.HeldItem));
                Projectile.direction = Dir.X >= 0f ? 1 : -1;
            }
            owner.ChangeDir(Projectile.direction);
            owner.heldProj = Projectile.whoAmI;
            Projectile.timeLeft = 2;

            if (hitStop > 0) {
                hitStop--;
            }
            else {
                float before = Progress;
                timeline++;
                if (before < GatherEnd && Progress >= GatherEnd) {
                    OnBurst(owner);
                }
                if (streakAge >= 0) {
                    streakAge++;
                }
            }
            if (timeline >= total) {
                Projectile.Kill();
                return;
            }
            int remain = (int)(total - timeline) + hitStop + 1;
            owner.itemTime = owner.itemAnimation = Math.Max(2, remain);

            prevExtension = extension;
            extension = ExtensionAt(Progress);
            owner.SetCompositeArmFront(true, Stretch, Dir.ToRotation() - MathHelper.PiOver2);
            owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Dir.ToRotation() - MathHelper.PiOver2 + 0.5f * Projectile.direction);
            owner.itemRotation = MathHelper.WrapAngle(Dir.ToRotation() + (Projectile.direction < 0 ? MathHelper.Pi : 0f));

            Vector2 tip = TipPos(owner);
            Projectile.Center = HandPos(owner) + Dir * (extension + SpearGeometry.TipFromPivot * 0.5f);
            Lighting.AddLight(tip, SpearGeometry.Blade.ToVector3() * (Live ? 0.7f : 0.35f));
        }

        /// <summary>爆发帧:出手音、冲击波起、前冲一小步、尖端火花</summary>
        private void OnBurst(Player owner) {
            streakAge = 0;
            if (Projectile.owner == Main.myPlayer && Math.Abs(owner.velocity.X) < 8f) {
                owner.velocity.X += Dir.X * 2.2f;
            }
            if (Main.dedServ) {
                return;
            }
            Vector2 tip = HandPos(owner) + Dir * (Overshoot + SpearGeometry.TipFromPivot);
            CEUtils.PlaySound("sf_spear_attak", Main.rand.NextFloat(0.95f, 1.1f), tip, 6, 0.75f * CEUtils.WeapSound);
            for (int i = 0; i < 7; i++) {
                VDVfx.Spark(tip, Dir.RotatedByRandom(0.18f) * Main.rand.NextFloat(10f, 22f), i % 2 == 0 ? Color.White : SpearGeometry.Blade, Main.rand.NextFloat(0.4f, 0.65f), 1f, Main.rand.Next(6, 10));
            }
            PRTLoader.NewParticle<PRT_DirectionalPulseRing>(tip, Dir * 2f, SpearGeometry.Blade, 0.1f).Configure(new Vector2(0.45f, 1f), Dir.ToRotation(), 0.6f, 12);
        }

        public override bool? CanDamage() => Live ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Player owner = Projectile.GetOwner();
            Vector2 tip = TipPos(owner);
            return CEUtils.LineThroughRect(HandPos(owner), tip + Dir * StreakNow, targetHitbox, 28);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            modifiers.HitDirectionOverride = Projectile.direction;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Player owner = Projectile.GetOwner();
            Vector2 hand = HandPos(owner);
            //接触点:目标中心投影到戳击线上,再夹进目标判定框
            Vector2 onLine = hand + Dir * Vector2.Dot(target.Center - hand, Dir);
            Vector2 contact = Vector2.Clamp(onLine, target.TopLeft, target.BottomRight);

            CEUtils.PlaySound("spearImpact", Main.rand.NextFloat(0.9f, 1.05f), contact, 6, 0.8f * CEUtils.WeapSound);
            for (int i = 0; i < 10; i++) {
                VDVfx.Spark(contact, Dir.RotatedByRandom(0.35f) * Main.rand.NextFloat(8f, 20f), i % 3 == 0 ? Color.White : SpearGeometry.Blade, Main.rand.NextFloat(0.45f, 0.8f), 1f, Main.rand.Next(10, 16), gravity: true);
            }
            if (triggered) {
                return;
            }
            triggered = true;
            hitStop = HitStopFrames;
            CEUtils.SetShake(contact - Dir * 40f, 4f, 1600f);

            IEntitySource src = Projectile.GetSource_FromThis();
            int burstDamage = (int)(Projectile.damage * MatterDissociationSpear.BurstMult);
            Projectile.NewProjectile(src, contact, Vector2.Zero, ModContent.ProjectileType<MatterDissociationBurst>(), burstDamage, Projectile.knockBack, Projectile.owner);
            int boltDamage = Math.Max(1, (int)(Projectile.damage * MatterDissociationSpear.BoltMult));
            for (int i = -1; i <= 1; i++) {
                Vector2 v = (-Dir).RotatedBy(i * 0.85f) * Main.rand.NextFloat(5.5f, 7f);
                Projectile.NewProjectile(src, contact, v, ModContent.ProjectileType<MatterDissociationBolt>(), boltDamage, Projectile.knockBack * 0.3f, Projectile.owner, target.whoAmI);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Player owner = Projectile.GetOwner();
            if (total == 0) {
                return false;
            }
            Texture2D glow = GlowMask.Value;
            HeldSprite spear = Sprite(owner, extension);
            Vector2 tip = TipPos(owner);

            //爆发这几帧沿轴拖两道虚影(直线运动的拖影叠在同一条线上,读作模糊而不是扇面)
            float travel = extension - prevExtension;
            if (travel > 6f) {
                VDWeaponFx.BeginAdditive();
                for (int i = 1; i <= 2; i++) {
                    HeldSprite ghost = Sprite(owner, extension - travel * i / 3f);
                    ghost.Draw(glow, SpearGeometry.Blade * (0.45f - i * 0.12f));
                }
                VDWeaponFx.End();
            }

            spear.Draw(lightColor);
            spear.Draw(glow, Color.White);

            VDWeaponFx.BeginAdditive();
            float hot = Live ? 1f : 0.35f;
            VDWeaponFx.Glow(tip, SpearGeometry.Blade * (0.5f * hot), 22f);
            VDWeaponFx.Glow(tip, Color.White * (0.4f * hot), 8f);
            VDWeaponFx.End();

            if (streakAge >= 0 && streakAge < StreakFrames) {
                float life = streakAge / (float)StreakFrames;
                float fade = 1f - life * life;
                Vector2 end = tip + Dir * StreakNow;
                VDBeamDraw.DrawTapered(tip, end, 22f * fade, 3f * fade, SpearGeometry.Blade, Color.White, 1f, fade, Projectile.whoAmI * 0.31f, endGlow: false, capStart: 0f);
            }
            return false;
        }
    }

    /// <summary>
    /// 掷出的矛。ai[0] 为状态:0 蓄势(贴在手上后拉 8 帧,拥有者在第 8 帧按当前光标出手并同步),1 飞行。
    /// Projectile.Center 就是矛尖(撞墙与判定都从尖端算),飞行两次更新、20 帧后开始下坠;
    /// 每飞过 <see cref="BoltSpacing"/> 像素从矛杆中段留下一发虚空弹(最多 <see cref="MaxBolts"/> 发),命中目标时目标下方显形 4 发投影三叉戟
    /// </summary>
    public class MatterDissociationSpearThrow : ModProjectile
    {
        public const float BoltSpacing = 96f;
        public const int MaxBolts = 7;
        public const int WindUpFrames = 8;
        /// <summary>每次更新的初速(两次更新,实际每帧两倍)</summary>
        public const float LaunchSpeed = 15f;
        public const int TrailLength = 14;

        public override string Texture => SpearGeometry.TexturePath;

        private readonly List<Vector2> trail = new List<Vector2>();
        private int windUp;
        private float aim;
        /// <summary>蓄势时矛的实际朝向(光标方向略微上扬)</summary>
        private float heldAim;
        private bool flying;
        private int flightUpdates;
        private float travelled;
        private int bolts;

        public static bool WindingUp(Player player) {
            int type = ModContent.ProjectileType<MatterDissociationSpearThrow>();
            foreach (Projectile p in Main.ActiveProjectiles) {
                if (p.owner == player.whoAmI && p.type == type && p.ai[0] == 0f) {
                    return true;
                }
            }
            return false;
        }

        private Vector2 Dir => flying ? Projectile.velocity.SafeNormalize(Vector2.UnitX) : heldAim.ToRotationVector2();

        public override void SetDefaults() {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 320;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition() => flying;

        public override bool? CanDamage() => flying ? null : false;

        public override void AI() {
            Player owner = Projectile.GetOwner();
            if (Projectile.ai[0] == 0f) {
                WindUpAI(owner);
                return;
            }
            if (!flying) {
                StartFlight(owner);
            }
            FlightAI();
        }

        /// <summary>蓄势:矛贴手后拉、手臂向后上方扬起;拥有者在最后一帧按光标出手</summary>
        private void WindUpAI(Player owner) {
            if (!owner.active || owner.dead || owner.CCed) {
                Projectile.Kill();
                return;
            }
            bool isOwner = Projectile.owner == Main.myPlayer;
            if (isOwner) {
                owner.Entropy().MouseWorldListener = true;
            }
            float want = (owner.Entropy().MouseWorld - owner.MountedCenter).ToRotation();
            if (windUp == 0) {
                aim = want;
            }
            aim = CEUtils.RotateTowardsAngle(aim, want, 0.3f, false);
            windUp++;
            Projectile.timeLeft = 320;
            int facing = MathF.Cos(aim) >= 0f ? 1 : -1;
            owner.ChangeDir(facing);
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = owner.itemAnimation = Math.Max(owner.itemAnimation, 2);

            float k = MathHelper.Clamp(windUp / (float)WindUpFrames, 0f, 1f);
            float cock = k * k * (3f - 2f * k);
            float armRot = aim - facing * 1.9f * cock - MathHelper.PiOver2;
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
            Vector2 hand = owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRot) + new Vector2(0f, owner.gfxOffY);
            //握点在手上,矛尖指向光标(略微上扬)
            heldAim = aim - facing * 0.12f * cock;
            Projectile.Center = hand + heldAim.ToRotationVector2() * SpearGeometry.TipFromPivot;
            Projectile.velocity = Vector2.Zero;

            if (windUp >= WindUpFrames && isOwner) {
                Projectile.velocity = aim.ToRotationVector2() * LaunchSpeed + owner.velocity * 0.15f;
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
        }

        /// <summary>各端在看到状态切到飞行的第一帧放出手演出</summary>
        private void StartFlight(Player owner) {
            flying = true;
            trail.Clear();
            if (Main.dedServ) {
                return;
            }
            CEUtils.PlaySound("throw", Main.rand.NextFloat(0.82f, 0.92f), Projectile.Center, 4, 0.85f * CEUtils.WeapSound);
            CEUtils.PlaySound("vbdisapear", 1.5f, Projectile.Center, 3, 0.35f);
            Vector2 dir = Dir;
            for (int i = 0; i < 8; i++) {
                VDVfx.Spark(Projectile.Center, dir.RotatedByRandom(0.25f) * Main.rand.NextFloat(6f, 16f), SpearGeometry.Blade, Main.rand.NextFloat(0.4f, 0.7f), 1f, 10);
            }
        }

        private void FlightAI() {
            flightUpdates++;
            int perFrame = Projectile.extraUpdates + 1;
            if (flightUpdates > 20 * perFrame) {
                Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.1f, 12f);
            }
            Projectile.tileCollide = flightUpdates > 2;
            Vector2 dir = Dir;
            Projectile.rotation = dir.ToRotation();

            travelled += Projectile.velocity.Length();
            if (travelled >= BoltSpacing && bolts < MaxBolts) {
                travelled -= BoltSpacing;
                bolts++;
                Vector2 at = Projectile.Center - dir * (SpearGeometry.TipFromPivot * 0.5f);
                if (Projectile.owner == Main.myPlayer) {
                    int boltDamage = Math.Max(1, (int)(Projectile.damage * MatterDissociationSpear.BoltMult));
                    Vector2 drift = dir.RotatedBy(MathHelper.PiOver2 * (bolts % 2 == 0 ? 1f : -1f)) * 1.6f + dir * 1.2f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), at, drift, ModContent.ProjectileType<MatterDissociationBolt>(), boltDamage, Projectile.knockBack * 0.3f, Projectile.owner, -1f);
                }
                if (!Main.dedServ) {
                    PRTLoader.NewParticle<PRT_ShineParticle>(at, Vector2.Zero, MatterDissociationBolt.GlowColor, 0.22f).Configure(1f, true, PRTDrawModeEnum.AdditiveBlend, 0f, 10);
                    for (int i = 0; i < 4; i++) {
                        VDVfx.Spark(at, CEUtils.randomPointInCircle(3f) - dir * 2f, MatterDissociationBolt.GlowColor, Main.rand.NextFloat(0.35f, 0.55f), 1f, 14);
                    }
                }
            }

            if (flightUpdates % perFrame == 0) {
                trail.Insert(0, Projectile.Center);
                if (trail.Count > TrailLength) {
                    trail.RemoveAt(trail.Count - 1);
                }
            }
            Lighting.AddLight(Projectile.Center, SpearGeometry.Blade.ToVector3() * 0.6f);
            if (!Main.dedServ && Main.rand.NextBool(3)) {
                Vector2 on = Projectile.Center - dir * Main.rand.NextFloat(10f, SpearGeometry.BladeFromPivot + 40f);
                VDVfx.Spark(on, -dir * Main.rand.NextFloat(1f, 3f) + CEUtils.randomPointInCircle(1f), SpearGeometry.Blade, Main.rand.NextFloat(0.3f, 0.5f), 0.9f, 14);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Vector2 dir = Dir;
            return CEUtils.LineThroughRect(Projectile.Center - dir * 120f, Projectile.Center + dir * 8f, targetHitbox, 18);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            CEUtils.SetShake(target.Center, 5f, 1800f);
            int tridentDamage = Math.Max(1, (int)(Projectile.damage * MatterDissociationSpear.TridentMult));
            for (int i = 0; i < 4; i++) {
                float side = i - 1.5f;
                Vector2 spawn = new Vector2(target.Center.X + side * 46f, target.Bottom.Y + 200f + Math.Abs(side) * 22f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<MatterDissociationTrident>(), tridentDamage, Projectile.knockBack * 0.5f, Projectile.owner, target.whoAmI, i);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            CEUtils.PlaySound("metalhit", Main.rand.NextFloat(0.85f, 1f), Projectile.Center, 4, 0.55f);
            return true;
        }

        /// <summary>矛体解离:沿矛杆碎成品红碎块与虚空烟,命中与撞墙同一个收尾</summary>
        public override void OnKill(int timeLeft) {
            if (Main.dedServ || !flying) {
                return;
            }
            Vector2 dir = Dir;
            CEUtils.PlaySound("spearImpact", Main.rand.NextFloat(0.75f, 0.85f), Projectile.Center, 4, 0.8f * CEUtils.WeapSound);
            CEUtils.PlaySound("vbdisapear", 1.1f, Projectile.Center, 3, 0.5f);
            for (int i = 0; i < 14; i++) {
                Vector2 at = Projectile.Center - dir * Main.rand.NextFloat(0f, SpearGeometry.Length * 0.8f);
                Vector2 v = CEUtils.randomPointInCircle(4f) + dir * Main.rand.NextFloat(1f, 4f);
                VDVfx.VoidPuff(at, v * 0.5f, Main.rand.NextFloat(0.8f, 1.2f), 0.6f);
                VDVfx.Spark(at, v, i % 3 == 0 ? Color.White : SpearGeometry.Blade, Main.rand.NextFloat(0.4f, 0.7f), 1f, Main.rand.Next(14, 22), gravity: true);
            }
            MatterDissociationBurst.SpawnShards(Projectile.Center - dir * 40f, dir * 3f, 16, 1f);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = MatterDissociationSpearStab.GlowMask.Value;
            Vector2 dir = Dir;
            int facing = dir.X >= 0f ? 1 : -1;
            Vector2 pivotWorld = Projectile.Center - dir * SpearGeometry.TipFromPivot;
            HeldSprite spear = HeldSprite.Along(tex, SpearGeometry.Pivot, SpearGeometry.AxisAngle, pivotWorld, dir.ToRotation(), facing);

            if (flying && trail.Count > 2) {
                VDWeaponFx.BeginAdditive();
                VDWeaponFx.Ribbon(trail, t => 18f * (1f - t), t => Color.Lerp(Color.White, SpearGeometry.Blade, MathF.Min(t * 3f, 1f)) * (1f - t) * 0.85f);
                VDWeaponFx.Ribbon(trail, t => 46f * (1f - t), t => VDVfx.VoidPurple * ((1f - t) * 0.35f));
                VDWeaponFx.End();
            }
            spear.Draw(lightColor);
            spear.Draw(glow, Color.White);
            VDWeaponFx.BeginAdditive();
            VDWeaponFx.Glow(Projectile.Center, SpearGeometry.Blade * 0.55f, 24f);
            VDWeaponFx.Glow(Projectile.Center, Color.White * 0.45f, 9f);
            VDWeaponFx.End();
            return false;
        }
    }

    /// <summary>
    /// 友方虚空弹(驱逐舰紫水晶弹贴图,朝上):前 <see cref="HomingDelay"/> 帧按初速滑出并悬停自转、慢慢充亮;
    /// 到时锁定(ai[0] 为优先目标,失效时由拥有者就近重选并同步),之后限角速度转向、加速到 <see cref="MaxSpeed"/> 扑去
    /// </summary>
    public class MatterDissociationBolt : ModProjectile
    {
        public const int HomingDelay = 30;
        public const float MaxSpeed = 12f;
        public const int TrailLength = 10;
        public static readonly Color GlowColor = new Color(190, 60, 255);

        public override string Texture => "CalamityEntropy/Content/Projectiles/VoidDestroyer/VDVoidBolt";

        private readonly List<Vector2> trail = new List<Vector2>();
        private float spin;
        private int lockFlash;

        public int Age => (int)Projectile.localAI[0];

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage() => Age > HomingDelay ? null : false;

        private NPC Target {
            get {
                int i = (int)Projectile.ai[0];
                if (i < 0 || i >= Main.maxNPCs) {
                    return null;
                }
                NPC n = Main.npc[i];
                return n.CanBeChasedBy(Projectile) ? n : null;
            }
        }

        public override void AI() {
            Projectile.localAI[0]++;
            bool owner = Projectile.owner == Main.myPlayer;
            if (Age <= HomingDelay) {
                Projectile.velocity *= 0.88f;
                spin = MathHelper.Lerp(spin, 0.05f, 0.08f) + (Age == 1 ? 0.35f : 0f);
                Projectile.rotation += spin;
                if (Age == HomingDelay) {
                    lockFlash = 12;
                    if (owner && Target == null) {
                        Retarget();
                    }
                    if (!Main.dedServ) {
                        CEUtils.PlaySound("vb_hit", 1.9f, Projectile.Center, 3, 0.2f);
                    }
                }
            }
            else {
                if (owner && Target == null && Age % 8 == 0) {
                    Retarget();
                }
                NPC target = Target;
                float speed = Projectile.velocity.Length();
                if (target != null) {
                    float t = MathHelper.Clamp((Age - HomingDelay) / 60f, 0f, 1f);
                    float rate = MathHelper.Lerp(0.07f, 0.2f, t);
                    float cur = speed < 0.5f ? (target.Center - Projectile.Center).ToRotation() : Projectile.velocity.ToRotation();
                    float want = (target.Center - Projectile.Center).ToRotation();
                    float turn = MathHelper.Clamp(MathHelper.WrapAngle(want - cur), -rate, rate);
                    speed = Math.Min(speed + 0.35f + speed * 0.04f, MaxSpeed);
                    Projectile.velocity = (cur + turn).ToRotationVector2() * speed;
                }
                else {
                    Projectile.velocity *= 0.96f;
                }
                if (Projectile.velocity.LengthSquared() > 1f) {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
            if (lockFlash > 0) {
                lockFlash--;
            }
            trail.Insert(0, Projectile.Center);
            if (trail.Count > TrailLength) {
                trail.RemoveAt(trail.Count - 1);
            }
            Lighting.AddLight(Projectile.Center, GlowColor.ToVector3() * 0.4f);
            if (!Main.dedServ && Age % 5 == 0) {
                VDVfx.Spark(Projectile.Center, -Projectile.velocity * 0.15f + CEUtils.randomPointInCircle(1.2f), GlowColor, Main.rand.NextFloat(0.3f, 0.5f), 0.9f, 14);
            }
        }

        private void Retarget() {
            int best = -1;
            float bestD = 1100f;
            foreach (NPC n in Main.ActiveNPCs) {
                if (!n.CanBeChasedBy(Projectile)) {
                    continue;
                }
                float d = Vector2.Distance(n.Center, Projectile.Center);
                if (d < bestD) {
                    bestD = d;
                    best = n.whoAmI;
                }
            }
            if (best != (int)Projectile.ai[0]) {
                Projectile.ai[0] = best;
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft) {
            if (Main.dedServ) {
                return;
            }
            CEUtils.PlaySound("vb_hit", Main.rand.NextFloat(1.3f, 1.6f), Projectile.Center, 6, 0.3f);
            for (int i = 0; i < 6; i++) {
                Vector2 v = CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(2f, 5f);
                VDVfx.Spark(Projectile.Center, v, GlowColor, Main.rand.NextFloat(0.4f, 0.7f), 1f, 18, gravity: true);
            }
            PRTLoader.NewParticle<PRT_ShineParticle>(Projectile.Center, Vector2.Zero, GlowColor, 0.2f).Configure(1f, true, PRTDrawModeEnum.AdditiveBlend, 0f, 10);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            float charge = MathHelper.Clamp(Age / (float)HomingDelay, 0f, 1f);
            float pulse = 1f + 0.08f * MathF.Sin(Age * 0.25f);

            VDWeaponFx.BeginAdditive();
            if (trail.Count > 2 && Projectile.velocity.LengthSquared() > 4f) {
                VDWeaponFx.Ribbon(trail, t => 12f * (1f - t), t => Color.Lerp(SpearGeometry.Blade, GlowColor, t) * ((1f - t) * 0.7f));
            }
            VDWeaponFx.Glow(Projectile.Center, GlowColor * (0.3f + 0.35f * charge), 22f * pulse);
            if (Age < HomingDelay) {
                VDWeaponFx.Ring(Projectile.Center, GlowColor * (0.5f * charge), 16f - 6f * charge);
            }
            if (lockFlash > 0) {
                float f = lockFlash / 12f;
                VDWeaponFx.Glow(Projectile.Center, Color.White * f, 14f);
                VDWeaponFx.Ring(Projectile.Center, SpearGeometry.Blade * f, 10f + 16f * (1f - f));
            }
            VDWeaponFx.End();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 0.75f * pulse, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>
    /// 暗影爆炸:接触点先坍缩 <see cref="CollapseFrames"/> 帧(暗核长大、紫环向内收、紫点被吸入),再爆发,
    /// 爆发后 <see cref="DamageFrames"/> 帧内对半径 <see cref="Radius"/> 的圆形判定。暗核走 AlphaBlend 真 alpha 压暗,
    /// 其余光走加法;爆发时溅出品红「物质碎块」(本地模拟的方形碎片)
    /// </summary>
    public class MatterDissociationBurst : ModProjectile
    {
        public const int CollapseFrames = 7;
        public const int DamageFrames = 4;
        public const int Lifetime = 30;
        public const float Radius = 100f;
        public static readonly Color Dark = new Color(10, 0, 22);

        public override string Texture => "CalamityEntropy/Assets/Extra/Empty";

        public int Age => (int)Projectile.localAI[0];

        public override void SetDefaults() {
            Projectile.width = (int)(Radius * 2f);
            Projectile.height = (int)(Radius * 2f);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => Age > CollapseFrames && Age <= CollapseFrames + DamageFrames ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(nearest, Projectile.Center) <= Radius * Radius;
        }

        public override void AI() {
            Projectile.localAI[0]++;
            Lighting.AddLight(Projectile.Center, VDVfx.VoidPurple.ToVector3() * (Age > CollapseFrames ? 1f : 0.4f));
            if (Main.dedServ) {
                return;
            }
            if (Age == 1) {
                CEUtils.PlaySound("VoidAnticipation", 1.55f, Projectile.Center, 3, 0.45f);
                for (int i = 0; i < 12; i++) {
                    Vector2 from = Projectile.Center + CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(70f, 110f);
                    VDVfx.Spark(from, (Projectile.Center - from) / CollapseFrames, VDVfx.VoidPurple, Main.rand.NextFloat(0.35f, 0.6f), 1f, CollapseFrames);
                }
            }
            if (Age == CollapseFrames + 1) {
                CEUtils.PlaySound("VoidBomb", Main.rand.NextFloat(0.75f, 0.85f), Projectile.Center, 4, 0.75f);
                CEUtils.SetShake(Projectile.Center, 6f, 1800f);
                VDVfx.SparkBurst(Projectile.Center, VDVfx.VoidPurple, 16, 4f, 12f, 26, 0.55f, 1f);
                VDVfx.SparkBurst(Projectile.Center, VDVfx.VoidPink, 8, 2f, 7f, 20, 0.4f, 0.75f);
                for (int i = 0; i < 12; i++) {
                    Vector2 v = CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(3f, 8f);
                    VDVfx.VoidPuff(Projectile.Center + v * 2f, v, Main.rand.NextFloat(1.1f, 1.6f), 0.75f);
                }
                for (int i = 0; i < 7; i++) {
                    Vector2 v = CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(1.5f, 4f);
                    PRTLoader.NewParticle<PRT_HeavySmokeCal>(Projectile.Center + v * 5f, v, new Color(36, 12, 58), Main.rand.NextFloat(0.6f, 0.9f)).Configure(0.85f, 44, Main.rand.NextFloat(-0.03f, 0.03f));
                }
                SpawnShards(Projectile.Center, Vector2.Zero, 22, 1f);
            }
        }

        /// <summary>品红碎块:小方块向外崩散、自转、缩小消失(物质解离的标识);加法、纯本地</summary>
        public static void SpawnShards(Vector2 center, Vector2 baseVel, int count, float power) {
            if (Main.dedServ) {
                return;
            }
            for (int i = 0; i < count; i++) {
                Vector2 v = baseVel + CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(2f, 8f) * power;
                Color c = Main.rand.NextBool(3) ? Color.White : (Main.rand.NextBool() ? SpearGeometry.Blade : VDVfx.VoidPurple);
                PRTLoader.NewParticle<PRT_Pixel>(center, Vector2.Zero, c, 1f).Configure(center, center + v * 6f, center + v * 11f + new Vector2(0f, 10f), Main.rand.Next(18, 30), c, VDVfx.VoidDeep * 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            int a = Age;
            Vector2 c = Projectile.Center;
            Texture2D glow = VDWeaponFx.GlowTex;
            if (a <= CollapseFrames) {
                float k = a / (float)CollapseFrames;
                Main.spriteBatch.Draw(glow, c - Main.screenPosition, null, Dark * (0.85f * k), 0f, glow.Size() / 2f, (24f + 40f * k) * 2f / glow.Width, SpriteEffects.None, 0f);
                VDWeaponFx.BeginAdditive();
                VDWeaponFx.Ring(c, VDVfx.VoidPurple * (0.3f + 0.6f * k), MathHelper.Lerp(110f, 30f, k * k));
                VDWeaponFx.Glow(c, VDVfx.VoidPink * (0.25f * k), 30f);
                VDWeaponFx.End();
                return false;
            }
            float d = (a - CollapseFrames) / (float)(Lifetime - CollapseFrames);
            float ease = VDVfx.EaseOut(d);
            float darkR = MathHelper.Lerp(50f, 125f, ease);
            Main.spriteBatch.Draw(glow, c - Main.screenPosition, null, Dark * (0.9f * MathF.Pow(1f - d, 1.5f)), 0f, glow.Size() / 2f, darkR * 2f / glow.Width, SpriteEffects.None, 0f);
            VDWeaponFx.BeginAdditive();
            if (a <= CollapseFrames + 3) {
                float f = 1f - (a - CollapseFrames) / 3f;
                VDWeaponFx.Glow(c, Color.White * f, 70f);
            }
            VDWeaponFx.Ring(c, Color.Lerp(VDVfx.VoidPink, VDVfx.VoidPurple, d) * (1f - d), MathHelper.Lerp(30f, Radius * 1.2f, ease));
            VDWeaponFx.Ring(c, VDVfx.VoidDeep * (0.8f * (1f - d)), MathHelper.Lerp(20f, Radius * 0.85f, ease));
            VDWeaponFx.Glow(c, VDVfx.VoidPurple * (0.4f * (1f - d)), darkR * 1.1f);
            VDWeaponFx.End();
            return false;
        }
    }

    /// <summary>
    /// 投影三叉戟(近战):在目标下方显形 <see cref="MaterializeFrames"/> 帧(全息闪烁淡入、脚下一枚标记环、始终对准目标预判点),
    /// 随即三次更新、每次 32px 极速射出,贯穿路上每个敌人各一次,飞行 <see cref="FlightFrames"/> 帧后淡出。ai[0] 目标,ai[1] 序号(0 号负责整组的音效)
    /// </summary>
    public class MatterDissociationTrident : ModProjectile
    {
        public const int MaterializeFrames = 6;
        public const int FlightFrames = 12;
        public const float Speed = 32f;
        public static readonly Color HoloColor = new Color(255, 120, 255);

        public override string Texture => "CalamityEntropy/Assets/Extra/Empty";

        private readonly List<Vector2> trail = new List<Vector2>();
        private Vector2 aimDir = -Vector2.UnitY;

        public int Updates => (int)Projectile.localAI[0];
        public int Frames => Updates / (Projectile.extraUpdates + 1);
        public bool Flying => Frames >= MaterializeFrames;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 600;
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = (MaterializeFrames + FlightFrames + 4) * 3;
            Projectile.extraUpdates = 2;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition() => Flying;

        public override bool? CanDamage() => Flying && Frames < MaterializeFrames + FlightFrames ? null : false;

        private NPC Target {
            get {
                int i = (int)Projectile.ai[0];
                if (i < 0 || i >= Main.maxNPCs) {
                    return null;
                }
                NPC n = Main.npc[i];
                return n.active && n.life > 0 ? n : null;
            }
        }

        public override void AI() {
            Projectile.localAI[0]++;
            bool lead = Projectile.ai[1] == 0f;
            if (!Flying) {
                NPC target = Target;
                if (target != null) {
                    aimDir = (target.Center + target.velocity * 3f - Projectile.Center).SafeNormalize(-Vector2.UnitY);
                }
                Projectile.rotation = aimDir.ToRotation() + MathHelper.PiOver4;
                if (Updates == 1 && lead && !Main.dedServ) {
                    CEUtils.PlaySound("vbapear", 1.45f, Projectile.Center, 3, 0.55f);
                }
                if (Frames == MaterializeFrames - 1 && Updates % (Projectile.extraUpdates + 1) == 0) {
                    Projectile.velocity = aimDir * Speed;
                    if (lead && !Main.dedServ) {
                        CEUtils.PlaySound("CruiserDash", 1.6f, Projectile.Center, 2, 0.45f);
                    }
                }
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Updates % (Projectile.extraUpdates + 1) == 0) {
                trail.Insert(0, Projectile.Center);
                if (trail.Count > 8) {
                    trail.RemoveAt(trail.Count - 1);
                }
            }
            Lighting.AddLight(Projectile.Center, HoloColor.ToVector3() * 0.5f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Vector2 dir = Projectile.velocity.SafeNormalize(aimDir);
            return CEUtils.LineThroughRect(Projectile.Center - dir * 60f, Projectile.Center + dir * 14f, targetHitbox, 18);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Vector2 dir = Projectile.velocity.SafeNormalize(aimDir);
            for (int i = 0; i < 5; i++) {
                VDVfx.Spark(Projectile.Center, dir.RotatedByRandom(0.4f) * Main.rand.NextFloat(6f, 14f), HoloColor, Main.rand.NextFloat(0.4f, 0.65f), 1f, 12);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            float f = Frames;
            float opacity;
            if (!Flying) {
                float k = (f + 1f) / MaterializeFrames;
                //显形闪烁:前半段按帧明灭,后半段稳定
                opacity = k * (k < 0.6f ? (Updates % 6 < 3 ? 0.4f : 1f) : 1f);
            }
            else {
                opacity = MathHelper.Clamp((MaterializeFrames + FlightFrames + 3 - f) / 3f, 0f, 1f);
            }
            if (opacity <= 0.01f) {
                return false;
            }
            Vector2 dir = Projectile.velocity.LengthSquared() > 0.01f ? Projectile.velocity.SafeNormalize(aimDir) : aimDir;

            VDWeaponFx.BeginAdditive();
            if (!Flying) {
                float k = (f + 1f) / MaterializeFrames;
                VDWeaponFx.Ring(Projectile.Center, HoloColor * (0.7f * k), MathHelper.Lerp(34f, 14f, k));
                VDWeaponFx.Glow(Projectile.Center, HoloColor * (0.4f * k), 26f);
            }
            else if (trail.Count > 1) {
                VDWeaponFx.Streak(Projectile.Center, trail[^1] - dir * 30f, 10f * opacity, Color.White * opacity, HoloColor * opacity);
            }
            VDWeaponFx.End();

            Main.instance.LoadProjectile(ProjectileID.UnholyTridentHostile);
            Texture2D tex = TextureAssets.Projectile[ProjectileID.UnholyTridentHostile].Value;
            VDHologramDraw.Draw(tex, Projectile.Center - Main.screenPosition, null, HoloColor, opacity * 0.95f, Projectile.rotation, tex.Size() / 2f, 1.25f, SpriteEffects.None);
            return false;
        }
    }
}
