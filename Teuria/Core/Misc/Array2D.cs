using System.Collections;
using System.Collections.Generic;

namespace Teuria;

public sealed class Array2D<T> : IEnumerable<T>
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
            if (index < 0) { return default; }
            return array[GetIndex(x, y)];
        }
        set 
        {
            var index = GetIndex(x, y);
            array[index] = value;
        }
    }

    public Array2D(int numRows, int numColumns)  
    {
        this.numRows = numRows;
        this.numColumns = numColumns;
        array = new T[numRows * numColumns];
    }

    public static Array2D<T> FromArrays(int numRows, int numColumns, T[,] grid2D) 
    {
        var array = new Array2D<T>(numRows, numColumns);
        for (int x = 0; x < numColumns; x++) 
            for (int y = 0; y < numRows; y++) 
                array[x, y] = grid2D[x, y];

        return array;
    }

    private int GetIndex(int row, int column) 
    {
        if (row < this.numColumns && column < this.numRows) 
        {
            return row * this.numRows + column;
        }
        return -1;
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }
}