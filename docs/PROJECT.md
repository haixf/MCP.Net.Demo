# MCP.Net.Demo 專案說明

本文件說明 MCP.Net.Demo 專案的目的、架構與主要元件，協助開發者快速了解如何擴充或整合此範例。

## 專案目標

- 透過 .NET 8 實作最小可用的 Model Context Protocol (MCP) 伺服器。
- 提供多種示例工具，展示如何在 MCP 主機中呼叫與維護狀態。
- 協助開發者串接常見的 MCP Client（Claude Desktop、GitHub Copilot、Semantic Kernel）。

## 專案結構

```
MCP.Net.Demo/
├── src/McpDemo/              # MCP 伺服器主程式與工具實作
│   ├── Program.cs            # 設定伺服器、註冊工具的進入點
│   └── Tools/                # 內建的示例工具
│       ├── EchoTool.cs       # 回聲工具，示範最基本的輸入/輸出
│       ├── TimeTool.cs       # 時間工具，提供取得 UTC 時間的 API
│       └── TodoTool.cs       # 待辦工具，示範狀態維護與多個操作
├── clients/                  # 各式客戶端設定與示例
│   └── semantic-kernel/      # 使用 Semantic Kernel 呼叫 MCP 的範例程式
└── README.md                 # 快速使用教學
```

## 技術重點

- **McpServer**：使用官方 `ModelContextProtocol.Server` 套件註冊工具，透過 `stdio` 提供 JSON-RPC 服務。
- **工具註冊**：透過 `[McpServerToolType]` 與 `[McpServerTool]` 屬性宣告工具類別與方法。
- **狀態管理**：`TodoTool` 利用靜態集合與 lock 展示如何在單一行程中儲存記憶體狀態。
- **本地開發體驗**：程式執行後於 `stderr` 輸出日誌，避免干擾 `stdout` 的 JSON-RPC 訊息。

## 建置與執行

```bash
dotnet restore src/McpDemo/McpDemo.csproj
dotnet run --project src/McpDemo/McpDemo.csproj
```

伺服器啟動後會等待客戶端連線，您可以使用下列其中一種方式驗證：

- 透過 Claude Desktop 開啟 MCP 主機，呼叫 `echo` 或 `todo` 相關工具。
- 在 VS Code Copilot Agent Mode 設定中加入範例設定。
- 執行 `clients/semantic-kernel` 範例程式觀察呼叫流程。

## 延伸開發建議

- 將工具拆分成獨立類別，並引入依賴注入以管理更複雜的狀態。
- 加入持久化儲存層（例如 SQLite、Redis），提供跨行程的資料維持能力。
- 以 ASP.NET Core + HTTP/SSE 架構部署遠端 MCP 伺服器，搭配身份驗證與反向代理。
- 建立自動化測試確保工具輸入輸出格式穩定。

