using TeuJson;
using Microsoft.Xna.Framework;
using System.IO;
using System;

namespace Teuria;

public static class JsonLoader 
{
    public enum DeserializationMode 
    {
        Json,
        Binary
    }
    public static JsonValue LoadBinDynamic(string path, out DeserializationMode mode) 
    {
        mode = DeserializationMode.Binary;
        if (!File.Exists(path)) 
        {
            path = path.Replace(".bin", ".json");
            mode = DeserializationMode.Json;
        }
        using var container = TitleContainer.OpenStream(path);
        return mode switch 
        {
            DeserializationMode.Binary => JsonBinaryReader.FromStream(container),
            DeserializationMode.Json => JsonTextReader.FromStream(container),
            _ => throw new InvalidOperationException()
        };
    }

    public static JsonValue LoadBinDynamic(string path) 
    {
        return LoadBinDynamic(path, out _);
    }

    public static JsonValue LoadText(string path) 
    {
        using var container = TitleContainer.OpenStream(path);
        return JsonTextReader.FromStream(container);
    }

    public static JsonValue LoadBin(string path) 
    {
        using var container = TitleContainer.OpenStream(path);
        return JsonBinaryReader.FromStream(container);
    }
}