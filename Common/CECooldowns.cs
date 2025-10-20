using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Common
{
    public class CooldownShort
    {
        public int Time;
        public string ID;
        public CooldownShort(int time, string id)
        {
            Time = time;
            ID = id;
        }
    }
    public static class CECooldowns
    {
        public static List<CooldownShort> cooldowns = new List<CooldownShort>();
        public static int BMLightCD = 0;
        public static int BMProphecy = 0;
        public static int BMAbyss = 0;
        public static int BMSilva = 0;
        public static int BMVoid = 0;
        public static int BMAuric = 0;
        public static int MineBoxCd = 0;
        public static int BMTaurus = 0;
        public static void Update()
        {
            CountDown(ref BMLightCD);
            CountDown(ref BMProphecy);
            CountDown(ref BMAbyss);
            CountDown(ref BMSilva);
            CountDown(ref BMVoid);
            CountDown(ref BMAuric);
            CountDown(ref MineBoxCd);
            CountDown(ref BMTaurus);

            for (int i = 0; i < cooldowns.Count; i++)
            {
                cooldowns[i].Time--;
            }
            for (int i = cooldowns.Count - 1; i >= 0; i--)
            {
                if (cooldowns[i].Time <= 0)
                {
                    cooldowns.RemoveAt(i);
                }
            }
        }
        public static void AddCooldown(string id, int time)
        {
            cooldowns.Add(new CooldownShort(time.ApplyCdDec(Main.LocalPlayer), id));
        }
        public static bool HasCooldown(string id)
        {
            return cooldowns.Find((cd) => cd.ID == id) != null;
        }
        public static void CountDown(ref int value)
        {
            if (value > 0)
            {
                value--;
            }
        }
        public static bool CheckCD(string id, int maxValue = 60, bool reset = true)
        {
            var cd = cooldowns.Find((cd) => cd.ID == id);
            if (cd != null)
            {
                return false;
            }

            if (reset)
            {
                AddCooldown(id, maxValue);
            }
            return true;
        }
        public static bool CheckCD(ref int value, int maxValue = 60, bool reset = true)
        {
            if (value > 0)
            {
                return false;
            }

            if (reset)
            {
                value = maxValue;
            }
            return true;
        }
    }
}