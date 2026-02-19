// pages/exam/exam.js
const { generateExam, calculateResult, saveExamResult } = require('../../utils/exam');
const { playWord, stopAudio } = require('../../utils/audio');

Page({
  data: {
    // 考试状态：'setup' | 'active' | 'finished'
    examState: 'setup',
    questionCount: 10,
    questions: [],
    currentIndex: 0,
    currentQuestion: null,
    selectedOption: null,
    showFeedback: false,
    isCorrect: false,
    progress: 0
  },

  _audioContext: null,
  _startTime: null,

  onUnload() {
    stopAudio(this._audioContext);
  },

  // 设置题目数量
  onCountChange(e) {
    this.setData({ questionCount: parseInt(e.detail.value, 10) });
  },

  // 开始考试
  startExam() {
    const questions = generateExam(this.data.questionCount);
    if (questions.length === 0) {
      wx.showToast({ title: '无法生成题目', icon: 'none' });
      return;
    }

    this._startTime = Date.now();
    this.setData({
      examState: 'active',
      questions: questions,
      currentIndex: 0,
      currentQuestion: questions[0],
      selectedOption: null,
      showFeedback: false,
      progress: 0
    });

    // 自动播放当前题目单词
    this._playCurrentWord();
  },

  // 播放当前题目的单词
  _playCurrentWord() {
    stopAudio(this._audioContext);
    if (this.data.currentQuestion) {
      this._audioContext = playWord(this.data.currentQuestion.word);
    }
  },

  // 手动重新播放
  onReplay() {
    this._playCurrentWord();
  },

  // 选择答案
  onSelectOption(e) {
    if (this.data.showFeedback) return; // 防止重复选择

    const optionIndex = e.currentTarget.dataset.index;
    const option = this.data.currentQuestion.options[optionIndex];
    const isCorrect = option.isCorrect;

    // 更新题目数据
    const questions = this.data.questions;
    questions[this.data.currentIndex].userAnswer = option.symbol;
    questions[this.data.currentIndex].isCorrect = isCorrect;

    this.setData({
      selectedOption: optionIndex,
      showFeedback: true,
      isCorrect: isCorrect,
      questions: questions
    });

    // 1.5 秒后自动进入下一题
    setTimeout(() => {
      this._nextQuestion();
    }, 1500);
  },

  // 进入下一题
  _nextQuestion() {
    const nextIndex = this.data.currentIndex + 1;

    if (nextIndex >= this.data.questions.length) {
      // 考试结束
      this._finishExam();
      return;
    }

    this.setData({
      currentIndex: nextIndex,
      currentQuestion: this.data.questions[nextIndex],
      selectedOption: null,
      showFeedback: false,
      isCorrect: false,
      progress: Math.round((nextIndex / this.data.questions.length) * 100)
    });

    this._playCurrentWord();
  },

  // 完成考试
  _finishExam() {
    const result = calculateResult(this.data.questions);
    const duration = Math.round((Date.now() - this._startTime) / 1000);
    result.duration = duration;

    // 保存考试记录
    saveExamResult(result);

    // 跳转到结果页
    wx.redirectTo({
      url: `/pages/result/result?score=${result.score}&correct=${result.correctCount}&total=${result.totalQuestions}&duration=${duration}`
    });
  }
});
