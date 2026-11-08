package com.phonoark.ui.exam

import androidx.compose.animation.animateColorAsState
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.interaction.collectIsPressedAsState
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
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.VolumeUp
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.ExposedDropdownMenuDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.MenuAnchorType
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Slider
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.phonoark.R
import com.phonoark.data.model.ExampleWord
import com.phonoark.ui.theme.Green500
import com.phonoark.ui.theme.Red500

@Composable
private fun formatFeedback(feedback: FeedbackType): String {
    return when (feedback) {
        is FeedbackType.None -> ""
        is FeedbackType.Correct -> stringResource(R.string.feedback_correct_template, feedback.word, feedback.ipa)
        is FeedbackType.Incorrect -> stringResource(R.string.feedback_incorrect_template, feedback.correctWord, feedback.correctIpa)
        is FeedbackType.Completed -> stringResource(R.string.feedback_exam_completed_template, feedback.correct, feedback.total, feedback.score)
        is FeedbackType.NoQuestions -> stringResource(R.string.feedback_no_questions)
    }
}

@OptIn(ExperimentalMaterial3Api::class, ExperimentalLayoutApi::class)
@Composable
fun ExamScreen(
    viewModel: ExamViewModel = hiltViewModel()
) {
    val state by viewModel.uiState.collectAsState()

    Column(
        modifier = Modifier
            .fillMaxSize()
            .verticalScroll(rememberScrollState())
            .padding(16.dp)
    ) {
        Text(
            text = stringResource(R.string.practice_exam),
            style = MaterialTheme.typography.headlineMedium
        )

        Spacer(modifier = Modifier.height(16.dp))

        if (!state.isExamActive && !state.examCompleted) {
            ExamSetup(
                questionCount = state.questionCount,
                examScope = state.examScope,
                onQuestionCountChange = { viewModel.updateQuestionCount(it) },
                onExamScopeChange = { viewModel.updateExamScope(it) },
                onStartExam = { viewModel.startExam() }
            )
        } else if (state.isExamActive) {
            ActiveExam(
                state = state,
                onPlayPhoneme = { viewModel.playCurrentPhoneme() },
                onSelectAnswer = { viewModel.selectAnswer(it) },
                onNextQuestion = { viewModel.nextQuestion() },
                onEndExam = { viewModel.endExam() }
            )
        } else if (state.examCompleted) {
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surfaceVariant)
            ) {
                Column(modifier = Modifier.padding(24.dp)) {
                    Text(
                        text = formatFeedback(state.feedback),
                        style = MaterialTheme.typography.titleLarge,
                        textAlign = TextAlign.Center,
                        modifier = Modifier.fillMaxWidth()
                    )
                    Spacer(modifier = Modifier.height(16.dp))
                    Button(
                        onClick = { viewModel.resetExam() },
                        modifier = Modifier.align(Alignment.CenterHorizontally)
                    ) {
                        Text(stringResource(R.string.start_exam))
                    }
                }
            }
        }

        if (state.feedback is FeedbackType.NoQuestions && !state.examCompleted && !state.isExamActive) {
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                text = formatFeedback(state.feedback),
                color = MaterialTheme.colorScheme.error,
                style = MaterialTheme.typography.bodyMedium
            )
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun ExamSetup(
    questionCount: Int,
    examScope: String,
    onQuestionCountChange: (Int) -> Unit,
    onExamScopeChange: (String) -> Unit,
    onStartExam: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Text(
                text = stringResource(R.string.number_of_questions),
                style = MaterialTheme.typography.titleSmall
            )
            Spacer(modifier = Modifier.height(4.dp))
            Row(
                verticalAlignment = Alignment.CenterVertically,
                modifier = Modifier.fillMaxWidth()
            ) {
                Slider(
                    value = questionCount.toFloat(),
                    onValueChange = { onQuestionCountChange(it.toInt()) },
                    valueRange = 5f..50f,
                    steps = 8,
                    modifier = Modifier.weight(1f)
                )
                Spacer(modifier = Modifier.width(8.dp))
                Text(
                    text = "$questionCount",
                    style = MaterialTheme.typography.bodyLarge,
                    fontWeight = FontWeight.Bold
                )
            }

            Spacer(modifier = Modifier.height(16.dp))

            Text(
                text = stringResource(R.string.exam_scope),
                style = MaterialTheme.typography.titleSmall
            )
            Spacer(modifier = Modifier.height(4.dp))

            var scopeExpanded by remember { mutableStateOf(false) }
            val scopeOptions = listOf("All" to R.string.all_phonemes, "Favorites" to R.string.favorites_only)

            ExposedDropdownMenuBox(
                expanded = scopeExpanded,
                onExpandedChange = { scopeExpanded = it }
            ) {
                OutlinedTextField(
                    value = stringResource(scopeOptions.first { it.first == examScope }.second),
                    onValueChange = {},
                    readOnly = true,
                    trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = scopeExpanded) },
                    modifier = Modifier
                        .fillMaxWidth()
                        .menuAnchor(MenuAnchorType.PrimaryNotEditable)
                )
                ExposedDropdownMenu(
                    expanded = scopeExpanded,
                    onDismissRequest = { scopeExpanded = false }
                ) {
                    scopeOptions.forEach { (value, labelRes) ->
                        DropdownMenuItem(
                            text = { Text(stringResource(labelRes)) },
                            onClick = {
                                onExamScopeChange(value)
                                scopeExpanded = false
                            }
                        )
                    }
                }
            }

            Spacer(modifier = Modifier.height(24.dp))

            Button(
                onClick = onStartExam,
                modifier = Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(8.dp)
            ) {
                Text(
                    text = stringResource(R.string.start_exam),
                    fontSize = 16.sp,
                    modifier = Modifier.padding(vertical = 4.dp)
                )
            }
        }
    }
}

@OptIn(ExperimentalLayoutApi::class)
@Composable
private fun ActiveExam(
    state: ExamUiState,
    onPlayPhoneme: () -> Unit,
    onSelectAnswer: (ExampleWord) -> Unit,
    onNextQuestion: () -> Unit,
    onEndExam: () -> Unit
) {
    val question = state.currentQuestion ?: return

    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.SpaceBetween
    ) {
        Text(
            text = "${stringResource(R.string.question_label)} ${state.currentQuestionIndex + 1} ${stringResource(R.string.of_label)} ${state.totalQuestions}",
            style = MaterialTheme.typography.titleMedium
        )
        Text(
            text = "${stringResource(R.string.correct_label)} ${state.correctAnswers}",
            style = MaterialTheme.typography.titleMedium,
            color = Green500
        )
    }

    Spacer(modifier = Modifier.height(16.dp))

    // 播放音素的框体居中显示（视觉焦点更明确）。
    Box(modifier = Modifier.fillMaxWidth()) {
        Card(
            modifier = Modifier
                .fillMaxWidth(0.85f)
                .align(Alignment.Center),
            shape = RoundedCornerShape(12.dp)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(150.dp)
                    .padding(16.dp),
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center
            ) {
                val interactionSource = remember { MutableInteractionSource() }
                val isPressed by interactionSource.collectIsPressedAsState()
                val buttonColor by animateColorAsState(
                    targetValue = if (isPressed)
                        MaterialTheme.colorScheme.primary.copy(alpha = 0.7f)
                    else
                        MaterialTheme.colorScheme.primary,
                    label = "playButtonColor"
                )

                Button(
                    onClick = onPlayPhoneme,
                    interactionSource = interactionSource,
                    modifier = Modifier
                        .fillMaxWidth(0.7f)
                        .height(56.dp),
                    shape = RoundedCornerShape(8.dp),
                    colors = ButtonDefaults.buttonColors(containerColor = buttonColor)
                ) {
                    Icon(Icons.Default.VolumeUp, contentDescription = null, modifier = Modifier.size(28.dp))
                    Spacer(modifier = Modifier.width(8.dp))
                    Text(stringResource(R.string.play_phoneme), fontSize = 18.sp)
                }

                Spacer(modifier = Modifier.height(8.dp))

                Text(
                    text = "${stringResource(R.string.target_phoneme_label)} ${question.phoneme.symbol}",
                    style = MaterialTheme.typography.bodyLarge,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }
    }

    Spacer(modifier = Modifier.height(16.dp))

    Text(
        text = stringResource(R.string.select_word),
        style = MaterialTheme.typography.titleSmall
    )
    Spacer(modifier = Modifier.height(8.dp))

    FlowRow(
        horizontalArrangement = Arrangement.spacedBy(8.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp),
        modifier = Modifier.fillMaxWidth()
    ) {
        question.options.forEach { option ->
            val buttonColor = when {
                !state.isAnswered -> MaterialTheme.colorScheme.primaryContainer
                option.word == question.correctAnswer.word -> Green500
                option.word == question.userAnswer?.word && !question.isCorrect -> Red500
                else -> MaterialTheme.colorScheme.surfaceVariant
            }
            val textColor = when {
                !state.isAnswered -> MaterialTheme.colorScheme.onPrimaryContainer
                option.word == question.correctAnswer.word -> Color.White
                option.word == question.userAnswer?.word && !question.isCorrect -> Color.White
                else -> MaterialTheme.colorScheme.onSurfaceVariant
            }

            Button(
                onClick = { onSelectAnswer(option) },
                enabled = !state.isAnswered,
                colors = ButtonDefaults.buttonColors(
                    containerColor = buttonColor,
                    contentColor = textColor,
                    disabledContainerColor = buttonColor,
                    disabledContentColor = textColor
                ),
                shape = RoundedCornerShape(8.dp),
                modifier = Modifier
                    .fillMaxWidth(0.47f)
                    .height(56.dp)
            ) {
                Text(text = option.word, fontWeight = FontWeight.Bold)
            }
        }
    }

    Spacer(modifier = Modifier.height(16.dp))

    if (state.feedback !is FeedbackType.None) {
        val feedbackText = formatFeedback(state.feedback)
        Card(
            modifier = Modifier.fillMaxWidth(),
            colors = CardDefaults.cardColors(
                containerColor = if (state.feedback is FeedbackType.Correct)
                    Green500.copy(alpha = 0.15f)
                else
                    Red500.copy(alpha = 0.15f)
            )
        ) {
            Text(
                text = feedbackText,
                modifier = Modifier.padding(12.dp),
                style = MaterialTheme.typography.bodyLarge
            )
        }
    }

    Spacer(modifier = Modifier.height(16.dp))

    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        if (state.isAnswered) {
            Button(
                onClick = onNextQuestion,
                modifier = Modifier.weight(1f),
                shape = RoundedCornerShape(8.dp)
            ) {
                Text(stringResource(R.string.next_question))
            }
        }
        OutlinedButton(
            onClick = onEndExam,
            modifier = Modifier.weight(1f),
            shape = RoundedCornerShape(8.dp)
        ) {
            Text(stringResource(R.string.end_exam))
        }
    }
}
