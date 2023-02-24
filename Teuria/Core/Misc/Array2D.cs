using System;
using System.Collections;
using System.Collections.Generic;

namespace Teuria;

public sealed class Array2D<T> 
{
    private T[] array;
    private int numRows;
    private int numColumns;

    public int Rows => numRows;
    public int Columns => numColumns;

    public T this[int x, int y] 
    {
        get 
        {
            var index = GetIndex(x, y);
            if (index < 0) 
                throw new IndiciesOutOfBoundsException(x, y);
            return array[GetIndex(x, y)];
        }
        set 
        {
            var index = GetIndex(x, y);
            if (index < 0) 
                throw new IndiciesOutOfBoundsException(x, y);
            array[index] = value;
        }
    }

    public Array2D(int numRows, int numColumns)  
    {
        this.numRows = numRows;
        this.numColumns = numColumns;
        array = new T[numRows * numColumns];
    }

    public static Array2D<T> FromArray(int numRows, int numColumns, T[] grid) 
    {
        var array = new Array2D<T>(numRows, numColumns);
        array.array = grid;
        return array;
    }

    public static Array2D<T> FromArray2D(int numRows, int numColumns, T[,] grid2D) 
    {
        var array = new Array2D<T>(numRows, numColumns);
        for (int x = 0; x < numColumns; x++) 
            for (int y = 0; y < numRows; y++) 
                array[x, y] = grid2D[x, y];

        return array;
    }

    private int GetIndex(int row, int column) 
    {
        if (row < this.numRows && column < this.numColumns) 
        {
            return row * this.numColumns + column;
        }
        return -1;
    }

    public T[] ToArray() 
    {
        return array;
    }
}

public ref struct StackArray2D<T> 
{
    private Span<T> array;
    private int numRows;
    private int numColumns;

    public int Rows => numRows;
    public int Columns => numColumns;

    public T this[int x, int y] 
    {
        get 
        {
            var index = GetIndex(x, y);
            if (index < 0) 
                throw new IndiciesOutOfBoundsException(x, y);
            return array[GetIndex(x, y)];
        }
        set 
        {
            var index = GetIndex(x, y);
            if (index < 0) 
                throw new IndiciesOutOfBoundsException(x, y);
            array[index] = value;
        }
    }

    public StackArray2D(int numRows, int numColumns)  
    {
        this.numRows = numRows;
        this.numColumns = numColumns;
        array = new T[numRows * numColumns];
    }

    public static StackArray2D<T> FromArray(int numRows, int numColumns, T[] grid) 
    {
        var array = new StackArray2D<T>(numRows, numColumns);
        array.array = grid;
        return array;
    }

    public static StackArray2D<T> FromArray2D(int numRows, int numColumns, T[,] grid2D) 
    {
        var array = new StackArray2D<T>(numRows, numColumns);
        for (int x = 0; x < numColumns; x++) 
            for (int y = 0; y < numRows; y++) 
                array[x, y] = grid2D[x, y];

        return array;
    }

    private int GetIndex(int row, int column) 
    {
        if (row < this.numRows && column < this.numColumns) 
        {
            return row * this.numColumns + column;
        }
        return -1;
    }

    public T[] ToArray() 
    {
        return array.ToArray();
    }

    public Span<T> ToSpanArray() 
    {
        return array;
    }
}

[Serializable]
public class IndiciesOutOfBoundsException : Exception 
{
    public IndiciesOutOfBoundsException(int indexX, int indexY) 
        : base($"Indicies Out of bounds Exception X: {indexX} Y: {indexY}") {}
}