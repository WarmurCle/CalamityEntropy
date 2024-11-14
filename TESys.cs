using CalamityEntropy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public class TEData
    {
        Dictionary<string, int> data_int = new Dictionary<string, int>();
        Dictionary<string, string> data_string = new Dictionary<string, string>();
        Dictionary<string, bool> data_bool = new Dictionary<string, bool>();
        public void putInt(string key, int value)
        {
            data_int[key] = value;
        }
        public void putString(string key, string value)
        {
            data_string[key] = value;
        }
        public void putBoolean(string key, bool value) { 
            data_bool[key] = value; 
        }
        public int getInt(string key)
        {
            if (!data_int.ContainsKey(key))
            {
                return 0;
            }
            return data_int[key];
        }
        public string getString(string key)
        {
            if (!data_string.ContainsKey(key))
            {
                return "";
            }
            return data_string[key];
        }
        public bool getBoolean(string key) {
            if (!data_bool.ContainsKey(key))
            {
                return false;
            }
            return data_bool[key]; 
        }
    }
    
    public class TESys : ModSystem
    {
        public List<Vector2> tePos = new List<Vector2>();
        public List<TEData> tEDatas = new List<TEData>();
        public void AddPos(Vector2 pos)
        {
            tePos.Add(pos);
        }
        

        public override void PostUpdateNPCs()
        {
            int c = 0;
            while (c < tePos.Count)
            {
                Vector2 pos = tePos[c];
                if (Main.tile[(int)(pos.X), (int)(pos.Y)].HasTile)
                {

                }
                else
                {
                    tePos.RemoveAt(c);
                    tEDatas.RemoveAt(c);
                    c--;
                }
            }
        }

        public TEData getData(Vector2 pos)
        {
            for (int i = 0; i < tePos.Count; i++)
            {
                if (tePos[i] == pos)
                {
                    return tEDatas[i];
                }
            }
            tePos.Add(pos);
            TEData td = new TEData();
            tEDatas.Add(td);
            return td;
        }
    }
}
