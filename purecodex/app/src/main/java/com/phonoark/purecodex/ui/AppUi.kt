package com.phonoark.purecodex.ui

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.BarChart
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material.icons.filled.Favorite
import androidx.compose.material.icons.filled.FavoriteBorder
import androidx.compose.material.icons.filled.History
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material.icons.filled.School
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.FilterChip
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Slider
import androidx.compose.material3.Switch
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.navigation.NavGraph.Companion.findStartDestination
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.phonoark.purecodex.model.AppSettings
import com.phonoark.purecodex.model.Phoneme
import com.phonoark.purecodex.model.PhonemeType

private data class TabItem(
    val route: String,
    val label: String,
    val icon: @Composable () -> Unit,
)

private val tabs = listOf(
    TabItem("ipa", "音标图表", { Icon(Icons.Default.School, contentDescription = null) }),
    TabItem("favorites", "收藏", { Icon(Icons.Default.Favorite, contentDescription = null) }),
    TabItem("exam", "测验", { Icon(Icons.Default.BarChart, contentDescription = null) }),
    TabItem("history", "历史", { Icon(Icons.Default.History, contentDescription = null) }),
    TabItem("settings", "设置", { Icon(Icons.Default.Settings, contentDescription = null) }),
)

@Composable
fun PureCodexApp(
    viewModel: AppViewModel,
    onSpeak: (String) -> Unit,
    isDark: Boolean,
) {
    val navController = rememberNavController()
    val currentRoute = navController.currentBackStackEntryAsState().value?.destination?.route ?: "ipa"

    Scaffold(
        bottomBar = {
            NavigationBar {
                tabs.forEach { tab ->
                    NavigationBarItem(
                        selected = currentRoute == tab.route,
                        onClick = {
                            navController.navigate(tab.route) {
                                popUpTo(navController.graph.findStartDestination().id) {
                                    saveState = true
                                }
                                launchSingleTop = true
                                restoreState = true
                            }
                        },
                        icon = tab.icon,
                        label = { Text(tab.label) },
                    )
                }
            }
        },
    ) { padding ->
        NavHost(
            navController = navController,
            startDestination = "ipa",
            modifier = Modifier.padding(padding),
        ) {
            composable("ipa") { IpaScreen(viewModel, onSpeak, isDark) }
            composable("favorites") { FavoritesScreen(viewModel, onSpeak) }
            composable("exam") { ExamScreen(viewModel, onSpeak) }
            composable("history") { HistoryScreen(viewModel) }
            composable("settings") { SettingsScreen(viewModel) }
        }
    }
}

@Composable
private fun IpaScreen(viewModel: AppViewModel, onSpeak: (String) -> Unit, isDark: Boolean) {
    val types = PhonemeType.entries
    val selectedType by viewModel.selectedType.collectAsState()
    val selected by viewModel.selectedPhoneme.collectAsState()
    val favorites by viewModel.favorites.collectAsState()
    val phonemes = viewModel.phonemesBySelectedType()

    LazyColumn(
        modifier = Modifier.fillMaxSize().padding(12.dp),
        verticalArrangement = Arrangement.spacedBy(10.dp),
    ) {
        item {
            Card(modifier = Modifier.fillMaxWidth()) {
                Column(modifier = Modifier.padding(12.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text("PureCodex 安卓原生模式", fontWeight = FontWeight.Bold)
                    Text("主题：${if (isDark) "深色" else "浅色"}  ·  与 Avalonia 结构对齐")
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        types.forEach { type ->
                            FilterChip(
                                selected = type == selectedType,
                                onClick = { viewModel.switchType(type) },
                                label = { Text(type.title) },
                            )
                        }
                    }
                }
            }
        }

        items(phonemes) { phoneme ->
            PhonemeCard(
                phoneme = phoneme,
                isFavorite = phoneme.symbol in favorites,
                onSelect = { viewModel.selectPhoneme(phoneme) },
                onToggleFavorite = { viewModel.toggleFavorite(phoneme.symbol) },
            )
        }

        selected?.let { current ->
            item {
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(
                        modifier = Modifier.padding(12.dp),
                        verticalArrangement = Arrangement.spacedBy(10.dp),
                    ) {
                        Text("当前音标: /${current.symbol}/", style = MaterialTheme.typography.titleMedium)
                        Text(current.description)
                        OutlinedButton(onClick = { onSpeak(current.symbol) }) {
                            Text("播放音标")
                        }
                        current.exampleWords.forEach { word ->
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.SpaceBetween,
                            ) {
                                Text("${word.word}  ${word.ipaTranscription}")
                                OutlinedButton(onClick = { onSpeak(word.word) }) {
                                    Text("播放")
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

@Composable
private fun PhonemeCard(
    phoneme: Phoneme,
    isFavorite: Boolean,
    onSelect: () -> Unit,
    onToggleFavorite: () -> Unit,
) {
    Card(onClick = onSelect, modifier = Modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier.fillMaxWidth().padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
        ) {
            Column {
                Text("/${phoneme.symbol}/", style = MaterialTheme.typography.titleLarge)
                Text(phoneme.description)
            }
            OutlinedButton(onClick = onToggleFavorite) {
                Icon(
                    if (isFavorite) Icons.Default.Favorite else Icons.Default.FavoriteBorder,
                    contentDescription = null,
                )
            }
        }
    }
}

@Composable
private fun FavoritesScreen(viewModel: AppViewModel, onSpeak: (String) -> Unit) {
    val favorites = viewModel.favoritePhonemes()
    LazyColumn(
        modifier = Modifier.fillMaxSize().padding(12.dp),
        verticalArrangement = Arrangement.spacedBy(10.dp),
    ) {
        item {
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text("收藏音标 (${favorites.size})", style = MaterialTheme.typography.titleLarge)
                OutlinedButton(onClick = viewModel::clearFavorites) {
                    Icon(Icons.Default.Delete, contentDescription = null)
                    Text("清空")
                }
            }
        }

        if (favorites.isEmpty()) {
            item { Text("暂无收藏，请在音标图表中添加。") }
        } else {
            items(favorites) { phoneme ->
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(modifier = Modifier.padding(12.dp), verticalArrangement = Arrangement.spacedBy(6.dp)) {
                        Text("/${phoneme.symbol}/", style = MaterialTheme.typography.titleLarge)
                        Text(phoneme.description)
                        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                            OutlinedButton(onClick = { onSpeak(phoneme.symbol) }) { Text("播放") }
                            OutlinedButton(onClick = { viewModel.toggleFavorite(phoneme.symbol) }) { Text("取消收藏") }
                        }
                    }
                }
            }
        }
    }
}

@Composable
private fun ExamScreen(viewModel: AppViewModel, onSpeak: (String) -> Unit) {
    val questions by viewModel.examQuestions.collectAsState()
    val index by viewModel.examIndex.collectAsState()
    val selectedOption by viewModel.selectedOption.collectAsState()
    val feedback by viewModel.feedback.collectAsState()
    val settings by viewModel.settings.collectAsState()
    val current = viewModel.currentQuestion()

    if (questions.isEmpty() || current == null) {
        StartExamCard(
            settings = settings,
            onSettingsChange = viewModel::updateSettings,
            onStart = viewModel::startExam,
        )
        return
    }

    LazyColumn(
        modifier = Modifier.fillMaxSize().padding(12.dp),
        verticalArrangement = Arrangement.spacedBy(10.dp),
    ) {
        item {
            Text("题目 ${index + 1}/${questions.size}", style = MaterialTheme.typography.titleMedium)
            Text(current.prompt, style = MaterialTheme.typography.titleLarge)
            OutlinedButton(onClick = { onSpeak(current.phonemeSymbol) }) {
                Text("播放音标")
            }
        }

        items(current.options.indices.toList()) { optionIndex ->
            val option = current.options[optionIndex]
            val label = when {
                selectedOption == null -> option
                optionIndex == current.correctIndex -> "✓ $option"
                optionIndex == selectedOption -> "✗ $option"
                else -> option
            }
            Button(
                onClick = { viewModel.submitAnswer(optionIndex) },
                enabled = selectedOption == null,
                modifier = Modifier.fillMaxWidth(),
                contentPadding = PaddingValues(12.dp),
            ) {
                Text(label)
            }
        }

        feedback?.let {
            item { Text(it, fontWeight = FontWeight.Bold) }
        }

        item {
            OutlinedButton(
                onClick = viewModel::nextQuestionOrFinish,
                enabled = selectedOption != null,
                modifier = Modifier.fillMaxWidth(),
            ) {
                Text(if (index == questions.lastIndex) "完成并保存" else "下一题")
            }
        }
    }
}

@Composable
private fun StartExamCard(
    settings: AppSettings,
    onSettingsChange: (AppSettings) -> Unit,
    onStart: () -> Unit,
) {
    Card(modifier = Modifier.fillMaxWidth().padding(12.dp)) {
        Column(modifier = Modifier.padding(12.dp), verticalArrangement = Arrangement.spacedBy(10.dp)) {
            Text("开始测验", style = MaterialTheme.typography.titleLarge)
            Text("题目数量：${settings.examQuestionCount}")
            Slider(
                value = settings.examQuestionCount.toFloat(),
                valueRange = 5f..30f,
                onValueChange = { onSettingsChange(settings.copy(examQuestionCount = it.toInt())) },
            )
            Button(onClick = onStart, modifier = Modifier.fillMaxWidth()) {
                Text("开始")
            }
        }
    }
}

@Composable
private fun HistoryScreen(viewModel: AppViewModel) {
    val history by viewModel.history.collectAsState()
    LazyColumn(
        modifier = Modifier.fillMaxSize().padding(12.dp),
        verticalArrangement = Arrangement.spacedBy(10.dp),
    ) {
        item {
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text("测验历史 (${history.size})", style = MaterialTheme.typography.titleLarge)
                OutlinedButton(onClick = viewModel::clearHistory) {
                    Icon(Icons.Default.Delete, contentDescription = null)
                    Text("清空")
                }
            }
        }

        if (history.isEmpty()) {
            item { Text("暂无历史记录。") }
        } else {
            items(history) { item ->
                val wrongCount = item.questionCount - item.correctCount
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(modifier = Modifier.padding(12.dp), verticalArrangement = Arrangement.spacedBy(6.dp)) {
                        Text("成绩：${item.correctCount}/${item.questionCount}", fontWeight = FontWeight.Bold)
                        Text("错误题数：$wrongCount")
                        val wrongSymbols = item.records.filterNot { it.isCorrect }.groupBy { it.phonemeSymbol }
                        if (wrongSymbols.isNotEmpty()) {
                            Text(
                                "高频错误：" + wrongSymbols.entries
                                    .sortedByDescending { it.value.size }
                                    .take(3)
                                    .joinToString { "/${it.key}/ x${it.value.size}" },
                            )
                        }
                    }
                }
            }
        }
    }
}

@Composable
private fun SettingsScreen(viewModel: AppViewModel) {
    val settings by viewModel.settings.collectAsState()

    val accentOptions = listOf("GenAm", "RP")
    val languageOptions = listOf("zh-CN", "en-US")

    LazyColumn(
        modifier = Modifier.fillMaxSize().padding(12.dp),
        verticalArrangement = Arrangement.spacedBy(12.dp),
    ) {
        item {
            Card(modifier = Modifier.fillMaxWidth()) {
                Column(modifier = Modifier.padding(12.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text("设置", style = MaterialTheme.typography.titleLarge)
                    Text("口音：${settings.accent}")
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        accentOptions.forEach { option ->
                            FilterChip(
                                selected = settings.accent == option,
                                onClick = {
                                    viewModel.updateSettings(settings.copy(accent = option))
                                },
                                label = { Text(option) },
                            )
                        }
                    }
                    Text("音量：${(settings.volume * 100).toInt()}%")
                    Slider(
                        value = settings.volume,
                        valueRange = 0f..1f,
                        onValueChange = { viewModel.updateSettings(settings.copy(volume = it)) },
                    )
                    Text("默认题目数：${settings.examQuestionCount}")
                    Slider(
                        value = settings.examQuestionCount.toFloat(),
                        valueRange = 5f..30f,
                        onValueChange = { viewModel.updateSettings(settings.copy(examQuestionCount = it.toInt())) },
                    )
                    Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                        Text("深色模式")
                        Switch(
                            checked = settings.darkMode,
                            onCheckedChange = { viewModel.updateSettings(settings.copy(darkMode = it)) },
                        )
                    }
                    Text("语言：${settings.language}")
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        languageOptions.forEach { option ->
                            FilterChip(
                                selected = settings.language == option,
                                onClick = {
                                    viewModel.updateSettings(settings.copy(language = option))
                                },
                                label = { Text(option) },
                            )
                        }
                    }
                }
            }
        }
    }
}
