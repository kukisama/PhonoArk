# 音律方舟 - 微信小程序架构文档

## 1. 项目概述

音律方舟（PhonoArk）微信小程序版本是一个完全独立设计的移动端 IPA（国际音标）学习应用，专门针对微信小程序平台和手机触控操作进行优化。

### 核心功能
- **音标学习**：按元音、双元音、辅音分类浏览 44 个国际音标
- **音标详情**：点击音标后展示其形状、发音描述，以及 5 个关联示例单词（含发音和音标标注）
- **音标考试**：随机出题，根据发音选择正确音标，实时反馈并显示最终成绩

## 2. 技术架构

### 2.1 技术栈
- **框架**：微信小程序原生框架（WXML + WXSS + JavaScript）
- **发音**：微信小程序内置 `wx.createInnerAudioContext()` 播放在线 TTS 音频
- **数据存储**：`wx.setStorageSync` / `wx.getStorageSync` 用于本地持久化考试记录
- **无需后端**：所有音标数据内置于小程序内，离线可用

### 2.2 目录结构

```
wechat/
├── docs/
│   └── architecture.md          # 本架构文档
├── miniprogram/
│   ├── app.js                   # 小程序入口（全局初始化）
│   ├── app.json                 # 小程序配置（页面路由、窗口样式等）
│   ├── app.wxss                 # 全局样式
│   ├── data/
│   │   └── phonemes.js          # 44 个音标完整数据（含示例单词与音标标注）
│   ├── utils/
│   │   ├── audio.js             # 音频播放工具（TTS 发音）
│   │   └── exam.js              # 考试逻辑（出题、评分、结果统计）
│   ├── pages/
│   │   ├── index/               # 首页（导航入口）
│   │   │   ├── index.js
│   │   │   ├── index.json
│   │   │   ├── index.wxml
│   │   │   └── index.wxss
│   │   ├── learn/               # 音标学习页（分类列表）
│   │   │   ├── learn.js
│   │   │   ├── learn.json
│   │   │   ├── learn.wxml
│   │   │   └── learn.wxss
│   │   ├── detail/              # 音标详情页（形状 + 单词 + 发音）
│   │   │   ├── detail.js
│   │   │   ├── detail.json
│   │   │   ├── detail.wxml
│   │   │   └── detail.wxss
│   │   ├── exam/                # 考试页（答题流程）
│   │   │   ├── exam.js
│   │   │   ├── exam.json
│   │   │   ├── exam.wxml
│   │   │   └── exam.wxss
│   │   └── result/              # 考试结果页
│   │       ├── result.js
│   │       ├── result.json
│   │       ├── result.wxml
│   │       └── result.wxss
│   └── project.config.json      # 微信开发者工具项目配置
├── tests/
│   ├── phonemes.test.js         # 音标数据完整性测试
│   └── exam.test.js             # 考试逻辑测试
└── package.json                 # 测试依赖
```

## 3. 数据模型

### 3.1 音标数据 (`Phoneme`)
```javascript
{
  symbol: "iː",                        // IPA 符号
  type: "vowel",                        // 分类：vowel / diphthong / consonant
  name: "长元音",                       // 中文名称
  description: "长元音，如 see 中的发音", // 发音描述
  examples: [                           // 5 个示例单词
    { word: "see",  ipa: "/siː/" },
    { word: "sea",  ipa: "/siː/" },
    { word: "meet", ipa: "/miːt/" },
    { word: "key",  ipa: "/kiː/" },
    { word: "tree", ipa: "/triː/" }
  ]
}
```

### 3.2 考试题目 (`ExamQuestion`)
```javascript
{
  phoneme: { ... },          // 目标音标
  correctWord: { ... },      // 正确答案单词
  options: [ ... ],          // 4 个选项（含正确答案）
  userAnswer: null,          // 用户选择
  isCorrect: false           // 是否答对
}
```

### 3.3 考试结果 (`ExamResult`)
```javascript
{
  date: "2026-02-19T08:00:00Z",
  totalQuestions: 10,
  correctCount: 8,
  score: 80,
  duration: 120          // 秒
}
```

## 4. 页面设计

### 4.1 首页 (`pages/index`)
- 大标题 "音律方舟"
- 两个主要入口卡片：音标学习、音标考试
- 简洁的卡片式布局，符合手机操作直觉

### 4.2 音标学习页 (`pages/learn`)
- 顶部 Tab 切换：元音 / 双元音 / 辅音
- 网格布局展示音标符号（大字体、易点击）
- 点击音标进入详情页

### 4.3 音标详情页 (`pages/detail`)
- 顶部大字展示音标符号及中英文描述
- 下方列表展示 5 个示例单词
- 每个单词包含：单词文本、音标标注、播放按钮
- 点击播放按钮或单词卡片发音

### 4.4 考试页 (`pages/exam`)
- 显示当前题号 / 总题数
- 展示目标音标符号
- 4 个单词选项按钮
- 选择后即时反馈（正确/错误动画）
- 自动进入下一题

### 4.5 结果页 (`pages/result`)
- 环形图展示得分百分比
- 正确/错误题数统计
- 重新考试 / 返回首页按钮

## 5. 音频方案

使用在线 TTS API 实现单词发音：
- 主要方案：通过 `wx.createInnerAudioContext()` 播放在线 TTS 音频 URL
- 备选方案：使用微信小程序插件或云函数调用 TTS 服务

## 6. 测试策略

- **数据完整性测试**：验证 44 个音标数据的完整性、格式正确性
- **考试逻辑测试**：验证出题随机性、评分准确性、边界条件处理
- 使用 Jest 作为测试框架，可在 Node.js 环境运行
