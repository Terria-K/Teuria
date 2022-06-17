using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Label : Entity 
{
    protected FontText fontText;
    public string Text { get => fontText.Text; set => fontText.Text = value; }

    public Label(FontText fontText)
    {
        this.fontText = fontText;
        AddComponent(fontText);
    }
}