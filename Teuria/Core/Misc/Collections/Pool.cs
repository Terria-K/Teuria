using System.Collections.Generic;

namespace Teuria;

public static class Pool<T> 
where T : IPoolable<T>, new()
{
    private static Queue<T> pooledObjects = new();

    public static void Warm(int count)
    {
        count -= pooledObjects.Count;
        if (count <= 0)
            return;
        
        for (int i = 0; i < count; i++)
        {
            pooledObjects.Enqueue(new T());
        }
    }

    public static void Cold() 
    {
        pooledObjects.Clear();
    }

    public static T Create() 
    {
        if (pooledObjects.Count > 0)
            return pooledObjects.Dequeue();
        
        return new T();
    }

    public static void Destroy(T obj) 
    {
        pooledObjects.Enqueue(obj);
        
        obj.Destroy();
    } 
}

public interface IPoolable<TSelf>
{
    void Destroy();
}