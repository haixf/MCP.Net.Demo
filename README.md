# MCP.Net.Demo

此範例專案示範如何使用 .NET 8 建立一個最小可用的 Model Context Protocol (MCP) 伺服器，並提供連接 Claude Desktop、GitHub Copilot 以及 Semantic Kernel 的設定說明。

## 目錄

- [`src/McpDemo`](src/McpDemo): MCP 伺服器主程式與示例工具。
- [`clients/claude`](clients/claude): Claude Desktop 設定範本。
- [`clients/copilot`](clients/copilot): VS Code Copilot Agent Mode 設定範本。
- [`clients/semantic-kernel`](clients/semantic-kernel): 使用 Semantic Kernel 透過 MCP 呼叫工具的範例程式碼。
- [`docs/PROJECT.md`](docs/PROJECT.md): 專案架構與開發重點說明。
- [`docs/SOLUTION.md`](docs/SOLUTION.md): 常見問題與對應解決方法。

## MCP 伺服器快速上手

1. 安裝 .NET 8 SDK。
2. 還原套件並執行：

   ```bash
   dotnet restore src/McpDemo/McpDemo.csproj
   dotnet run --project src/McpDemo/McpDemo.csproj
   ```

   伺服器會透過 **stdio** 暴露工具，日誌輸出會寫入 `stderr`，避免干擾 JSON-RPC 傳輸。

3. 伺服器啟動後可透過任何支援 MCP 的主機進行 `ListTools`/`CallTool` 呼叫。

### 範例工具

- `EchoTool.Echo(message)`：將輸入訊息回傳。
- `TimeTool.UtcNow()`：回傳目前 UTC 時間。
- `TodoTool.AddTodo(title, description?, dueDate?)`：新增一筆儲存在記憶體中的待辦事項。
- `TodoTool.ListTodos(includeCompleted?)`：列出待辦清單，可選擇是否包含已完成項目。
- `TodoTool.CompleteTodo(id)` / `TodoTool.ClearCompleted()`：標記完成或清除已完成的待辦事項。

`TodoTool` 會在伺服器執行期間維持一份簡易的待辦清單，適合用於示範如何透過 MCP 呼叫需要維護狀態的工具。

## Claude Desktop 設定

在 `~/Library/Application Support/Claude/claude_desktop_config.json` (macOS) 或對應平台設定檔加入以下片段：

```json
{
  "mcpServers": {
    "mcp-dotnet-demo": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/MCP.Net.Demo/src/McpDemo/McpDemo.csproj"
      ],
      "env": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
  }
}
```

重啟 Claude Desktop，於「開發者工具」畫面即可看到 `mcp-dotnet-demo` 伺服器並呼叫 `echo`、`utcNow` 等工具。

## GitHub Copilot (VS Code Agent Mode)

在 VS Code 的設定 (`settings.json`) 中加入：

```json
"github.copilot.mcp.servers": {
  "mcp-dotnet-demo": {
    "command": "dotnet",
    "args": [
      "run",
      "--project",
      "/path/to/MCP.Net.Demo/src/McpDemo/McpDemo.csproj"
    ]
  }
}
```

儲存後重新啟動 VS Code，切換至 Copilot Agent 面板即可選擇此 MCP 伺服器。

## Semantic Kernel 範例

[`clients/semantic-kernel/Program.cs`](clients/semantic-kernel/Program.cs) 示範如何在 Semantic Kernel 中以 `StdioMcpTransport` 啟動此伺服器並呼叫工具。執行方式：

```bash
dotnet restore clients/semantic-kernel/SemanticKernelSample.csproj
dotnet run --project clients/semantic-kernel/SemanticKernelSample.csproj
```

輸出會顯示 `echo` 與 `utcNow` 工具的呼叫結果，說明如何將 MCP 伺服器整合進自訂 Agent Flow 中。

## 擴充到 HTTP + SSE

若需部署到遠端環境，可改用 HTTP + Server-Sent Events 傳輸層，並加上 OAuth/Bearer 驗證與反向代理。C# SDK 提供 `WithHttpServerTransport` 擴充方法，可於 ASP.NET Core 專案中設定：

```csharp
builder.Services
    .AddMcpServer()
    .WithHttpServerTransport(options =>
    {
        options.MapRoute = "/mcp";
        options.AllowedOrigins = ["https://your-host" ];
    })
    .WithToolsFromAssembly(typeof(Program).Assembly);
```

## 安全建議

- 採白名單控管允許的 MCP 伺服器與版本。
- 全程使用最小權限與細粒度的 API 金鑰管理。
- 於 Host 端啟用人工核准與審計紀錄。
- 設定網路邊界與 SIEM 監控，避免惡意工具濫用權限。

## 授權

此範例以 MIT 授權釋出，可自由使用與修改。
