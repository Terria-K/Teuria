using System.IO;
using TeuJson;
using Microsoft.Xna.Framework;

namespace Teuria;

public static class JsonLoader 
{
    public static JsonValue Load(string path)
    {
        var fullPath = path + ".bin";
        if (File.Exists(fullPath))
        {
            return LoadBin(fullPath);
        }
        fullPath = path + ".json";
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found", path);
        }
        return LoadText(fullPath);
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