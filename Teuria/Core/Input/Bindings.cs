namespace Teuria;

public interface IBinding 
{
    bool Pressed();
    bool JustPressed();
    bool Released();
}

public interface IAxisBinding
{
    void Update();
    int GetValue();
    void Intercept(int button);
}