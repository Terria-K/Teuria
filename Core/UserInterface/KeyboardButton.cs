using Teuria;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public abstract class KeyboardButton : Entity 
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

    public KeyboardButton(SpriteFont fontText, string text, bool firstSelected = false) 
    {
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
        if (TInput.Keyboard.JustPressed(Keys.C)) 
        {
            OnEnter();
            return;
        }
        if (TInput.Keyboard.JustPressed(Keys.Up) && UpFocus != null) 
        {
            UpFocus.Selected = true;
            Selected = false;
            return;
        }
        if (TInput.Keyboard.JustPressed(Keys.Down) && DownFocus != null) 
        {
            DownFocus.Selected = true;
            Selected = false;
            return;
        }
        if (TInput.Keyboard.JustPressed(Keys.Left) && LeftFocus != null) 
        {
            LeftFocus.Selected = true;
            Selected = false;
            return;
        }
        if (TInput.Keyboard.JustPressed(Keys.Right) && RightFocus != null) 
        {
            RightFocus.Selected = true;
            Selected = false;
        }
        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(fontText, text, this.Position, Modulate);
        base.Draw(spriteBatch);
    }

    public virtual void OnFocus() {}
    public virtual void OnRelease() {}
    public virtual void OnEnter() 
    {
        OnConfirm?.Invoke();
    }

}
