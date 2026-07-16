using System.Xml.Linq;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace MyService.Api.DocumentProcessors;

public class TagDescriptionProcessor : IDocumentProcessor
{
    private const string ControllerNs = "MyService.Api.Controllers";

    public void Process(DocumentProcessorContext context)
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, "MyService.Api.xml");
        if (!File.Exists(xmlPath)) return;

        var xmlDoc = XDocument.Load(xmlPath);

        var tagNames = context.Document.Paths
            .SelectMany(p => p.Value.Values)
            .SelectMany(o => o.Tags ?? [])
            .Distinct()
            .ToList();

        context.Document.Tags ??= [];

        foreach (var name in tagNames)
        {
            if (context.Document.Tags.Any(t => t.Name == name)) continue;

            var memberName = $"T:{ControllerNs}.{name}Controller";
            var summary = xmlDoc.Descendants("member")
                .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)
                ?.Element("summary")?.Value.Trim();

            if (!string.IsNullOrEmpty(summary))
            {
                context.Document.Tags.Add(new OpenApiTag { Name = name, Description = summary });
            }
        }
    }
}
