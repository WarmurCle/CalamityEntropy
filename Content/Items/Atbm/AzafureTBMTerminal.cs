using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Atbm
{
    public class AzafureTBMTerminal : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Generic;
            Item.width = 62;
            Item.height = 44;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.knockBack = 0;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<ATBMProjectile>();
            Item.shootSpeed = 2f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item8;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Orange;
        }
        public override bool? UseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
            {
                foreach(Projectile p in Main.ActiveProjectiles)
                {
                    if (p.owner == player.whoAmI && p.type == Item.shoot)
                    {
                        p.Kill();
                    }
                }
            }
            else
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + Vector2.UnitY * 240, Vector2.UnitY * 24, Item.shoot, Item.damage, 0, player.whoAmI);
            }
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<HellIndustrialComponents>(12)
                .AddRecipeGroup(RecipeGroupID.IronBar, 40)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
    public class AtbmPlayer : ModPlayer
    {
        public bool LastActive = false;
        public int tbmtype = 0;
        public Vector2 opos;
        public bool CanDraw = false;
        public override void PostUpdateMiscEffects()
        {
            if(tbmtype == 0)
            {
                tbmtype = ModContent.ProjectileType<ATBMProjectile>();
            }
            if(Active)
            {
                camPos = Vector2.Lerp(camPos, proj.Center - Main.ScreenSize.ToVector2() / 2f, 0.1f);
            }
            else
            {
                camPos = Main.screenPosition;
            }
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (Active && !LastActive)
                {
                    opos = Player.Center;
                }
                if (Active)
                {
                    Player.buffImmune[BuffID.OnFire] = true;
                    Player.lavaImmune = true;
                    Player.velocity = new Vector2(0, -0.01f);
                    Player.breath = Player.breathMax + 1;
                    Player.Center = proj.Center;
                }
                if (!Active && LastActive)
                {
                    Player.Center = opos;
                }
                
            }
            LastActive = Active;
        }
        public override void ModifyScreenPosition()
        {
            if(tbmtype > 0 && Active)
            {
                Main.screenPosition = camPos;
            }
        }
        public bool ControlJump = false;
        public bool ControlW = false;
        public bool ControlS = false;
        public bool ControlA = false;
        public bool ControlD = false;
        public override void SetControls()
        {
            if (tbmtype > 0 && Active)
            {
                ControlJump = Player.controlJump;
                ControlA = Player.controlLeft;
                ControlD = Player.controlRight;
                ControlS = Player.controlDown;
                ControlW = Player.controlUp;

                Player.controlJump = false;
                Player.controlDown = false;
                Player.controlUp = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlHook = false;
                Player.controlMap = false;
                Player.controlMount = false;
            }
        }
        public bool Active
        {
            get
            {
                if (Player.ownedProjectileCounts[tbmtype] <= 0)
                {
                    return false;
                }
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type == tbmtype && p.owner == Player.whoAmI)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public Projectile proj { get
            {
                foreach(Projectile p in Main.ActiveProjectiles)
                {
                    if(p.type == tbmtype && p.owner == Player.whoAmI)
                    {
                        return p;
                    }
                }
                return null;
            } 
        }
        public Vector2 camPos = Vector2.Zero;
    }
    public class ATBMProjectile : ModProjectile
    {
        public Player player => Projectile.GetOwner();
        public AtbmPlayer mplayer => Projectile.GetOwner().GetModPlayer<AtbmPlayer>();
        public Item item => player.HeldItem;
        public bool Digging => player.controlJump;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.netImportant = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Active;
        }
        public bool InGround = false;
        public bool Active => mplayer.ControlJump;
        public override void AI()
        {
            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
            if (segs.Count == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    segs.Add(new Seg(Projectile.Center - Projectile.velocity.normalize() * 30 * i, Projectile.velocity.ToRotation(), 30));
                }
            }
            if(!player.dead)
                Projectile.timeLeft = 3;
            InGround = false;
            if(CEUtils.CheckSolidTile(Projectile.Center.getRectCentered(42, 42)) || Projectile.wet)
            {
                InGround = true;
            }
            if (CEUtils.inWorld((Projectile.Center / 16).ToPoint().X, (Projectile.Center / 16).ToPoint().Y) && Main.tile[(Projectile.Center / 16).ToPoint()].WallType != WallID.None)
                InGround = true;
            float MoveSpeed = Active ? 0.3f : 0.9f;
            Vector2 moveVect = Projectile.rotation.ToRotationVector2() * MoveSpeed * 0.6f + 0.4f * new Vector2(((mplayer.ControlA ? -1 : 0) + (mplayer.ControlD ? 1 : 0)) * MoveSpeed, ((mplayer.ControlW ? -1 : 0) + (mplayer.ControlS ? 1 : 0)) * MoveSpeed);

            Vector2 pct = Main.netMode == NetmodeID.MultiplayerClient ? mplayer.opos : player.Center;
            if (Projectile.Distance(pct) > 12000)
            {
                moveVect *= (Utils.Remap(Projectile.Distance(pct), 12000, 20000, 1, 0));
            }
            if (!(mplayer.ControlA || mplayer.ControlS || mplayer.ControlW || mplayer.ControlD))
            {
                moveVect *= 0;
            }
            if(InGround)
            {
                Projectile.velocity += moveVect;
                Projectile.velocity *= 0.9f;
                if(Active)
                {
                    Dig();
                }
            }
            else
            {
                Projectile.velocity *= 0.97f;
                Projectile.velocity += new Vector2(0, 0.35f);
            }
            if(Projectile.velocity.Length() > 0.01f)
            {
                Projectile.rotation = CEUtils.RotateTowardsAngle(Projectile.rotation, Projectile.velocity.ToRotation(), 0.1f, false);
            }
            segs[0].Follow(Projectile.Center + Projectile.velocity, Projectile.rotation);
            for (int i = 1; i < segs.Count; i++)
            {
                segs[i].Follow(segs[i - 1].Position, segs[i - 1].Rotation);
            }
            SpawnLighting();
            counter += Projectile.velocity.Length() * (Active ? 2 : 1) * 0.1f + (Active ? 0.25f : 0);
            if(counter > 1)
            {
                counter--;
                Frame++;
                if(Frame > 4)
                {
                    Frame = 0;
                }
            }
            PickUpNearbyItems();
        }
        public void PickUpNearbyItems()
        {
            foreach (Item i in Main.ActiveItems)
            {
                if(Projectile.Distance(i.Center) < 68 && player.Distance(i.Center) > 256)
                {
                    i.Center = player.Center;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                EParticle.NewParticle(new EMediumSmoke(), Projectile.Center + CEUtils.randomPointInCircle(12 * Projectile.scale), CEUtils.randomPointInCircle(12 * Projectile.scale), Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()), Main.rand.NextFloat(0.4f, 1f) * Projectile.scale, 1, true, BlendState.AlphaBlend, CEUtils.randomRot(), 70);
            }
            CEUtils.PlaySound("chainsaw_break", 1, Projectile.Center);
        }
        public void Dig()
        {
            if (player.whoAmI == Main.myPlayer)
            {
                List<Point> pos = new List<Point>();
                pos.Add(((Projectile.Center + Projectile.rotation.ToRotationVector2() * 8) / 16f).ToPoint());
                pos.Add(((Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver4) * 8) / 16f).ToPoint());
                pos.Add(((Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver4) * 8) / 16f).ToPoint());
                bool flag = false;
                foreach (Point p in pos)
                {
                    if (CEUtils.TryKillTileAndChest(p.X, p.Y, player))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    SpawnDiggingParticle();
                }
            }
        }
        public int Frame = 0;
        public float counter = 0;
        public void SpawnDiggingParticle()
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                mplayer.CanDraw = true;
                Main.PlayerRenderer.DrawPlayer(Main.Camera, player, mplayer.opos - new Vector2(player.width, player.height) / 2f, 0, Vector2.Zero);
                mplayer.CanDraw = false;
            }
            if(player.HeldItem.dye > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
                GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(player.HeldItem.type), Projectile);
            }
            
            Texture2D head = Projectile.GetTexture();
            Texture2D body = this.getTextureAlt("Seg");
            Texture2D body2 = this.getTextureAlt("Seg2");
            Texture2D end = this.getTextureAlt("End");
            Rectangle GetCutTexRect(Texture2D tex, int count, int frame)
            {
                return new Rectangle(frame * (tex.Width / count), 0, tex.Width / count - 2, tex.Height);
            }
            for (int i = segs.Count - 1; i >= 0; i--)
            {
                Texture2D tex = ((i == segs.Count - 1) ? end : (i % 2 == 0 ? body : body2));

                Main.EntitySpriteDraw(tex, segs[i].Position - Main.screenPosition, GetCutTexRect(tex, 5, Frame), Lighting.GetColor((segs[i].Position / 16f).ToPoint()), segs[i].Rotation + MathHelper.PiOver2, new Vector2((tex.Width - 10f) / 10f, tex.Height * 0.75f), Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(head, Projectile.Center - Main.screenPosition, GetCutTexRect(head, 5, Frame), lightColor, Projectile.rotation + MathHelper.PiOver2, new Vector2((head.Width - 10f) / 10f, head.Height / 2f), Projectile.scale, SpriteEffects.None);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(CEUtils.getExtraTex("GlowCone"), Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, Projectile.rotation, new Vector2(0, 250), new Vector2(1.4f, 0.8f), SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            
            return false;
        }
        public void SpawnLighting()
        {
            float rot = Projectile.velocity.ToRotation();
            for (float i = 0; i < 700; i += 10)
            {
                CEUtils.AddLight(Projectile.Center + Projectile.velocity + rot.ToRotationVector2() * i, Color.White * 0.14f, float.Max(4, i * 0.06f));
            }
        }
        public List<Seg> segs = new List<Seg>();
        public class Seg
        {
            public Vector2 Position;
            public float Rotation;
            public float Spacing;
            public Seg(Vector2 pos, float rot, float spacing = 30)
            {
                Position = pos;
                Rotation = rot;
                Spacing = spacing;
            }
            public void Follow(Vector2 pos, float rot)
            {
                this.Rotation = CEUtils.RotateTowardsAngle((pos - Position).ToRotation(), rot, 0.08f, false);
                this.Position = (pos - this.Rotation.ToRotationVector2() * Spacing);
            }
        }
    }
}
