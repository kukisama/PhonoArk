package com.phonoark.data.local

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "favorites")
data class FavoriteEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val symbol: String,
    val groupName: String = "Default",
    val createdAt: Long = System.currentTimeMillis()
)
