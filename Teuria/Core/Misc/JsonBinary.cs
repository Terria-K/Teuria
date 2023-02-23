using System;
using LightJson;
using LightJson.Serialization;

namespace Teuria;

[Obsolete("ContentManager is not recommended on this engine. So the Json Importer here will be remove soon.")]
public class JsonBinary 
{
    public byte[]? JsonBytes { get; set; }


    public JsonValue ToJson() 
    {
        var binary = JsonBinaryReader.Parse(JsonBytes);
        return binary;
    }
}