using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Teuria;

// https://github.com/prime31/Nez/blob/master/Nez.Portable/Utils/Collections/FastList.cs
public class WeakList<T> 
{
    private T[] buffer;
    public int Count;

    public T this[int idx] => buffer[idx];

    public WeakList(int size) 
    {
        buffer = new T[size];
    }

    public WeakList() : this(5) {}

    public void Add(T item) 
    {
        if (Count == buffer.Length)
            Array.Resize(ref buffer, Math.Max(buffer.Length << 1, 10));
        buffer[Count++] = item;
    }

    public void Remove(T item) 
    {
        var comp = EqualityComparer<T>.Default;
        for (int i = 0; i < Count; i++) 
        {
            if (comp.Equals(buffer[i], item)) 
            {
                RemoveAt(i);
                return;
            }
        }
    }

    public void RemoveAt(int index) 
    {
        Debug.Assert(index < Count, $"Index {index} out of range!");
        Count--;
        if (index < Count)
            Array.Copy(buffer, index + 1, buffer, index, Count - index);
        buffer[Count] = default;
    }

    public bool Contains(T item) 
    {
        var comp = EqualityComparer<T>.Default;
        for (int i = 0; i < Count; ++i) 
        {
            if (comp.Equals(buffer[i], item))
                return true;
        }
        return false;
    }

    public void EnsureCapacity(int addition = 1) 
    {
        if (Count + addition >= buffer.Length)
            Array.Resize(ref buffer, Math.Max(buffer.Length << 1, Count + addition));
    }

    public void AddRange(IEnumerable<T> array) 
    {
        foreach (var item in array)
            Add(item);
    }

    public void Sort(IComparer comparer) 
    {
        Array.Sort(buffer, 0, Count, comparer);
    }

    public void Sort(IComparer<T> comparer) 
    {
        Array.Sort<T>(buffer, 0, Count, comparer);
    }

    public void Sort(Comparison<T> comparison) 
    {
        if (Count > 0) 
        {
            var comparer = new WeakComparer<T>(comparison);
            Array.Sort<T>(buffer, 0, Count, comparer);
        }

    }

    public void Clear() 
    {
        Array.Clear(buffer, 0, Count);
        Count = 0;
    }
}

internal struct WeakComparer<T> : IComparer<T>
{
    private Comparison<T> comparison;

    public WeakComparer(Comparison<T> comparison) 
    {
        this.comparison = comparison;
    }

    public int Compare(T x, T y)
    {
        return comparison(x, y);
    }
}