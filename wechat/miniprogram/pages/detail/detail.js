// pages/detail/detail.js
const { getPhonemeBySymbol } = require('../../data/phonemes');
const { playWord, stopAudio } = require('../../utils/audio');

Page({
  data: {
    phoneme: null,
    playingIndex: -1
  },

  _audioContext: null,

  onLoad(options) {
    const symbol = decodeURIComponent(options.symbol || '');
    const phoneme = getPhonemeBySymbol(symbol);
    if (phoneme) {
      this.setData({ phoneme });
    } else {
      wx.showToast({ title: '音标未找到', icon: 'none' });
      setTimeout(() => wx.navigateBack(), 1500);
    }
  },

  onUnload() {
    stopAudio(this._audioContext);
  },

  // 播放单词发音
  onPlayWord(e) {
    const index = e.currentTarget.dataset.index;
    const word = this.data.phoneme.examples[index].word;

    // 停止之前的播放
    stopAudio(this._audioContext);

    this.setData({ playingIndex: index });
    this._audioContext = playWord(word);

    // 播放结束后重置状态
    if (this._audioContext) {
      this._audioContext.onEnded(() => {
        this.setData({ playingIndex: -1 });
      });
      this._audioContext.onError(() => {
        this.setData({ playingIndex: -1 });
      });
    }
  },

  // 播放音标本身（用第一个示例单词发音）
  onPlayPhoneme() {
    stopAudio(this._audioContext);
    if (this.data.phoneme && this.data.phoneme.examples.length > 0) {
      this._audioContext = playWord(this.data.phoneme.examples[0].word);
    }
  }
});
