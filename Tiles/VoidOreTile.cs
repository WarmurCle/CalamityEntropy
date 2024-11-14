
using CalamityMod;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityEntropy.Tiles
{
	public class VoidOreTile : ModTile
	{
		public override void SetStaticDefaults() {
            Main.tileLighted[base.Type] = true;
            Main.tileSolid[base.Type] = true;
            Main.tileBlockLight[base.Type] = true;
            Main.tileSpelunker[base.Type] = true;
            Main.tileOreFinderPriority[base.Type] = 1000;
            Main.tileShine[base.Type] = 3500;
            Main.tileShine2[base.Type] = false;
            CalamityUtils.MergeWithGeneral(base.Type);
            TileID.Sets.Ore[base.Type] = true;
            TileID.Sets.OreMergesWithMud[base.Type] = true;
            base.DustType = 173;
            AddMapEntry(Color.DarkBlue, CreateMapEntryName());
            base.MineResist = 5f;
            base.MinPick = 250;
            base.HitSound = AuricOre.MineSound;
        }

		// Example of how to enable the Biome Sight buff to highlight this tile. Biome Sight is technically intended to show "infected" tiles, so this example is purely for demonstration purposes.
		public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor) {
			sightColor = Color.Purple;
			return true;
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Lighting.AddLight(new Vector2(i, j) * 16, 0.4f, 0.4f, 1.2f);
        }
    }

	// ExampleOreSystem contains code related to spawning ExampleOre. It contains both spawning ore during world generation, seen in ModifyWorldGenTasks, and spawning ore after defeating a boss, seen in BlessWorldWithExampleOre and MinionBossBody.OnKill.
	public class VoidOreSystem : ModSystem
	{
		public static LocalizedText VoidOrePassMessage { get; private set; }
		public static LocalizedText BlessedWithVoidOreMessage { get; private set; }

		public override void SetStaticDefaults() {
			BlessedWithVoidOreMessage = Mod.GetLocalization($"WorldGen.VoidOreSpawn");
		}

		public static void BlessWorldWithOre() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return; // This should not happen, but just in case.
			}

			ThreadPool.QueueUserWorkItem(_ => {
				if (Main.netMode == NetmodeID.SinglePlayer) {
					Main.NewText(BlessedWithVoidOreMessage.Value, 50, 255, 130);
				}
				else if (Main.netMode == NetmodeID.Server) {
					ChatHelper.BroadcastChatMessage(BlessedWithVoidOreMessage.ToNetworkText(), new Color(50, 255, 130));
				}

				// 100 controls how many splotches of ore are spawned into the world, scaled by world size. For comparison, the first 3 times altars are smashed about 275, 190, or 120 splotches of the respective hardmode ores are spawned. 
				int splotches = (int)(9 * (Main.maxTilesX / 4200f));
				int highestY = 80;
				int downY = 180;
				for (int iteration = 0; iteration < splotches; iteration++) {
					int i = WorldGen.genRand.Next(60, Main.maxTilesX - 60);
					int j = WorldGen.genRand.Next(highestY, downY);
					float dist = 0;
					float angle = (float)(WorldGen.genRand.NextDouble() * Math.PI * 2);
					int size = WorldGen.genRand.Next(240, 320);

                    for (int so = 0; so < size; so ++)
					{
						Vector2 pos = new Vector2(i, j) + angle.ToRotationVector2() * dist;
						float os = (3f * (1f - (float)so / (float)size));
						if (os > 0.9f)
						{
							spawnOreCircle((int)Math.Round(pos.X, 0), (int)Math.Round(pos.Y, 0), os);
						}
                        dist += 0.08f;
                        
						if (size % 2 == 0)
						{
                            angle += (float)MathHelper.Pi * 0.02f;
                        }
						else
						{
                            angle -= (float)MathHelper.Pi * 0.02f;
                        }
					}
					
                    
                }
			});
		}
		public static void spawnOreCircle(int ci, int cj, float r)
		{
            for (int si = -12 + ci; si < 13 + ci; si++)
            {
                for (int sj = -12 + cj; sj < 13 + cj; sj++)
                {
					if (Util.Util.getDistance(new Vector2(si, sj), new Vector2(ci, cj)) <= r)
					{
						tryToSpawn(si, sj);
					}
                }
            }
        }
		public static bool tryToSpawn(int si, int sj)
		{
            if (Util.Util.inWorld(si, sj))
            {
                if (!Main.tile[si, sj].HasTile)
                {
                    ushort type = (ushort)ModContent.TileType<VoidOreTile>();
                    Tile t = Main.tile[si, sj];
                    t.TileType = type;
                    t.HasTile = true;
                    WorldGen.SquareTileFrame(si, sj);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, si, sj);
					return true;
                }
            }
			return false;
        }

	}

}
