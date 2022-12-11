using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Teuria;

public class ButtonContainer : Entity 
{
    private List<KeyboardButton> buttons = new List<KeyboardButton>();
    private readonly int offset;
    private int slot;

    public ButtonContainer(int offset) 
    {
        this.offset = offset;
    }

    public override void EnterScene(Scene scene, ContentManager content)
    {
        base.EnterScene(scene, content);
        foreach (var button in buttons) 
        {
            button.Position = Position;
            button.Tags = Tags;
            button.PauseMode = PauseMode;
            scene.Add(button, PauseMode);
        }
    }

    public override void Ready()
    {
        var dynamicOffset = 0f;   
        var length = buttons.Count;
        for (int i = 0; i < length; i++) 
        {
            var button = buttons[i];
            button.Position = new Vector2(button.Position.X, button.Position.Y + dynamicOffset);
            dynamicOffset += offset;
            slot = (i + 1) % length;
            if (slot != 0) 
            {
                button.DownFocus = buttons[slot];
            }
            slot = i - 1;
            if (slot == -1) 
            {
                button.Selected = true;
                continue;
            }
            button.UpFocus = buttons[slot];
        }
        base.Ready();
    }

    public override void Update()
    {
        foreach (var button in buttons) 
        {
            button.Position = new Vector2(Position.X, button.Position.Y);
        }
        base.Update();
    }

    public void AddItem(KeyboardButton button) 
    {
        buttons.Add(button);
    }

    public void RemoveItem(KeyboardButton button) 
    {
        buttons.Remove(button);
    }
}