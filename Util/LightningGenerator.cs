using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.Util
{
    public class LightningGenerator
    {

        /// <summary>
        /// 生成类似闪电的点列表
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="displacement">初始偏移量</param>
        /// <param name="detailLevel">递归深度（决定细节程度）</param>
        /// <returns>点列表</returns>
        public static List<Vector2> GenerateLightning(Vector2 start, Vector2 end, float displacement, int detailLevel)
        {
            var points = new List<Vector2> { start, end };
            Subdivide(points, displacement, detailLevel);
            return points;
        }

        /// <summary>
        /// 递归细分线段
        /// </summary>
        /// <param name="points">点列表</param>
        /// <param name="displacement">偏移量</param>
        /// <param name="depth">当前深度</param>
        private static void Subdivide(List<Vector2> points, float displacement, int depth)
        {
            var random = Main.rand;
            if (depth <= 0 || displacement <= 0)
                return;

            for (int i = points.Count - 1; i > 0; i--)
            {
                // 找到当前线段的起点和终点
                var start = points[i - 1];
                var end = points[i];

                // 计算中点
                var mid = (start + end) / 2;

                // 添加随机偏移
                var offsetX = (float)(random.NextDouble() * 2 - 1) * displacement;
                var offsetY = (float)(random.NextDouble() * 2 - 1) * displacement;
                var offset = new Vector2(offsetX, offsetY);

                var midPoint = mid + offset;

                // 插入中点
                points.Insert(i, midPoint);
            }

            // 减小偏移量并递归
            Subdivide(points, displacement / 2, depth - 1);
        }
    }
}
