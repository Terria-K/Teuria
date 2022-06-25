using System.Collections;

namespace Teuria;

public readonly struct RefCoroutine 
{
    public IEnumerator Corou { get; init; }
    public Coroutine Handler { get; init; }

    public bool IsRunning => Corou != null && Handler.IsYielding(Corou);

    public RefCoroutine(Coroutine handler, IEnumerator corou) 
    {
        Handler = handler;
        Corou = corou;
    }
}