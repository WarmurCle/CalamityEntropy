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

                                                                                public static List<Vector2> GenerateLightning(Vector2 start, Vector2 end, float displacement, int detailLevel)
        {
            var points = new List<Vector2> { start, end };
            Subdivide(points, displacement, detailLevel);
            return points;
        }

                                                              private static void Subdivide(List<Vector2> points, float displacement, int depth)
        {
            var random = Main.rand;
            if (depth <= 0 || displacement <= 0)
                return;

            for (int i = points.Count - 1; i > 0; i--)
            {
                                 var start = points[i - 1];
                var end = points[i];

                                 var mid = (start + end) / 2;

                                 var offsetX = (float)(random.NextDouble() * 2 - 1) * displacement;
                var offsetY = (float)(random.NextDouble() * 2 - 1) * displacement;
                var offset = new Vector2(offsetX, offsetY);

                var midPoint = mid + offset;

                                 points.Insert(i, midPoint);
            }

                         Subdivide(points, displacement / 2, depth - 1);
        }
    }
}
