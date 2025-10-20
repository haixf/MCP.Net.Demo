using System.ComponentModel;
using ModelContextProtocol.Server;
using ModelContextProtocol.Server.Attributes;

namespace McpDemo.Tools;

[McpServerToolType]
public static class TimeTool
{
    [McpServerTool]
    [Description("Returns the current UTC timestamp.")]
    public static string UtcNow()
    {
        return DateTimeOffset.UtcNow.ToString("O");
    }
}
