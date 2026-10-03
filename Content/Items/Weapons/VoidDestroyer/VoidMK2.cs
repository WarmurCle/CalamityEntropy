using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.NPCs.VoidDestroyer;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Particles.CalamityPorts;
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
    /// 虚空 MK2:虚空驱逐舰掉落的机枪。手持弹幕持枪(握把在手),逐发自取弹药,射出的一律是专用虚空弹头。
    /// 持续开火累计 <see cref="DroneCharge"/> 帧,机匣顶部的发射模块放出一架追踪无人机,贴到敌怪爆炸并施加 3 秒虚空之火;充能跨点射保留
    /// </summary>
    public class VoidMK2 : ModItem
    {
        public const int DroneCharge = 60;
        public const float DroneDamageMult = 2.5f;
        /// <summary>无人机充能,只在拥有者本端维护;手持弹幕生成时经 ai[1] 带给各端,收枪时写回</summary>
        internal int droneCharge;

        public override bool RangedPrefix() => true;

        public override void SetDefaults() {
            Item.width = 102;
            Item.height = 30;
            Item.damage = 105;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(platinum: 2, gold: 50);
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VoidMK2Held>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = 6;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(2);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            Projectile.NewProjectile(source, player.MountedCenter, velocity.SafeNormalize(Vector2.UnitX * player.direction), Item.shoot, damage, knockback, player.whoAmI, 0f, droneCharge);
            return false;
        }
    }

    /// <summary>
    /// MK2 手持弹幕:按住即连射,射击间隔取物品使用时间(吃射速加成)。首发用使用时已扣的那发弹药,之后每发由拥有者自取。
    /// 表现全是冲量 + 衰减:枪身沿枪管后坐、枪口上跳、抛壳、三帧枪口焰;持续开火加热枪管下的线圈,顶部发射模块随无人机充能变亮。
    /// ai[1] 为无人机充能(帧),各端同步累加,只有拥有者生成无人机
    /// </summary>
    public class VoidMK2Held : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/VoidDestroyer/VoidMK2";

        //发光层(贴图里的品红能量件),加载期由 VaultLoaden 赋值,仅绘制路径读取
        [VaultLoaden("CalamityEntropy/Content/Items/Weapons/VoidDestroyer/VoidMK2_Glow")]
        internal static Asset<Texture2D> GlowMask;

        //贴图锚点(像素坐标,贴图朝右)
        public static readonly Vector2 Grip = new Vector2(46f, 47f);
        public static readonly Vector2 Muzzle = new Vector2(203f, 27.5f);
        public static readonly Vector2 EjectPort = new Vector2(72f, 18f);
        public static readonly Vector2 Launcher = new Vector2(95f, 6f);
        public static readonly Vector2 CoilCenter = new Vector2(173f, 39f);
        public static readonly Rectangle LauncherRect = new Rectangle(80, 4, 30, 18);
        public static readonly Rectangle CoilRect = new Rectangle(150, 32, 46, 14);

        public static readonly Color TracerColor = new Color(215, 95, 255);

        public const float RecoilKick = 4.5f;
        public const float ClimbImpulse = 0.016f;
        public const float HeatPerShot = 0.03f;

        private bool initialized;
        private float aim;
        private int fireTimer;
        private int shots;
        private float kick;
        private float climb;
        private float climbVel;
        private float heat;
        private int flash;
        private float flashRot;
        private int launchFlash;

        public float DroneCharge { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public override void SetDefaults() {
            Projectile.HeldProjSetDefaults(DamageClass.Ranged);
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool? CanCutTiles() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI() {
            Player player = Projectile.GetOwner();
            bool isOwner = Projectile.owner == Main.myPlayer;
            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<VoidMK2>()) {
                Projectile.Kill();
                return;
            }
            //松手只由拥有者判定(远端的 channel 可能滞后),远端等拥有者的销毁包
            if (isOwner && (!player.channel || player.CCed || player.noItems)) {
                Projectile.Kill();
                return;
            }
            if (isOwner) {
                player.Entropy().MouseWorldListener = true;
            }
            float want = (player.Entropy().MouseWorld - player.MountedCenter).ToRotation();
            if (!initialized) {
                initialized = true;
                aim = want;
            }
            aim = CEUtils.RotateTowardsAngle(aim, want, 0.45f, false);
            player.ChangeDir(MathF.Cos(aim) >= 0f ? 1 : -1);

            Projectile.timeLeft = 3;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 3;

            kick *= 0.72f;
            climb += climbVel;
            climbVel *= 0.55f;
            climb *= 0.84f;
            if (flash > 0) {
                flash--;
            }
            if (launchFlash > 0) {
                launchFlash--;
            }

            if (--fireTimer <= 0) {
                fireTimer = CombinedHooks.TotalUseTime(player.HeldItem.useTime, player, player.HeldItem);
                if (!Fire(player)) {
                    Projectile.Kill();
                    return;
                }
            }

            DroneCharge++;
            if (DroneCharge >= VoidMK2.DroneCharge) {
                DroneCharge = 0f;
                LaunchDrone(player);
            }

            ApplyPose(player);
            Projectile.Center = player.MountedCenter;
            HeldSprite gun = GunSprite(player);
            if (flash > 0) {
                Lighting.AddLight(gun.ToWorld(Muzzle), VDVfx.VoidPink.ToVector3() * (0.35f * flash));
            }
            if (heat > 0.05f) {
                Lighting.AddLight(gun.ToWorld(CoilCenter), TracerColor.ToVector3() * (0.5f * heat));
            }
        }

        /// <summary>当前持枪姿态:握点钉在前手,枪身沿枪管后坐、枪口上跳</summary>
        private HeldSprite GunSprite(Player player) {
            int facing = player.direction;
            float rot = aim - climb * facing;
            float armRot = aim + 0.42f * facing - MathHelper.PiOver2;
            Vector2 hand = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRot) + new Vector2(0f, player.gfxOffY);
            Vector2 world = hand - rot.ToRotationVector2() * kick;
            return HeldSprite.Gun(TextureAssets.Projectile[Type].Value, Grip, world, rot, facing);
        }

        private void ApplyPose(Player player) {
            int facing = player.direction;
            float armRot = aim + 0.42f * facing - MathHelper.PiOver2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, aim + 0.12f * facing - MathHelper.PiOver2);
            player.itemRotation = MathHelper.WrapAngle(aim + (facing < 0 ? MathHelper.Pi : 0f));
        }

        /// <summary>开一枪:拥有者取弹并生成弹头(取不到弹 = 收枪),各端各自播后坐、抛壳、枪口焰与枪声</summary>
        private bool Fire(Player player) {
            bool owner = Projectile.owner == Main.myPlayer;
            if (owner && shots > 0) {
                if (!player.PickAmmo(player.HeldItem, out _, out _, out int damage, out float knockback, out _)) {
                    return false;
                }
                Projectile.damage = damage;
                Projectile.knockBack = knockback;
            }
            HeldSprite gun = GunSprite(player);
            Vector2 dir = aim.ToRotationVector2();
            Vector2 muzzle = gun.ToWorld(Muzzle);
            if (owner) {
                Vector2 spawn = Collision.CanHitLine(player.MountedCenter, 1, 1, muzzle, 1, 1) ? muzzle : player.MountedCenter;
                Vector2 vel = dir.RotatedByRandom(0.04f) * player.HeldItem.shootSpeed;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, vel, ModContent.ProjectileType<VoidMK2Bullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            shots++;
            kick += RecoilKick;
            climbVel += ClimbImpulse;
            heat = Math.Min(1f, heat + HeatPerShot);
            flash = 3;
            flashRot = Main.rand.NextFloat(MathHelper.TwoPi);

            if (!Main.dedServ) {
                CEUtils.PlaySound("gunshot_small" + (1 + shots % 3), Main.rand.NextFloat(1.2f, 1.45f), muzzle, 16, 0.32f);
                int facing = player.direction;
                Vector2 casingVel = dir.RotatedBy(-facing * 2.2f) * Main.rand.NextFloat(3.5f, 5.5f) + player.velocity * 0.6f + CEUtils.randomPointInCircle(1f);
                PRTLoader.NewParticle<PRT_ShellParticle>(gun.ToWorld(EjectPort), casingVel, Color.White, 1f).Configure(1f, false, PRTDrawModeEnum.AlphaBlend, CEUtils.randomRot());
                for (int i = 0; i < 2; i++) {
                    VDVfx.Spark(muzzle, dir.RotatedByRandom(0.3f) * Main.rand.NextFloat(5f, 11f), VDVfx.VoidPink, Main.rand.NextFloat(0.3f, 0.5f), 1f, Main.rand.Next(5, 8));
                }
            }
            return true;
        }

        /// <summary>顶部模块弹出一架无人机:模块爆闪、枪口多跳一下、一团排气烟</summary>
        private void LaunchDrone(Player player) {
            HeldSprite gun = GunSprite(player);
            Vector2 at = gun.ToWorld(Launcher);
            Vector2 dir = aim.ToRotationVector2();
            Vector2 up = -Vector2.UnitY;
            if (Projectile.owner == Main.myPlayer) {
                Vector2 vel = (dir * 0.55f + up * 0.85f).SafeNormalize(up) * 9f + player.velocity * 0.4f;
                int damage = (int)(Projectile.damage * VoidMK2.DroneDamageMult);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), at, vel, ModContent.ProjectileType<VoidMK2Drone>(), damage, Projectile.knockBack * 2f, Projectile.owner, -1f);
            }
            launchFlash = 10;
            climbVel += 0.03f;
            kick += 3f;
            if (Main.dedServ) {
                return;
            }
            CEUtils.PlaySound("pulseBlast", 1.35f, at, 4, 0.45f);
            for (int i = 0; i < 3; i++) {
                Vector2 v = (up.RotatedByRandom(0.7f) - dir * 0.4f) * Main.rand.NextFloat(1.5f, 3f);
                PRTLoader.NewParticle<PRT_HeavySmokeCal>(at, v, new Color(120, 105, 140), Main.rand.NextFloat(0.35f, 0.5f)).Configure(0.6f, 34, Main.rand.NextFloat(-0.02f, 0.02f));
            }
            for (int i = 0; i < 6; i++) {
                VDVfx.Spark(at, up.RotatedByRandom(0.6f) * Main.rand.NextFloat(3f, 8f), VDVfx.VoidPink, Main.rand.NextFloat(0.35f, 0.6f), 1f, 12);
            }
        }

        public override void OnKill(int timeLeft) {
            Player player = Projectile.GetOwner();
            if (Projectile.owner == Main.myPlayer && player.HeldItem.ModItem is VoidMK2 mk2) {
                mk2.droneCharge = (int)DroneCharge;
            }
            player.itemTime = player.itemAnimation = 0;
            if (Main.dedServ || heat < 0.4f || !initialized) {
                return;
            }
            //长连射后松手:线圈冒几缕热烟
            HeldSprite gun = GunSprite(player);
            Vector2 coil = gun.ToWorld(CoilCenter);
            int puffs = (int)(3 + 5 * heat);
            for (int i = 0; i < puffs; i++) {
                Vector2 at = coil + gun.Rotation.ToRotationVector2() * Main.rand.NextFloat(-24f, 24f);
                Vector2 v = new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), -Main.rand.NextFloat(0.8f, 1.8f));
                PRTLoader.NewParticle<PRT_HeavySmokeCal>(at, v, new Color(110, 95, 130), Main.rand.NextFloat(0.25f, 0.4f)).Configure(0.45f * heat, 50, Main.rand.NextFloat(-0.015f, 0.015f));
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Player player = Projectile.GetOwner();
            if (!initialized) {
                return false;
            }
            HeldSprite gun = GunSprite(player);
            Texture2D glow = GlowMask.Value;
            gun.Draw(lightColor);
            gun.Draw(glow, Color.White);

            VDWeaponFx.BeginAdditive();
            if (heat > 0.02f) {
                gun.DrawPart(glow, CoilRect, Color.White * (0.9f * heat));
                VDWeaponFx.GlowStretched(gun.ToWorld(CoilCenter), TracerColor * (0.4f * heat), gun.Rotation, 76f, 22f);
            }
            float charge = MathHelper.Clamp(DroneCharge / VoidMK2.DroneCharge, 0f, 1f);
            float lf = launchFlash / 10f;
            gun.DrawPart(glow, LauncherRect, Color.White * Math.Max(charge * charge, lf));
            VDWeaponFx.Glow(gun.ToWorld(Launcher + new Vector2(0f, 6f)), VDVfx.VoidPink * (0.1f + 0.4f * charge * charge + 0.6f * lf), 12f + 14f * lf);
            if (flash > 0) {
                float f = flash / 3f;
                Vector2 muzzle = gun.ToWorld(Muzzle);
                Vector2 dir = gun.Rotation.ToRotationVector2();
                VDWeaponFx.GlowStretched(muzzle + dir * 16f, VDVfx.VoidPink * (0.9f * f), gun.Rotation, 50f * (0.7f + 0.3f * f), 18f);
                VDWeaponFx.GlowStretched(muzzle + dir * 8f, Color.White * (0.85f * f), gun.Rotation, 24f, 8f);
                Texture2D star = CEUtils.getExtraTex("Star2");
                Main.spriteBatch.Draw(star, muzzle - Main.screenPosition, null, VDVfx.VoidWhite * f, flashRot, star.Size() / 2f, 0.3f * (0.6f + 0.4f * f), SpriteEffects.None, 0f);
            }
            VDWeaponFx.End();
            return false;
        }
    }

    /// <summary>虚空 MK2 弹头:三次更新的高速直飞,头部贴图 + 一道头亮尾淡的曳光;命中与撞墙的火花在 OnKill(各端都跑)</summary>
    public class VoidMK2Bullet : ModProjectile
    {
        public const int TrailPoints = 6;
        private readonly Vector2[] trail = new Vector2[TrailPoints];
        private int trailCount;

        public override void SetDefaults() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = TrailPoints - 1; i > 0; i--) {
                trail[i] = trail[i - 1];
            }
            trail[0] = Projectile.Center;
            trailCount = Math.Min(trailCount + 1, TrailPoints);
            Lighting.AddLight(Projectile.Center, VoidMK2Held.TracerColor.ToVector3() * 0.25f);
        }

        public override void OnKill(int timeLeft) {
            if (Main.dedServ || timeLeft <= 0) {
                return;
            }
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 3; i++) {
                VDVfx.Spark(Projectile.Center, (-dir).RotatedByRandom(1.1f) * Main.rand.NextFloat(3f, 7f), VDVfx.VoidPink, Main.rand.NextFloat(0.3f, 0.45f), 1f, Main.rand.Next(8, 12), gravity: true);
            }
            PRTLoader.NewParticle<PRT_ShineParticle>(Projectile.Center, Vector2.Zero, VoidMK2Held.TracerColor, 0.12f).Configure(1f, true, PRTDrawModeEnum.AdditiveBlend, 0f, 8);
            CEUtils.PlaySound("beast_lavaball_rise1", Main.rand.NextFloat(2.3f, 2.7f), Projectile.Center, 30, 0.22f);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 head = Projectile.Center + dir * 6f;
            Vector2 tail = trailCount > 1 ? trail[trailCount - 1] - dir * 8f : Projectile.Center - dir * 30f;

            VDWeaponFx.BeginAdditive();
            VDWeaponFx.Streak(head, tail, 6f, Color.White, VoidMK2Held.TracerColor);
            VDWeaponFx.Glow(head, VoidMK2Held.TracerColor * 0.5f, 10f);
            VDWeaponFx.End();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>
    /// 虚空 MK2 追踪无人机(借驱逐舰孢子无人机贴图,贴图朝上):弹出后减速展开 <see cref="DeployFrames"/> 帧并锁定,
    /// 锁定目标由拥有者选(光标附近优先)并写进 ai[0] 同步,之后限角速度转向、加速扑去;贴到敌怪、撞墙或超时都爆炸。
    /// 机体不判伤,伤害全由 <see cref="VoidMK2Explosion"/> 结算
    /// </summary>
    public class VoidMK2Drone : ModProjectile
    {
        public const int DeployFrames = 14;
        /// <summary>每次更新的巡航速度(两次更新,实际每帧两倍)</summary>
        public const float CruiseSpeed = 11f;
        public const float TurnRate = 0.085f;
        public const int TrailLength = 14;

        public override string Texture => "CalamityEntropy/Content/Projectiles/VoidDestroyer/VDSporeDrone";

        private readonly List<Vector2> trail = new List<Vector2>();
        private int lockFlash;

        public int Age => (int)Projectile.localAI[0];
        public int Frames => Age / (Projectile.extraUpdates + 1);

        public override void SetDefaults() {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

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

        /// <summary>光标 600px 内离光标最近的敌怪优先,否则取离无人机最近的</summary>
        private int PickTarget() {
            Vector2 mouse = Projectile.GetOwner().Entropy().MouseWorld;
            int best = -1;
            float bestD = 600f;
            foreach (NPC n in Main.ActiveNPCs) {
                if (!n.CanBeChasedBy(Projectile)) {
                    continue;
                }
                float d = Vector2.Distance(n.Center, mouse);
                if (d < bestD) {
                    bestD = d;
                    best = n.whoAmI;
                }
            }
            if (best >= 0) {
                return best;
            }
            bestD = 1400f;
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
            return best;
        }

        public override void AI() {
            Projectile.localAI[0]++;
            bool owner = Projectile.owner == Main.myPlayer;
            Projectile.tileCollide = Frames > 4;

            if (Frames < DeployFrames) {
                Projectile.velocity *= 0.955f;
            }
            else {
                if (Frames == DeployFrames && Age % (Projectile.extraUpdates + 1) == 0) {
                    lockFlash = 16;
                    if (!Main.dedServ) {
                        CEUtils.PlaySound("vbapear", 1.6f, Projectile.Center, 6, 0.35f);
                    }
                }
                if (owner && Target == null && Age % 10 == 0) {
                    int t = PickTarget();
                    if (t != (int)Projectile.ai[0]) {
                        Projectile.ai[0] = t;
                        Projectile.netUpdate = true;
                    }
                }
                NPC target = Target;
                float speed = Projectile.velocity.Length();
                if (target != null) {
                    float cur = Projectile.velocity.ToRotation();
                    float want = (target.Center - Projectile.Center).ToRotation();
                    float turn = MathHelper.Clamp(MathHelper.WrapAngle(want - cur), -TurnRate, TurnRate);
                    speed = Math.Min(speed * 1.035f + 0.12f, CruiseSpeed);
                    Projectile.velocity = (cur + turn).ToRotationVector2() * speed;
                }
                else {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY) * Math.Min(speed * 1.01f + 0.05f, 8f);
                }
                if (owner && TouchingEnemy()) {
                    Projectile.Kill();
                    return;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (lockFlash > 0) {
                lockFlash--;
            }

            Vector2 back = -Projectile.velocity.SafeNormalize(Vector2.UnitY);
            if (Age % 2 == 0) {
                trail.Insert(0, Projectile.Center + back * 12f);
                if (trail.Count > TrailLength) {
                    trail.RemoveAt(trail.Count - 1);
                }
            }
            Lighting.AddLight(Projectile.Center, VDVfx.VoidPink.ToVector3() * 0.45f);
            if (!Main.dedServ && Main.rand.NextBool(4)) {
                VDVfx.Spark(Projectile.Center + back * 12f, back * Main.rand.NextFloat(1.5f, 4f) + CEUtils.randomPointInCircle(0.8f), VDVfx.VoidPink, Main.rand.NextFloat(0.3f, 0.5f), 0.9f, 12);
            }
        }

        private bool TouchingEnemy() {
            Rectangle box = Projectile.Hitbox;
            foreach (NPC n in Main.ActiveNPCs) {
                if (n.CanBeChasedBy(Projectile) && box.Intersects(n.Hitbox)) {
                    return true;
                }
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => true;

        public override void OnKill(int timeLeft) {
            if (Projectile.owner == Main.myPlayer) {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VoidMK2Explosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 back = -Projectile.velocity.SafeNormalize(Vector2.UnitY);
            float thrust = MathHelper.Clamp(Projectile.velocity.Length() / CruiseSpeed, 0.35f, 1f);

            VDWeaponFx.BeginAdditive();
            if (trail.Count > 2) {
                VDWeaponFx.Ribbon(trail, t => (10f - 8f * t) * thrust, t => Color.Lerp(Color.White, VDVfx.VoidPurple, t) * ((1f - t) * 0.8f));
            }
            VDWeaponFx.GlowStretched(Projectile.Center + back * 13f, VDVfx.VoidPink * (0.8f * thrust), back.ToRotation(), 30f * thrust, 12f);
            VDWeaponFx.Glow(Projectile.Center, VDVfx.VoidPurple * 0.35f, 26f);
            VDWeaponFx.End();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 0.75f, SpriteEffects.None, 0f);
            if (lockFlash > 0) {
                VDWeaponFx.BeginAdditive();
                float f = lockFlash / 16f;
                VDWeaponFx.Glow(Projectile.Center, Color.White * f, 10f + 18f * (1f - f));
                VDWeaponFx.Ring(Projectile.Center, VDVfx.VoidPink * f, 8f + 22f * (1f - f));
                VDWeaponFx.End();
            }
            return false;
        }
    }

    /// <summary>无人机爆炸:半径 <see cref="Radius"/> 的圆形判定,前 <see cref="DamageFrames"/> 帧有效,命中施加 3 秒虚空之火;首帧在各端放演出</summary>
    public class VoidMK2Explosion : ModProjectile
    {
        public const float Radius = 90f;
        public const int Lifetime = 16;
        public const int DamageFrames = 4;

        public override string Texture => "CalamityEntropy/Assets/Extra/Empty";

        public int Age => (int)Projectile.localAI[0];

        public override void SetDefaults() {
            Projectile.width = (int)(Radius * 2f);
            Projectile.height = (int)(Radius * 2f);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => Age <= DamageFrames ? null : false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(nearest, Projectile.Center) <= Radius * Radius;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<VoidFire>(), 180);
        }

        public override void AI() {
            Projectile.localAI[0]++;
            if (Age != 1) {
                return;
            }
            Lighting.AddLight(Projectile.Center, VDVfx.VoidPink.ToVector3() * 1.2f);
            if (Main.dedServ) {
                return;
            }
            CEUtils.PlaySound("VoidBomb", Main.rand.NextFloat(1.1f, 1.25f), Projectile.Center, 4, 0.55f);
            CEUtils.SetShake(Projectile.Center, 3f, 1600f);
            VDVfx.SparkBurst(Projectile.Center, VDVfx.VoidPink, 12, 4f, 11f, 22, 0.45f, 0.85f);
            VDVfx.SparkBurst(Projectile.Center, Color.White, 5, 3f, 7f, 14, 0.35f, 0.6f);
            for (int i = 0; i < 8; i++) {
                Vector2 v = CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(2f, 5f);
                VDVfx.VoidPuff(Projectile.Center + v * 3f, v, 1.1f, 0.6f);
            }
            for (int i = 0; i < 5; i++) {
                Vector2 v = CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(1f, 3f);
                PRTLoader.NewParticle<PRT_HeavySmokeCal>(Projectile.Center + v * 6f, v, new Color(70, 45, 95), Main.rand.NextFloat(0.5f, 0.8f)).Configure(0.7f, 40, Main.rand.NextFloat(-0.03f, 0.03f));
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            float life = Age / (float)Lifetime;
            VDWeaponFx.BeginAdditive();
            if (Age < 6) {
                float f = 1f - Age / 6f;
                VDWeaponFx.Glow(Projectile.Center, Color.White * f, Radius * 0.7f * (0.6f + 0.4f * f));
            }
            VDWeaponFx.Glow(Projectile.Center, VDVfx.VoidPurple * (0.75f * (1f - life)), Radius * 1.4f);
            float ring = MathHelper.Lerp(0.25f, 1.1f, VDVfx.EaseOut(life)) * Radius;
            VDWeaponFx.Ring(Projectile.Center, Color.Lerp(VDVfx.VoidPink, VDVfx.VoidPurple, life) * (1f - life), ring);
            VDWeaponFx.End();
            return false;
        }
    }
}
