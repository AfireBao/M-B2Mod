# 创意工坊上传配置说明

官方程序：`TaleWorlds.MountAndBlade.SteamWorkshop.exe`  
参数：一个 **XML 配置文件路径**（不是 `steam_workshop_uploader.txt`）。

| 文件 | 作用 |
|------|------|
| `UploadNew.xml` | 首次创建并上传 |
| `UpdateExisting.xml` | 更新已有物品（需填 ItemId） |
| `upload.bat new` / `upload.bat update` | 快捷调用 |
| `steam_workshop_uploader.txt` | **运行后**在 exe 同目录生成的**日志**，可忽略或用来排错 |

## 步骤

1. Steam 登录，开着客户端  
2. 模组目录与 `Preview.png` 已就绪  
3. 首次：`upload.bat new`（或按 XML 头注释手跑命令）  
4. 控制台出现 Item ID 后写入 `UpdateExisting.xml`  
5. 工坊页设置 Required Items：TOR_Core、Harmony；Visibility 可改为 Public  

## Visibility

`Private` / `FriendsOnly` / `Public`
