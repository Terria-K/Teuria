using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Panel : Control 
{
    public override void Draw(SpriteBatch spriteBatch)
    {
        Canvas.DrawRectangle(
            spriteBatch, PosX + RectSize.X, PosY + RectSize.Y, 
            RectSize.Width, RectSize.Height, currentStyle.BackgroundColor);
        base.Draw(spriteBatch);
    }
}