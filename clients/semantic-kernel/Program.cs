using System.Linq;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.Client;
using ModelContextProtocol.Client.Transports;

Console.WriteLine("Starting Semantic Kernel + MCP demo...");

var projectPath = Path.GetFullPath(Path.Combine("..", "..", "src", "McpDemo", "McpDemo.csproj"));

var client = new McpClientBuilder()
    .WithClientInfo("semantic-kernel-sample", "1.0.0")
    .WithStdioTransport(new StdioClientTransportOptions
    {
        Command = "dotnet",
        Arguments = new[]
        {
            "run",
            "--project",
            projectPath
        }
    })
    .Build();

await client.InitializeAsync();

var tools = await client.ListToolsAsync();
Console.WriteLine($"Discovered {tools.Tools.Count} tools from MCP server.");

var echoResult = await client.CallToolAsync("echo", new
{
    message = "Semantic Kernel calling MCP tool!"
});
Console.WriteLine($"echo => {echoResult.FirstOrDefault()?.Text}");

var timeResult = await client.CallToolAsync("utcNow");
Console.WriteLine($"utcNow => {timeResult.FirstOrDefault()?.Text}");

var builder = Kernel.CreateBuilder();
var kernel = builder.Build();

var history = new ChatHistory();
history.AddSystemMessage("You are a helpful assistant that can call MCP tools when appropriate.");
history.AddUserMessage("請幫我呼叫 dotnet MCP 範例伺服器的 echo 工具，輸入 hello world。");

var assistantMessage = await kernel.ChatCompletionService.GetChatMessageContentAsync(history);
Console.WriteLine($"Assistant suggestion: {assistantMessage.Content}");

await client.DisposeAsync();
