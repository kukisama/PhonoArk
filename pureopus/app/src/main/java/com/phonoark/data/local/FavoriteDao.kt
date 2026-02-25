package com.phonoark.data.local

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import kotlinx.coroutines.flow.Flow

@Dao
interface FavoriteDao {
    @Query("SELECT * FROM favorites ORDER BY createdAt DESC")
    fun getAll(): Flow<List<FavoriteEntity>>

    @Query("SELECT * FROM favorites WHERE groupName = :groupName ORDER BY createdAt DESC")
    fun getByGroup(groupName: String): Flow<List<FavoriteEntity>>

    @Query("SELECT DISTINCT groupName FROM favorites ORDER BY groupName")
    fun getAllGroups(): Flow<List<String>>

    @Query("SELECT EXISTS(SELECT 1 FROM favorites WHERE symbol = :symbol)")
    suspend fun isFavorite(symbol: String): Boolean

    @Insert
    suspend fun insert(favorite: FavoriteEntity): Long

    @Query("DELETE FROM favorites WHERE symbol = :symbol")
    suspend fun deleteBySymbol(symbol: String)

    @Query("DELETE FROM favorites")
    suspend fun deleteAll()

    @Query("SELECT * FROM favorites")
    suspend fun getAllSync(): List<FavoriteEntity>
}
