# 常見問題與解決方法

本文件整理在開發或操作 MCP.Net.Demo 專案時可能遇到的問題與對應的解決方式。

## dotnet restore 失敗

**症狀**：執行 `dotnet restore` 時出現無法解析套件來源或 SSL 相關錯誤。

**解決方法**：

1. 確認本機已安裝 .NET 8 SDK，並使用 `dotnet --info` 檢查版本。
2. 若為企業環境，需設定 NuGet Proxy 或加入信任的憑證。
3. 檢查 `NuGet.Config` 是否存在自訂來源，必要時暫時移除。

## 程式啟動後立即結束

**症狀**：執行 `dotnet run --project src/McpDemo/McpDemo.csproj` 後主控台立即返回，無法接收請求。

**解決方法**：

- 確保未使用 VS Code 等工具的「自動終止」功能，可改以終端機直接啟動專案。
- 確認未透過排程或背景服務啟動，MCP 伺服器需持續開啟以接受 JSON-RPC 請求。

## 客戶端無法連線

**症狀**：Claude Desktop 或 Copilot 無法列出 `mcp-dotnet-demo` 伺服器。

**解決方法**：

1. 檢查設定檔中的 `command` 與 `args` 是否指向正確的 `.csproj` 路徑。
2. 確認 `dotnet` 已加入系統 PATH；在 macOS/Linux 可透過 `which dotnet` 驗證。
3. 若使用 Windows，請確保指令行使用 `dotnet.exe` 並允許執行政策。

## TodoTool 狀態未保存

**症狀**：呼叫 `TodoTool.AddTodo` 後再次列出清單沒有資料。

**解決方法**：

- 確認伺服器行程未重新啟動；目前範例僅在記憶體維護狀態，重啟後資料會重置。
- 需要跨行程保存時，請參考 `docs/PROJECT.md` 的延伸建議改用持久化儲存。

## Semantic Kernel 範例執行錯誤

**症狀**：`clients/semantic-kernel` 範例無法啟動或出現傳輸錯誤。

**解決方法**：

1. 確保已還原範例專案套件：`dotnet restore clients/semantic-kernel/SemanticKernelSample.csproj`。
2. 先啟動 MCP 伺服器，再執行範例程式，避免因連線逾時導致失敗。
3. 若系統無法同時開啟多個 stdin/stdout 管道，可改以 `--wait-for-debugger` 參數讓伺服器先啟動再連線。

