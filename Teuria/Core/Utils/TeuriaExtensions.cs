using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Teuria;

public static class TeuriaExtension 
{
    public static void PlayImmediate(this SoundEffectInstance soundEffectInstance) 
    {
        soundEffectInstance.Stop(true);
        soundEffectInstance.Play();
    }

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

    public static int[,] ToIntArray(this string[,] arr) 
    {
        var first = arr.GetLength(1);
        var second = arr.GetLength(0);
        int[,] array2D = new int[second, first];

        for (int y = 0; y < first; y++) 
        {
            for (int x = 0; x < second; x++) 
            {
                array2D[x, y] = int.Parse(arr[x, y]);
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