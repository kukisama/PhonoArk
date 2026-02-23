// utils/audio.js
// 音律方舟 - 音频播放工具

/**
 * 使用在线 TTS 播放单词发音
 * @param {string} word - 要发音的单词
 * @returns {Object} audio context 实例（可用于停止播放）
 */
function playWord(word) {
  if (!word) return null;

  const innerAudioContext = wx.createInnerAudioContext();
  // 使用有道词典 TTS API（免费、稳定）
  const encodedWord = encodeURIComponent(word);
  innerAudioContext.src = `https://dict.youdao.com/dictvoice?audio=${encodedWord}&type=2`;
  innerAudioContext.autoplay = true;

  innerAudioContext.onError(function (err) {
    console.warn('音频播放失败:', err);
    innerAudioContext.destroy();
  });

  innerAudioContext.onEnded(function () {
    innerAudioContext.destroy();
  });

  return innerAudioContext;
}

/**
 * 停止所有正在播放的音频
 * @param {Object} audioContext - 要停止的 audio context
 */
function stopAudio(audioContext) {
  if (audioContext) {
    try {
      audioContext.stop();
      audioContext.destroy();
    } catch (e) {
      // 忽略已销毁的实例
    }
  }
}

module.exports = {
  playWord,
  stopAudio
};
