# RepoOPS — 仓库脚本任务执行器

RepoOPS 是一个轻量级的命令行工具，用于定义和执行仓库级别的运维脚本任务。通过 JSON 配置文件管理任务定义，支持任务依赖、超时控制、环境变量注入等功能。

## 项目结构

```
RepoOPS/
├── RepoOPS.csproj          # 项目文件
├── Program.cs              # CLI 入口
├── Models/
│   ├── TaskDefinition.cs   # 任务定义模型
│   ├── TaskResult.cs       # 执行结果模型
│   └── TaskConfig.cs       # 配置文件根对象
├── Services/
│   ├── ITaskRunner.cs      # 任务执行器接口
│   └── TaskRunner.cs       # 进程级任务执行器
├── Helpers/
│   └── ConfigLoader.cs     # JSON 配置加载与验证
├── tasks.json              # 示例任务配置
└── README.md
```

## 环境要求

- .NET 10.0 SDK 或更高版本

## 编译

```bash
# 调试构建
dotnet build

# 发布为 Windows 单文件 EXE
dotnet publish -c Release -r win-x64 --self-contained

# 发布为 Linux 单文件
dotnet publish -c Release -r linux-x64 --self-contained

# 发布为 macOS ARM 单文件
dotnet publish -c Release -r osx-arm64 --self-contained
```

发布产物位于 `bin/Release/net10.0/{rid}/publish/` 目录。

## 配置文件

任务通过 JSON 配置文件（默认 `tasks.json`）定义。格式如下：

```json
{
  "description": "项目运维任务",
  "tasks": [
    {
      "name": "clean",
      "description": "清理构建产物",
      "command": "dotnet",
      "arguments": "clean",
      "useShell": false,
      "timeoutSeconds": 60
    },
    {
      "name": "build",
      "description": "构建项目",
      "command": "dotnet",
      "arguments": "build --configuration Release",
      "dependsOn": ["clean"],
      "timeoutSeconds": 120
    },
    {
      "name": "test",
      "description": "运行测试",
      "command": "dotnet",
      "arguments": "test --no-build --configuration Release",
      "dependsOn": ["build"],
      "timeoutSeconds": 300
    }
  ]
}
```

### 任务定义字段

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | string | ✔ | 任务唯一名称 |
| `description` | string | | 任务描述 |
| `command` | string | ✔ | 要执行的命令或可执行文件 |
| `arguments` | string | | 命令参数 |
| `workingDirectory` | string | | 工作目录（相对于配置文件或绝对路径） |
| `timeoutSeconds` | int | | 超时时间（秒），0 = 无超时 |
| `environmentVariables` | object | | 附加环境变量 |
| `dependsOn` | string[] | | 前置依赖任务名称列表 |
| `useShell` | bool | | 是否通过 shell 执行（Windows: cmd /c, Linux: sh -c） |

## 使用方式

```bash
# 生成示例配置文件
repoops init

# 验证配置文件
repoops validate
repoops validate --config path/to/tasks.json

# 列出所有任务
repoops list

# 运行指定任务（自动运行其依赖）
repoops run build

# 运行指定任务并显示详细输出
repoops run build --verbose

# 运行所有任务
repoops run-all

# 使用自定义配置文件
repoops run-all --config ci-tasks.json
```

## 发布为 Windows EXE

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

生成的 `repoops.exe` 位于 `bin/Release/net10.0/win-x64/publish/` 目录，可直接在 Windows 上运行，无需安装 .NET Runtime。

## 错误处理

- 配置文件缺失或格式无效时给出明确提示
- 任务名称重复、依赖缺失等配置错误在执行前检查
- 进程超时自动终止并报告
- 依赖循环或无法满足的依赖链给出诊断信息
- 非零退出码标记为任务失败
