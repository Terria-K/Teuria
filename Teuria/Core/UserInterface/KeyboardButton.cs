using Teuria;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Teuria;

public class KeyboardButton : Entity 
{
    public Action OnConfirm;
    public KeyboardButton UpFocus;
    public KeyboardButton DownFocus;
    public KeyboardButton LeftFocus;
    public KeyboardButton RightFocus;
    public bool Selected 
    {
        get => selected;
        set 
        {
            if (!selected && value) 
            {
                selected = true;
                canHandleInput = false;
                OnFocus();
                return;
            }
            if (selected && !value) 
            {
                selected = false;
                OnRelease();
            }
        }
    }
    private bool selected = false;
    private bool canHandleInput = false;
    private string text;
    private SpriteFont fontText;
    private bool centered;

    public KeyboardButton(SpriteFont fontText, string text, bool firstSelected = false, bool center = false) 
    {
        centered = center;
        this.fontText = fontText;
        this.text = text;
        selected = firstSelected;
        if (selected) 
        {
            OnFocus();
        }
    }

    public override void Update()
    {
        if (!canHandleInput) 
        {
            canHandleInput = true;
            return;
        }
        if (!selected) return;
        if (TInput.IsKeyJustPressed(Keys.C)) 
        {
            OnEnter();
            return;
        }
        if (TInput.IsKeyJustPressed(Keys.Up) && UpFocus != null) 
        {
            UpFocus.Selected = true;
            Selected = false;
            return;
        }
        if (TInput.IsKeyJustPressed(Keys.Down) && DownFocus != null) 
        {
            DownFocus.Selected = true;
            Selected = false;
            return;
        }
        if (TInput.IsKeyJustPressed(Keys.Left) && LeftFocus != null) 
        {
            LeftFocus.Selected = true;
            Selected = false;
            return;
        }
        if (TInput.IsKeyJustPressed(Keys.Right) && RightFocus != null) 
        {
            RightFocus.Selected = true;
            Selected = false;
        }
        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var position = this.Position;
        if (centered) 
        {
            position = new Vector2(fontText.MeasureScreenString(text, TeuriaEngine.ScreenWidth - this.Position.X), this.Position.Y);
        }
        spriteBatch.DrawString(fontText, text, position, Modulate);
        base.Draw(spriteBatch);
    }

    public virtual void OnFocus() {}
    public virtual void OnRelease() {}
    public virtual void OnEnter() 
    {
        OnConfirm?.Invoke();
    }

}
