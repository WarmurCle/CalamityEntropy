using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.NPCs.VoidDestroyer;
using CalamityEntropy.Content.Projectiles.VoidDestroyer;
using CalamityEntropy.Content.Rarities;
using InnoVault;
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
    /// 首召占 BaseSlots,在场时再用占 1 栏并加 StackBonus,不会多召一架
    /// 空栏不够时,遥控器先换掉别的召唤物;这里不走原版献祭,原版按 3 栏腾出空位,会把叠了层的自己换掉
    /// </summary>
    public class VoidDroneRemote : ModItem
    {
        public const int BaseSlots = 3;
        public const float StackBonus = 0.33f;

        public override void SetStaticDefaults() {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = BaseSlots;
        }

        public override void SetDefaults() {
            Item.damage = 180;
            Item.DamageType = DamageClass.Summon;
            Item.width = 50;
            Item.height = 44;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 3f;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<ProjectionDrone>();
            Item.shootSpeed = 1f;
            Item.value = Item.buyPrice(platinum: 2, gold: 50);
            Item.UseSound = CEUtils.GetSound("beep", 1.35f, 3, 0.45f);
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.mana = 10;
            Item.buffType = ModContent.BuffType<ProjectionDroneBuff>();
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        private static float SlotsNeeded(Projectile drone) => drone != null ? drone.minionSlots + 1f : BaseSlots;

        /// <summary>
        /// 首召要召唤上限装得下 3 栏,叠层要装得下现有栏位再加 1;空位不够在 Shoot 里腾。
        /// 右键是原版的召唤物指定目标(altFunctionUse == 2,不会走到 Shoot),栏位满了也要能用
        /// </summary>
        public override bool CanUseItem(Player player) {
            if (player.altFunctionUse == 2) {
                return true;
            }
            return player.maxMinions + 0.001f >= SlotsNeeded(ProjectionDrone.FindProjectile(player.whoAmI));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.AddBuff(Item.buffType, 3);
            Projectile drone = ProjectionDrone.FindProjectile(player.whoAmI);
            FreeSlots(player, drone, SlotsNeeded(drone));
            if (drone != null) {
                //叠层的音效与闪光由无人机看到 ai[0] 变大后在各端自己放
                drone.ai[0] += 1f;
                drone.ai[1] = Item.damage;
                drone.netUpdate = true;
                return false;
            }
            Vector2 spawn = player.MountedCenter - Vector2.UnitY * ProjectionDrone.HoverHeight;
            Projectile.NewProjectile(source, spawn, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, Item.damage);
            return false;
        }

        /// <summary>别的召唤物按占位从小到大被换掉,直到其余占位加 need 不超过上限;星尘龙这类多节召唤物整条跳过,拆一节会把它拆散</summary>
        private static void FreeSlots(Player player, Projectile drone, float need) {
            List<Projectile> others = new List<Projectile>();
            float used = 0f;
            foreach (Projectile p in Main.ActiveProjectiles) {
                if (p.owner != player.whoAmI || !p.minion || p == drone) {
                    continue;
                }
                used += p.minionSlots;
                if (p.minionSlots > 0f && !IsSegmentedMinion(p.type)) {
                    others.Add(p);
                }
            }
            others.Sort((a, b) => a.minionSlots.CompareTo(b.minionSlots));
            foreach (Projectile p in others) {
                if (used + need <= player.maxMinions + 0.001f) {
                    break;
                }
                used -= p.minionSlots;
                p.Kill();
            }
        }

        private static bool IsSegmentedMinion(int type) {
            return type == ProjectileID.StardustGuardian || (type >= ProjectileID.StardustDragon1 && type <= ProjectileID.StardustDragon4);
        }
    }

    public class ProjectionDroneBuff : BaseMinionBuff
    {
        public override int ProjType => ModContent.ProjectileType<ProjectionDrone>();
    }

    /// <summary>召唤上限掉了(卸饰品、换套装、药水到期)时,赶在弹幕更新之前替无人机减层:原版超栏判定在弹幕 AI 之前,会把整架无人机直接杀掉</summary>
    public class ProjectionDronePlayer : ModPlayer
    {
        public override void PostUpdate() {
            if (Player.whoAmI == Main.myPlayer) {
                ProjectionDrone.ShedStacks(Player);
            }
        }
    }

    /// <summary>
    /// ai[0] 叠层,ai[1] 召唤时的物品基础伤害,同步后各端推栏位和 originalDamage
    /// 射击和投影只在拥有者端生成,闪光由弹幕各端回调
    /// </summary>
    public class ProjectionDrone : ModProjectile
    {
        public const float HoverHeight = 92f;
        /// <summary>玩家每帧位移里直接带走的比例,剩下的由弹簧追</summary>
        public const float CarryRatio = 0.85f;
        public const int AppearFrames = 16;
        public const int ShotInterval = 30;
        public const int TortoiseInterval = 30;
        public const int DevilInterval = 60;
        public const int WyvernInterval = 120;
        public const float TargetRange = 1400f;
        public const float TortoiseMult = 0.55f;
        public const float DevilMult = 0.6f;
        public const float WyvernMult = 0.8f;
        public const int TortoiseCap = 6;
        public const int DevilCap = 5;
        public const int WyvernCap = 3;
        public static readonly Color GlowColor = new Color(220, 120, 255);
        public static readonly Color PipColor = new Color(167, 232, 252);

        //贴图锚点(134×72,像素坐标):机身中心、底部投影口、两侧吊舱
        public static readonly Vector2 Pivot = new Vector2(67f, 36f);
        public static readonly Vector2 Aperture = new Vector2(67f, 70f);
        public static readonly Vector2[] Pods = { new Vector2(24f, 49f), new Vector2(110f, 49f) };
        /// <summary>机翼上 6 盏青色叠层灯,按点亮顺序排(由内向外、左右交替)</summary>
        public static readonly Vector2[] Pips = { new(31f, 21f), new(103f, 21f), new(27f, 21f), new(107f, 21f), new(23f, 21f), new(111f, 21f) };

        //发光层(贴图里的灯),加载期由 VaultLoaden 赋值,仅绘制路径读取
        [VaultLoaden("CalamityEntropy/Content/Items/Weapons/VoidDestroyer/ProjectionDrone_Glow")]
        internal static Asset<Texture2D> GlowMask;

        private int shotTimer = ShotInterval - 12;
        private int tortoiseTimer = TortoiseInterval - 10;
        private int devilTimer = DevilInterval - 30;
        private int wyvernTimer = WyvernInterval - 60;
        private int nextPod;
        private int tortoiseSlot;
        private int devilSlot;
        private int wyvernSide = 1;
        private int targetIndex = -1;
        private int lastStacks = -1;
        private int age;
        private float bob;
        private float motionX;
        private float tilt;
        private float tiltVel;
        private int facing = 1;
        private readonly float[] podFlash = new float[2];
        private float projectFlash;
        private Color projectColor = GlowColor;
        private float pipFlash;

        public int Stacks => Math.Max(0, (int)Projectile.ai[0]);
        public int BaseDamage => (int)Projectile.ai[1];
        public float DamageMult => 1f + VoidDroneRemote.StackBonus * Stacks;
        /// <summary>当前锁定的敌怪索引(-1 无);全息体丢了目标时拿它接着打</summary>
        public int CurrentTarget => targetIndex;
        public Vector2 ApertureWorld => AnchorWorld(Aperture);

        public override void SetStaticDefaults() {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 96;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.minion = true;
            Projectile.minionSlots = VoidDroneRemote.BaseSlots;
            Projectile.aiStyle = -1;
            Projectile.netImportant = true;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;

        /// <summary>该玩家的投影无人机,每人最多一架</summary>
        public static Projectile FindProjectile(int owner) {
            int type = ModContent.ProjectileType<ProjectionDrone>();
            foreach (Projectile p in Main.ActiveProjectiles) {
                if (p.owner == owner && p.type == type) {
                    return p;
                }
            }
            return null;
        }

        /// <summary>带缓存的查找:cache 是调用方存的弹幕索引,失效才重新扫</summary>
        public static ProjectionDrone Find(int owner, ref int cache) {
            if (cache >= 0 && cache < Main.maxProjectiles) {
                Projectile p = Main.projectile[cache];
                if (p.active && p.owner == owner && p.ModProjectile is ProjectionDrone cached) {
                    return cached;
                }
            }
            Projectile found = FindProjectile(owner);
            cache = found?.whoAmI ?? -1;
            return found?.ModProjectile as ProjectionDrone;
        }

        /// <summary>召唤上限装不下「其余召唤物 + 3 + 叠层」时减层,并立刻改掉本帧栏位(超栏判定就在接下来的弹幕更新里)</summary>
        public static void ShedStacks(Player player) {
            Projectile drone = FindProjectile(player.whoAmI);
            if (drone == null || drone.ModProjectile is not ProjectionDrone d || d.Stacks <= 0) {
                return;
            }
            float others = 0f;
            foreach (Projectile p in Main.ActiveProjectiles) {
                if (p.owner == player.whoAmI && p.minion && p.whoAmI != drone.whoAmI) {
                    others += p.minionSlots;
                }
            }
            int allowed = Math.Max(0, (int)MathF.Floor(player.maxMinions - others - VoidDroneRemote.BaseSlots + 0.001f));
            if (allowed < d.Stacks) {
                drone.ai[0] = allowed;
                drone.minionSlots = VoidDroneRemote.BaseSlots + allowed;
                drone.netUpdate = true;
            }
        }

        /// <summary>贴图内一点的世界坐标(不碰贴图,服务端也能算);朝左时水平镜像</summary>
        public Vector2 AnchorWorld(Vector2 texPoint) {
            Vector2 d = texPoint - Pivot;
            if (facing < 0) {
                d.X = -d.X;
            }
            return Projectile.Center + d.RotatedBy(tilt);
        }

        /// <summary>吊舱射出一发(子弹首帧在各端回调):吊舱闪光,机身挨一下后坐</summary>
        public void OnShot(int pod, Vector2 dir) {
            pod = Math.Clamp(pod, 0, 1);
            podFlash[pod] = 1f;
            Projectile.velocity -= dir * 1.4f;
            tiltVel -= dir.X * 0.035f;
        }

        /// <summary>全息体显形期间每帧回调:投影口亮起,颜色跟着被投的东西走</summary>
        public void OnProject(Color color) {
            projectFlash = 1f;
            projectColor = color;
        }

        public override void AI() {
            Player player = Projectile.GetOwner();
            Projectile.MinionCheck<ProjectionDroneBuff>();
            Projectile.minionSlots = VoidDroneRemote.BaseSlots + Stacks;
            if (BaseDamage > 0) {
                Projectile.originalDamage = (int)(BaseDamage * DamageMult);
            }
            age++;
            StackFeedback();
            Hover(player);

            NPC target = PickTarget(player);
            if (target != null) {
                float dx = target.Center.X - Projectile.Center.X;
                if (Math.Abs(dx) > 12f) {
                    facing = Math.Sign(dx);
                }
            }
            else {
                facing = player.direction;
            }
            //机身倾斜:跟水平运动走的弹簧,射击后坐往里加冲量
            float want = MathHelper.Clamp(motionX * 0.03f, -0.32f, 0.32f);
            tiltVel += (want - tilt) * 0.14f;
            tiltVel *= 0.78f;
            tilt += tiltVel;

            podFlash[0] *= 0.8f;
            podFlash[1] *= 0.8f;
            projectFlash *= 0.86f;
            pipFlash *= 0.92f;

            if (target != null && Projectile.owner == Main.myPlayer) {
                RunAttacks(player, target);
            }
            Lighting.AddLight(Projectile.Center, GlowColor.ToVector3() * (0.3f + 0.3f * projectFlash));
            if (!Main.dedServ && projectFlash > 0.3f && Main.rand.NextBool(3)) {
                Vector2 ap = ApertureWorld;
                VDVfx.Spark(ap, new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(1f, 3f)), projectColor, Main.rand.NextFloat(0.3f, 0.5f), 0.9f, 12);
            }
        }

        /// <summary>出场与叠层的反馈:各端看到 ai[0] 变大就放,不依赖使用物品的那一端</summary>
        private void StackFeedback() {
            int s = Stacks;
            if (lastStacks < 0) {
                lastStacks = s;
                if (!Main.dedServ) {
                    VDVfx.HoloBurst(Projectile.Center, GlowColor);
                    CEUtils.PlaySound("vmspawn", 1f, Projectile.Center, 4, 0.55f);
                }
                return;
            }
            if (s > lastStacks && !Main.dedServ) {
                pipFlash = 1f;
                Vector2 pip = AnchorWorld(Pips[Math.Min(s, Pips.Length) - 1]);
                CEUtils.PlaySound("vbapear", 1.2f + 0.04f * Math.Min(s, 8), Projectile.Center, 4, 0.5f);
                VDVfx.SparkBurst(pip, PipColor, 10, 2f, 6f, 18, 0.4f, 0.8f);
                VDVfx.HoloBurst(Projectile.Center, GlowColor);
            }
            lastStacks = s;
        }

        private void Hover(Player player) {
            Vector2 carry = player.position - player.oldPosition;
            if (carry.LengthSquared() > 160f * 160f) {
                carry = Vector2.Zero;
            }
            Projectile.position += carry * CarryRatio;
            bob += 0.045f;
            Vector2 home = player.MountedCenter + new Vector2(-player.direction * 12f, -HoverHeight + MathF.Sin(bob) * 6f);
            Vector2 to = home - Projectile.Center;
            float dist = to.Length();
            if (dist > 2000f) {
                Projectile.Center = home;
                Projectile.velocity = Vector2.Zero;
                if (!Main.dedServ) {
                    VDVfx.HoloBurst(home, GlowColor);
                }
                return;
            }
            Vector2 accel = to * 0.05f;
            float maxAccel = 1.2f + dist / 150f;
            if (accel.LengthSquared() > maxAccel * maxAccel) {
                accel = accel / accel.Length() * maxAccel;
            }
            Projectile.velocity = (Projectile.velocity + accel) * 0.85f;
            motionX = carry.X * CarryRatio + Projectile.velocity.X;
        }

        /// <summary>锁定:玩家指定的目标优先,其次沿用当前目标,丢了才找离玩家最近的</summary>
        private NPC PickTarget(Player player) {
            if (player.HasMinionAttackTargetNPC) {
                NPC forced = Main.npc[player.MinionAttackTargetNPC];
                if (forced.CanBeChasedBy(Projectile) && forced.Distance(player.Center) < TargetRange * 1.4f) {
                    targetIndex = forced.whoAmI;
                    return forced;
                }
            }
            if (targetIndex >= 0) {
                NPC cur = Main.npc[targetIndex];
                if (cur.CanBeChasedBy(Projectile) && cur.Distance(player.Center) < TargetRange * 1.15f) {
                    return cur;
                }
                targetIndex = -1;
            }
            float best = TargetRange;
            foreach (NPC n in Main.ActiveNPCs) {
                if (!n.CanBeChasedBy(Projectile)) {
                    continue;
                }
                float d = n.Distance(player.Center);
                if (d < best) {
                    best = d;
                    targetIndex = n.whoAmI;
                }
            }
            return targetIndex >= 0 ? Main.npc[targetIndex] : null;
        }

        /// <summary>拥有者端的节拍:吊舱交替射击,投影口按各自间隔投出全息体(没目标时计时停住,接敌时不会一股脑全放)</summary>
        private void RunAttacks(Player player, NPC target) {
            IEntitySource src = Projectile.GetSource_FromThis();
            Vector2 aperture = ApertureWorld;
            if (++shotTimer >= ShotInterval) {
                shotTimer = 0;
                int pod = nextPod;
                nextPod ^= 1;
                Vector2 from = AnchorWorld(Pods[pod]);
                Vector2 aim = (target.Center + target.velocity * 6f - from).SafeNormalize(Vector2.UnitY);
                Projectile.NewProjectile(src, from, aim * ProjectionDroneShot.Speed, ModContent.ProjectileType<ProjectionDroneShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI, pod);
            }
            if (++tortoiseTimer >= TortoiseInterval) {
                tortoiseTimer = 0;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ProjectionTortoise>()] < TortoiseCap) {
                    SpawnHolo<ProjectionTortoise>(src, aperture, target, TortoiseMult, tortoiseSlot);
                    tortoiseSlot = (tortoiseSlot + 1) % ProjectionTortoise.SlotOffsets.Length;
                }
            }
            if (++devilTimer >= DevilInterval) {
                devilTimer = 0;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ProjectionRedDevil>()] < DevilCap) {
                    SpawnHolo<ProjectionRedDevil>(src, aperture, target, DevilMult, devilSlot);
                    devilSlot = (devilSlot + 1) % ProjectionRedDevil.Slots.Length;
                }
            }
            if (++wyvernTimer >= WyvernInterval) {
                wyvernTimer = 0;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ProjectionWyvern>()] < WyvernCap) {
                    SpawnHolo<ProjectionWyvern>(src, aperture, target, WyvernMult, wyvernSide);
                    wyvernSide = -wyvernSide;
                }
            }
        }

        private void SpawnHolo<T>(IEntitySource src, Vector2 at, NPC target, float mult, float ai1) where T : ModProjectile {
            int dmg = Math.Max(1, (int)(Projectile.damage * mult));
            Projectile.NewProjectile(src, at, Vector2.Zero, ModContent.ProjectileType<T>(), dmg, Projectile.knockBack, Projectile.owner, target.whoAmI, ai1);
        }

        private static Rectangle PipRect(int i) => new Rectangle((int)Pips[i].X - 1, (int)Pips[i].Y - 1, 2, 2);

        public override bool PreDraw(ref Color lightColor) {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            HeldSprite body = new HeldSprite {
                Texture = tex,
                World = Projectile.Center,
                Pivot = Pivot,
                Rotation = tilt,
                Effects = facing < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                Scale = 1f
            };
            float appear = MathHelper.Clamp(age / (float)AppearFrames, 0f, 1f);

            if (appear < 1f) {
                //出场:先是一层全息扫描影,机身随后实体化
                VDHologramDraw.Draw(tex, Projectile.Center - Main.screenPosition, null, GlowColor, 1f - appear, tilt, body.Origin, 1f, body.Effects);
            }
            body.Draw(lightColor * appear);
            body.Draw(GlowMask.Value, Color.White * appear);
            //没点亮的叠层灯改回暗色(发光层里 6 盏灯默认全亮)
            Color off = new Color(lightColor.R / 4, lightColor.G / 4, lightColor.B / 4) * appear;
            for (int i = Math.Min(Stacks, Pips.Length); i < Pips.Length; i++) {
                body.DrawPart(tex, PipRect(i), off);
            }

            VDWeaponFx.BeginAdditive();
            int lit = Math.Min(Stacks, Pips.Length);
            for (int i = 0; i < lit; i++) {
                VDWeaponFx.Glow(body.ToWorld(Pips[i]), PipColor * ((0.45f + 0.55f * pipFlash) * appear), 5f + 7f * pipFlash);
            }
            for (int p = 0; p < 2; p++) {
                if (podFlash[p] > 0.03f) {
                    Vector2 at = body.ToWorld(Pods[p]);
                    VDWeaponFx.Glow(at, GlowColor * (0.8f * podFlash[p]), 10f + 10f * podFlash[p]);
                    VDWeaponFx.Glow(at, Color.White * (0.7f * podFlash[p]), 4f + 3f * podFlash[p]);
                }
            }
            //投影口:常亮一点,投影时爆亮成被投物的颜色
            Vector2 ap = body.ToWorld(Aperture);
            float pulse = 0.25f + 0.08f * MathF.Sin(age * 0.12f);
            Color apColor = Color.Lerp(GlowColor, projectColor, projectFlash);
            VDWeaponFx.Glow(ap, apColor * ((pulse + 0.6f * projectFlash) * appear), 10f + 14f * projectFlash);
            VDWeaponFx.Glow(ap, Color.White * (0.5f * projectFlash * appear), 4f + 4f * projectFlash);
            //叠层超过 6 盏灯就只能靠这层机身晕看出来
            float aura = MathHelper.Clamp(Stacks * 0.06f, 0f, 0.36f);
            if (aura > 0.01f) {
                body.Draw(tex, GlowColor * (aura * (0.75f + 0.25f * MathF.Sin(age * 0.07f)) * appear));
            }
            VDWeaponFx.End();
            return false;
        }
    }

    /// <summary>吊舱子弹:两次更新、恒速微追踪(只追 ai[0] 那个目标,丢了就直飞);首帧通知无人机闪对应吊舱(ai[1])</summary>
    public class ProjectionDroneShot : ModProjectile
    {
        /// <summary>每次更新的速度(两次更新,每帧 30px)</summary>
        public const float Speed = 15f;
        public const float TurnRate = 0.045f;
        public const int TrailLength = 6;
        public static readonly Color GlowColor = new Color(230, 140, 255);

        public override string Texture => "CalamityEntropy/Assets/Extra/Empty";

        private readonly Vector2[] trail = new Vector2[TrailLength];
        private int trailCount;
        private int droneCache = -1;

        public int Age => (int)Projectile.localAI[0];

        public override void SetStaticDefaults() {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 110;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = -1;
        }

        public override void AI() {
            Projectile.localAI[0]++;
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            if (Age == 1) {
                ProjectionDrone.Find(Projectile.owner, ref droneCache)?.OnShot((int)Projectile.ai[1], dir);
                if (!Main.dedServ) {
                    CEUtils.PlaySound("lasershoot", Main.rand.NextFloat(1.3f, 1.5f), Projectile.Center, 8, 0.22f);
                    for (int i = 0; i < 3; i++) {
                        VDVfx.Spark(Projectile.Center, dir.RotatedByRandom(0.35f) * Main.rand.NextFloat(3f, 7f), GlowColor, Main.rand.NextFloat(0.3f, 0.45f), 1f, 8);
                    }
                }
            }
            int idx = (int)Projectile.ai[0];
            NPC target = idx >= 0 && idx < Main.maxNPCs && Main.npc[idx].CanBeChasedBy(Projectile) ? Main.npc[idx] : null;
            if (target != null && Age > 3) {
                float cur = Projectile.velocity.ToRotation();
                float turn = MathHelper.Clamp(MathHelper.WrapAngle((target.Center - Projectile.Center).ToRotation() - cur), -TurnRate, TurnRate);
                Projectile.velocity = (cur + turn).ToRotationVector2() * Speed;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Age % 2 == 0) {
                for (int i = TrailLength - 1; i > 0; i--) {
                    trail[i] = trail[i - 1];
                }
                trail[0] = Projectile.Center;
                trailCount = Math.Min(trailCount + 1, TrailLength);
            }
            Lighting.AddLight(Projectile.Center, GlowColor.ToVector3() * 0.25f);
        }

        public override void OnKill(int timeLeft) {
            if (Main.dedServ || timeLeft <= 0) {
                return;
            }
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 4; i++) {
                VDVfx.Spark(Projectile.Center, (-dir).RotatedByRandom(1f) * Main.rand.NextFloat(2f, 5f), GlowColor, Main.rand.NextFloat(0.3f, 0.5f), 1f, Main.rand.Next(8, 13), gravity: true);
            }
            CEUtils.PlaySound("vb_hit", Main.rand.NextFloat(1.7f, 1.9f), Projectile.Center, 6, 0.16f);
        }

        public override bool PreDraw(ref Color lightColor) {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 head = Projectile.Center + dir * 5f;
            Vector2 tail = trailCount > 1 ? trail[trailCount - 1] - dir * 4f : Projectile.Center - dir * 24f;
            VDWeaponFx.BeginAdditive();
            VDWeaponFx.Streak(head, tail, 5f, Color.White, GlowColor);
            VDWeaponFx.Glow(head, GlowColor * 0.6f, 9f);
            VDWeaponFx.Glow(head, Color.White * 0.7f, 3.5f);
            VDWeaponFx.End();
            return false;
        }
    }

    /// <summary>
    /// 全息生物公共部分:ai[0] 目标敌怪,ai[1] 槽位 / 侧向,ai[2] 为 1 表示拥有者已判定收尾(丢了目标且换不到),各端看到就开始渐隐。
    /// 显形的 <see cref="MaterializeFrames"/> 帧里无人机投影口亮着,一道投影光锥从投影口打到本体上
    /// </summary>
    public abstract class ProjectionHoloBase : ModProjectile
    {
        public const int FadeFrames = 14;
        public const int ConeTail = 6;

        public override string Texture => "CalamityEntropy/Assets/Extra/Empty";
        public abstract Color HoloColor { get; }
        public virtual int MaterializeFrames => 12;

        public int Age => (int)Projectile.localAI[0];
        protected bool Fading => Projectile.ai[2] == 1f;
        /// <summary>光锥是否还打在本体上(闪现走了的就不再画)</summary>
        protected virtual bool ConeVisible => true;

        private int droneCache = -1;
        protected ProjectionDrone Drone => ProjectionDrone.Find(Projectile.owner, ref droneCache);

        protected NPC Target {
            get {
                int idx = (int)Projectile.ai[0];
                if (idx < 0 || idx >= Main.maxNPCs) {
                    return null;
                }
                NPC npc = Main.npc[idx];
                return npc.CanBeChasedBy(Projectile) ? npc : null;
            }
        }

        public override void SetStaticDefaults() {
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 900;
        }

        /// <summary>AI 开头调用:计龄;显形期间点亮投影口;拥有者判了收尾就压寿命</summary>
        protected void Tick() {
            Projectile.localAI[0]++;
            if (Age <= MaterializeFrames) {
                Drone?.OnProject(HoloColor);
            }
            if (Fading && Projectile.timeLeft > FadeFrames) {
                Projectile.timeLeft = FadeFrames;
            }
        }

        /// <summary>
        /// 取目标:目标没了时,允许换目标的由拥有者换成无人机当前目标或最近的敌怪并同步;换不到就由拥有者判收尾。
        /// 远端在拥有者的决定到达前只返回 null、原地待着,不自己收尾(寿命不同步,自己收尾会和拥有者对不上)
        /// </summary>
        protected NPC KeepTarget(bool allowRetarget) {
            NPC t = Target;
            if (t != null || Projectile.owner != Main.myPlayer) {
                return t;
            }
            if (allowRetarget && !Fading) {
                int pick = PickNearest();
                if (pick >= 0) {
                    Projectile.ai[0] = pick;
                    Projectile.netUpdate = true;
                    return Main.npc[pick];
                }
            }
            BeginFade();
            return null;
        }

        protected void BeginFade() {
            if (Projectile.owner == Main.myPlayer && !Fading) {
                Projectile.ai[2] = 1f;
                Projectile.netUpdate = true;
            }
        }

        private int PickNearest() {
            ProjectionDrone d = Drone;
            if (d != null && d.CurrentTarget >= 0 && Main.npc[d.CurrentTarget].CanBeChasedBy(Projectile)) {
                return d.CurrentTarget;
            }
            int best = -1;
            float bestD = ProjectionDrone.TargetRange;
            foreach (NPC n in Main.ActiveNPCs) {
                if (!n.CanBeChasedBy(Projectile)) {
                    continue;
                }
                float dd = n.Distance(Projectile.Center);
                if (dd < bestD) {
                    bestD = dd;
                    best = n.whoAmI;
                }
            }
            return best;
        }

        /// <summary>显形位置:投影口斜下方、朝目标那一侧(side ±1),避开玩家头顶</summary>
        protected Vector2 AppearPoint(float side, Vector2 offset) {
            ProjectionDrone d = Drone;
            Vector2 ap = d != null ? d.ApertureWorld : Projectile.Center;
            return ap + new Vector2(offset.X * side, offset.Y);
        }

        protected static float SideOf(NPC target, Vector2 from) {
            return target != null && target.Center.X < from.X ? -1f : 1f;
        }

        protected float HoloOpacity(float peak = 0.9f) {
            float fadeIn = MathHelper.Clamp(Age / (float)MaterializeFrames, 0f, 1f);
            float fadeOut = MathHelper.Clamp(Projectile.timeLeft / (float)FadeFrames, 0f, 1f);
            return peak * Math.Min(fadeIn, fadeOut);
        }

        /// <summary>
        /// 显形期间从投影口打到本体的光锥:投影口一端细、成像一端宽到 imageWidth;本体离投影口越远越淡(140px 起淡、400px 消失),
        /// 本体闪走或飞走时光锥自己收掉,不会拉成一道打向目标、像在造成伤害的长光柱。进出批次保持 Deferred/AlphaBlend
        /// </summary>
        protected void DrawProjectionCone(float imageWidth) {
            if (Age > MaterializeFrames + ConeTail || !ConeVisible) {
                return;
            }
            ProjectionDrone d = Drone;
            if (d == null) {
                return;
            }
            Vector2 from = d.ApertureWorld;
            Vector2 to = Projectile.Center;
            float dist = Vector2.Distance(from, to);
            float reach = MathHelper.Clamp(1f - (dist - 140f) / 260f, 0f, 1f);
            if (dist < 10f || reach <= 0f) {
                return;
            }
            float k = Age <= MaterializeFrames ? 1f : 1f - (Age - MaterializeFrames) / (float)ConeTail;
            float flicker = 0.8f + 0.2f * MathF.Sin(Age * 1.9f + Projectile.identity);
            VDBeamDraw.DrawTapered(from, to, 5f, imageWidth, HoloColor, Color.White, 1f, 0.45f * k * reach * flicker, Projectile.identity * 0.37f, endGlow: false, capStart: 0f, capEnd: 0.2f);
        }

        public override void OnKill(int timeLeft) {
            if (!Main.dedServ) {
                VDVfx.HoloBurst(Projectile.Center, HoloColor);
            }
        }
    }

    /// <summary>
    /// 运动是龄期的纯函数,目标位置各端同步,落地那 3 帧判定
    /// 同时在场的几只按 ai[1] 左右错开
    /// </summary>
    public class ProjectionTortoise : ProjectionHoloBase
    {
        public const int AppearFrames = 12;
        public const int Bounces = 5;
        public const float Gravity = 2.3f;
        /// <summary>首落高度与 4 次回弹的顶点高度(像素)</summary>
        public static readonly float[] Heights = { 150f, 120f, 96f, 78f, 62f };
        /// <summary>与渐隐同长,第 5 次落地后整体淡出不会先跳暗一档</summary>
        public const int VanishFrames = FadeFrames;
        public const int ContactFrames = 3;
        public const float HitRadius = 32f;
        /// <summary>壳心停在目标顶部上方的距离:壳底压住头顶</summary>
        public const float RestAbove = 16f;
        public const float Scale = 1.25f;
        /// <summary>槽位的水平错位(乘目标宽度相关的展开量),相邻两只落在两侧</summary>
        public static readonly float[] SlotOffsets = { -1f, 0.35f, -0.35f, 1f };

        private static float[] segStart;

        public override int MaterializeFrames => AppearFrames;
        public override Color HoloColor => VDHologramDraw.JungleGreen;
        protected override bool ConeVisible => !blinked;

        private float spin;
        private float side = 1f;
        private float x;
        private int landings;
        private int contact;
        private float punch;
        private int landFlash;
        private Vector2 landPoint;
        private bool blinked;
        private readonly Vector2[] ghosts = new Vector2[3];
        private int ghostCount;

        /// <summary>弹跳段起点(帧,从闪现那一刻算):第 0 段首落,之后每段一次回弹;第 k 段末尾是第 k+1 次落地</summary>
        private static float[] SegStart {
            get {
                if (segStart == null) {
                    float[] s = new float[Bounces + 1];
                    float t = 0f;
                    for (int i = 0; i < Bounces; i++) {
                        s[i] = t;
                        float fall = MathF.Sqrt(2f * Heights[i] / Gravity);
                        t += i == 0 ? fall : 2f * fall;
                    }
                    s[Bounces] = t;
                    segStart = s;
                }
                return segStart;
            }
        }

        public static int BounceFrames => (int)MathF.Ceiling(SegStart[Bounces]);

        /// <summary>闪现后 t 帧时壳离落地点的高度,与已落地次数</summary>
        private static float HeightAt(float t, out int landed) {
            float[] s = SegStart;
            for (int i = 0; i < Bounces; i++) {
                if (t < s[i + 1]) {
                    landed = i;
                    float tau = t - s[i];
                    if (i == 0) {
                        return Math.Max(0f, Heights[0] - 0.5f * Gravity * tau * tau);
                    }
                    float v0 = MathF.Sqrt(2f * Gravity * Heights[i]);
                    return Math.Max(0f, v0 * tau - 0.5f * Gravity * tau * tau);
                }
            }
            landed = Bounces;
            return (t - s[Bounces]) * 3f;
        }

        public override void SetDefaults() {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = AppearFrames + BounceFrames + VanishFrames;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => contact > 0 && !Fading ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(nearest, Projectile.Center) <= HitRadius * HitRadius;
        }

        private float SlotX(NPC target) {
            float spread = MathHelper.Clamp(target.width * 0.3f, 10f, 56f);
            return target.Center.X + SlotOffsets[(int)Projectile.ai[1] % SlotOffsets.Length] * spread;
        }

        public override void AI() {
            Tick();
            NPC target = KeepTarget(false);
            if (Age == 1) {
                side = SideOf(target, Projectile.Center);
            }
            if (contact > 0) {
                contact--;
            }
            if (landFlash > 0) {
                landFlash--;
            }
            punch *= 0.8f;
            for (int i = ghosts.Length - 1; i > 0; i--) {
                ghosts[i] = ghosts[i - 1];
            }
            ghosts[0] = Projectile.Center;
            ghostCount = Math.Min(ghostCount + 1, ghosts.Length);

            if (!blinked) {
                //显形:投影口斜下方原地快转、由小长大
                Projectile.Center = AppearPoint(side, new Vector2(58f, 30f));
                spin += 0.5f * side;
                Projectile.rotation = spin;
                if (Age >= AppearFrames && target != null) {
                    Blink(target);
                }
                return;
            }
            if (target == null) {
                spin += 0.12f * side;
                Projectile.rotation = spin;
                return;
            }

            float t = Age - AppearFrames;
            float h = HeightAt(t, out int landed);
            x = MathHelper.Lerp(x, SlotX(target), 0.35f);
            if (landed > landings) {
                landings = landed;
                h = 0f;
                OnLand(target);
            }
            Projectile.Center = new Vector2(x, target.Top.Y - RestAbove - h);
            spin += (landed >= Bounces ? 0.12f : 0.3f) * side;
            Projectile.rotation = spin;
            if (landed >= Bounces && Projectile.timeLeft > VanishFrames) {
                Projectile.timeLeft = VanishFrames;
            }
            Lighting.AddLight(Projectile.Center, HoloColor.ToVector3() * 0.4f);
        }

        private void Blink(NPC target) {
            blinked = true;
            Vector2 from = Projectile.Center;
            x = SlotX(target);
            Projectile.Center = new Vector2(x, target.Top.Y - RestAbove - Heights[0]);
            ghostCount = 0;
            if (!Main.dedServ) {
                VDVfx.HoloBurst(from, HoloColor);
                VDVfx.HoloBurst(Projectile.Center, HoloColor);
                CEUtils.PlaySound("vbapear", Main.rand.NextFloat(1.15f, 1.3f), Projectile.Center, 4, 0.4f);
            }
        }

        private void OnLand(NPC target) {
            contact = ContactFrames;
            punch = 1f;
            landFlash = 8;
            landPoint = new Vector2(x, target.Top.Y);
            if (Main.dedServ) {
                return;
            }
            CEUtils.PlaySound("shellLand", Main.rand.NextFloat(0.95f, 1.1f) + 0.05f * landings, landPoint, 4, 0.4f);
            for (int i = 0; i < 6; i++) {
                float s = i % 2 == 0 ? 1f : -1f;
                Vector2 v = new Vector2(s * Main.rand.NextFloat(3f, 7f), -Main.rand.NextFloat(0.5f, 3f));
                VDVfx.Spark(landPoint, v, HoloColor, Main.rand.NextFloat(0.4f, 0.7f), 1f, Main.rand.Next(10, 16), gravity: true);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            float opacity = HoloOpacity();
            if (opacity <= 0.01f) {
                return false;
            }
            DrawProjectionCone(56f);
            Main.instance.LoadNPC(NPCID.GiantTortoise);
            Texture2D tex = TextureAssets.Npc[NPCID.GiantTortoise].Value;
            int frames = Math.Max(1, Main.npcFrameCount[NPCID.GiantTortoise]);
            int frameH = tex.Height / frames;
            //缩壳:显形前段走原版缩壳帧 5→6,之后一直是闭壳帧 7
            int frame = Math.Min(Age < 4 ? 5 : Age < 8 ? 6 : 7, frames - 1);
            Rectangle src = new Rectangle(0, frame * frameH, tex.Width, frameH);
            Vector2 origin = new Vector2(tex.Width / 2f, frameH / 2f);
            float grow = blinked ? 1f : MathHelper.Lerp(0.3f, 1f, VDVfx.EaseOut(Age / (float)AppearFrames));
            float vanish = Projectile.timeLeft < VanishFrames && landings >= Bounces ? Projectile.timeLeft / (float)VanishFrames : 1f;
            float scale = Scale * grow * (0.6f + 0.4f * vanish) * (1f + 0.12f * punch);
            SpriteEffects fx = side < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            VDWeaponFx.BeginAdditive();
            VDWeaponFx.Glow(Projectile.Center, HoloColor * (0.28f * opacity), 40f * scale);
            if (landFlash > 0) {
                float f = landFlash / 8f;
                VDWeaponFx.Ring(landPoint, HoloColor * (0.8f * f * opacity), MathHelper.Lerp(46f, 14f, f));
            }
            VDWeaponFx.End();

            VDHologramDraw.Begin();
            //落得快时拖两道残影
            if (blinked && ghostCount > 1 && Vector2.DistanceSquared(ghosts[0], ghosts[1]) > 100f) {
                for (int i = 1; i < ghostCount; i++) {
                    VDHologramDraw.DrawPart(tex, ghosts[i] - Main.screenPosition, src, HoloColor, opacity * (0.28f - i * 0.08f), Projectile.rotation - side * i * 0.3f, origin, scale, fx);
                }
            }
            VDHologramDraw.DrawPart(tex, Projectile.Center - Main.screenPosition, src, HoloColor, opacity, Projectile.rotation, origin, scale, fx);
            VDHologramDraw.End();
            return false;
        }
    }

    /// <summary>ai[1] 是四个站位之一;本体无判定,目标没了由拥有者换</summary>
    public class ProjectionRedDevil : ProjectionHoloBase
    {
        public const int Life = 240;
        public const int AppearFrames = 10;
        public const int ThrowInterval = 45;
        public const int FirstThrow = 28;
        public const int WindUp = 12;
        /// <summary>每次更新的速度(两次更新,每帧 48px)</summary>
        public const float TridentSpeed = 24f;
        /// <summary>四个站位(相对目标中心,大目标按体型外扩):上左、上右、平左、平右</summary>
        public static readonly Vector2[] Slots = { new(-150f, -120f), new(150f, -120f), new(-230f, -10f), new(230f, -10f) };

        public override int MaterializeFrames => AppearFrames;
        public override Color HoloColor => VDHologramDraw.HellRed;

        private float side = 1f;
        private int facing = 1;
        private float lurch;
        private float lurchVel;
        private float windup;
        private readonly Vector2[] ghosts = new Vector2[4];
        private int ghostCount;

        private Vector2 HandPos => Projectile.Center + new Vector2(facing * 16f, -10f);

        public override void SetDefaults() {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 40;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Life;
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI() {
            Tick();
            NPC target = KeepTarget(true);
            if (Age == 1) {
                side = SideOf(target, Projectile.Center);
                facing = (int)side;
            }
            for (int i = ghosts.Length - 1; i > 0; i--) {
                ghosts[i] = ghosts[i - 1];
            }
            ghosts[0] = Projectile.Center;
            ghostCount = Math.Min(ghostCount + 1, ghosts.Length);
            lurch += lurchVel;
            lurchVel *= 0.7f;
            lurch *= 0.8f;

            if (Age <= AppearFrames) {
                Projectile.Center = AppearPoint(side, new Vector2(64f, 18f));
                return;
            }
            if (target == null) {
                windup = 0f;
                return;
            }
            float dx = target.Center.X - Projectile.Center.X;
            if (Math.Abs(dx) > 8f) {
                facing = Math.Sign(dx);
            }
            //出手节拍:离下一投还剩几帧,最后 WindUp 帧蓄力
            int k = Age - FirstThrow;
            int untilThrow = k < 0 ? -k : (ThrowInterval - k % ThrowInterval) % ThrowInterval;
            windup = untilThrow <= WindUp && !Fading ? 1f - untilThrow / (float)WindUp : 0f;

            float grow = 1f + Math.Max(0f, Math.Max(target.width, target.height) - 60f) / 300f;
            Vector2 slot = Slots[(int)Projectile.ai[1] % Slots.Length] * grow;
            Vector2 toTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            Vector2 desired = target.Center + slot + new Vector2(0f, MathF.Sin(Age * 0.08f + Projectile.ai[1]) * 10f)
                - toTarget * (16f * windup * windup) + toTarget * lurch;
            Projectile.Center += (desired - Projectile.Center) * 0.2f;

            if (k >= 0 && k % ThrowInterval == 0 && !Fading) {
                Throw(target, toTarget);
            }
            Lighting.AddLight(Projectile.Center, HoloColor.ToVector3() * 0.4f);
        }

        private void Throw(NPC target, Vector2 dir) {
            Vector2 hand = HandPos;
            if (Projectile.owner == Main.myPlayer) {
                Vector2 aim = (target.Center + target.velocity * 5f - hand).SafeNormalize(dir);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), hand, aim * TridentSpeed, ModContent.ProjectileType<ProjectionTrident>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            lurchVel += 9f;
            if (!Main.dedServ) {
                CEUtils.PlaySound("throw", Main.rand.NextFloat(0.95f, 1.1f), hand, 4, 0.4f);
                for (int i = 0; i < 6; i++) {
                    VDVfx.Spark(hand, dir.RotatedByRandom(0.4f) * Main.rand.NextFloat(4f, 9f), HoloColor, Main.rand.NextFloat(0.35f, 0.6f), 1f, 12);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            float opacity = HoloOpacity(0.88f);
            if (opacity <= 0.01f) {
                return false;
            }
            DrawProjectionCone(56f);
            Main.instance.LoadNPC(NPCID.RedDevil);
            Texture2D tex = TextureAssets.Npc[NPCID.RedDevil].Value;
            int frames = Math.Max(1, Main.npcFrameCount[NPCID.RedDevil]);
            int frameH = tex.Height / frames;
            Rectangle src = new Rectangle(0, Age / 5 % frames * frameH, tex.Width, frameH);
            Vector2 origin = new Vector2(tex.Width / 2f, frameH / 2f);
            //原版贴图朝左,面朝右时翻转
            SpriteEffects fx = facing > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rot = -facing * 0.25f * windup * windup + facing * 0.012f * lurch;

            VDWeaponFx.BeginAdditive();
            VDWeaponFx.Glow(Projectile.Center, HoloColor * (0.26f * opacity), 46f);
            if (windup > 0.01f) {
                Vector2 hand = HandPos;
                VDWeaponFx.Glow(hand, new Color(255, 150, 110) * (0.9f * windup * opacity), 8f + 16f * windup);
                VDWeaponFx.Glow(hand, Color.White * (0.6f * windup * windup * opacity), 4f + 5f * windup);
            }
            VDWeaponFx.End();

            VDHologramDraw.Begin();
            //疾飞时拖残影
            if (ghostCount > 1 && Vector2.DistanceSquared(ghosts[0], ghosts[1]) > 64f) {
                for (int i = 1; i < ghostCount; i++) {
                    VDHologramDraw.DrawPart(tex, ghosts[i] - Main.screenPosition, src, HoloColor, opacity * (0.3f - i * 0.07f), rot, origin, 1f, fx);
                }
            }
            VDHologramDraw.DrawPart(tex, Projectile.Center - Main.screenPosition, src, HoloColor, opacity, rot, origin, 1f, fx);
            VDHologramDraw.End();
            return false;
        }
    }

    /// <summary>全息三叉戟(召唤):红恶魔掷出,两次更新直飞,一道红色曳光;命中或飞完在各端碎成全息碎片</summary>
    public class ProjectionTrident : ModProjectile
    {
        public const int TrailLength = 8;

        public override string Texture => "CalamityEntropy/Assets/Extra/Empty";

        private readonly Vector2[] trail = new Vector2[TrailLength];
        private int trailCount;

        public int Age => (int)Projectile.localAI[0];

        public override void SetStaticDefaults() {
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 600;
        }

        public override void SetDefaults() {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = -1;
        }

        public override void AI() {
            Projectile.localAI[0]++;
            //原版邪恶三叉戟贴图尖端朝右上
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Age % 2 == 1) {
                for (int i = TrailLength - 1; i > 0; i--) {
                    trail[i] = trail[i - 1];
                }
                trail[0] = Projectile.Center;
                trailCount = Math.Min(trailCount + 1, TrailLength);
            }
            Lighting.AddLight(Projectile.Center, VDHologramDraw.HellRed.ToVector3() * 0.4f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            return CEUtils.LineThroughRect(Projectile.Center - dir * 40f, Projectile.Center + dir * 16f, targetHitbox, 14);
        }

        public override void OnKill(int timeLeft) {
            if (Main.dedServ) {
                return;
            }
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 8; i++) {
                VDVfx.Spark(Projectile.Center, dir.RotatedByRandom(0.9f) * Main.rand.NextFloat(3f, 9f), VDHologramDraw.HellRed, Main.rand.NextFloat(0.4f, 0.7f), 1f, Main.rand.Next(10, 16), gravity: true);
            }
            if (timeLeft > 0) {
                CEUtils.PlaySound("vb_hit", Main.rand.NextFloat(1.25f, 1.4f), Projectile.Center, 4, 0.25f);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            float opacity = MathHelper.Clamp(Age / 6f, 0f, 1f) * MathHelper.Clamp(Projectile.timeLeft / 10f, 0f, 1f) * 0.95f;
            if (opacity <= 0.01f) {
                return false;
            }
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            if (trailCount > 1) {
                VDWeaponFx.BeginAdditive();
                VDWeaponFx.Streak(Projectile.Center + dir * 10f, trail[trailCount - 1] - dir * 20f, 10f * opacity, Color.White * opacity, VDHologramDraw.HellRed * opacity);
                VDWeaponFx.Glow(Projectile.Center, VDHologramDraw.HellRed * (0.45f * opacity), 18f);
                VDWeaponFx.End();
            }
            Main.instance.LoadProjectile(ProjectileID.UnholyTridentHostile);
            Texture2D tex = TextureAssets.Projectile[ProjectileID.UnholyTridentHostile].Value;
            VDHologramDraw.Draw(tex, Projectile.Center - Main.screenPosition, null, VDHologramDraw.HellRed, opacity, Projectile.rotation, tex.Size() / 2f, 1.1f, SpriteEffects.None);
            return false;
        }
    }

    /// <summary>
    /// 节距按原版 42px 折算,朝左水平翻转,腿不翻到背上
    /// 任一已出镜的节都算命中,每目标 20 帧一次
    /// 拥有者每 30 帧同步头部位置和速度
    /// </summary>
    public class ProjectionWyvern : ProjectionHoloBase
    {
        public const int Life = 300;
        public const float BodyScale = 0.75f;
        /// <summary>原版白龙节距 42px(NPC.cs 蠕虫跟随里 type 87~92 写死),按缩放折算</summary>
        public const float SegmentSpacing = 42f * BodyScale;
        public const float Speed = 21f;
        public const float ChargeTurn = 0.055f;
        public const float ReturnTurn = 0.17f;
        public const int OvershootFrames = 14;
        public const float HitRadius = 15f;
        /// <summary>原版节序(NPC.cs 白龙头生成身体的循环):头、身、腿、六节身、腿、两节身、身 2、身 3、尾</summary>
        public static readonly int[] SegmentTypes = {
            NPCID.WyvernHead, NPCID.WyvernBody, NPCID.WyvernLegs,
            NPCID.WyvernBody, NPCID.WyvernBody, NPCID.WyvernBody, NPCID.WyvernBody, NPCID.WyvernBody, NPCID.WyvernBody,
            NPCID.WyvernLegs, NPCID.WyvernBody, NPCID.WyvernBody, NPCID.WyvernBody2, NPCID.WyvernBody3, NPCID.WyvernTail
        };

        public override int MaterializeFrames => 8;
        public override Color HoloColor => VDHologramDraw.SkyBlue;

        private Vector2[] segments;
        private bool[] emerged;
        private Vector2 spawnPoint;
        /// <summary>0 冲撞,1 越过后直飞,2 掉头</summary>
        private int mode;
        private int modeTimer;
        private float speedMult = 0.6f;

        public override void SetDefaults() {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Life;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI() {
            Tick();
            NPC target = KeepTarget(true);
            int count = SegmentTypes.Length;
            if (segments == null) {
                segments = new Vector2[count];
                emerged = new bool[count];
                spawnPoint = Projectile.Center;
                for (int i = 0; i < count; i++) {
                    segments[i] = spawnPoint;
                }
                Vector2 dir = target != null ? (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) : Vector2.UnitY;
                Projectile.velocity = dir;
                if (!Main.dedServ) {
                    CEUtils.PlaySound("CruiserDash", 1.15f, Projectile.Center, 2, 0.35f);
                }
            }
            if (target != null) {
                Steer(target);
            }
            speedMult = MathHelper.Lerp(speedMult, mode == 2 ? 0.8f : 1f, 0.12f);
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * Speed * speedMult;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.owner == Main.myPlayer && Age % 30 == 0) {
                Projectile.netUpdate = true;
            }

            //蠕虫跟随:头用本帧移动后的位置。还没出镜的节留在无人机当前的投影口里,前一节离投影口超过节距才把它拉出来;
            //出镜后同原版,钉在前一节后方定长处
            ProjectionDrone drone = Drone;
            if (drone != null) {
                spawnPoint = drone.ApertureWorld;
            }
            segments[0] = Projectile.Center + Projectile.velocity;
            emerged[0] = true;
            for (int i = 1; i < count; i++) {
                if (!emerged[i]) {
                    segments[i] = spawnPoint;
                }
                Vector2 diff = segments[i] - segments[i - 1];
                float len = diff.Length();
                if (!emerged[i]) {
                    if (len > SegmentSpacing) {
                        segments[i] = segments[i - 1] + diff / len * SegmentSpacing;
                        emerged[i] = true;
                    }
                    continue;
                }
                if (len < 0.01f) {
                    diff = -Projectile.velocity;
                    len = diff.Length();
                }
                segments[i] = segments[i - 1] + diff / Math.Max(len, 0.01f) * SegmentSpacing;
            }
            Lighting.AddLight(Projectile.Center, HoloColor.ToVector3() * 0.5f);
            if (!Main.dedServ && Main.rand.NextBool(2)) {
                int idx = Main.rand.Next(count);
                if (emerged[idx]) {
                    VDVfx.Spark(segments[idx] + CEUtils.randomPointInCircle(10f), CEUtils.randomPointInCircle(1.2f), HoloColor, Main.rand.NextFloat(0.3f, 0.55f), 0.8f, 14);
                }
            }
        }

        private void Steer(NPC target) {
            Vector2 toTarget = target.Center - Projectile.Center;
            float cur = Projectile.velocity.ToRotation();
            float diff = MathHelper.WrapAngle(toTarget.ToRotation() - cur);
            switch (mode) {
                case 0: {
                    Projectile.velocity = (cur + MathHelper.Clamp(diff, -ChargeTurn, ChargeTurn)).ToRotationVector2();
                    if (Vector2.Dot(toTarget, Projectile.velocity) < 0f && toTarget.Length() > 90f) {
                        mode = 1;
                        modeTimer = 0;
                    }
                    break;
                }
                case 1: {
                    if (++modeTimer >= OvershootFrames) {
                        mode = 2;
                        modeTimer = 0;
                    }
                    break;
                }
                default: {
                    Projectile.velocity = (cur + MathHelper.Clamp(diff, -ReturnTurn, ReturnTurn)).ToRotationVector2();
                    if (Math.Abs(diff) < 0.3f || ++modeTimer > 40) {
                        mode = 0;
                        if (!Main.dedServ) {
                            CEUtils.PlaySound("CruiserDash", Main.rand.NextFloat(1.05f, 1.2f), Projectile.Center, 2, 0.3f);
                        }
                    }
                    break;
                }
            }
        }

        public override bool? CanDamage() => Age > 6 && !Fading ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (segments == null) {
                return false;
            }
            for (int i = 0; i < segments.Length; i++) {
                if (!emerged[i]) {
                    continue;
                }
                float r = i == 0 ? HitRadius * 1.3f : HitRadius;
                Vector2 nearest = Vector2.Clamp(segments[i], targetHitbox.TopLeft(), targetHitbox.BottomRight());
                if (Vector2.DistanceSquared(nearest, segments[i]) <= r * r) {
                    return true;
                }
            }
            return false;
        }

        public override void OnKill(int timeLeft) {
            if (Main.dedServ || segments == null) {
                return;
            }
            for (int i = 0; i < segments.Length; i += 4) {
                if (emerged[i]) {
                    VDVfx.HoloBurst(segments[i], HoloColor);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            if (segments == null) {
                return false;
            }
            float opacity = HoloOpacity(0.85f);
            if (opacity <= 0.01f) {
                return false;
            }
            DrawProjectionCone(40f);
            for (int i = 0; i < SegmentTypes.Length; i++) {
                Main.instance.LoadNPC(SegmentTypes[i]);
            }
            VDHologramDraw.Begin();
            //尾先画,头压在最上面
            for (int i = segments.Length - 1; i >= 0; i--) {
                if (!emerged[i]) {
                    continue;
                }
                Texture2D tex = TextureAssets.Npc[SegmentTypes[i]].Value;
                Vector2 here = segments[i];
                Vector2 ahead = i == 0 ? here + Projectile.velocity : segments[i - 1];
                Vector2 d = ahead - here;
                float rot = d.ToRotation() + MathHelper.PiOver2;
                SpriteEffects fx = d.X < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                VDHologramDraw.DrawPart(tex, here - Main.screenPosition, null, HoloColor, opacity, rot, tex.Size() / 2f, BodyScale, fx);
            }
            VDHologramDraw.End();
            return false;
        }
    }
}
