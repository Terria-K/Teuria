using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Teuria;

public class Button : Entity
{
    public event Action OnHover;
    public event Action OnClick;

    private Sprite sprite;
    private bool isHovered;
    private bool isClicked;

    public Button(Texture2D texture) 
    {
        sprite = new Sprite(texture);
        AddComponent(sprite);
    }

    public override Rectangle Rectangle { get => SpriteFixedRectangle(sprite); }

    public override void Update()
    {
        Hovering();
        base.Update();
    }

    private void Hovering() 
    {
        var mouseState = Mouse.GetState();
        var point = new Point(mouseState.X, mouseState.Y);
        if (isHovered) 
        {
            OnHover?.Invoke();
        }
        if (Rectangle.Contains(point)) 
        {
            isHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed && !isClicked) 
            {
                isClicked = true;
                OnClick?.Invoke();
            }
            return;
        }
        isHovered = false;
        isClicked = false;
    }
}