package com.phonoark.ui.ipachart

import android.content.res.Configuration
import android.util.Log
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.ExperimentalLayoutApi
import androidx.compose.foundation.layout.FlowRow
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.DeleteSweep
import androidx.compose.material.icons.filled.Favorite
import androidx.compose.material.icons.filled.FavoriteBorder
import androidx.compose.material.icons.filled.Star
import androidx.compose.material.icons.filled.SwapHoriz
import androidx.compose.material.icons.filled.VolumeUp
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.phonoark.R
import com.phonoark.data.model.Accent
import com.phonoark.data.model.Phoneme
import com.phonoark.data.model.PhonemeType
import com.phonoark.ui.theme.Orange500

@OptIn(ExperimentalLayoutApi::class)
@Composable
fun IpaChartScreen(
    viewModel: IpaChartViewModel = hiltViewModel()
) {
    val state by viewModel.uiState.collectAsState()
    Log.i(
        "PhonoArk",
        "IpaChartScreen: composing, vowels=${state.vowels.size}, diphthongs=${state.diphthongs.size}, consonants=${state.consonants.size}, selected=${state.selectedPhoneme?.symbol}, err=${state.loadError}"
    )
    val configuration = LocalConfiguration.current
    val isLandscape = configuration.orientation == Configuration.ORIENTATION_LANDSCAPE

    // 当用户切换音标时，确保下方“示例单词”区域回到顶部，方便立刻开始关联测试。
    // 注意：这里用 key=selectedPhoneme.symbol 来触发回顶，避免点击同一音标重复滚动。
    val detailScrollState = rememberScrollState()
    LaunchedEffect(state.selectedPhoneme?.symbol) {
        detailScrollState.scrollTo(0)
    }

    if (!state.loadError.isNullOrBlank()) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp),
            verticalArrangement = Arrangement.Center,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = "首页数据加载失败",
                style = MaterialTheme.typography.headlineSmall,
                fontWeight = FontWeight.SemiBold
            )
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                text = state.loadError!!,
                style = MaterialTheme.typography.bodyMedium,
                color = MaterialTheme.colorScheme.onSurfaceVariant,
                textAlign = TextAlign.Center
            )
            Spacer(modifier = Modifier.height(12.dp))
            Text(
                text = "请检查 assets 是否包含 phoneme-word-bank.json 以及相关音频资源目录（如 US-Jenny）。",
                style = MaterialTheme.typography.bodySmall,
                color = MaterialTheme.colorScheme.onSurfaceVariant,
                textAlign = TextAlign.Center
            )
        }
        return
    }

    // 明确兜底：当列表为空但又没有抛出异常时，给出可见提示。
    if (state.vowels.isEmpty() && state.diphthongs.isEmpty() && state.consonants.isEmpty()) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp),
            verticalArrangement = Arrangement.Center,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = "未加载到音标数据",
                style = MaterialTheme.typography.headlineSmall,
                fontWeight = FontWeight.SemiBold
            )
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                text = "请确认 assets/phoneme-word-bank.json 可被读取，且 JSON 中 phonemes 列表非空。",
                style = MaterialTheme.typography.bodyMedium,
                color = MaterialTheme.colorScheme.onSurfaceVariant,
                textAlign = TextAlign.Center
            )
        }
        return
    }

    if (isLandscape && state.selectedPhoneme != null) {
        // Landscape split: left = phoneme list, right = detail panel
        Row(modifier = Modifier.fillMaxSize().padding(8.dp)) {
            Column(
                modifier = Modifier
                    .weight(1.15f)
                    .fillMaxHeight()
                    .verticalScroll(rememberScrollState())
                    .padding(8.dp)
            ) {
                IpaChartHeader(state = state, onSwitchAccent = { viewModel.switchAccent() })
                Spacer(modifier = Modifier.height(8.dp))
                BatchFavoriteButtons(
                    state = state,
                    onToggleBatch = { viewModel.toggleFavoriteBatch(it) },
                    onClearAll = { viewModel.clearAllFavorites() }
                )
                Spacer(modifier = Modifier.height(8.dp))
                PhonemeGrid(state = state, onPhonemeClick = { viewModel.selectPhoneme(it) })
            }
            Column(
                modifier = Modifier
                    .weight(0.85f)
                    .fillMaxHeight()
                    .verticalScroll(detailScrollState)
                    .padding(8.dp)
            ) {
                PhonemeDetailPanel(
                    phoneme = state.selectedPhoneme!!,
                    isFavorite = state.isFavorite,
                    playingWordId = state.playingWordId,
                    onPlayClick = { viewModel.speakPhoneme() },
                    onFavoriteClick = { viewModel.toggleFavorite() },
                    onWordClick = { viewModel.speakWord(it) }
                )
            }
        }
    } else if (state.selectedPhoneme != null) {
        // Portrait split: top = phoneme list, bottom = detail panel
        Column(modifier = Modifier.fillMaxSize().padding(8.dp)) {
            Column(
                modifier = Modifier
                    .weight(1f)
                    .verticalScroll(rememberScrollState())
                    .padding(8.dp)
            ) {
                IpaChartHeader(state = state, onSwitchAccent = { viewModel.switchAccent() })
                Spacer(modifier = Modifier.height(8.dp))
                BatchFavoriteButtons(
                    state = state,
                    onToggleBatch = { viewModel.toggleFavoriteBatch(it) },
                    onClearAll = { viewModel.clearAllFavorites() }
                )
                Spacer(modifier = Modifier.height(8.dp))
                PhonemeGrid(state = state, onPhonemeClick = { viewModel.selectPhoneme(it) })
            }
            Column(
                modifier = Modifier
                    .weight(1f)
                    .verticalScroll(detailScrollState)
                    .padding(8.dp)
            ) {
                PhonemeDetailPanel(
                    phoneme = state.selectedPhoneme!!,
                    isFavorite = state.isFavorite,
                    playingWordId = state.playingWordId,
                    onPlayClick = { viewModel.speakPhoneme() },
                    onFavoriteClick = { viewModel.toggleFavorite() },
                    onWordClick = { viewModel.speakWord(it) }
                )
            }
        }
    } else {
        // No selection: full screen phoneme list
        Column(
            modifier = Modifier
                .fillMaxSize()
                .verticalScroll(rememberScrollState())
                .padding(16.dp)
        ) {
            IpaChartHeader(state = state, onSwitchAccent = { viewModel.switchAccent() })
            Spacer(modifier = Modifier.height(8.dp))
            BatchFavoriteButtons(
                state = state,
                onToggleBatch = { viewModel.toggleFavoriteBatch(it) },
                onClearAll = { viewModel.clearAllFavorites() }
            )
            Spacer(modifier = Modifier.height(16.dp))
            PhonemeGrid(state = state, onPhonemeClick = { viewModel.selectPhoneme(it) })
            Spacer(modifier = Modifier.height(16.dp))
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant)
            ) {
                Text(
                    text = stringResource(R.string.select_phoneme_hint),
                    modifier = Modifier.padding(24.dp),
                    style = MaterialTheme.typography.bodyLarge,
                    textAlign = TextAlign.Center
                )
            }
        }
    }
}

@Composable
private fun IpaChartHeader(state: IpaChartUiState, onSwitchAccent: () -> Unit) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.SpaceBetween,
        verticalAlignment = Alignment.CenterVertically
    ) {
        Text(
            text = stringResource(R.string.ipa_title),
            style = MaterialTheme.typography.headlineMedium,
            modifier = Modifier.weight(1f)
        )
        OutlinedButton(onClick = onSwitchAccent) {
            Icon(Icons.Default.SwapHoriz, contentDescription = null, modifier = Modifier.size(18.dp))
            Spacer(modifier = Modifier.width(4.dp))
            Text(
                text = when (state.currentAccent) {
                    Accent.US_JENNY -> stringResource(R.string.accent_usjenny)
                    Accent.GEN_AM -> stringResource(R.string.accent_genam)
                    Accent.RP -> stringResource(R.string.accent_rp)
                }
            )
        }
    }
}

@Composable
private fun BatchFavoriteButtons(
    state: IpaChartUiState,
    onToggleBatch: (PhonemeType) -> Unit,
    onClearAll: () -> Unit
) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.spacedBy(4.dp)
    ) {
        TextButton(onClick = { onToggleBatch(PhonemeType.VOWEL) }, modifier = Modifier.weight(1f)) {
            Icon(
                if (state.allVowelsFavorited) Icons.Default.Star else Icons.Default.FavoriteBorder,
                contentDescription = null,
                modifier = Modifier.size(16.dp),
                tint = if (state.allVowelsFavorited) Orange500 else MaterialTheme.colorScheme.onSurface
            )
            Spacer(modifier = Modifier.width(2.dp))
            Text(stringResource(R.string.vowels), fontSize = 12.sp, maxLines = 1)
        }
        TextButton(onClick = { onToggleBatch(PhonemeType.DIPHTHONG) }, modifier = Modifier.weight(1f)) {
            Icon(
                if (state.allDiphthongsFavorited) Icons.Default.Star else Icons.Default.FavoriteBorder,
                contentDescription = null,
                modifier = Modifier.size(16.dp),
                tint = if (state.allDiphthongsFavorited) Orange500 else MaterialTheme.colorScheme.onSurface
            )
            Spacer(modifier = Modifier.width(2.dp))
            Text(stringResource(R.string.diphthongs), fontSize = 12.sp, maxLines = 1)
        }
        TextButton(onClick = { onToggleBatch(PhonemeType.CONSONANT) }, modifier = Modifier.weight(1f)) {
            Icon(
                if (state.allConsonantsFavorited) Icons.Default.Star else Icons.Default.FavoriteBorder,
                contentDescription = null,
                modifier = Modifier.size(16.dp),
                tint = if (state.allConsonantsFavorited) Orange500 else MaterialTheme.colorScheme.onSurface
            )
            Spacer(modifier = Modifier.width(2.dp))
            Text(stringResource(R.string.consonants), fontSize = 12.sp, maxLines = 1)
        }
        TextButton(onClick = onClearAll) {
            Icon(Icons.Default.DeleteSweep, contentDescription = null, modifier = Modifier.size(16.dp))
        }
    }
}

@OptIn(ExperimentalLayoutApi::class)
@Composable
private fun PhonemeGrid(state: IpaChartUiState, onPhonemeClick: (Phoneme) -> Unit) {
    PhonemeSection(
        title = stringResource(R.string.vowels),
        phonemes = state.vowels,
        selectedPhoneme = state.selectedPhoneme,
        favoriteSymbols = state.favoriteSymbols,
        onPhonemeClick = onPhonemeClick
    )
    Spacer(modifier = Modifier.height(12.dp))
    PhonemeSection(
        title = stringResource(R.string.diphthongs),
        phonemes = state.diphthongs,
        selectedPhoneme = state.selectedPhoneme,
        favoriteSymbols = state.favoriteSymbols,
        onPhonemeClick = onPhonemeClick
    )
    Spacer(modifier = Modifier.height(12.dp))
    PhonemeSection(
        title = stringResource(R.string.consonants),
        phonemes = state.consonants,
        selectedPhoneme = state.selectedPhoneme,
        favoriteSymbols = state.favoriteSymbols,
        onPhonemeClick = onPhonemeClick
    )
}

@OptIn(ExperimentalLayoutApi::class)
@Composable
private fun PhonemeSection(
    title: String,
    phonemes: List<Phoneme>,
    selectedPhoneme: Phoneme?,
    favoriteSymbols: Set<String>,
    onPhonemeClick: (Phoneme) -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(modifier = Modifier.padding(12.dp)) {
            Text(
                text = title,
                style = MaterialTheme.typography.titleMedium,
                fontWeight = FontWeight.Bold
            )
            Spacer(modifier = Modifier.height(8.dp))
            FlowRow(
                // 增大音标之间的间距，避免“挤”。
                horizontalArrangement = Arrangement.spacedBy(10.dp),
                verticalArrangement = Arrangement.spacedBy(10.dp)
            ) {
                phonemes.forEach { phoneme ->
                    val isSelected = selectedPhoneme?.symbol == phoneme.symbol
                    val isFav = phoneme.symbol in favoriteSymbols
                    Box(
                        modifier = Modifier
                            .size(width = 68.dp, height = 56.dp)
                            .then(
                                if (isFav) Modifier.border(2.dp, Orange500, RoundedCornerShape(12.dp))
                                else Modifier
                            )
                            .background(
                                color = if (isSelected) MaterialTheme.colorScheme.primary
                                else MaterialTheme.colorScheme.primaryContainer,
                                shape = RoundedCornerShape(12.dp)
                            )
                            .clickable { onPhonemeClick(phoneme) },
                        contentAlignment = Alignment.Center
                    ) {
                        Text(
                            text = phoneme.symbol,
                            // 格子足够大，适当放大字号，提升可读性。
                            fontSize = 26.sp,
                            fontWeight = FontWeight.Bold,
                            color = if (isSelected) MaterialTheme.colorScheme.onPrimary
                            else MaterialTheme.colorScheme.onPrimaryContainer
                        )
                    }
                }
            }
        }
    }
}

@Composable
private fun PhonemeDetailPanel(
    phoneme: Phoneme,
    isFavorite: Boolean,
    playingWordId: String?,
    onPlayClick: () -> Unit,
    onFavoriteClick: () -> Unit,
    onWordClick: (com.phonoark.data.model.ExampleWord) -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        elevation = CardDefaults.cardElevation(defaultElevation = 4.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = phoneme.symbol,
                    fontSize = 48.sp,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.primary
                )
                Row {
                    IconButton(onClick = onPlayClick) {
                        Icon(
                            Icons.Default.VolumeUp,
                            contentDescription = stringResource(R.string.play_sound),
                            tint = MaterialTheme.colorScheme.primary,
                            modifier = Modifier.size(32.dp)
                        )
                    }
                    IconButton(onClick = onFavoriteClick) {
                        Icon(
                            if (isFavorite) Icons.Default.Favorite else Icons.Default.FavoriteBorder,
                            contentDescription = null,
                            tint = if (isFavorite) Color.Red else MaterialTheme.colorScheme.onSurface,
                            modifier = Modifier.size(32.dp)
                        )
                    }
                }
            }

            Text(
                text = phoneme.description,
                style = MaterialTheme.typography.bodyMedium,
                color = MaterialTheme.colorScheme.onSurfaceVariant
            )

            Spacer(modifier = Modifier.height(12.dp))

            Text(
                text = stringResource(R.string.example_words),
                style = MaterialTheme.typography.titleSmall,
                fontWeight = FontWeight.SemiBold
            )

            Spacer(modifier = Modifier.height(8.dp))

            phoneme.exampleWords.forEach { word ->
                val isPlaying = playingWordId == word.word
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .then(
                            if (isPlaying) Modifier.background(
                                MaterialTheme.colorScheme.primaryContainer,
                                RoundedCornerShape(4.dp)
                            ) else Modifier
                        )
                        .clickable { onWordClick(word) }
                        .padding(vertical = 4.dp, horizontal = 4.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Icon(
                        Icons.Default.VolumeUp,
                        contentDescription = null,
                        modifier = Modifier.size(16.dp),
                        tint = if (isPlaying) MaterialTheme.colorScheme.primary
                        else MaterialTheme.colorScheme.onSurfaceVariant
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(
                        text = word.word,
                        style = MaterialTheme.typography.bodyLarge,
                        fontWeight = if (isPlaying) FontWeight.Bold else FontWeight.Medium
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(
                        text = word.ipaTranscription,
                        style = MaterialTheme.typography.bodyMedium,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }
        }
    }
}
