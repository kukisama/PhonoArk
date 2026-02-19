// pages/result/result.js
Page({
  data: {
    score: 0,
    correctCount: 0,
    totalQuestions: 0,
    duration: 0,
    scoreLevel: '',
    scoreColor: '',
    formattedDuration: ''
  },

  onLoad(options) {
    const score = parseInt(options.score, 10) || 0;
    const correct = parseInt(options.correct, 10) || 0;
    const total = parseInt(options.total, 10) || 0;
    const duration = parseInt(options.duration, 10) || 0;

    let scoreLevel, scoreColor;
    if (score >= 90) {
      scoreLevel = '优秀 🎉';
      scoreColor = '#52C41A';
    } else if (score >= 70) {
      scoreLevel = '良好 👍';
      scoreColor = '#4A90D9';
    } else if (score >= 60) {
      scoreLevel = '及格 😊';
      scoreColor = '#FAAD14';
    } else {
      scoreLevel = '继续加油 💪';
      scoreColor = '#FF4D4F';
    }

    // 格式化时长
    const minutes = Math.floor(duration / 60);
    const seconds = duration % 60;
    const formattedDuration = minutes > 0
      ? `${minutes} 分 ${seconds} 秒`
      : `${seconds} 秒`;

    this.setData({
      score,
      correctCount: correct,
      totalQuestions: total,
      duration,
      scoreLevel,
      scoreColor,
      formattedDuration
    });
  },

  // 重新考试
  onRetry() {
    wx.redirectTo({ url: '/pages/exam/exam' });
  },

  // 返回首页
  onGoHome() {
    wx.reLaunch({ url: '/pages/index/index' });
  },

  // 继续学习
  onGoLearn() {
    wx.redirectTo({ url: '/pages/learn/learn' });
  }
});
