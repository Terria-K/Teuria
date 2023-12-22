using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeuJson;

namespace Teuria;

public partial class Button : Control, IDeserialize
{
    public Action? OnLeftClicked;
    public Action? OnRightClicked;
    public Action? OnMiddleClicked;

    public Button? LeftItem;
    public Button? RightItem;
    public Button? UpItem;
    public Button? DownItem;

    public InputButton? InputLeft;
    public InputButton? InputRight;
    public InputButton? InputUp;
    public InputButton? InputDown;
    public InputButton? InputEnter;
    public bool Selected;
    public int AreaPadding;

    public override Rectangle MouseDetectionArea()
    {
        return new Rectangle((int)PosX + AreaPadding, (int)PosY + AreaPadding, (int)RectSize.X, (int)RectSize.Y);
    }

    public override void Update()
    {
        if (InputEnter != null && InputEnter.JustPressed)
        {
            OnLeftClicked?.Invoke();
        }
        else if (InputUp != null && InputUp.JustPressed && UpItem != null && UpItem.Visible)
        {
            UpItem.Selected = true;
            Selected = false;
        }
        else if (InputDown != null && InputDown.JustPressed && DownItem != null && DownItem.Visible)
        {
            DownItem.Selected = true;
            Selected = false;
        }
        else if (InputLeft != null && InputLeft.JustPressed && LeftItem != null && LeftItem.Visible)
        {
            LeftItem.Selected = true;
            Selected = false;
        }
        else if (InputRight != null && InputRight.JustPressed && RightItem != null && RightItem.Visible)
        {
            RightItem.Selected = true;
            Selected = false;
        }
        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Canvas.DrawRectangle(
            spriteBatch, PosX, PosY, 
            RectSize.X, RectSize.Y, currentStyle.BackgroundColor);
        base.Draw(spriteBatch);
    }


    public override void OnMouseEvent(MouseButton state)
    {
        switch (state) 
        {
        case MouseButton.LeftClicked:
            OnLeftClicked?.Invoke();
            break;
        case MouseButton.RightClicked:
            OnRightClicked?.Invoke();
            break;
        case MouseButton.MiddleClicked:
            OnMiddleClicked?.Invoke();
            break;
        }
    }
}