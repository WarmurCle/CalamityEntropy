
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Util;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityEntropy.Content.Items.Books
{
    public abstract class EntropyBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 64;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 26;
            Item.width = Item.height = 40;
            Item.knockBack = 1.4f;
            Item.crit = 4;
            Item.mana = 8;
            Item.rare = ItemRarityID.Orange;
        }
        public virtual int HeldProjectileType => -1;
        public virtual int SlotCount => 6;
        public virtual Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark1").Value;
        public override void UpdateInventory(Player player)
        {
            if(player.HeldItem == Item)
            {
                if (player.ownedProjectileCounts[HeldProjectileType] <= 0)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, HeldProjectileType, 0, 0, player.whoAmI, Item.type);
                }
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Calamity Entropy: Entropy Book Info", Mod.GetLocalization("EBookTooltip").Value) { OverrideColor = Color.Yellow });
        }
    }

    public class EBookStatModifer
    {
        public float Damage = 1;
        public float Knockback = 1;
        public float shotSpeed = 1;
        public float Homing = 0;
        public float Size = 1;
        public float Crit = 0;
        public float HomingRange = 1;
        public int PenetrateAddition = 0;
        public float attackSpeed = 1;
        public int armorPenetration = 0;
    }

    public abstract class EntropyBookHeldProjectile : ModProjectile
    {
        public int ItemType => (int)Projectile.ai[0];
        public int openAnim = 0;
        public bool UIOpen = false;
        public int UIOpenAnm = 0;
        public int shotCooldown = 0;
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = Projectile.width = 8;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public virtual EBookStatModifer getBaseModifer()
        {
            EBookStatModifer modifer = new EBookStatModifer() { Damage = 1, Knockback = 0, Crit = Projectile.getOwner().GetTotalCritChance(Projectile.DamageType)};
            return modifer;
        }

        public virtual string OpenAnimationPath => "";
        public virtual Texture2D[] OpenAnimations()
        {
            Texture2D[] texs = new Texture2D[3];
            for (int i = 0; i < 3; i++)
            {
                texs[i] = ModContent.Request<Texture2D>(OpenAnimationPath + i.ToString(), AssetRequestMode.ImmediateLoad).Value;
            }
            return texs;
        }
        public virtual string PageAnimationPath => "";
        public virtual Texture2D[] PageAnimations()
        {
            Texture2D[] texs = new Texture2D[5];
            for (int i = 0; i < 5; i++)
            {
                texs[i] = ModContent.Request<Texture2D>(PageAnimationPath + i.ToString(), AssetRequestMode.ImmediateLoad).Value;
            }
            return texs;
        }
        public virtual string UIOpenAnimationPath => "";
        public virtual Texture2D[] UIOpenAnimations()
        {
            Texture2D[] texs = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                texs[i] = ModContent.Request<Texture2D>(UIOpenAnimationPath + i.ToString(), AssetRequestMode.ImmediateLoad).Value;
            }
            return texs;
        }
        public int pageTurnAnm = 0;
        public virtual void playTurnPageAnimation()
        {
            Util.Util.PlaySound("pageflip", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center, 4, 0.5f);
            pageTurnAnm = 4;
            Projectile.frameCounter = 0;
        }
        public bool active = false;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(active);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            active = reader.ReadBoolean();
        }

        public virtual Texture2D getTexture()
        {
            if (UIOpen)
            {
                return UIOpenAnimations()[UIOpenAnm];
            }
            else
            {
                if (openAnim < 2)
                {
                    return OpenAnimations()[openAnim];
                }
                else
                {
                    return PageAnimations()[pageTurnAnm];
                }
            }
        }
        public virtual void setVel()
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            Vector2 newVel = (Main.MouseWorld - Projectile.getOwner().Center).SafeNormalize(Vector2.UnitX);
            if(Projectile.velocity != newVel)
            {
                Projectile.velocity = newVel;
                Projectile.netUpdate = true;
            }
        }
        public virtual Vector2 heldOffset => new Vector2(14, 6);
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public virtual int baseProjectileType => ModContent.ProjectileType<RuneBullet>();
        public virtual int getShootProjectileType()
        {
            return baseProjectileType;
        }
        public virtual bool Shoot()
        {
            int type = getShootProjectileType();
            ShootSingleProjectile(type, Projectile.Center, Projectile.velocity);
            return true;
        }
        public virtual void ShootSingleProjectile(int type, Vector2 pos, Vector2 velocity, float damageMul = 1)
        {
            EBookStatModifer modifer = getBaseModifer();
            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, Main.LocalPlayer.HeldItem), EBookUI.stackItems.Count); i++)
            {
                if (EBookUI.stackItems[i].ModItem is BookMark bm)
                {
                    bm.ModiferStat(modifer);
                }
            }
            Projectile proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, velocity.normalize() * Projectile.getOwner().HeldItem.shootSpeed * modifer.shotSpeed, type, (int)(Projectile.getOwner().GetTotalDamage(Projectile.DamageType).ApplyTo(Projectile.getOwner().HeldItem.damage * modifer.Damage * damageMul * (Projectile.Entropy().ttindex < 0 ? 1 : TwistedTwinMinion.damageMul))), Projectile.getOwner().GetTotalKnockback(Projectile.DamageType).ApplyTo(Projectile.getOwner().HeldItem.knockBack * modifer.Knockback), Projectile.owner).ToProj();
            proj.penetrate += modifer.PenetrateAddition;
            proj.CritChance = Projectile.getOwner().HeldItem.crit + (int)modifer.Crit;
            proj.Size *= modifer.Size;
            proj.ArmorPenetration += (int)(Projectile.getOwner().GetTotalArmorPenetration(Projectile.DamageType) + modifer.armorPenetration);
            if(proj.ModProjectile is EBookBaseProjectile bp)
            {
                bp.homing += modifer.Homing;
                bp.homingRange *= modifer.HomingRange;
                bp.attackSpeed = modifer.attackSpeed;
                for(int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, Main.LocalPlayer.HeldItem), EBookUI.stackItems.Count); i++)
                {
                    if (EBookUI.stackItems[i].ModItem is BookMark bm)
                    {
                        if(bm.getEffect() != null)
                        {
                            bp.ProjectileEffects.Add(bm.getEffect());
                        }
                    }
                }
            }
        }
        public bool mouseRightLast = false;
        public virtual bool CanShoot()
        {
            return true;
        }
        public virtual int frameChange => 4;
        public override void AI()
        {
            var player = Projectile.getOwner();
            
            
            if(player.HeldItem.type != ItemType && !UIOpen)
            {
                Projectile.Kill();
                return;
            }
            if (EBookUI.active)
            {
                UIOpen = true;
            }
            if (!UIOpen)
            {
                EBookUI.bookItem = player.HeldItem;
            }
            if(UIOpen && player.HeldItem.ModItem is EntropyBook eb)
            {
                if(player.HeldItem.type != ItemType)
                {
                    Projectile.Kill();
                    UIOpen = false;
                    EBookUI.active = false;
                }
            }
            if (!player.mouseInterface && Main.myPlayer == Projectile.owner)
            {
                if(Main.mouseRight && !mouseRightLast)
                {
                    UIOpen = !UIOpen;
                    EBookUI.active = UIOpen;
                    if (UIOpen)
                    {
                        UIOpenAnm = 0;
                        Main.playerInventory = true;
                    }
                }
            }
            if (UIOpen && !Main.playerInventory) {
                UIOpen = false;
            }
            mouseRightLast = Main.mouseRight;
            setVel();
            if (!UIOpen)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.rotation = 0;
            }
            Projectile.Center = Projectile.getOwner().Center + (UIOpen ? Vector2.UnitY * -52 : new Vector2(heldOffset.X, heldOffset.Y * (Projectile.velocity.X > 0 ? 1 : -1))).RotatedBy(Projectile.rotation);
            if(Main.myPlayer == Projectile.owner)
            {
                bool flag = Main.mouseLeft && !Main.LocalPlayer.mouseInterface && !UIOpen;
                if(flag != active)
                {
                    active = flag;
                    Projectile.netUpdate = true;
                }
                if (active)
                {
                    player.heldProj = Projectile.whoAmI;
                    player.itemTime = 3;
                    player.itemAnimation = 3;
                }
                if (!UIOpen)
                {
                    if (Projectile.velocity.X > 0)
                    {
                        player.direction = 1;
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
                    }
                    else
                    {
                        player.direction = -1;
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
                    }
                }
                if (active && openAnim >= 2)
                {
                    if(shotCooldown <= 0 && CanShoot())
                    {
                        if (Projectile.getOwner().CheckMana(Projectile.getOwner().HeldItem.mana, true) && Shoot())
                        {
                            if (active && pageTurnAnm == 0)
                            {
                                playTurnPageAnimation();
                            }
                            shotCooldown = Projectile.getOwner().HeldItem.useTime;
                            EBookStatModifer m = getBaseModifer();
                            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, Main.LocalPlayer.HeldItem), EBookUI.stackItems.Count); i++)
                            {
                                if (EBookUI.stackItems[i].ModItem is BookMark bm)
                                {
                                    bm.ModiferStat(m);
                                }
                            }
                            shotCooldown = (int)((float)shotCooldown / m.attackSpeed);

                        }
                    }
                    else
                    {
                        shotCooldown--;
                    }
                }
            }
            Projectile.frameCounter++;
            if(!active && !UIOpen && openAnim == 0)
            {
                Projectile.frameCounter = 0;
            }
            if (Projectile.frameCounter >= frameChange)
            {
                Projectile.frameCounter = 0;
                if (pageTurnAnm > 0)
                {
                    pageTurnAnm--;
                }
                if (UIOpen)
                {
                    Projectile.rotation = 0;
                    if (UIOpenAnm < 3)
                    {
                        UIOpenAnm++;
                    }
                    if (openAnim > 0)
                    {
                        openAnim--;
                    }
                }
                else
                {
                    if (UIOpenAnm > 0)
                    {
                        UIOpenAnm--;
                    }
                    if (active)
                    {
                        if (openAnim < 2)
                        {
                            openAnim++;
                        }
                    }
                    else {
                        if (openAnim > 0)
                        {
                            openAnim--;
                        }
                    }
                }
            }
            Projectile.getOwner().heldProj = Projectile.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = getTexture();
            Main.EntitySpriteDraw(texture, Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, (Projectile.velocity.X > 0 || UIOpen ? SpriteEffects.None : SpriteEffects.FlipVertically), 0);
            return false;
        }
    }
    public abstract class EBookBaseProjectile : ModProjectile
    {
        public float homing = 0;
        public float homingRange = 460;
        public bool init = true;
        public bool sync = false;
        public bool EffectInit = true;
        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(homing);
            writer.Write(homingRange);
            writer.Write(Projectile.penetrate);
            writer.Write(Projectile.scale);
            writer.Write(Projectile.CritChance);

            writer.Write(ProjectileEffects.Count);
            foreach (var effect in ProjectileEffects)
            {
                writer.Write(effect.RegisterName());
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            homing = reader.ReadSingle();
            homingRange = reader.ReadSingle();
            Projectile.penetrate = reader.ReadInt32();
            Projectile.scale = reader.ReadSingle();
            Projectile.CritChance = reader.ReadInt32();

            this.ProjectileEffects.Clear();
            for(int i = 0; i < reader.ReadInt32(); i++)
            {
                this.ProjectileEffects.Add(EBookProjectileEffect.findByName(reader.ReadString()));
            }
            sync = true;
        }
        public List<EBookProjectileEffect> ProjectileEffects = new List<EBookProjectileEffect>();
        public float attackSpeed = 1;

        public virtual void ApplyHoming()
        {
            if(homing <= 0)
            {
                return;
            }
            NPC homingTarget = Projectile.FindTargetWithinRange(this.homingRange, (Projectile.tileCollide ? true : false));
            if (homingTarget != null)
            {
                Projectile.velocity *= 1f - (homing * 0.014f);
                Projectile.velocity += (homingTarget.Center - Projectile.Center).normalize() * homing * 1.2f;
            }
        }
        public override void AI()
        {
            if (init)
            {
                init = false;
                bool ownerClient = Main.myPlayer == Projectile.owner;
                if (ownerClient)
                {
                    sync = true;
                }
                if (ownerClient)
                {
                    Util.Util.SyncProj(Projectile.whoAmI);
                }
            }
            if (sync)
            {
                if (EffectInit)
                {
                    EffectInit = false;
                    foreach(var effect in  ProjectileEffects)
                    {
                        effect.OnProjectileSpawn(Projectile, Main.myPlayer == Projectile.owner);
                    }
                }
            }
            foreach (var effect in ProjectileEffects)
            {
                effect.UpdateProjectile(Projectile, Main.myPlayer == Projectile.owner);
            }
            this.ApplyHoming();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach(var effect in this.ProjectileEffects)
            {
                effect.onHitNPC(Projectile, target, damageDone);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (var effect in this.ProjectileEffects)
            {
                effect.modifyHitNPC(Projectile, target, ref modifiers);
            }
        }
    }
    public abstract class EBookBaseLaser : EBookBaseProjectile
    {
        public int segLength = 25;
        public int segCounts = 140;
        public int penetrate = 1;
        public float width => 32 * Projectile.scale;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public List<Vector2> _points = new List<Vector2>();
        public override void ApplyHoming() { }
        public virtual List<Vector2> getSamplePoints()
        {
            return _points;
        }
        public List<Vector2> cauculatePoints()
        {
            var points = new List<Vector2>();
            bool laserHoming = homing > 0;
            Vector2 startPos = Projectile.Center;
            List<NPC> hited = new List<NPC>();
            Vector2 nowPos = startPos;
            Vector2 addVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * segLength;
            Vector2 lastPos = startPos;
            var activenpcs = new List<NPC>();
            foreach(var n in Main.ActiveNPCs)
            {
                if (!n.friendly && !n.dontTakeDamage)
                {
                    activenpcs.Add(n);
                }
            }
            for (int i = 0; i < segCounts; i++)
            {
                NPC homingTarget = null;
                float dist = homingRange;
                foreach (NPC npc in activenpcs)
                {
                    if (!hited.Contains(npc) && !npc.friendly && npc.CanBeChasedBy(Projectile))
                    {
                        float r = Util.Util.getDistance(nowPos, npc.Center);
                        if (r < dist)
                        {
                            dist = r;
                            homingTarget = npc;
                        }
                    }
                    if (hited.Contains(npc) || npc.dontTakeDamage)
                    {
                        continue;
                    }
                    if (Util.Util.LineThroughRect(lastPos, nowPos, npc.getRect(), (int)width))
                    {
                        hited.Add(npc);
                    }
                }
                if (hited.Count >= penetrate)
                {
                    points.Add(nowPos);
                    return points;
                }
                if (laserHoming)
                {
                    Vector2 oldPos = Projectile.Center;
                    Projectile.Center = nowPos;

                    if (homingTarget != null)
                    {
                        addVel += (homingTarget.Center - Projectile.Center).normalize() * homing;
                        addVel *= 1 - homing * 0.024f;
                    }
                    Projectile.Center = oldPos;
                }
                lastPos = nowPos;
                points.Add(nowPos);
                nowPos += addVel.SafeNormalize(Vector2.UnitX) * segLength;
            }
            return points;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var points = this.getSamplePoints();
            for(int i = 1; i < points.Count; i++)
            {
                if (Util.Util.LineThroughRect(points[i - 1], points[i], targetHitbox, (int)width))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool PreAI()
        {
            Projectile.localNPCHitCooldown = (int)(10f / this.attackSpeed);
            
            return base.PreAI();
        }
        public override void AI()
        {
            if (this.penetrate < Projectile.penetrate)
            {
                this.penetrate = Projectile.penetrate;
            }
            Projectile.penetrate = -1;
            
            base.AI();
        }
        public override void PostAI()
        {
            _points = cauculatePoints();
        }
    }
    public abstract class EBookProjectileEffect : ModType
    {
        public static List<EBookProjectileEffect> instances;
        protected sealed override void Register()
        {
            if (instances == null)
            {
                instances = new List<EBookProjectileEffect>();
            }
            instances.Add(this);
        }
        public override void Unload()
        {
            instances = null;
        }
        public static EBookProjectileEffect findByName(string name)
        {
            if(instances == null)
            {
                return null;
            }
            foreach (EBookProjectileEffect eff in instances)
            {
                if (eff.RegisterName() == name)
                {
                    return eff;
                }
            }
            return null;
        }
        public virtual string RegisterName()
        {
            return this.Name;
        }
        public virtual void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {

        }
        public virtual void UpdateProjectile(Projectile projectile, bool ownerClient)
        {

        }

        public virtual void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {

        }

        public virtual void modifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {

        }
    }
}