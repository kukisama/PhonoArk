# pureopus 编译说明（arm64 真机）

本文记录 `pureopus` 在 Windows 环境下的最小可用编译流程，目标是 **arm64 真机安装使用**，不包含模拟器测试。

## 1. 前置条件

- 已安装 Android SDK（含 Build-Tools / Platform-Tools）
- 终端可用 `bash`（用于执行仓库内 `gradlew`）
- 当前仓库路径：`c:\Scripts\PhonoArk\PhonoArk\pureopus`

## 2. 编译命令

在 `pureopus` 目录执行：

- `bash ./gradlew :app:assembleDebug -x test`

说明：

- `-x test`：跳过测试，仅做打包编译
- 当前项目可直接产出可安装到 arm64 真机的 Debug APK

## 3. 产物位置

- APK：`app/build/outputs/apk/debug/app-debug.apk`
- 元数据：`app/build/outputs/apk/debug/output-metadata.json`

## 4. arm64 说明

本次编译后的 APK 中包含 `arm64-v8a` 原生库目录（同时也包含其它 ABI）。

- 结论：可直接用于 arm64 真机安装与调试。

## 5. 常见问题

### 5.1 `gradlew.bat` 不存在

该仓库当前仅提供 `gradlew`，在 Windows 下请通过 `bash ./gradlew ...` 执行。

### 5.2 Kotlin 编译报错导致构建失败

优先修复源码编译错误后再重试打包。可先执行一次：

- `bash ./gradlew :app:assembleDebug -x test`

观察首个报错文件与行号，按最小改动修复。