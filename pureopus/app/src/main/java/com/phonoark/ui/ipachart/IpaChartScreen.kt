package com.phonoark.ui.ipachart

import android.speech.tts.TextToSpeech
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.ExperimentalLayoutApi
import androidx.compose.foundation.layout.FlowRow
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Favorite
import androidx.compose.material.icons.filled.FavoriteBorder
import androidx.compose.material.icons.filled.SwapHoriz
import androidx.compose.material.icons.filled.VolumeUp
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.DisposableEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.phonoark.R
import com.phonoark.data.model.Accent
import com.phonoark.data.model.Phoneme
import java.util.Locale

@OptIn(ExperimentalLayoutApi::class)
@Composable
fun IpaChartScreen(
    viewModel: IpaChartViewModel = hiltViewModel()
) {
    val state by viewModel.uiState.collectAsState()
    val context = LocalContext.current

    DisposableEffect(Unit) {
        viewModel.tts = TextToSpeech(context) { status ->
            if (status == TextToSpeech.SUCCESS) {
                viewModel.tts?.language = Locale.US
            }
        }
        onDispose { }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        // Header
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
            OutlinedButton(onClick = { viewModel.switchAccent() }) {
                Icon(Icons.Default.SwapHoriz, contentDescription = null, modifier = Modifier.size(18.dp))
                Spacer(modifier = Modifier.width(4.dp))
                Text(
                    text = if (state.currentAccent == Accent.GEN_AM)
                        stringResource(R.string.accent_genam)
                    else
                        stringResource(R.string.accent_rp)
                )
            }
        }

        Spacer(modifier = Modifier.height(16.dp))

        // Vowels Section
        PhonemeSection(
            title = stringResource(R.string.vowels),
            phonemes = state.vowels,
            selectedPhoneme = state.selectedPhoneme,
            onPhonemeClick = { viewModel.selectPhoneme(it) }
        )

        Spacer(modifier = Modifier.height(12.dp))

        // Diphthongs Section
        PhonemeSection(
            title = stringResource(R.string.diphthongs),
            phonemes = state.diphthongs,
            selectedPhoneme = state.selectedPhoneme,
            onPhonemeClick = { viewModel.selectPhoneme(it) }
        )

        Spacer(modifier = Modifier.height(12.dp))

        // Consonants Section
        PhonemeSection(
            title = stringResource(R.string.consonants),
            phonemes = state.consonants,
            selectedPhoneme = state.selectedPhoneme,
            onPhonemeClick = { viewModel.selectPhoneme(it) }
        )

        Spacer(modifier = Modifier.height(16.dp))

        // Detail Panel
        if (state.selectedPhoneme != null) {
            PhonemeDetailPanel(
                phoneme = state.selectedPhoneme!!,
                isFavorite = state.isFavorite,
                onPlayClick = { viewModel.speakPhoneme() },
                onFavoriteClick = { viewModel.toggleFavorite() },
                onWordClick = { viewModel.speakWord(it) }
            )
        } else {
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

@OptIn(ExperimentalLayoutApi::class)
@Composable
private fun PhonemeSection(
    title: String,
    phonemes: List<Phoneme>,
    selectedPhoneme: Phoneme?,
    onPhonemeClick: (Phoneme) -> Unit
) {
    Text(
        text = title,
        style = MaterialTheme.typography.titleMedium,
        fontWeight = FontWeight.Bold
    )
    Spacer(modifier = Modifier.height(8.dp))
    FlowRow(
        horizontalArrangement = Arrangement.spacedBy(8.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        phonemes.forEach { phoneme ->
            val isSelected = selectedPhoneme?.symbol == phoneme.symbol
            Box(
                modifier = Modifier
                    .size(48.dp)
                    .clip(CircleShape)
                    .background(
                        if (isSelected) MaterialTheme.colorScheme.primary
                        else MaterialTheme.colorScheme.primaryContainer
                    )
                    .clickable { onPhonemeClick(phoneme) },
                contentAlignment = Alignment.Center
            ) {
                Text(
                    text = phoneme.symbol,
                    fontSize = 18.sp,
                    fontWeight = FontWeight.Bold,
                    color = if (isSelected) MaterialTheme.colorScheme.onPrimary
                    else MaterialTheme.colorScheme.onPrimaryContainer
                )
            }
        }
    }
}

@Composable
private fun PhonemeDetailPanel(
    phoneme: Phoneme,
    isFavorite: Boolean,
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
            // Phoneme symbol and controls
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

            phoneme.exampleWords.take(10).forEach { word ->
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .clickable { onWordClick(word) }
                        .padding(vertical = 4.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Icon(
                        Icons.Default.VolumeUp,
                        contentDescription = null,
                        modifier = Modifier.size(16.dp),
                        tint = MaterialTheme.colorScheme.primary
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(
                        text = word.word,
                        style = MaterialTheme.typography.bodyLarge,
                        fontWeight = FontWeight.Medium
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
