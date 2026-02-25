package com.phonoark.data.model

import java.time.LocalDateTime

data class FavoritePhoneme(
    val id: Long = 0,
    val symbol: String,
    val groupName: String = "Default",
    val createdAt: LocalDateTime = LocalDateTime.now()
)
