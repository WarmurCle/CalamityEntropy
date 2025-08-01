using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static CalamityEntropy.Common.BookMarkLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public abstract class EntropyBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 44;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 26;
            Item.width = Item.height = 40;
            Item.knockBack = 1.4f;
            Item.useStyle = -1;
            Item.channel = true;
            Item.noMelee = true;
            Item.crit = 4;
            Item.mana = 4;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Orange;
        }
        public virtual int HeldProjectileType => -1;
        public virtual int SlotCount => 6;
        public virtual Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark1").Value;
        public virtual void CheckSpawn(Player player)
        {
            if (Main.myPlayer == player.whoAmI && !EBookUI.active)
            {
                if (player.HeldItem == Item)
                {
                    if (player.ownedProjectileCounts[HeldProjectileType] <= 0)
                    {
                        ((EntropyBookHeldProjectile)Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, HeldProjectileType, 0, 0, player.whoAmI, Item.type).ToProj().ModProjectile).bookItem = Item;

                        foreach (Projectile p in Main.projectile)
                        {
                            if (p.active && p.type == ModContent.ProjectileType<TwistedTwinMinion>() && p.owner == Main.myPlayer)
                            {
                                int phd = Projectile.NewProjectile(player.GetSource_ItemUse(Item), p.Center, Vector2.Zero, HeldProjectileType, 0, 0, player.whoAmI, Item.type);
                                Projectile ph = phd.ToProj();
                                (ph.ModProjectile as EntropyBookHeldProjectile).bookItem = Item;
                                ph.scale *= 0.8f;
                                ph.Entropy().IndexOfTwistedTwinShootedThisProj = p.identity;
                                p.netUpdate = true;
                                ph.netUpdate = true;
                                ph.damage = (int)(ph.damage * TwistedTwinMinion.damageMul);
                            }
                        }
                    }
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
        public int lifeSteal = 0;
    }

    public abstract class EntropyBookHeldProjectile : ModProjectile
    {
        public override void OnKill(int timeLeft)
        {
            active = false;
        }
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
            Projectile.netImportant = true;

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
            EBookStatModifer modifer = new EBookStatModifer() { Damage = 1, Knockback = Projectile.GetOwner().GetTotalKnockback(Projectile.DamageType).ApplyTo(bookItem.knockBack), Crit = Projectile.GetOwner().GetTotalCritChance(Projectile.DamageType) + Projectile.CritChance, attackSpeed = Projectile.GetOwner().GetTotalAttackSpeed(Projectile.DamageType), armorPenetration = Projectile.ArmorPenetration };
            return modifer;
        }

        public virtual EBookProjectileEffect getEffect()
        {
            return null;
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
            playPageSound();
            pageTurnAnm = 4;
            Projectile.frameCounter = 0;
        }
        public virtual void playPageSound()
        {
            CEUtils.PlaySound("pageflip", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center, 4, 0.5f);
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
            Vector2 newVel = (Main.MouseWorld - Projectile.GetOwner().Center).SafeNormalize(Vector2.UnitX);
            if (Projectile.velocity != newVel)
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
            int r = baseProjectileType;
            for (int i = 0; i < Projectile.GetOwner().GetMyMaxActiveBookMarks(bookItem); i++)
            {
                if (BookMarkLoader.IsABookMark(Projectile.GetOwner().Entropy().EBookStackItems[i]))
                {
                    Item it = Projectile.GetOwner().Entropy().EBookStackItems[i];
                    int b = BookMarkLoader.ModifyBaseProjectile(it);
                    if (b >= 0)
                    {
                        r = b; break;
                    }
                }
            }
            return r;
        }
        public virtual bool Shoot()
        {
            int type = getShootProjectileType();
            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
            {
                var bm = Projectile.owner.ToPlayer().Entropy().EBookStackItems[i];
                if (BookMarkLoader.IsABookMark(bm))
                {
                    int pn = BookMarkLoader.ModifyProjectile(bm, type);
                    if (pn >= 0)
                    {
                        type = pn;
                    }
                }
            }
            ShootSingleProjectile(type, Projectile.Center, Projectile.velocity);

            return true;
        }
        public Item bookItem;
        public virtual float randomShootRotMax => 0.1f;
        public virtual bool canApplyShootCDModifer => true;
        public int CauculateProjectileDamage(EBookStatModifer modifer, float mult = 1)
        {
            return (int)(Projectile.GetOwner().GetTotalDamage(Projectile.DamageType).ApplyTo(bookItem.damage * modifer.Damage * mult * (Projectile.Entropy().IndexOfTwistedTwinShootedThisProj < 0 ? 1 : TwistedTwinMinion.damageMul)));
        }
        public int CauculateProjectileDamage(float mult = 1)
        {
            var modifer = GetProjectileModifer();
            return (int)(Projectile.GetOwner().GetTotalDamage(Projectile.DamageType).ApplyTo(bookItem.damage * modifer.Damage * mult * (Projectile.Entropy().IndexOfTwistedTwinShootedThisProj < 0 ? 1 : TwistedTwinMinion.damageMul)));
        }
        public EBookStatModifer GetProjectileModifer()
        {
            EBookStatModifer modifer = getBaseModifer();
            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
            {
                Item it = Projectile.GetOwner().Entropy().EBookStackItems[i];
                if (BookMarkLoader.IsABookMark(it))
                {
                    BookMarkLoader.ModifyStat(it, modifer);
                }
            }
            return modifer;
        }
        public virtual void ShootSingleProjectile(int type, Vector2 pos, Vector2 velocity, float damageMul = 1, float scaleMul = 1, float shotSpeedMul = 1, Action<Projectile> initAction = null)
        {
            var modifer = GetProjectileModifer();
            Vector2 shootVel = (velocity.normalize() * bookItem.shootSpeed * modifer.shotSpeed * shotSpeedMul).RotatedByRandom(this.randomShootRotMax);
            float kb = Projectile.GetOwner().GetTotalKnockback(Projectile.DamageType).ApplyTo(bookItem.knockBack * modifer.Knockback);
            int dmg = CauculateProjectileDamage(modifer, damageMul);
            bookItem.channel = false;
            if (ItemLoader.Shoot(bookItem, Projectile.GetOwner(), new Terraria.DataStructures.EntitySource_ItemUse_WithAmmo(Projectile.GetOwner(), bookItem, 0), pos, velocity * ContentSamples.ProjectilesByType[type].MaxUpdates, type, dmg, kb))
            {
                Projectile proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, shootVel, type, dmg, kb, Projectile.owner).ToProj();
                proj.penetrate += modifer.PenetrateAddition;
                proj.CritChance = bookItem.crit + (int)modifer.Crit;
                proj.scale *= modifer.Size * scaleMul;
                proj.ArmorPenetration += (int)(Projectile.GetOwner().GetTotalArmorPenetration(Projectile.DamageType) + modifer.armorPenetration);
                if (proj.ModProjectile is EBookBaseProjectile bp)
                {
                    bp.ShooterModProjectile = this;
                    bp.homing += modifer.Homing;
                    bp.homingRange *= modifer.HomingRange;
                    bp.attackSpeed = modifer.attackSpeed;
                    bp.lifeSteal += modifer.lifeSteal;
                    for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
                    {
                        Item it = Projectile.GetOwner().Entropy().EBookStackItems[i];
                        if (BookMarkLoader.IsABookMark(it))
                        {
                            if (BookMarkLoader.GetEffect(it) != null)
                            {
                                bp.ProjectileEffects.Add(BookMarkLoader.GetEffect(it));
                            }
                        }
                    }
                    if (this.getEffect() != null)
                    {
                        bp.ProjectileEffects.Add(this.getEffect());
                    }
                }
                initAction?.Invoke(proj);
            }
            bookItem.channel = true;
        }
        public bool mouseRightLast = false;
        public virtual bool CanShoot()
        {
            return true;
        }
        public virtual int GetShootCd()
        {
            int _shotCooldown = bookItem.useTime;

            EBookStatModifer m = getBaseModifer();
            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
            {
                Item it = Projectile.GetOwner().Entropy().EBookStackItems[i];
                if (BookMarkLoader.IsABookMark(it))
                {
                    BookMarkLoader.ModifyStat(it, m);
                    if (this.canApplyShootCDModifer)
                    {
                        BookMarkLoader.modifyShootCooldown(it, ref _shotCooldown);
                    }
                }
            }
            return (int)((float)_shotCooldown / m.attackSpeed);
        }
        public virtual int frameChange => 4;
        public override void AI()
        {
            var player = Projectile.GetOwner();
            if (Projectile.Entropy().IndexOfTwistedTwinShootedThisProj >= 0 && !Projectile.Entropy().IndexOfTwistedTwinShootedThisProj.ToProj().active)
            {
                Projectile.Kill();
                return;
            }
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }

            if (player.HeldItem.type != ItemType && !UIOpen)
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
            Projectile.timeLeft++;
            if (UIOpen && player.HeldItem.ModItem is EntropyBook eb)
            {
                if (player.HeldItem.type != ItemType)
                {
                    Projectile.Kill();
                    UIOpen = false;
                    EBookUI.active = false;
                }
            }
            if (!player.mouseInterface && Main.myPlayer == Projectile.owner)
            {
                if (Main.mouseRight && !mouseRightLast)
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
            if (UIOpen && !Main.playerInventory)
            {
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
            shotCooldown--;
            Projectile.Center = Projectile.GetOwner().Center + (UIOpen ? Vector2.UnitY * -52 : new Vector2(heldOffset.X, heldOffset.Y * (Projectile.velocity.X > 0 ? 1 : -1))).RotatedBy(Projectile.rotation);
            if (Main.myPlayer == Projectile.owner)
            {
                bool flag = Main.mouseLeft && !Main.LocalPlayer.mouseInterface && !UIOpen && Projectile.GetOwner().CheckMana(bookItem.mana, false);
                if (flag != active)
                {
                    active = flag;
                    Projectile.netUpdate = true;
                    if (active)
                    {
                        for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
                        {
                            Item it = Projectile.GetOwner().Entropy().EBookStackItems[i];
                            if (BookMarkLoader.IsABookMark(it))
                            {
                                var e = BookMarkLoader.GetEffect(it);
                                if (e != null)
                                {
                                    e.OnActive(this);
                                }
                            }
                        }
                        if (this.getEffect() != null)
                            this.getEffect().OnActive(this);
                    }
                }
                if (active)
                {
                    player.heldProj = Projectile.whoAmI;
                    player.itemTime = 3;
                    player.itemAnimation = 3;
                    player.channel = true;

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
                    if (shotCooldown <= 0 && CanShoot())
                    {
                        if (Projectile.GetOwner().CheckMana(bookItem.mana, true) && Shoot())
                        {
                            if (active && pageTurnAnm == 0)
                            {
                                playTurnPageAnimation();
                            }
                            shotCooldown = bookItem.useTime;

                            EBookStatModifer m = getBaseModifer();
                            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
                            {
                                Item it = Projectile.GetOwner().Entropy().EBookStackItems[i];
                                if (BookMarkLoader.IsABookMark(it))
                                {
                                    var e = BookMarkLoader.GetEffect(it);
                                    BookMarkLoader.ModifyStat(it, m);
                                    if (this.canApplyShootCDModifer)
                                    {
                                        BookMarkLoader.modifyShootCooldown(it, ref shotCooldown);
                                    }
                                    if (e != null)
                                    {
                                        e.OnShoot(this);
                                    }
                                }
                            }
                            if (this.getEffect() != null)
                            {
                                var e = this.getEffect();
                                if (e != null)
                                {
                                    e.OnShoot(this);
                                }

                            }
                            shotCooldown = (int)((float)shotCooldown / m.attackSpeed);

                        }
                        else
                        {
                            active = false;
                        }
                    }
                }
            }
            if (active)
            {
                for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Projectile.GetOwner(), bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
                {
                    Item item = Projectile.GetOwner().Entropy().EBookStackItems[i];
                    if (BookMarkLoader.IsABookMark(item))
                    {
                        if (BookMarkLoader.GetEffect(item) != null)
                        {
                            BookMarkLoader.GetEffect(item).BookUpdate(Projectile, Main.myPlayer == Projectile.owner);
                        }
                    }
                }
            }
            Projectile.frameCounter++;
            if (!active && !UIOpen && openAnim == 0)
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
                    else
                    {
                        if (openAnim > 0)
                        {
                            openAnim--;
                        }
                    }
                }
            }
            Projectile.GetOwner().heldProj = Projectile.whoAmI;
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
        public int hitCount = 0;
        public float homing = 0;
        public float homingRange = 460;
        public bool init = true;
        public bool sync = false;
        public bool EffectInit = true;
        public int lifeSteal = 0;
        public float gravity = 0;
        public virtual Color baseColor => Color.White;
        public Color color;
        public bool initColor = true;
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Projectile.Center.getRectCentered(hitbox.Width * Projectile.scale, hitbox.Height * Projectile.scale);
        }
        public override bool PreAI()
        {
            if (initColor)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                initColor = false;
                color = baseColor;
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1000;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(homing);
            writer.Write(homingRange);
            writer.Write(Projectile.penetrate);
            writer.Write(Projectile.scale);
            writer.Write(Projectile.CritChance);
            writer.Write(lifeSteal);
            writer.Write(gravity);

            writer.Write(ProjectileEffects.Count);
            foreach (var effect in ProjectileEffects)
            {
                writer.Write(effect.RegisterName());
                writer.Write(effect.BMOtherMod_Name);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            homing = reader.ReadSingle();
            homingRange = reader.ReadSingle();
            Projectile.penetrate = reader.ReadInt32();
            Projectile.scale = reader.ReadSingle();
            Projectile.CritChance = reader.ReadInt32();
            lifeSteal = reader.ReadInt32();
            gravity = reader.ReadSingle();

            this.ProjectileEffects.Clear();
            int r = reader.ReadInt32();
            for (int i = 0; i < r; i++)
            {
                var bef = EBookProjectileEffect.findByName(reader.ReadString());
                string omN = reader.ReadString();
                if (omN != string.Empty)
                {
                    bef = new BookmarkEffect_OtherMod() { BMOtherMod_Name = omN };
                }
                this.ProjectileEffects.Add(bef);
            }
            sync = true;
        }
        public List<EBookProjectileEffect> ProjectileEffects = new List<EBookProjectileEffect>();
        public float attackSpeed = 1;
        public ModProjectile ShooterModProjectile = null;

        public virtual void ApplyHoming()
        {
            if (homing <= 0)
            {
                return;
            }
            NPC homingTarget = Projectile.FindTargetWithinRange(this.homingRange, (Projectile.tileCollide ? true : false));
            if (homingTarget != null)
            {
                Projectile.velocity *= 1f - (homing * 0.075f);
                Projectile.velocity += (homingTarget.Center - Projectile.Center).normalize() * homing * 4.2f;
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
                    CEUtils.SyncProj(Projectile.whoAmI);
                }
            }
            if (sync)
            {
                if (EffectInit)
                {
                    EffectInit = false;
                    foreach (var effect in ProjectileEffects)
                    {
                        effect.OnProjectileSpawn(Projectile, Main.myPlayer == Projectile.owner);
                    }
                }
            }
            Projectile.velocity.Y += this.gravity;
            foreach (var effect in ProjectileEffects)
            {
                effect.UpdateProjectile(Projectile, Main.myPlayer == Projectile.owner);
            }
            this.ApplyHoming();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitCount++;
            foreach (var effect in this.ProjectileEffects)
            {
                effect.OnHitNPC(Projectile, target, damageDone);
            }
            if (lifeSteal > 0)
            {
                Projectile.GetOwner()?.Entropy().TryHealMeWithCd(lifeSteal);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (var effect in this.ProjectileEffects)
            {
                effect.ModifyHitNPC(Projectile, target, ref modifiers);
            }
        }
    }
    public abstract class EBookBaseLaser : EBookBaseProjectile
    {
        public int segLength = 30;
        public int segCounts = 100;
        public int penetrate = 1;
        public int quickTime = -1;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.localNPCHitCooldown = hitCd;
        }
        public virtual float width => 32 * Projectile.scale;
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
        public virtual int OnHitEffectProb => 4;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(OnHitEffectProb))
            {
                base.OnHitNPC(target, hit, damageDone);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(quickTime);
            writer.Write(penetrate);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            quickTime = reader.ReadInt32();
            penetrate = reader.ReadInt32();
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
            foreach (var n in Main.ActiveNPCs)
            {
                if (!n.friendly && !n.dontTakeDamage && n.CanBeChasedBy(Projectile))
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
                    if (!hited.Contains(npc))
                    {
                        float r = CEUtils.getDistance(nowPos, npc.Center);
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
                    if (CEUtils.LineThroughRect(lastPos, nowPos, npc.getRect(), (int)width))
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
                        addVel += (homingTarget.Center - Projectile.Center).normalize() * homing * 2.3f;
                        addVel *= 1 - homing * 0.12f;
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
            for (int i = 1; i < points.Count; i++)
            {
                if (CEUtils.LineThroughRect(points[i - 1], points[i], targetHitbox, (int)width))
                {
                    return true;
                }
            }
            return false;
        }
        public virtual int hitCd => 10;
        public override bool PreAI()
        {
            Projectile.localNPCHitCooldown = (int)((float)hitCd / this.attackSpeed);
            if (quickTime > 0)
            {
                quickTime--;
                if (quickTime == 0)
                {
                    Projectile.Kill();
                    return false;
                }
            }
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
        public string BMOtherMod_Name = string.Empty;
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
        public virtual void BookUpdate(Projectile projectile, bool ownerClient)
        {

        }
        public static EBookProjectileEffect findByName(string name)
        {
            if (instances == null)
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

        public virtual void OnShoot(EntropyBookHeldProjectile book)
        {

        }
        public virtual void OnActive(EntropyBookHeldProjectile book)
        {

        }
        public virtual void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {

        }
        public virtual void UpdateProjectile(Projectile projectile, bool ownerClient)
        {

        }

        public virtual void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {

        }

        public virtual void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {

        }
    }
}