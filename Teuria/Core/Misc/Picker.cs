using System;
using System.Collections.Generic;

namespace Teuria;

public class Picker<T>
{
    private List<Option> options = new List<Option>();

    public float EvaluatedWeight { get; private set; }
    public bool CanPick => EvaluatedWeight > 0;
    public int Count => options.Count;

    public Picker() {}

    public Picker(T firstOption, float weight) 
    {
        AddOption(firstOption, weight);
    }

    public void AddOption(T option, float weight) 
    {
        var w = weight;
        w = Math.Max(weight, 0);
        EvaluatedWeight += weight;
        options.Add(new Option(option, w));
    }

    public void AddOption(Span<T> options, float weight) 
    {
        foreach (var option in options) 
        {
            AddOption(option, weight);
        }
    }

    public void AddOption(ReadOnlySpan<T> options, float weight) 
    {
        foreach (var option in options) 
        {
            AddOption(option, weight);
        }
    }

    public void AddOption(List<T> options, float weight) 
    {
        foreach (var option in options) 
        {
            AddOption(option, weight);
        }
    }

    public void AddOption(T[] options, float weight) 
    {
        foreach (var option in options) 
        {
            AddOption(option, weight);
        }
    }

    public T ForcePick() 
    {
        if (options.Count == 1)
            return options[0].Value;

        var w = 0f;
        var roll = MathUtils.Randomizer.NextDouble() * EvaluatedWeight;
        var optionCount = options.Count - 1;

        for (int i = 0; i < optionCount; i++)
        {
            var option = options[i];
            w += option.Weight;
            if (roll < w)
                return option.Value;
        }
        return options[optionCount].Value;
    }

    public T? Pick()
    {
        if (options.Count == 1)
            return options[0].Value;
        if (!CanPick)
            return default;
        
        var w = 0f;
        var roll = MathUtils.Randomizer.NextDouble() * EvaluatedWeight;
        var optionCount = options.Count - 1;

        for (int i = 0; i < optionCount; i++)
        {
            var option = options[i];
            w += option.Weight;
            if (roll < w)
                return option.Value;
        }
        return options[optionCount].Value;
    }

    public void Clear() 
    {
        EvaluatedWeight = 0;
        options.Clear();
    }

    private class Option 
    {
        public T Value;
        public float Weight;

        public Option(T value, float weight) 
        {
            Value = value;
            Weight = weight;
        }
    }
}