# TOR Companion Assimilation Skip

独立补丁：阻止 TOR `AssimilationCampaignBehavior.SwapTroopsIfNeeded` 对**玩家招募的流浪者同伴**做文化换兵。

## 做什么

战后分赃 / 招兵 / 驻军日 tick 时，TOR 会把「首领文化 ≠ 士兵文化」的部队按氏族模板同档随机替换。主角已豁免，但同伴没有——异族流浪者带着你的矮人兵时会被洗成同级兄弟线（如雷鸣↔弩手）。

本模组仅当 `Hero.IsPlayerCompanion == true` 时跳过换兵；AI 领主同化不受影响。

## 依赖

- Bannerlord.Harmony
- TOR_Core

## 构建

```bash
dotnet build -c Release
```

`Bannerlord.BuildResources` 会把模块输出到游戏 `Modules` 目录（见 csproj 中 `GameFolder`）。
