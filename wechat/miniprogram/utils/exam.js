// utils/exam.js
// 音律方舟 - 考试逻辑工具

const { getAllPhonemes } = require('../data/phonemes');

/**
 * Fisher-Yates 洗牌算法
 * @param {Array} array - 要打乱的数组
 * @returns {Array} 打乱后的新数组
 */
function shuffle(array) {
  const arr = array.slice();
  for (let i = arr.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [arr[i], arr[j]] = [arr[j], arr[i]];
  }
  return arr;
}

/**
 * 生成考试题目
 * 题型：给出一个单词，从 4 个音标中选出该单词所属的音标
 * @param {number} count - 题目数量（默认 10）
 * @returns {Array} 考试题目数组
 */
function generateExam(count) {
  if (typeof count !== 'number' || count < 1) {
    count = 10;
  }

  const allPhonemes = getAllPhonemes();
  if (allPhonemes.length < 4) {
    return [];
  }

  const maxCount = Math.min(count, allPhonemes.length);
  const selectedPhonemes = shuffle(allPhonemes).slice(0, maxCount);
  const questions = [];

  for (let i = 0; i < selectedPhonemes.length; i++) {
    const correctPhoneme = selectedPhonemes[i];
    const examples = correctPhoneme.examples;
    const correctWord = examples[Math.floor(Math.random() * examples.length)];

    // 从其他音标中选 3 个作为干扰项
    const otherPhonemes = allPhonemes.filter(p => p.symbol !== correctPhoneme.symbol);
    const distractors = shuffle(otherPhonemes).slice(0, 3);

    const options = shuffle([
      { symbol: correctPhoneme.symbol, name: correctPhoneme.name, isCorrect: true },
      ...distractors.map(d => ({ symbol: d.symbol, name: d.name, isCorrect: false }))
    ]);

    questions.push({
      index: i,
      word: correctWord.word,
      wordIpa: correctWord.ipa,
      correctSymbol: correctPhoneme.symbol,
      options: options,
      userAnswer: null,
      isCorrect: null
    });
  }

  return questions;
}

/**
 * 计算考试结果
 * @param {Array} questions - 已作答的题目数组
 * @returns {Object} 考试结果
 */
function calculateResult(questions) {
  if (!Array.isArray(questions) || questions.length === 0) {
    return { totalQuestions: 0, correctCount: 0, score: 0 };
  }

  const answered = questions.filter(q => q.userAnswer !== null);
  const correct = answered.filter(q => q.isCorrect === true);

  return {
    totalQuestions: questions.length,
    answeredCount: answered.length,
    correctCount: correct.length,
    score: questions.length > 0
      ? Math.round((correct.length / questions.length) * 100)
      : 0
  };
}

/**
 * 保存考试记录到本地存储
 * @param {Object} result - 考试结果
 */
function saveExamResult(result) {
  try {
    const history = wx.getStorageSync('exam_history') || [];
    history.unshift({
      ...result,
      date: new Date().toISOString()
    });
    // 最多保留 50 条记录
    if (history.length > 50) {
      history.length = 50;
    }
    wx.setStorageSync('exam_history', history);
  } catch (e) {
    console.warn('保存考试记录失败:', e);
  }
}

/**
 * 获取考试历史记录
 * @returns {Array} 考试历史
 */
function getExamHistory() {
  try {
    return wx.getStorageSync('exam_history') || [];
  } catch (e) {
    return [];
  }
}

module.exports = {
  shuffle,
  generateExam,
  calculateResult,
  saveExamResult,
  getExamHistory
};
