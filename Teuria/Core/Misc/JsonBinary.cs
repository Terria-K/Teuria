using LightJson;
using LightJson.Serialization;

namespace Teuria;

public class JsonBinary 
{
    public byte[] JsonBytes { get; set; }


    public JsonValue ToJson() 
    {
        var binary = JsonBinaryReader.Parse(JsonBytes);
        return binary;
    }
}