using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy
{
    public static class ScreenShaker
    {
        public class ScreenShake
        {
            public Vector2 Direction;
            public float amplitude;
            public float Counter = 0;
            public bool active => ScreenShaker.shakes.Contains(this);
            public ScreenShake(Vector2 direction, float amplitude)
            {
                this.Direction = direction;
                this.amplitude = amplitude;
            }
            public virtual void Update()
            {
                amplitude -= 0.1f;
                amplitude *= 0.9f;
                if(amplitude < 0)
                    amplitude = 0;
                Counter += amplitude;
            }
            public virtual Vector2 GetShiftVec()
            {
                return Direction * (0.5f + ((float)(Math.Cos(Counter * 0.034f)) * 0.5f + 0.5f)) * amplitude;
            }
        }
        public static List<ScreenShake> shakes;
        public static void Init()
        {
            shakes = new List<ScreenShake>();
        }
        public static void Unload()
        {
            shakes = null;
        }
        public static void Update()
        {
            foreach(var ss in shakes)
            {
                ss.Update();
            }
            for(int i = shakes.Count - 1; i >= 0; i--)
            {
                if (shakes[i].amplitude < 0.01f)
                {
                    shakes.RemoveAt(i);
                }
            }
        }
        public static void AddShake(ScreenShake shake)
        {
            shakes.Add(shake);
        }
        public static ScreenShake AddShake(float direction, float amp)
        {
            var s = new ScreenShake(direction.ToRotationVector2(), amp);
            AddShake(s);
            return s;
        }
        public static ScreenShake AddShake(Vector2 direction, float amp)
        {
            var s = new ScreenShake(direction, amp);
            AddShake(s);
            return s;
        }
        public static Vector2 GetShiftVec()
        {
            Vector2 vec = Vector2.Zero;
            foreach(var ss in shakes)
            {
                vec += ss.GetShiftVec();
            }    
            return vec;
        }
    }
}
