namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public static class BMCooldowns
    {
        public static int BMLightCD = 0;
        public static int BMProphecy = 0;
        public static int BMAbyss = 0;
        public static int BMSilva = 0;
        public static int BMVoid = 0;
        public static int BMAuric = 0;

        public static void Update()
        {
            CountDown(ref BMLightCD);
            CountDown(ref BMProphecy);
            CountDown(ref BMAbyss);
            CountDown(ref BMSilva);
            CountDown(ref BMVoid);
            CountDown(ref BMAuric);
        }
        public static void CountDown(ref int value)
        {
            if (value > 0)
            {
                value--;
            }
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