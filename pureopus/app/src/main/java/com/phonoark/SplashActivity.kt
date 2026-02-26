package com.phonoark

import android.content.Intent
import android.graphics.Matrix
import android.os.Bundle
import android.widget.ImageView
import androidx.activity.ComponentActivity
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

class SplashActivity : ComponentActivity() {

    companion object {
        private const val SPLASH_DELAY_MS = 300L
        private const val CROP_FOCUS_Y = 0.5f
    }

    private var splashImage: ImageView? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        if (!isTaskRoot &&
            intent?.action == Intent.ACTION_MAIN &&
            intent?.hasCategory(Intent.CATEGORY_LAUNCHER) == true
        ) {
            finish()
            return
        }

        splashImage = ImageView(this).apply {
            scaleType = ImageView.ScaleType.MATRIX
            try {
                val inputStream = assets.open("logo.jpg")
                val bitmap = android.graphics.BitmapFactory.decodeStream(inputStream)
                inputStream.close()
                setImageBitmap(bitmap)
            } catch (_: Exception) {
                setBackgroundColor(android.graphics.Color.WHITE)
            }
            post { applyCrop() }
        }

        setContentView(splashImage)

        lifecycleScope.launch {
            delay(SPLASH_DELAY_MS)
            startActivity(Intent(this@SplashActivity, MainActivity::class.java).apply {
                addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            })
            finish()
        }
    }

    private fun applyCrop() {
        val imageView = splashImage ?: return
        val drawable = imageView.drawable ?: return
        val viewWidth = imageView.width.toFloat()
        val viewHeight = imageView.height.toFloat()
        val drawableWidth = drawable.intrinsicWidth.toFloat()
        val drawableHeight = drawable.intrinsicHeight.toFloat()

        if (viewWidth <= 0 || viewHeight <= 0 || drawableWidth <= 0 || drawableHeight <= 0) {
            imageView.scaleType = ImageView.ScaleType.CENTER_CROP
            return
        }

        val scale = maxOf(viewWidth / drawableWidth, viewHeight / drawableHeight)
        val scaledWidth = drawableWidth * scale
        val scaledHeight = drawableHeight * scale

        val dx = (viewWidth - scaledWidth) / 2f
        val dy = (viewHeight - scaledHeight) * CROP_FOCUS_Y

        val matrix = Matrix()
        matrix.setScale(scale, scale)
        matrix.postTranslate(dx, dy)

        imageView.scaleType = ImageView.ScaleType.MATRIX
        imageView.imageMatrix = matrix
    }
}
