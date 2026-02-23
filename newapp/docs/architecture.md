# PhonoArk Mobile — 架构设计文档

## 1. 项目概述

PhonoArk Mobile 是一款面向 Android 平台的英语音标学习应用，提供音标学习与音标考试两大核心功能。
本项目为完全重写版本，采用 .NET MAUI 技术栈，遵循 Android 原生设计规范，适配手机触控交互。

## 2. 功能需求

### 2.1 音标学习
- 按类别（元音、双元音、辅音）浏览所有国际音标
- 点击音标卡片进入详情页：
  - 显示音标符号（大号居中）
  - 显示音标描述
  - 展示 5 个示例单词
  - 每个单词可点击发音（使用系统 TTS）
  - 每个单词下方显示完整音标标注

### 2.2 音标考试
- 随机出题：给出一个单词的发音，让用户选择正确的音标
- 支持多种题型：
  - 听音选音标（根据单词选出包含的音标）
  - 看音标选单词（根据音标选出对应单词）
- 实时反馈正确/错误
- 考试结束后显示得分与错题回顾

## 3. 技术架构

### 3.1 分层设计

```
┌─────────────────────────────────────┐
│         PhonoArk.Mobile.App         │  ← MAUI Android UI 层
│  (Pages, Shell, Resources, TTS)     │
├─────────────────────────────────────┤
│        PhonoArk.Mobile.Core         │  ← 业务逻辑层（可独立测试）
│  (Models, Services, ViewModels)     │
├─────────────────────────────────────┤
│       PhonoArk.Mobile.Tests         │  ← xUnit 回归测试
│  (Service Tests, ViewModel Tests)   │
└─────────────────────────────────────┘
```

### 3.2 项目职责

| 项目 | 目标框架 | 职责 |
|------|----------|------|
| PhonoArk.Mobile.Core | net10.0 | 模型、服务接口与实现、ViewModel |
| PhonoArk.Mobile.App | net10.0-android | MAUI 页面、导航、平台 TTS |
| PhonoArk.Mobile.Tests | net10.0 | xUnit 单元测试 |

### 3.3 设计原则

- **MVVM**：ViewModel 持有状态与命令，View 通过数据绑定驱动
- **依赖注入**：服务通过接口注册，ViewModel 通过构造函数注入
- **可测试性**：所有业务逻辑在 Core 层，不依赖平台 API
- **单一职责**：每个类只负责一件事

## 4. 模型设计

### 4.1 核心模型

```csharp
Phoneme         // 音标：符号、类型、描述、示例单词列表
ExampleWord     // 示例单词：单词文本、音标标注
PhonemeCategory // 枚举：Vowel, Diphthong, Consonant
ExamQuestion    // 考题：题干、选项列表、正确答案索引、题型
ExamResult      // 考试结果：总题数、正确数、错题列表
```

## 5. 服务设计

| 服务接口 | 实现 | 职责 |
|----------|------|------|
| IPhonemeService | PhonemeService | 提供音标数据、按类别过滤、获取详情 |
| IExamService | ExamService | 生成随机考题、判断答案、计算成绩 |

## 6. ViewModel 设计

| ViewModel | 职责 |
|-----------|------|
| HomeViewModel | 首页导航（学习/考试入口） |
| PhonemeListViewModel | 音标列表，按类别过滤 |
| PhonemeDetailViewModel | 音标详情，展示符号与单词 |
| ExamViewModel | 考试流程，出题/答题/反馈 |
| ExamResultViewModel | 考试结果展示 |

## 7. 页面导航

```
HomePage
  ├── PhonemeListPage (音标学习)
  │     └── PhonemeDetailPage (音标详情)
  └── ExamPage (音标考试)
        └── ExamResultPage (考试结果)
```

## 8. UI 设计要点

- 首页：大按钮卡片式布局，「音标学习」与「音标考试」并列
- 音标列表：三个 Tab（元音/双元音/辅音），网格布局展示音标卡片
- 音标详情：音标符号居中大字显示，下方列表展示 5 个单词卡片
- 考试页：题目居中，4 个选项按钮纵向排列，底部进度条
- 结果页：分数环形图 + 错题列表

## 9. 测试策略

- **PhonemeService 测试**：验证数据完整性、过滤逻辑
- **ExamService 测试**：验证出题逻辑、判题逻辑、成绩计算
- **ViewModel 测试**：验证状态变化、命令执行、导航触发
