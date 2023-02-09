using System.Collections.Generic;

namespace Teuria;

public class Canvases
{
    public List<CanvasLayer> CanvasList = new List<CanvasLayer>();
    private List<CanvasLayer> adding = new List<CanvasLayer>();
    private List<CanvasLayer> removing = new List<CanvasLayer>();
    private Scene scene;

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
            layer.PreDraw(scene);
        }
    }

    internal void Draw() 
    {
        foreach (var layer in CanvasList) 
        {
            if (!layer.Visible) { continue; }
            layer.Draw(scene);
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