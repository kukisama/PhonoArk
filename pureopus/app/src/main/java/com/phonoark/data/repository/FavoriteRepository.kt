package com.phonoark.data.repository

import com.phonoark.data.local.FavoriteDao
import com.phonoark.data.local.FavoriteEntity
import com.phonoark.data.model.FavoritePhoneme
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import java.time.Instant
import java.time.LocalDateTime
import java.time.ZoneId
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class FavoriteRepository @Inject constructor(
    private val favoriteDao: FavoriteDao
) {
    fun getAll(): Flow<List<FavoritePhoneme>> {
        return favoriteDao.getAll().map { entities ->
            entities.map { it.toDomain() }
        }
    }

    fun getByGroup(groupName: String): Flow<List<FavoritePhoneme>> {
        return favoriteDao.getByGroup(groupName).map { entities ->
            entities.map { it.toDomain() }
        }
    }

    fun getAllGroups(): Flow<List<String>> = favoriteDao.getAllGroups()

    suspend fun isFavorite(symbol: String): Boolean = favoriteDao.isFavorite(symbol)

    suspend fun addFavorite(symbol: String, groupName: String = "Default") {
        if (!isFavorite(symbol)) {
            favoriteDao.insert(FavoriteEntity(symbol = symbol, groupName = groupName))
        }
    }

    suspend fun removeFavorite(symbol: String) {
        favoriteDao.deleteBySymbol(symbol)
    }

    suspend fun clearAll() {
        favoriteDao.deleteAll()
    }

    suspend fun getFavoriteSymbols(): List<String> {
        return favoriteDao.getAllSync().map { it.symbol }
    }

    private fun FavoriteEntity.toDomain() = FavoritePhoneme(
        id = id,
        symbol = symbol,
        groupName = groupName,
        createdAt = LocalDateTime.ofInstant(Instant.ofEpochMilli(createdAt), ZoneId.systemDefault())
    )
}
