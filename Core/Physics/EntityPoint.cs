namespace Teuria;

public struct EntityPoint 
{
    public int X 
    { 
        get => x;
    }
    public int Y 
    {
        get => y;
    }
    public int x;
    public int y;
    public Actor Actor;

    public EntityPoint(int x, int y, Actor actor)
    {
        this.x = x;
        this.y = y;
        this.Actor = actor;
    }
}