// pages/learn/learn.js
const { vowels, diphthongs, consonants } = require('../../data/phonemes');

Page({
  data: {
    currentTab: 0,
    tabs: [
      { name: '元音', type: 'vowel' },
      { name: '双元音', type: 'diphthong' },
      { name: '辅音', type: 'consonant' }
    ],
    phonemes: []
  },

  onLoad() {
    this.switchTab(0);
  },

  // 切换分类 Tab
  onTabTap(e) {
    const index = e.currentTarget.dataset.index;
    this.switchTab(index);
  },

  switchTab(index) {
    let phonemes;
    switch (index) {
      case 0:
        phonemes = vowels;
        break;
      case 1:
        phonemes = diphthongs;
        break;
      case 2:
        phonemes = consonants;
        break;
      default:
        phonemes = vowels;
    }
    this.setData({
      currentTab: index,
      phonemes: phonemes
    });
  },

  // 点击音标进入详情页
  onPhonemeTap(e) {
    const symbol = e.currentTarget.dataset.symbol;
    wx.navigateTo({
      url: `/pages/detail/detail?symbol=${encodeURIComponent(symbol)}`
    });
  }
});
