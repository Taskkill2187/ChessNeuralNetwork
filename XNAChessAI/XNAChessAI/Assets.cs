using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XNAChessAI
{
    public static class Assets
    {
        public static SpriteFont Font;
        public static SpriteFont SmallFont;
        public static Texture2D White;
        
        public static void Load(ContentManager Content, GraphicsDevice GD)
        {
            White = new Texture2D(GD, 1, 1);
            Color[] Col = new Color[1];
            Col[0] = Color.White;
            White.SetData(Col);
            Font = Content.Load<SpriteFont>("Font");
            SmallFont = Content.Load<SpriteFont>("SmallFont");
        }

        public static void DrawRectangleBorder(Rectangle Rect, Color Col, int Width, SpriteBatch SB)
        {
            SB.Draw(White, new Rectangle(Rect.X, Rect.Y, Rect.Width, Width), Col);
            SB.Draw(White, new Rectangle(Rect.X, Rect.Y + Rect.Height - Width, Rect.Width, Width), Col);
            SB.Draw(White, new Rectangle(Rect.X, Rect.Y + Width, Width, Rect.Height - Width * 2), Col);
            SB.Draw(White, new Rectangle(Rect.X + Rect.Width - Width, Rect.Y + Width, Width, Rect.Height - Width * 2), Col);
        }
        public static void DrawLine(Vector2 End1, Vector2 End2, int Thickness, Color Col, SpriteBatch SB)
        {
            Vector2 Line = End1 - End2;
            SB.Draw(White, End1, null, Col, -(float)Math.Atan2(Line.X, Line.Y) - (float)Math.PI / 2, new Vector2(0, 0.5f), new Vector2(Line.Length(), Thickness), SpriteEffects.None, 0f);
        }
        public static void DrawCircle(Vector2 Pos, float Radius, Color Col, SpriteBatch SB)
        {
            if (Radius < 0)
                Radius *= -1;

            for (int i = -(int)Radius; i < (int)Radius; i++)
            {
                int HalfHeight = (int)Math.Sqrt(Radius * Radius - i * i);
                SB.Draw(White, new Rectangle((int)Pos.X + i, (int)Pos.Y - HalfHeight, 1, HalfHeight * 2), Col);
            }
        }
        public static void DrawCircle(Vector2 Pos, float Radius, float HeightMultiplikator, Color Col, SpriteBatch SB)
        {
            if (Radius < 0)
                Radius *= -1;

            for (int i = -(int)Radius; i < (int)Radius; i++)
            {
                int HalfHeight = (int)Math.Sqrt(Radius * Radius - i * i);
                SB.Draw(White, new Rectangle((int)Pos.X + i, (int)Pos.Y, 1, (int)(HalfHeight * HeightMultiplikator)), Col);
            }

            for (int i = -(int)Radius; i < (int)Radius; i++)
            {
                int HalfHeight = (int)Math.Sqrt(Radius * Radius - i * i);
                SB.Draw(White, new Rectangle((int)Pos.X + i + 1, (int)Pos.Y, -1, (int)(-HalfHeight * HeightMultiplikator)), Col);
            }
        }
        public static void DrawRoundedRectangle(Rectangle Rect, float PercentageOfRounding, Color Col, SpriteBatch SB)
        {
            float Rounding = PercentageOfRounding / 100;
            Rectangle RHorz = new Rectangle(Rect.X, (int)(Rect.Y + Rect.Height * (Rounding / 2)), Rect.Width, (int)(Rect.Height * (1-Rounding)));
            Rectangle RVert = new Rectangle((int)(Rect.X + Rect.Width * (Rounding / 2)), Rect.Y, (int)(Rect.Width * (1-Rounding)), (int)(Rect.Height * 0.999f));

            int RadiusHorz = (int)(Rect.Width * (Rounding / 2));
            int RadiusVert = (int)(Rect.Height * (Rounding / 2));

            if (RadiusHorz != 0)
            {
                // Top-Left
                DrawCircle(new Vector2(Rect.X + RadiusHorz, Rect.Y + RadiusVert), RadiusHorz, RadiusVert / (float)RadiusHorz, Col, SB);

                // Top-Right
                DrawCircle(new Vector2(Rect.X + Rect.Width - RadiusHorz - 1, Rect.Y + RadiusVert), RadiusHorz, RadiusVert / (float)RadiusHorz, Col, SB);

                // Bottom-Left
                DrawCircle(new Vector2(Rect.X + RadiusHorz, Rect.Y + RadiusVert + (int)(Rect.Height * (1 - Rounding))), RadiusHorz, RadiusVert / (float)RadiusHorz, Col, SB);

                // Bottom-Right
                DrawCircle(new Vector2(Rect.X + Rect.Width - RadiusHorz -1, Rect.Y + RadiusVert + (int)(Rect.Height * (1 - Rounding))), RadiusHorz, RadiusVert / (float)RadiusHorz, Col, SB);
            }

            SB.Draw(White, RHorz, Col);
            SB.Draw(White, RVert, Col);
        }
    }
}
