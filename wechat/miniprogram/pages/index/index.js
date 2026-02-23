// pages/index/index.js
Page({
  data: {
    appName: '音律方舟',
    subtitle: '国际音标学习助手'
  },

  // 跳转到音标学习页
  goToLearn() {
    wx.navigateTo({ url: '/pages/learn/learn' });
  },

  // 跳转到音标考试页
  goToExam() {
    wx.navigateTo({ url: '/pages/exam/exam' });
  }
});
