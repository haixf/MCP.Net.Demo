using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace McpDemo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.AddConsole(options =>
        {
            options.LogToStandardErrorThreshold = LogLevel.Information;
        });

        builder.Services
            .AddMcpServer()
            .WithToolsFromAssembly(typeof(Program).Assembly)
            .WithStdioServerTransport();

        await builder.Build().RunAsync();
    }
}
