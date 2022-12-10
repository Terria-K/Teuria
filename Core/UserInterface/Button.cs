using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

[Obsolete("This Button Entity has some major issues from Mouse Input. Please use the KeyboardButton for now")]
public class Button : Entity 
{
    public Action OnHover;
    public Action OnClick;

    private Sprite sprite;
    private bool isHovered;    
    private Shape collider;

    public Button(SpriteTexture texture) 
    {
        sprite = new Sprite(texture);
        collider = new RectangleShape(sprite.Texture.Width, sprite.Texture.Height, Position);
        AddComponent(sprite);
    }

    public Shape Collider 
    { 
        get => collider; 
        set 
        {
            if (value == collider) return;
#if DEBUG
            if (value.Entity != null)
                throw new Exception("This collider has been used by another Entity");
#endif
            collider?.Removed();
            collider = value;
            collider?.Added(this);
        }
    }

    public AABB BoundingArea => throw new NotImplementedException();

    public void Detect(ICollidableEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Detect(HashSet<ICollidableEntity> entity)
    {
        throw new NotImplementedException();
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
        if (collider.Collide(mousePos) && !isHovered) 
        {
            isHovered = true;
            return;
        }
        if (!collider.Collide(mousePos)) 
        {
            isHovered = false;
        }
    }
}