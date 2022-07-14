using System.Collections.Generic;

namespace Teuria;

public static class TeuriaExtension 
{
    public static T[,] To2D<T>(this List<List<T>> list) 
    {
        var first = list.Count;
        var second = list[0].Count;
        T[,] array2D = new T[first, second];

        for (int y = 0; y < first; y++) 
        {
            for (int x = 0; x < second; x++) 
            {
                array2D[y, x] = list[y][x];
            }
        }

        return array2D;
    }

    public static T[,] To2D<T>(this T[][] arr) 
    {
        var first = arr.Length;
        var second = arr[0].Length;
        T[,] array2D = new T[first, second];

        for (int y = 0; y < first; y++) 
        {
            for (int x = 0; x < second; x++) 
            {
                array2D[y, x] = arr[y][x];
            }
        }

        return array2D;
    }
}