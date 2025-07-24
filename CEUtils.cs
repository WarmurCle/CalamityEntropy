using CalamityEntropy.Common;
using CalamityEntropy.Content;
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Items.PrefixItem;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public static class CEUtils
    {
        public static void ExplotionParticleLOL(Vector2 pos)
        {
            EParticle.NewParticle(new RealisticExplosion(), pos, Vector2.Zero, Color.White, 2, 1, true, BlendState.Additive);
        }
        public static void AddBuff<T>(this NPC npc, int time, bool quiet = false) where T : ModBuff
        {
            npc.AddBuff(ModContent.BuffType<T>(), time, quiet);
        }
        public static Tile PlaceTile(int x, int y, ushort type)
        {
            int si = x;
            int sj = y;
            if (CEUtils.inWorld(si, sj))
            {
                Tile t = Main.tile[si, sj];
                if(t.HasTile)
                {
                    return new Tile();
                }
                t.TileType = type;
                t.HasTile = true;
                WorldGen.SquareTileFrame(si, sj);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, si, sj);
                return Main.tile[si, sj];

            }
            return new Tile();
        }
        public static void SetHandRot(this Player owner, float r)
        {
            if (r.ToRotationVector2().X > 0)
            {
                owner.direction = 1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, r - (float)(Math.PI * 0.5f));
            }
            else
            {
                owner.direction = -1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, r - (float)(Math.PI * 0.5f));
            }
        }
        public static Vector2 GetDrawCenter(this Player player)
        {
            return player.MountedCenter + player.gfxOffY * Vector2.UnitY;
        }
        public static Vector2 GetCircleIntersection(Vector2 vec1, float a, Vector2 vec2, float b)
        {
            float distance = Vector2.Distance(vec1, vec2);

            if (distance > a + b || distance < Math.Abs(a - b))
            {
                Vector2 direction = Vector2.Normalize(vec2 - vec1);
                return vec1 + direction * a;
            }

            float d = distance;
            float l = (a * a - b * b + d * d) / (2 * d);
            float h = (float)Math.Sqrt(a * a - l * l);

            Vector2 p0 = vec1 + (l / d) * (vec2 - vec1);

            Vector2 intersection1 = new Vector2(
                p0.X + (h / d) * (vec2.Y - vec1.Y),
                p0.Y - (h / d) * (vec2.X - vec1.X));

            Vector2 intersection2 = new Vector2(
                p0.X - (h / d) * (vec2.Y - vec1.Y),
                p0.Y + (h / d) * (vec2.X - vec1.X));

            return (intersection1.Y < intersection2.Y) ? intersection1 : intersection2;
        }
    public static void AddLight(Vector2 position, Color lightColor, float mult = 1)
        {
            Lighting.AddLight(position, lightColor.R / 255f * mult, lightColor.G / 255f * mult, lightColor.B / 255f * mult);
        }
        public static T[] Combine<T>(this T[] a, T[] b)
        {
            T[] ls = new T[a.Length + b.Length];
            int c = 0;
            foreach (var i in a) { ls[c] = i; c++; }
            foreach (var i in b) { ls[c] = i; c++; }
            return ls;
        }
        public static bool SetCartridge(this Item item, int m)
        {
            if(ModLoader.HasMod("CalamityOverhaul"))
            {
                CWRWeakRef.CWRRef.SetCartridge(item, m);
                return true;
            }
            return false;
        }
        public static bool IsPlayerStuck(Player player)
        {
            Rectangle playerHitbox = player.getRect();

            for (int i = 0; i < 4; i++)
            {
                Point checkPoint = new Point(
                    i < 2 ? playerHitbox.Left : playerHitbox.Right - 1,
                    i % 2 == 0 ? playerHitbox.Top : playerHitbox.Bottom - 1);

                if (WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(checkPoint.X / 16, checkPoint.Y / 16)))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool CheckSolidTile(Rectangle rect)
        {
            return Collision.SolidCollision(rect.TopLeft(), rect.Width, rect.Height);
        }
        public static void FriendlySetDefaults(this Projectile Projectile, DamageClass dmgClass, bool tileCollide = false, int penetrate = 1)
        {
            Projectile.DamageType = dmgClass;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = penetrate;
            Projectile.tileCollide = tileCollide;
        }
        public static int Softlimitation(this int num, int limit)
        {
            if (num <= limit)
                return num;
            return (int)Math.Round(limit + Math.Sqrt(num - limit));
        }
        public static DamageClass RogueDC => ModContent.GetInstance<CalamityMod.RogueDamageClass>();
        public static void SpawnExplotionHostile(IEntitySource source, Vector2 position, int damage, float r)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<CommonExplotion>(), damage, 0, 0, r);
        }
        public static void SpawnExplotionFriendly(IEntitySource source, Player player, Vector2 position, int damage, float r, DamageClass damageClass)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<CommonExplotionFriendly>(), damage, 0, player.whoAmI, r).ToProj().DamageType = damageClass;
        }
        public static void SetShake(Vector2 center, float strength, float MaxDist = 4000)
        {
            float s = Utils.Remap(Main.LocalPlayer.Distance(center), MaxDist, 0, 0f, strength);
            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < s)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = s;
        }
        public static List<Vector2> WrapPoints(List<Vector2> points, int d)
        {
            var ptd = new List<Vector2>();
            for (int i = 1; i < points.Count; i++)
            {
                for (int j = 0; j < d; j++)
                {
                    ptd.Add(Vector2.Lerp(points[i - 1], points[i], (float)j / d));
                }
            }
            return ptd;
        }
        public static int ApplyCdDec(this int orig, Player plr)
        {
            return (int)(orig * plr.Entropy().CooldownTimeMult);
        }
        public static bool HasEBookEffect<T>(this Projectile p) where T : EBookProjectileEffect
        {
            if (p.ModProjectile is EBookBaseProjectile ep)
            {
                foreach (var ef in ep.ProjectileEffects)
                {
                    if (ef.GetType() == typeof(T))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static NMSGItem TFAW(this Item item) => item.GetGlobalItem<NMSGItem>();
        public static NMSPLayer TFAW(this Player player) => player.GetModPlayer<NMSPLayer>();

        /// <summary>
        /// 用于将一个武器设置为手持刀剑类，这个函数若要正确设置物品的近战属性，需要让其在初始化函数中最后调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public static void SetKnifeHeld<T>(this Item item) where T : ModProjectile
        {
            item.noMelee = true;
            item.noUseGraphic = true;
            item.TFAW().IsShootCountCorlUse = true;
            item.shoot = ModContent.ProjectileType<T>();
        }
        public static Vector2 randVr(int min, int max)
        {
            return Main.rand.NextVector2Unit() * Main.rand.Next(min, max);
        }
        public static float GetCorrectRadian(float minusRadian)
        {
            return minusRadian < 0 ? (MathHelper.TwoPi + minusRadian) / MathHelper.TwoPi : minusRadian / MathHelper.TwoPi;
        }

        /// <summary>
        /// 获取纹理实例，类型为 Texture2D
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Texture2D GetT2DValue(string texture, bool immediateLoad = false)
        {
            return ModContent.Request<Texture2D>(texture, immediateLoad ? AssetRequestMode.AsyncLoad : AssetRequestMode.ImmediateLoad).Value;
        }
        /// <summary>
        /// 获取纹理实例，类型为 AssetTexture2D
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Asset<Texture2D> GetT2DAsset(string texture, bool immediateLoad = false)
        {
            return ModContent.Request<Texture2D>(texture, immediateLoad ? AssetRequestMode.AsyncLoad : AssetRequestMode.ImmediateLoad);
        }

        public static float RotTowards(this float curAngle, float targetAngle, float maxChange)
        {
            curAngle = MathHelper.WrapAngle(curAngle);
            targetAngle = MathHelper.WrapAngle(targetAngle);
            if (curAngle < targetAngle)
            {
                if (targetAngle - curAngle > (float)Math.PI)
                {
                    curAngle += (float)Math.PI * 2f;
                }
            }
            else if (curAngle - targetAngle > (float)Math.PI)
            {
                curAngle -= (float)Math.PI * 2f;
            }

            curAngle += MathHelper.Clamp(targetAngle - curAngle, 0f - maxChange, maxChange);
            return MathHelper.WrapAngle(curAngle);
        }
        public static Vector2 SmoothHomingBehavior(this Entity entity, Vector2 TargetCenter, float SpeedUpdates = 1, float HomingStrenght = 0.1f)
        {
            float targetAngle = (TargetCenter - entity.Center).ToRotation();
            float f = entity.velocity.ToRotation().RotTowards(targetAngle, HomingStrenght);
            Vector2 speed = f.ToRotationVector2() * entity.velocity.Length() * SpeedUpdates;
            entity.velocity = speed;
            return speed;
        }
        public static float Parabola(float t, float height)
        {
            return 4 * height * t * (1 - t);
        }
        public static NPC FindTarget_HomingProj(object atker, Vector2 center, float radians, Func<int, bool> filter = null)
        {
            NPC npc = null;
            float dist = radians;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.CanBeChasedBy(atker) && !n.friendly)
                {
                    if (getDistance(n.Center, center) <= dist && (filter == null || filter.Invoke(n.whoAmI)))
                    {
                        dist = getDistance(n.Center, center);
                        npc = n;
                    }
                }
            }
            return npc;
        }
        public static void SetSyncValue(this Projectile proj, string name, object value)
        {
            proj.Entropy().DataSynchronous[name].Value = value;
        }
        public static void DefineSynchronousData(this Projectile proj, SyncDataType type, string name, object defaultValue)
        {
            proj.Entropy().DataSynchronous[name] = new SynchronousData(type, name, defaultValue);
        }
        public static T GetSyncValue<T>(this Projectile proj, string name)
        {
            return proj.Entropy().DataSynchronous[name].GetValue<T>();
        }
        public static void SetSizeFormTexture(this Item item, float scale = 1)
        {
            Texture2D tex = ModContent.Request<Texture2D>(item.ModItem.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int w = (int)(scale * tex.Width);
            int h = (int)(scale * tex.Height);
            item.width = w; item.height = h;
        }
        public static bool IsArmorReforgeItem(this Item item, out ArmorPrefix prefix)
        {
            prefix = null;
            if (item.ModItem is PrefixClearKnife)
            {
                return true;
            }
            if (item.ModItem is BasePrefixItem bpi)
            {
                prefix = ArmorPrefix.findByName(bpi.PrefixName);
                return true;
            }
            return false;
        }
        public static void Shrink(this Item item, int count = 1)
        {
            item.stack -= count;
            if (item.stack <= 0)
            {
                item.TurnToAir();
            }
        }
        public static float CustomLerp1(float v)
        {
            float j = 0.6f;
            return (float)((Math.Cos(v * (MathHelper.Pi + j) - MathHelper.Pi) * 0.5f + 0.5f) / Math.Cos(j));
        }
        public static float GetRepeatedCosFromZeroToOne(float v, int repeat)
        {
            if (repeat <= 1)
            {
                return (float)(Math.Cos(v * MathHelper.Pi - MathHelper.Pi)) * 0.5f + 0.5f;
            }
            return (float)(Math.Cos(GetRepeatedCosFromZeroToOne(v, repeat - 1) * MathHelper.Pi - MathHelper.Pi)) * 0.5f + 0.5f;
        }
        public static void Replace(this List<TooltipLine> tooltips, string targetStr, string to)
        {
            if (!Main.dedServ)
            {
                tooltips.FindAndReplace(targetStr, to);
            }
        }
        public static void Replace(this List<TooltipLine> tooltips, string targetStr, int to)
        {
            if (!Main.dedServ)
            {
                tooltips.FindAndReplace(targetStr, to.ToString());
            }
        }
        public static void Replace(this List<TooltipLine> tooltips, string targetStr, float to)
        {
            if (!Main.dedServ)
            {
                tooltips.FindAndReplace(targetStr, to.ToString());
            }
        }
        public static int ToPercent(this float f)
        {
            return (int)(Math.Round(f * 100));
        }
        public static void FindAndReplace(this List<TooltipLine> tooltips, string replacedKey, string newKey)
        {
            TooltipLine tooltipLine = tooltips.FirstOrDefault((TooltipLine x) => x.Mod == "Terraria" && x.Text.Contains(replacedKey));
            if (tooltipLine != null)
            {
                tooltipLine.Text = tooltipLine.Text.Replace(replacedKey, newKey);
            }
        }
        public static Vector2 GetFrameOrigin(this PlayerDrawSet drawInfo)
        {
            return new Vector2(
            (int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2)),
            (int)(drawInfo.Position.Y - Main.screenPosition.Y + (drawInfo.drawPlayer.height - (drawInfo.drawPlayer.mount.Active ? drawInfo.drawPlayer.mount.HeightBoost : 0)) - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f));
        }
        public static Vector2 HeadPosition(this PlayerDrawSet drawInfo, bool addBob = false, bool vanillaStyle = false)
        {
            Vector2 drawPosition = GetFrameOrigin(drawInfo);

            if (vanillaStyle)
                drawPosition += drawInfo.drawPlayer.headPosition + drawInfo.headVect;
            else
            {
                if (drawInfo.drawPlayer.gravDir == -1)
                    drawPosition.Y = (int)drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.bodyFrame.Height - 4f;

                Vector2 headOffset = drawInfo.drawPlayer.headPosition + drawInfo.headVect;

                if (!drawInfo.drawPlayer.dead && drawInfo.drawPlayer.gravDir == -1)
                    headOffset.Y -= 6;

                headOffset.Y *= drawInfo.drawPlayer.gravDir;
                drawPosition += headOffset;
            }

            if (addBob)
                drawPosition += Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawInfo.drawPlayer.gravDir;

            return drawPosition + new Vector2(0, drawInfo.drawPlayer.height - 42);
        }
        public static Vector2 randomPointInCircle(float r)
        {
            return randomRot().ToRotationVector2() * Main.rand.NextFloat(-r, r);
        }
        public static void DrawChargeBar(float barScale, Vector2 position, float progress, Color color)
        {
            var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            Vector2 barOrigin = barBG.Size() * 0.5f;
            Vector2 drawPos = position;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(progress * barFG.Width), barFG.Height);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(barBG, drawPos, null, color, 0f, barOrigin, barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, barOrigin, barScale, 0f, 0f);
        }
        public static void ApplyGameShaderForPlayer(int id, Player player)
        {
            GameShaders.Armor.Apply(id, player);
        }
        public static string WhiteTexPath = "CalamityEntropy/Assets/Extra/white";
        public static Texture2D pixelTex => ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
        public static Texture2D GetTexture(this Projectile p)
        {
            return TextureAssets.Projectile[p.type].Value;
        }
        public static Texture2D getTextureAlt(this ModProjectile p, string n = "Alt")
        {
            return RequestTex(p.Texture + n);
        }
        public static Texture2D getTextureGlow(this ModProjectile p)
        {
            return RequestTex(p.Texture + "Glow");
        }
        public static Dictionary<string, Texture2D> TexCache;
        public static Texture2D RequestTex(string path)
        {
            if (!TexCache.ContainsKey(path))
            {
                TexCache[path] = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
            }
            return TexCache[path];
        }
        public static Vector2 normalize(this Vector2 v)
        {
            return v.SafeNormalize(Vector2.Zero);
        }
        public static Vector2 randomPoint(this Rectangle rect)
        {
            return new Vector2(Main.rand.NextFloat(rect.X, rect.Width), Main.rand.NextFloat(rect.Y, rect.Height));
        }
        public static bool Is<T>(this Item item) where T : ModItem
        {
            return item.type == ModContent.ItemType<T>();
        }

        public static void DrawGlow(Vector2 worldPos, Color color, float scale, bool additive = true, Texture2D tex = null)
        {
            Texture2D glow = tex == null ? getExtraTex("Glow2") : tex;
            SpriteBatch sb = Main.spriteBatch;
            var blend = BlendState.AlphaBlend;
            var sample = sb.GraphicsDevice.SamplerStates[0];
            var depth = sb.GraphicsDevice.DepthStencilState;
            var rasterizer = sb.GraphicsDevice.RasterizerState;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, additive ? BlendState.Additive : BlendState.NonPremultiplied, sample, depth, rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
            sb.Draw(glow, worldPos - Main.screenPosition, null, color, 0, glow.Size() * 0.5f, scale * 0.4f, SpriteEffects.None, 0);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, blend, sample, depth, rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

        }

        public static Terraria.DataStructures.DrawData getDrawData(this Projectile projectile, Color color, Texture2D texOverride = null, Vector2 overridePos = default)
        {
            Texture2D tx = projectile.GetTexture();
            if (texOverride != null)
            {
                tx = texOverride;
            }
            return new Terraria.DataStructures.DrawData(tx, (overridePos == default ? projectile.Center : overridePos) + projectile.gfxOffY * Vector2.UnitY - Main.screenPosition, Main.projFrames[projectile.type] <= 1 ? null : new Rectangle(0, (tx.Height / Main.projFrames[projectile.type]) * projectile.frame, tx.Width, (tx.Height / Main.projFrames[projectile.type]) - 2), color, projectile.rotation, new Vector2(tx.Width, Main.projFrames[projectile.type] > 1 ? (tx.Height / Main.projFrames[projectile.type]) - 2 : tx.Height) / 2, projectile.scale, SpriteEffects.None);
        }
        public static void showItemTooltip(Item item)
        {
            Main.HoverItem = item.Clone();
            Main.hoverItemName = item.HoverName;
        }
        public static void SyncProj(int proj)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
            }
        }
        public static void pushByOther(this Projectile proj, float strength)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == proj.type && p.Colliding(p.getRect(), proj.getRect()) && !(p.whoAmI == proj.whoAmI))
                {
                    proj.velocity += (proj.Center - p.Center).SafeNormalize(randomRot().ToRotationVector2()) * strength;
                }
            }
        }
        public static Vector2 randomVec(float max)
        {
            return new Vector2(Main.rand.NextFloat(-max, max), Main.rand.NextFloat(-max, max));
        }
        public static Vector2 Bezier(List<Vector2> points, float lerp)
        {
            if (points == null || points.Count == 0)
                return Vector2.Zero;

            if (points.Count == 1)
                return points[0];

            List<Vector2> newPoints = new List<Vector2>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                newPoints.Add(Vector2.Lerp(points[i], points[i + 1], lerp));
            }

            return Bezier(newPoints, lerp);
        }

        public static Texture2D getTexture(this NPC npc)
        {
            return TextureAssets.Npc[npc.type].Value;
        }
        public static float GetAngleBetweenVectors(Vector2 vector1, Vector2 vector2)
        {
            float dotProduct = Vector2.Dot(vector1, vector2);

            float magnitude1 = vector1.Length();
            float magnitude2 = vector2.Length();

            float cosTheta = dotProduct / (magnitude1 * magnitude2);

            float angleInRadians = (float)Math.Acos(cosTheta);


            return angleInRadians;
        }
        public static Vector2 GetSymmetryPoint(this Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            Vector2 lineVector = linePoint2 - linePoint1;
            if (lineVector == Vector2.Zero)
            {
                return point;
            }
            Vector2 aToPoint = point - linePoint1;
            float t = Vector2.Dot(aToPoint, lineVector) / lineVector.LengthSquared();
            Vector2 projection = linePoint1 + t * lineVector;
            Vector2 symmetryPoint = 2 * projection - point;
            return symmetryPoint;
        }
        public static Rectangle getRectCentered(this Vector2 center, float w, float h)
        {
            return new Rectangle((int)(center.X - w / 2), (int)(center.Y - h / 2), (int)w, (int)h);
        }
        public static void DrawLines(List<Vector2> points, Color color, float width)
        {
            for (int i = 1; i < points.Count; i++)
            {
                drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, points[i - 1], points[i], color, width, 2, true);
            }
        }
        public static SoundStyle GetSound(string name, float pitch = 1, int maxIns = 4, float volume = 1)
        {
            SoundStyle s = new SoundStyle("CalamityEntropy/Assets/Sounds/" + name);
            s.Pitch = pitch - 1;
            s.Volume = volume;
            s.MaxInstances = maxIns;
            return s;
        }
        public static void PlaySound(string name, float pitch = 1, Vector2? pos = null, int maxIns = 6, float volume = 1)
        {
            if (!Main.dedServ)
            {
                SoundStyle s = new SoundStyle("CalamityEntropy/Assets/Sounds/" + name);
                s.Pitch = pitch - 1;
                s.Volume = volume;
                s.MaxInstances = maxIns;
                SoundEngine.PlaySound(in s, pos);
            }
        }
        public static void UseSampleState_UI(this SpriteBatch sb, SamplerState sampler)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void UseBlendState_UI(this SpriteBatch sb, BlendState blend)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, blend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void UseBlendState(this SpriteBatch sb, BlendState blend, SamplerState s = null)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, blend, s == null ? Main.DefaultSamplerState : s, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
        }
        public static void UseSampleState(this SpriteBatch sb, SamplerState s)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, Main.graphics.GraphicsDevice.BlendState, s, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
        }
        public static void UseState_UI(this SpriteBatch sb, BlendState blend, SamplerState sampler)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, blend, sampler, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void begin_(this SpriteBatch sb)
        {
            sb.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
        }
        public static void DrawRectAlt(Rectangle rect, Color color, float width, int num = 16)
        {
            drawLine(new Vector2(rect.X + num, rect.Y), new Vector2(rect.X + rect.Width - num, rect.Y), color, width, 2);
            drawLine(new Vector2(rect.X + rect.Width - num, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + num), color, width, 2);
            drawLine(new Vector2(rect.X + rect.Width, rect.Y + num), new Vector2(rect.X + rect.Width, rect.Y + rect.Height - num), color, width, 2);
            drawLine(new Vector2(rect.X + rect.Width, rect.Y + rect.Height - num), new Vector2(rect.X + rect.Width - num, rect.Y + rect.Height), color, width, 2);
            drawLine(new Vector2(rect.X + num, rect.Y + rect.Height), new Vector2(rect.X + rect.Width - num, rect.Y + rect.Height), color, width, 2);
            drawLine(new Vector2(rect.X + num, rect.Y + rect.Height), new Vector2(rect.X, rect.Y + rect.Height - num), color, width, 2);
            drawLine(new Vector2(rect.X, rect.Y + num), new Vector2(rect.X, rect.Y + rect.Height - num), color, width, 2);
            drawLine(new Vector2(rect.X, rect.Y + num), new Vector2(rect.X + num, rect.Y), color, width, 2);

        }
        public static void recordOldPosAndRots(Projectile p, ref List<Vector2> odp, ref List<float> odr, int maxLength = 12)
        {
            odp.Add(p.Center);
            odr.Add(p.rotation);
            if (odp.Count > maxLength)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
        }
        public static float randomRot()
        {
            return (float)(Main.rand.NextDouble() * MathHelper.Pi * 2);
        }
        public static bool inWorld(int i, int j)
        {
            return !(i < 0 || j < 0 || i >= Main.tile.Width || j >= Main.tile.Height);
        }
        public static Projectile ToProj_Identity(this int id)
        {
            return Main.projectile.FirstOrDefault(x => x.identity == id);
        }
        public static bool isAir(int i, int j, bool plat = false)
        {
            return isAir(new Vector2(i * 16, j * 16), plat);
        }
        public static bool isAir(Vector2 dp, bool plat = false)
        {
            try
            {
                if (dp.X < 0 || dp.Y < 0 || (int)(dp.X / 16) > Main.tile.Width || (int)(dp.Y / 16) > Main.tile.Height)
                {
                    return true;
                }
                Tile tile = Main.tile[(int)(dp.X / 16), (int)(dp.Y / 16)];
                if (plat)
                {
                    if (tile != null && tile.HasTile)
                    {
                        if (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (tile != null && tile.HasTile)
                    {
                        if (Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public static NPC findTarget(Player player, Projectile proj, int maxDistance, bool check = false)
        {
            if (player.MinionAttackTargetNPC >= 0 && player.MinionAttackTargetNPC.ToNPC().active)
            {
                return player.MinionAttackTargetNPC.ToNPC();
            }
            return proj.FindTargetWithinRange(maxDistance, check);
        }
        public static Texture2D getExtraTex(string name)
        {
            return RequestTex("CalamityEntropy/Assets/Extra/" + name);
        }
        public static Asset<Texture2D> getExtraTexAsset(string name)
        {
            return ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/" + name, ReLogic.Content.AssetRequestMode.ImmediateLoad);
        }
        public static Rectangle GetCutTexRect(Texture2D tex, int count, int index, bool hor = true)
        {
            if (hor)
            {
                return new Rectangle(tex.Width / count * index, 0, tex.Width / count, tex.Height);
            }
            return new Rectangle(0, tex.Height / count * index, tex.Width, tex.Height / count);
        }
        public static void DrawAfterimage(Texture2D tx, List<Vector2> odp, List<float> odr, float scale = 1)
        {
            float ap = 1f / (float)odp.Count;
            for (int i = 0; i < odp.Count; i++)
            {
                Main.spriteBatch.Draw(tx, odp[i] - Main.screenPosition, null, Color.White * ap * 0.5f, odr[i], tx.Size() / 2, scale, SpriteEffects.None, 0);
                ap += 1f / (float)odp.Count;
            }
        }
        public static EGlobalItem Entropy(this Item item)
        {
            return item.GetGlobalItem<EGlobalItem>();
        }
        public static bool IsArmor(Item item, bool vanity = false)
        {
            return (vanity || !item.vanity) && (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1) && item.maxStack == 1;
        }
        public static EGlobalProjectile Entropy(this Projectile p)
        {
            return p.GetGlobalProjectile<EGlobalProjectile>();
        }
        public static EGlobalNPC Entropy(this NPC npc)
        {
            return npc.GetGlobalNPC<EGlobalNPC>();
        }
        public static NPC ToNPC(this int ins)
        {
            return Main.npc[ins];
        }

        public static EModPlayer Entropy(this Player player)
        {
            if (player.TryGetModPlayer<EModPlayer>(out var mp))
            {
                return mp;
            }
            return new EModPlayer();
        }

        public static Player GetOwner(this Projectile proj)
        {
            if (proj.owner < 0)
            {
                return null;
            }
            return proj.owner.ToPlayer();
        }
        public static Player ToPlayer(this int ins)
        {
            if (ins < 0 || !Main.player[ins].active)
            {
                return Main.LocalPlayer;
            }
            return Main.player[ins];
        }
        public static Projectile ToProj(this int ins)
        {
            return Main.projectile[ins];
        }
        public static float getRotateAngle(float rNow, float rTo, float rotateSpeed, bool sameSpeed = true)
        {
            float angleNow = MathHelper.ToDegrees(rNow);
            float angleTo = MathHelper.ToDegrees(rTo);
            if (angleNow > 180)
            {
                while (angleNow > 180)
                {
                    angleNow -= 360;
                }
            }
            if (angleNow < -180)
            {
                while (angleNow < -180)
                {
                    angleNow += 360;
                }
            }
            if (angleTo > 180)
            {
                while (angleTo > 180)
                {
                    angleTo -= 360;
                }
            }
            if (angleTo < -180)
            {
                while (angleTo < -180)
                {
                    angleTo += 360;
                }
            }
            float tz = 0;
            if (Math.Abs(angleNow + 360 - angleTo) < Math.Abs(angleTo - angleNow))
            {
                tz = angleTo - angleNow - 360;
            }
            else
            {
                if (Math.Abs(angleTo + 360 - angleNow) < Math.Abs(angleTo - angleNow))
                {
                    tz = angleTo + 360 - angleNow;
                }
                else
                {
                    tz = angleTo - angleNow;
                }
            }
            if (sameSpeed)
            {
                if (tz > rotateSpeed)
                {
                    tz = rotateSpeed;
                }
                if (tz < (rotateSpeed * -1))
                {
                    tz = rotateSpeed * -1;
                }
            }
            else
            {
                tz *= rotateSpeed;
            }
            return MathHelper.ToRadians(tz);

        }

        public static float ToRadians(this float f)
        {
            return MathHelper.ToRadians(f);
        }
        public static float RotateTowardsAngle(float currentRadians, float targetRadians, float rotateSpeed, bool useFixedSpeed = true)
        {
            currentRadians = MathHelper.WrapAngle(currentRadians);
            targetRadians = MathHelper.WrapAngle(targetRadians);
            
            float difference = targetRadians - currentRadians;
            float turnAmount = MathHelper.WrapAngle(difference);
            
            if (useFixedSpeed)
            {
                turnAmount = MathHelper.Clamp(turnAmount, -rotateSpeed, rotateSpeed);
            }
            else
            {
                turnAmount *= MathHelper.Clamp(rotateSpeed, 0f, 1f);
            }
            
            return currentRadians + turnAmount;
        }

        public static void wormFollow(int npc1, int npc2, int spacing = 48, bool type2 = false, float t2speed = 0.2f, float jrot = 0, float angleP = 0f)
        {
            if (type2)
            {
                NPC npc = Main.npc[npc1];
                NPC targetNPC = Main.npc[npc2];
                float rot = npc.rotation - jrot;
                npc.rotation = RotateTowardsAngle(rot, targetNPC.rotation + angleP - jrot, t2speed, false) + jrot;
                npc.Center = targetNPC.Center;
                Vector2 displacement = (RotateTowardsAngle(rot, targetNPC.rotation + angleP - jrot, t2speed, false)).ToRotationVector2() * -1 * spacing;
                npc.Center += displacement;
            }
            else
            {
                NPC npc = Main.npc[npc1];
                NPC targetNPC = Main.npc[npc2];
                float angle = (float)Math.Atan2(targetNPC.Center.Y - npc.Center.Y, targetNPC.Center.X - npc.Center.X);
                npc.rotation = angle + angleP + jrot;
                npc.Center = targetNPC.Center;
                Vector2 displacement = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -1 * spacing;
                npc.Center += displacement;

            }
        }
        public static void drawChain(Vector2 startPos, Vector2 endPos, int spacing, Texture2D tx, Color color)
        {
            int distance = ((int)Math.Sqrt(Math.Pow(endPos.X - startPos.X, 2) + Math.Pow(endPos.Y - startPos.Y, 2)));
            float rot = (endPos - startPos).ToRotation();
            float px = startPos.X;
            float py = startPos.Y;
            int num = ((int)(distance / spacing));
            Vector2 addVec = new Vector2((endPos.X - startPos.X) / num, (endPos.Y - startPos.Y) / num);
            addVec.Normalize();
            float adx = (endPos.X - startPos.X) / num;
            float ady = (endPos.Y - startPos.Y) / num;
            Vector2 drawPos = new Vector2(px, py);
            for (int i = 0; i <= num; i++)
            {
                Main.EntitySpriteDraw(tx, drawPos - Main.screenPosition, null, color, rot, new Vector2(tx.Width / 2, tx.Height / 2), (new Vector2(1, 1)), SpriteEffects.None, 0);
                drawPos.X += addVec.X * spacing;
                drawPos.Y += addVec.Y * spacing;
            }
        }
        public static void drawChain(Vector2 startPos, Vector2 endPos, int spacing, string texturePath, Color color)
        {
            drawChain(startPos, endPos, spacing, RequestTex(texturePath), color);
        }
        public static void drawChain(Vector2 startPos, Vector2 endPos, int spacing, string texturePath)
        {
            drawChain(startPos, endPos, spacing, RequestTex(texturePath), Color.White);
        }
        public static void drawChain(Vector2 startPos, Vector2 endPos, int spacing, Texture2D texture)
        {
            drawChain(startPos, endPos, spacing, texture, Color.White);
        }
        public static float getDistance(Vector2 v1, Vector2 v2)
        {
            return ((float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2)));
        }


        public static void drawTexture(Texture2D tex, Vector2 pos, float rotation, Color color, Vector2 scale, SpriteEffects eff = SpriteEffects.None)
        {
            Rectangle rectangle = new Rectangle(0, 0, tex.Width, tex.Height);
            Vector2 origin = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, rotation, origin, scale, eff, 0f);
        }
        public static bool LineThroughRect(Vector2 start, Vector2 end, Rectangle rect, int lineWidth = 4, int checkDistance = 8)
        {
            float point = 0f;
            return rect.Contains((int)start.X, (int)start.Y) || rect.Contains((int)end.X, (int)end.Y) || Collision.CheckAABBvLineCollision(rect.TopLeft(), rect.Size(), start, end, lineWidth, ref point);
        }

        public static void drawLine(SpriteBatch spriteBatch, Texture2D px, Vector2 start, Vector2 end, Color color, float width, int wa = 0, bool worldpos = true)
        {
            spriteBatch.Draw(px, start - (worldpos ? Main.screenPosition : Vector2.Zero), null, color, (end - start).ToRotation(), new Vector2(0, 0.5f), new Vector2(getDistance(start, end) + wa, width), SpriteEffects.None, 0);
        }
        public static void drawLine(Vector2 start, Vector2 end, Color color, float width, int wa = 0, bool worldpos = true)
        {
            Main.spriteBatch.Draw(getExtraTex("white"), start - (worldpos ? Main.screenPosition : Vector2.Zero), null, color, (end - start).ToRotation(), new Vector2(0, 0.5f), new Vector2(getDistance(start, end) + wa, width), SpriteEffects.None, 0);
        }
        public static void drawTextureToPoint(SpriteBatch sb, Texture2D texture, Color color, Vector2 lu, Vector2 ru, Vector2 ld, Vector2 rd)
        {
            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<ColoredVertex> ve = new List<ColoredVertex>();

            ve.Add(new ColoredVertex(lu,
                      new Vector3(0, 0, 1),
                      color));
            ve.Add(new ColoredVertex(ld,
                      new Vector3(0, 1, 1),
                      color));
            ve.Add(new ColoredVertex(ru,
                      new Vector3(1, 0, 1),
                      color));
            ve.Add(new ColoredVertex(rd,
                      new Vector3(1, 1, 1),
                      color));


            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Texture2D tx = texture;
            gd.Textures[0] = tx;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public static Vector2 getTxP(List<Vector2> points, float p)
        {
            float dl = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                float ds = getDistance(points[i], points[i + 1]);
                if (dl + ds < p)
                {
                    dl += ds;
                }
                else
                {
                    float pc = (p - dl) / ds;
                    return points[i] + (points[i + 1] - points[i]) * pc;
                }
            }

            return points[points.Count - 1];
        }
        public static void drawLaser(SpriteBatch sb, List<Texture2D> txs, List<Vector2> points, int txLength, Color color, int width = 64, int starttx = 0, float startRot = 0)
        {
            for (int j = 0; j < points.Count; j++)
            {
                points[j] -= Main.screenPosition;
            }
            float dl = 0;
            float al = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                al += getDistance(points[i], points[i + 1]);
            }
            int txc = starttx;
            float lr = startRot; Vector2 tp = Vector2.Zero;
            while (true)
            {
                Texture2D tx = txs[txc % txs.Count];
                if (dl > al)
                {
                    break;
                }
                if (dl + txLength > al)
                {
                    Vector2 dp = getTxP(points, dl);
                    Vector2 de = getTxP(points, dl + txLength);
                    tp = de;
                    float rot = (points[points.Count - 1] - points[points.Count - 2]).ToRotation();
                    Vector2 lrof = lr.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * width / 2;
                    Vector2 rof = rot.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * width / 2;
                    drawTextureToPoint(sb, tx, color, dp + lrof, de + rof, dp - lrof, de - rof);
                    lr = rot;
                    dl += txLength;
                    break;
                }
                else
                {
                    Vector2 dp = getTxP(points, dl);
                    Vector2 de = getTxP(points, dl + txLength);
                    tp = de;
                    float rot = (de - dp).ToRotation();
                    Vector2 lrof = lr.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * width / 2;
                    Vector2 rof = rot.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * width / 2;
                    drawTextureToPoint(sb, tx, color, dp + lrof, de + rof, dp - lrof, de - rof);
                    lr = rot;
                    dl += txLength;
                }

                txc += 1;
            }
        }
    }
}
