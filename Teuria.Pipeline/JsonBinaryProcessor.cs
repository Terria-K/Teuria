using LightJson;
using LightJson.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Teuria.Pipeline;

[ContentProcessor(DisplayName = "Json Binary - Teuria")]
class JsonBinaryProcessor : ContentProcessor<string, JsonBinary>
{
    public override JsonBinary Process(string input, ContentProcessorContext context)
    {
        var jsonFile = JsonTextReader.Parse(input);
        var binWriter = JsonBinaryWriter.Serialize(jsonFile);
        var jsonBinary = new JsonBinary {
            JsonBytes = binWriter
        };

        return jsonBinary;
    }
}

[ContentSerializerRuntimeType("Teuria.JsonBinary")]
public class JsonBinary 
{
    public byte[] JsonBytes { get; set; }
}