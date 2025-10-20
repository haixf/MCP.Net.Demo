using System.ComponentModel;
using ModelContextProtocol.Server;
using ModelContextProtocol.Server.Attributes;

namespace McpDemo.Tools;

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool]
    [Description("Echoes the provided message and demonstrates basic tool wiring.")]
    public static string Echo(string message)
    {
        return $"Hello from the MCP demo! You said: {message}";
    }
}
