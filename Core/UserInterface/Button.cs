using System;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

[Obsolete("This Button Entity has some major issues from Mouse Input. Please use the KeyboardButton for now")]
public class Button : Entity
{
    public Action OnHover;
    public Action OnClick;

    private Sprite sprite;
    private bool isHovered;    
    private Hitbox hitbox;

    public Button(Texture2D texture) 
    {
        sprite = new Sprite(texture);
        hitbox = new Hitbox(sprite.texture.Width, sprite.texture.Height, Position);
        AddComponent(sprite);
        AddComponent(hitbox);
    }

    public override void Update()
    {
        Hovering();
        base.Update();
    }

    private void Hovering() 
    {
        var mousePos = TInput.Mouse.Position;
        if (isHovered) 
        {
            OnHover?.Invoke();
            
            if (TInput.Mouse.JustLeftClicked()) 
            {
                OnClick?.Invoke();
            }
        }
        if (hitbox.Collide(mousePos) && !isHovered) 
        {
            isHovered = true;
            return;
        }
        if (!hitbox.Collide(mousePos)) 
        {
            isHovered = false;
        }
    }
}