package com.phonoark.ui.navigation

import android.util.Log
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.History
import androidx.compose.material.icons.filled.LibraryMusic
import androidx.compose.material.icons.filled.Quiz
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.Icon
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.res.stringResource
import androidx.navigation.NavDestination.Companion.hierarchy
import androidx.navigation.NavGraph.Companion.findStartDestination
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.phonoark.R
import com.phonoark.ui.exam.ExamScreen
import com.phonoark.ui.history.HistoryScreen
import com.phonoark.ui.ipachart.IpaChartScreen
import com.phonoark.ui.settings.SettingsScreen

sealed class Screen(val route: String, val labelResId: Int, val icon: ImageVector) {
    data object IpaChart : Screen("ipa_chart", R.string.nav_ipa_chart, Icons.Default.LibraryMusic)
    data object Exam : Screen("exam", R.string.nav_exam, Icons.Default.Quiz)
    data object History : Screen("history", R.string.nav_history, Icons.Default.History)
    data object Settings : Screen("settings", R.string.nav_settings, Icons.Default.Settings)
}

val bottomNavItems = listOf(
    Screen.IpaChart,
    Screen.Exam,
    Screen.History,
    Screen.Settings
)

@Composable
fun AppNavigation(darkTheme: Boolean, onDarkThemeChange: (Boolean) -> Unit) {
    Log.i("PhonoArk", "AppNavigation: composing")
    val navController = rememberNavController()

    Scaffold(
        bottomBar = {
            NavigationBar {
                val navBackStackEntry by navController.currentBackStackEntryAsState()
                val currentDestination = navBackStackEntry?.destination
                bottomNavItems.forEach { screen ->
                    NavigationBarItem(
                        icon = { Icon(screen.icon, contentDescription = stringResource(screen.labelResId)) },
                        label = { Text(stringResource(screen.labelResId)) },
                        selected = currentDestination?.hierarchy?.any { it.route == screen.route } == true,
                        onClick = {
                            navController.navigate(screen.route) {
                                popUpTo(navController.graph.findStartDestination().id) {
                                    saveState = true
                                }
                                launchSingleTop = true
                                restoreState = true
                            }
                        }
                    )
                }
            }
        }
    ) { innerPadding ->
        NavHost(
            navController = navController,
            startDestination = Screen.IpaChart.route,
            modifier = Modifier.padding(innerPadding)
        ) {
            composable(Screen.IpaChart.route) {
                Log.i("PhonoArk", "NavHost: IpaChart route")
                IpaChartScreen()
            }
            composable(Screen.Exam.route) { ExamScreen() }
            composable(Screen.History.route) { HistoryScreen() }
            composable(Screen.Settings.route) {
                SettingsScreen(
                    darkTheme = darkTheme,
                    onDarkThemeChange = onDarkThemeChange
                )
            }
        }
    }
}
