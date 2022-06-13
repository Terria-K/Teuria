using System.Collections;

namespace Teuria;

public struct RefCoroutine 
{
    public IEnumerator Corou { get; private set; }
    public Coroutine Handler { get; private set; }

    public bool IsRunning => Corou != null && Handler.IsYielding(Corou);

    public RefCoroutine(Coroutine handler, IEnumerator corou) 
    {
        Handler = handler;
        Corou = corou;
    }
}