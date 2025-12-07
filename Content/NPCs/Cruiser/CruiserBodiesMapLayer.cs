using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.Map.MapOverlayDrawContext;

namespace CalamityEntropy.Content.NPCs.Cruiser
{
    public class CruiserBodiesMapLayer : ModMapLayer
    {
        public static bool drawOutline = false;
        public override Position GetDefaultPosition() => new Before(IMapLayer.Spawn);
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            Texture2D crBody = CEUtils.RequestTex("CalamityEntropy/Content/NPCs/Cruiser/CruiserBody_Head_Boss");
            Texture2D crTail = CEUtils.RequestTex("CalamityEntropy/Content/NPCs/Cruiser/CruiserTail_Head_Boss");

            Texture2D crHead1 = CEUtils.RequestTex("CalamityEntropy/Content/NPCs/Cruiser/CruiserHead_Head_Boss");
            Texture2D crHead2 = CEUtils.RequestTex("CalamityEntropy/Content/NPCs/Cruiser/p2head");

            bool anythingToDraw = true;
            if (anythingToDraw)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    Texture2D tex = null;
                    float drawRot = 0;
                    bool needDraw = false;
                    Vector2 drawPos = npc.Center / 16;
                    float scale = 1;
                    if (npc.ModNPC != null)
                    {
                        if (npc.ModNPC is CruiserBody cb && !cb.Phase2)
                        {
                            needDraw = true;
                            tex = crBody;
                            drawRot = npc.rotation;
                        }
                        if (npc.ModNPC is CruiserTail ct && !ct.Phase2)
                        {
                            needDraw = true;
                            tex = crTail;
                            drawRot = npc.rotation;
                        }
                        /*if (npc.ModNPC is CruiserHead ch)
                        {
                            needDraw = true;
                            tex = ch.phaseTrans >= 120 ? crHead2 : crHead1;
                            drawRot = npc.rotation - MathHelper.PiOver2;
                        }*/
                    }
                    if (needDraw)
                        Draw(context, tex, drawPos, Color.White, new SpriteFrame(1, 1, 0, 0), 1 * scale, 1 * scale, drawRot, Alignment.Center, SpriteEffects.None);
                }
            }
        }
        public DrawResult Draw(MapOverlayDrawContext context, Texture2D texture, Vector2 position, Color color, SpriteFrame frame, float scaleIfNotSelected, float scaleIfSelected, float rotation, Alignment alignment, SpriteEffects spriteEffects)
        {
            Vector2 _mapPosition = (Vector2)typeof(MapOverlayDrawContext).GetField("_mapPosition", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(context);
            Vector2 _mapOffset = (Vector2)typeof(MapOverlayDrawContext).GetField("_mapOffset", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(context);
            float _mapScale = (float)typeof(MapOverlayDrawContext).GetField("_mapScale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(context);
            float _drawScale = (float)typeof(MapOverlayDrawContext).GetField("_drawScale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(context);

            Rectangle? _clippingRect = (Rectangle?)typeof(MapOverlayDrawContext).GetField("_clippingRect", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(context);

            position = (position - _mapPosition) * _mapScale + _mapOffset;
            if (_clippingRect.HasValue && !_clippingRect.Value.Contains(position.ToPoint()))
                return DrawResult.Culled;

            Rectangle sourceRectangle = frame.GetSourceRectangle(texture);
            Vector2 vector = sourceRectangle.Size() * alignment.OffsetMultiplier;
            Vector2 position2 = position;
            float num = scaleIfNotSelected;
            Vector2 vector2 = position - vector * num;
            bool num2 = new Rectangle((int)vector2.X, (int)vector2.Y, (int)((float)sourceRectangle.Width * num), (int)((float)sourceRectangle.Height * num)).Contains(Main.MouseScreen.ToPoint());
            float scale = num;
            if (num2)
                scale = scaleIfSelected;
            scale *= _mapScale;
            Main.spriteBatch.Draw(texture, position2, sourceRectangle, color, rotation, vector, scale * 0.25f, spriteEffects, 0f);

            return new DrawResult(num2);
        }
    }
}
