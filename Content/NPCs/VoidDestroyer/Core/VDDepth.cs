using System;
using Terraria;

namespace CalamityEntropy.Content.NPCs.VoidDestroyer.Core
{
    /// <summary>深度层位:远景层(墙后物块前)/ 平面(原层)/ 近景层(玩家之上)</summary>
    public enum VDDepthLayer : byte
    {
        Far,
        Plane,
        Near,
    }

    /// <summary>
    /// Z = 0 玩家平面,唯一有判定,Z &gt; 0 更深,Z &lt; 0 朝镜头,Scale = Focal / (Focal + Z)
    /// 服务端用目标玩家中心当相机代理
    /// </summary>
    public static class VDDepth
    {
        /// <summary>Z 对应的绘制缩放;近端钳在 <see cref="VDDirector.DepthNearClamp"/>,再近只会是一团糊</summary>
        public static float Scale(float z) {
            z = Math.Max(z, VDDirector.DepthNearClamp);
            return VDDirector.DepthFocal / (VDDirector.DepthFocal + z);
        }

        /// <summary>本地相机中心(世界坐标):原版缩放围绕它进行,所以投影后再经 GameViewMatrix 也自洽</summary>
        public static Vector2 CameraCenter => Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;

        /// <summary>透视投影:世界点按 Z 向相机中心收敛,返回仍是世界坐标(减去 screenPosition 就是屏幕坐标)</summary>
        public static Vector2 Project(Vector2 world, float z) => Project(world, z, CameraCenter);

        public static Vector2 Project(Vector2 world, float z, Vector2 camera) => camera + (world - camera) * Scale(z);

        /// <summary>表观偏移 → 世界偏移:想让一个 Z 处的东西看起来离相机中心 apparent 远,世界上要放到 apparent / Scale 处</summary>
        public static Vector2 WorldOffset(Vector2 apparentOffset, float z) => apparentOffset / Scale(z);

        /// <summary>服务端摆位:以代理相机(目标玩家中心)为准,把「表观偏移 + Z」换成世界坐标</summary>
        public static Vector2 WorldFromApparent(Vector2 proxyCamera, Vector2 apparentOffset, float z) => proxyCamera + WorldOffset(apparentOffset, z);

        /// <summary>透视权重 w = 1 / Scale(Z):平面为 1,越远越大,镜头前小于 1;1/w 在屏幕上线性插值,是透视校正的基础量</summary>
        public static float W(float z) => 1f / Scale(z);

        /// <summary>屏幕分数 t 对应世界分数 f,z0 = 2.5、z1 = 0 时 t = 0.5 处 f ≈ 0.78</summary>
        public static float ScreenToWorldFraction(float z0, float z1, float t) {
            t = MathHelper.Clamp(t, 0f, 1f);
            float w0 = W(z0);
            float w1 = W(z1);
            float denom = (1f - t) * w1 + t * w0;
            return denom <= 0.0001f ? t : t * w0 / denom;
        }

        /// <summary>直线段上屏幕分数 t 处的 Z(Z 沿世界分数线性,所以先把 t 换成 f 再插);子段射线求两端 Z 用</summary>
        public static float ZAtScreenFraction(float z0, float z1, float t) => MathHelper.Lerp(z0, z1, ScreenToWorldFraction(z0, z1, t));

        /// <summary>判定带半宽:按 Z 速度放宽,快弹穿越平面也留得住 3 帧碰撞</summary>
        public static float HitBand(float zVel) => Math.Max(VDDirector.DepthHitBandMin, VDDirector.DepthHitBandVelMult * Math.Abs(zVel));

        /// <summary>是否在判定带内(只有这里有碰撞)</summary>
        public static bool InHitBand(float z, float zVel = 0f) => Math.Abs(z) <= HitBand(zVel);

        /// <summary>按 Z 分层</summary>
        public static VDDepthLayer LayerOf(float z) {
            if (z >= VDDirector.DepthFarLayerZ) {
                return VDDepthLayer.Far;
            }
            if (z <= VDDirector.DepthNearLayerZ) {
                return VDDepthLayer.Near;
            }
            return VDDepthLayer.Plane;
        }

        /// <summary>远端雾化量 0..1:Z 从 0 到 DepthFogFullZ 线性;近端为 0</summary>
        public static float FogAmount(float z) => MathHelper.Clamp(z / VDDirector.DepthFogFullZ, 0f, 1f);

        /// <summary>远端雾色:向深空冷色插值并顺带去一点饱和</summary>
        public static Color Fog(Color color, float z) {
            float fog = FogAmount(z);
            if (fog <= 0f) {
                return color;
            }
            return Color.Lerp(color, VDVfx.FarFog, fog * 0.75f);
        }

        /// <summary>远端随缩放暗,留 55% 地板,近端从门槛线性淡到 DepthNearFadeZ,不超过剪影上限</summary>
        public static float Alpha(float z) {
            if (z >= 0f) {
                return MathHelper.Lerp(VDDirector.DepthFarAlphaFloor, 1f, Scale(z));
            }
            if (z >= VDDirector.DepthNearLayerZ) {
                //平面到近景门槛之间:从 1 平滑压到剪影上限,进近景层那一刻不跳变
                float t = z / VDDirector.DepthNearLayerZ;
                return MathHelper.Lerp(1f, VDDirector.DepthNearAlphaMax, t);
            }
            float fade = MathHelper.Clamp((z - VDDirector.DepthNearFadeZ) / (VDDirector.DepthNearLayerZ - VDDirector.DepthNearFadeZ), 0f, 1f);
            return VDDirector.DepthNearAlphaMax * fade;
        }

        /// <summary>多普勒音高:Z 从 DepthFogFullZ 到 0 由低到高</summary>
        public static float DopplerPitch(float z) {
            float t = 1f - MathHelper.Clamp(z / VDDirector.DepthFogFullZ, 0f, 1f);
            return MathHelper.Lerp(VDDirector.DepthDopplerLow, VDDirector.DepthDopplerHigh, t);
        }

        /// <summary>相机在地表下,或投影落在实心物块里,退回原层画</summary>
        public static bool FarLayerUsable(Vector2 projectedWorld) {
            if (Main.dedServ) {
                return false;
            }
            if (CameraCenter.Y > Main.worldSurface * 16f) {
                return false;
            }
            int tx = (int)(projectedWorld.X / 16f);
            int ty = (int)(projectedWorld.Y / 16f);
            if (!WorldGen.InWorld(tx, ty, 2)) {
                return true;
            }
            Tile tile = Framing.GetTileSafely(tx, ty);
            return !(tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]);
        }

        /// <summary>frames 帧正中到顶、走完回 0,半隐式积分早约一帧,不要把 Z 等于 0 当成时钟</summary>
        public static (float zVel, float zAccel) Parabola(float apex, int frames) {
            float half = frames * 0.5f;
            float accel = -2f * apex / (half * half);
            return (-accel * half, accel);
        }

        /// <summary>立方缓入:俯冲/入场的「朝镜头飞来」曲线(慢起、猛到)</summary>
        public static float DiveCurve(float t) {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t * t * t;
        }

        /// <summary>立方缓出:退入深处的曲线(猛起、慢停)</summary>
        public static float RetreatCurve(float t) {
            t = MathHelper.Clamp(t, 0f, 1f);
            float inv = 1f - t;
            return 1f - inv * inv * inv;
        }
    }
}
