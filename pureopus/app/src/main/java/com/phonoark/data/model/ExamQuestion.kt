package com.phonoark.data.model

data class ExamQuestion(
    val phoneme: Phoneme,
    val options: List<ExampleWord>,
    val correctAnswer: ExampleWord,
    var userAnswer: ExampleWord? = null
) {
    val isCorrect: Boolean
        get() = userAnswer?.word == correctAnswer.word

    val isAnswered: Boolean
        get() = userAnswer != null
}
