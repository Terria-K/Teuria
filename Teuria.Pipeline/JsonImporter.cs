using System.IO;
using LightJson;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Teuria.Pipeline;

[ContentImporter(".json", DisplayName = "Json Importer - Teuria", DefaultProcessor = nameof(JsonBinaryProcessor))]
public class JsonImporter : ContentImporter<string>
{
    public override string Import(string filename, ContentImporterContext context)
    {
        using var json = File.OpenText(filename);
        var content = json.ReadToEnd();
        return content;
    }
}
