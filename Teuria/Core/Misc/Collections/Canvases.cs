using System.Collections.Generic;

namespace Teuria;

public sealed class Canvases
{
    public readonly List<CanvasLayer> CanvasList = new();
    private readonly List<CanvasLayer> adding = new();
    private readonly List<CanvasLayer> removing = new();
    private readonly Scene scene;

    internal Canvases(Scene scene) 
    {
        this.scene = scene;
    }

    internal void UpdateLists() 
    {
        if (adding.Count > 0) 
        {
            foreach (var layer in adding) 
            {
                CanvasList.Add(layer);
            }
        }
        adding.Clear();
        if (removing.Count > 0) 
        {
            foreach (var layer in removing) 
            {
                CanvasList.Remove(layer);
                layer.Unload();
            }
        }
        removing.Clear();
    }

    internal void PreDraw() 
    {
        foreach (var layer in CanvasList) 
        {
            if (!layer.Visible) { continue; }
            layer.PreDraw(scene, Canvas.SpriteBatch);
        }
    }

    internal void Draw() 
    {
        foreach (var layer in CanvasList) 
        {
            if (!layer.Visible) { continue; }
            layer.Draw(scene, Canvas.SpriteBatch);
        }
    }

    internal void PostDraw() 
    {
        foreach (var layer in CanvasList) 
        {
            if (!layer.Visible) { continue; }
            layer.PostDraw(scene);
        }
    }

    internal void Unload() 
    {
        foreach (var layer in CanvasList) 
        {
            layer.Unload();
        }
    }

    public void Add(CanvasLayer layer) 
    {
        adding.Add(layer);
    }

    public void Remove(CanvasLayer layer) 
    {
        removing.Add(layer);
    }
}