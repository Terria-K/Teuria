using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeuJson;

namespace Teuria;

public partial class Panel : Control, IDeserialize 
{
    public override void Draw(SpriteBatch spriteBatch)
    {
        Canvas.DrawRectangle(
            spriteBatch, PosX, PosY, 
            RectSize.X, RectSize.Y, currentStyle.BackgroundColor);
        base.Draw(spriteBatch);
    }
}