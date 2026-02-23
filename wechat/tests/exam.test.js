// tests/exam.test.js
// 音律方舟 - 考试逻辑回归测试

// Mock wx 全局对象（小程序环境模拟）
global.wx = {
  getStorageSync: jest.fn(() => []),
  setStorageSync: jest.fn(),
  createInnerAudioContext: jest.fn(() => ({
    src: '',
    autoplay: false,
    onError: jest.fn(),
    onEnded: jest.fn(),
    stop: jest.fn(),
    destroy: jest.fn()
  }))
};

const { shuffle, generateExam, calculateResult, saveExamResult, getExamHistory } = require('../miniprogram/utils/exam');

describe('shuffle 洗牌算法', () => {

  test('洗牌后数组长度不变', () => {
    const arr = [1, 2, 3, 4, 5];
    const shuffled = shuffle(arr);
    expect(shuffled).toHaveLength(arr.length);
  });

  test('洗牌后包含所有原始元素', () => {
    const arr = [1, 2, 3, 4, 5];
    const shuffled = shuffle(arr);
    arr.forEach(item => {
      expect(shuffled).toContain(item);
    });
  });

  test('不修改原数组', () => {
    const arr = [1, 2, 3, 4, 5];
    const original = [...arr];
    shuffle(arr);
    expect(arr).toEqual(original);
  });

  test('空数组洗牌返回空数组', () => {
    expect(shuffle([])).toEqual([]);
  });

  test('单元素数组洗牌返回相同数组', () => {
    expect(shuffle([42])).toEqual([42]);
  });
});

describe('generateExam 考试出题', () => {

  test('默认生成 10 题', () => {
    const questions = generateExam(10);
    expect(questions).toHaveLength(10);
  });

  test('可指定题目数量', () => {
    const questions = generateExam(5);
    expect(questions).toHaveLength(5);
  });

  test('题目数量不超过音标总数', () => {
    const questions = generateExam(100);
    expect(questions.length).toBeLessThanOrEqual(44);
  });

  test('无效题数自动修正为 10', () => {
    expect(generateExam(0)).toHaveLength(10);
    expect(generateExam(-1)).toHaveLength(10);
  });

  test('每道题包含必要字段', () => {
    const questions = generateExam(5);
    questions.forEach((q, i) => {
      expect(q.index).toBe(i);
      expect(q.word).toBeTruthy();
      expect(q.wordIpa).toBeTruthy();
      expect(q.correctSymbol).toBeTruthy();
      expect(q.options).toHaveLength(4);
      expect(q.userAnswer).toBeNull();
      expect(q.isCorrect).toBeNull();
    });
  });

  test('每道题的选项中有且仅有一个正确答案', () => {
    const questions = generateExam(10);
    questions.forEach(q => {
      const correctOptions = q.options.filter(o => o.isCorrect);
      expect(correctOptions).toHaveLength(1);
      expect(correctOptions[0].symbol).toBe(q.correctSymbol);
    });
  });

  test('每道题的 4 个选项互不相同', () => {
    const questions = generateExam(10);
    questions.forEach(q => {
      const symbols = q.options.map(o => o.symbol);
      const uniqueSymbols = new Set(symbols);
      expect(uniqueSymbols.size).toBe(4);
    });
  });
});

describe('calculateResult 计算结果', () => {

  test('全部答对得 100 分', () => {
    const questions = [
      { userAnswer: 'iː', isCorrect: true },
      { userAnswer: 'æ', isCorrect: true },
      { userAnswer: 'p', isCorrect: true }
    ];
    const result = calculateResult(questions);
    expect(result.totalQuestions).toBe(3);
    expect(result.correctCount).toBe(3);
    expect(result.score).toBe(100);
  });

  test('全部答错得 0 分', () => {
    const questions = [
      { userAnswer: 'x', isCorrect: false },
      { userAnswer: 'y', isCorrect: false }
    ];
    const result = calculateResult(questions);
    expect(result.correctCount).toBe(0);
    expect(result.score).toBe(0);
  });

  test('部分答对计算正确比例', () => {
    const questions = [
      { userAnswer: 'iː', isCorrect: true },
      { userAnswer: 'x', isCorrect: false },
      { userAnswer: 'p', isCorrect: true },
      { userAnswer: 'y', isCorrect: false }
    ];
    const result = calculateResult(questions);
    expect(result.correctCount).toBe(2);
    expect(result.score).toBe(50);
  });

  test('空数组返回 0 分', () => {
    const result = calculateResult([]);
    expect(result.totalQuestions).toBe(0);
    expect(result.score).toBe(0);
  });

  test('非数组输入返回 0 分', () => {
    expect(calculateResult(null).score).toBe(0);
    expect(calculateResult(undefined).score).toBe(0);
  });

  test('包含未回答的题目', () => {
    const questions = [
      { userAnswer: 'iː', isCorrect: true },
      { userAnswer: null, isCorrect: null },
      { userAnswer: 'p', isCorrect: true }
    ];
    const result = calculateResult(questions);
    expect(result.totalQuestions).toBe(3);
    expect(result.answeredCount).toBe(2);
    expect(result.correctCount).toBe(2);
    // 得分基于总题数
    expect(result.score).toBe(67);
  });
});

describe('考试记录存储', () => {

  beforeEach(() => {
    jest.clearAllMocks();
    global.wx.getStorageSync.mockReturnValue([]);
  });

  test('saveExamResult 应调用 wx.setStorageSync', () => {
    const result = { totalQuestions: 10, correctCount: 8, score: 80 };
    saveExamResult(result);
    expect(global.wx.setStorageSync).toHaveBeenCalledWith('exam_history', expect.any(Array));
  });

  test('saveExamResult 添加的记录应包含日期', () => {
    const result = { totalQuestions: 10, correctCount: 8, score: 80 };
    saveExamResult(result);

    const savedHistory = global.wx.setStorageSync.mock.calls[0][1];
    expect(savedHistory[0].date).toBeTruthy();
    expect(savedHistory[0].score).toBe(80);
  });

  test('getExamHistory 应调用 wx.getStorageSync', () => {
    global.wx.getStorageSync.mockReturnValue([{ score: 90 }]);
    const history = getExamHistory();
    expect(history).toEqual([{ score: 90 }]);
  });

  test('getExamHistory 存储为空时返回空数组', () => {
    global.wx.getStorageSync.mockReturnValue(null);
    const history = getExamHistory();
    expect(history).toEqual([]);
  });

  test('历史记录最多保留 50 条', () => {
    const existingHistory = Array.from({ length: 50 }, (_, i) => ({ score: i }));
    global.wx.getStorageSync.mockReturnValue(existingHistory);

    saveExamResult({ totalQuestions: 10, correctCount: 5, score: 50 });

    const savedHistory = global.wx.setStorageSync.mock.calls[0][1];
    expect(savedHistory.length).toBeLessThanOrEqual(50);
  });
});
