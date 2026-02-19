using System;
using System.Collections.Generic;
using PhonoArk.Mobile.Core.Models;
using PhonoArk.Mobile.Core.Services;

namespace PhonoArk.Mobile.Core.ViewModels;

/// <summary>
/// 考试 ViewModel
/// </summary>
public class ExamViewModel : BaseViewModel
{
    private readonly IExamService _examService;
    private List<ExamQuestion> _questions = new();
    private List<AnswerRecord> _records = new();

    private int _currentIndex;
    private ExamQuestion? _currentQuestion;
    private int? _selectedOptionIndex;
    private bool _hasAnswered;
    private string _feedbackMessage = string.Empty;

    /// <summary>请求播放单词发音</summary>
    public event Action<string>? PlayWordRequested;

    /// <summary>考试完成，携带结果</summary>
    public event Action<ExamResult>? ExamCompleted;

    public ExamViewModel(IExamService examService)
    {
        _examService = examService ?? throw new ArgumentNullException(nameof(examService));
    }

    /// <summary>当前题目</summary>
    public ExamQuestion? CurrentQuestion
    {
        get => _currentQuestion;
        private set => SetProperty(ref _currentQuestion, value);
    }

    /// <summary>当前题号（从 1 开始）</summary>
    public int CurrentNumber => _currentIndex + 1;

    /// <summary>总题数</summary>
    public int TotalQuestions => _questions.Count;

    /// <summary>用户选中的选项索引</summary>
    public int? SelectedOptionIndex
    {
        get => _selectedOptionIndex;
        private set => SetProperty(ref _selectedOptionIndex, value);
    }

    /// <summary>是否已作答当前题</summary>
    public bool HasAnswered
    {
        get => _hasAnswered;
        private set => SetProperty(ref _hasAnswered, value);
    }

    /// <summary>反馈消息</summary>
    public string FeedbackMessage
    {
        get => _feedbackMessage;
        private set => SetProperty(ref _feedbackMessage, value);
    }

    /// <summary>进度百分比 (0-100)</summary>
    public double Progress =>
        TotalQuestions > 0 ? (double)_currentIndex / TotalQuestions * 100 : 0;

    /// <summary>是否为最后一题</summary>
    public bool IsLastQuestion => _currentIndex >= _questions.Count - 1;

    /// <summary>开始考试</summary>
    public void StartExam(int questionCount = 10)
    {
        _questions = _examService.GenerateExam(questionCount);
        _records = new List<AnswerRecord>();
        _currentIndex = 0;
        LoadCurrentQuestion();
    }

    /// <summary>提交答案</summary>
    public void SubmitAnswer(int optionIndex)
    {
        if (HasAnswered || CurrentQuestion == null)
            return;

        if (optionIndex < 0 || optionIndex >= CurrentQuestion.Options.Count)
            return;

        SelectedOptionIndex = optionIndex;
        HasAnswered = true;

        bool isCorrect = _examService.CheckAnswer(CurrentQuestion, optionIndex);
        FeedbackMessage = isCorrect ? "✓ 回答正确！" : $"✗ 回答错误，正确答案是：{CurrentQuestion.Options[CurrentQuestion.CorrectIndex]}";

        _records.Add(new AnswerRecord
        {
            Question = CurrentQuestion,
            SelectedIndex = optionIndex
        });

        OnPropertyChanged(nameof(Progress));
    }

    /// <summary>前进到下一题或完成考试</summary>
    public void Next()
    {
        if (!HasAnswered)
            return;

        if (IsLastQuestion)
        {
            var result = _examService.CalculateResult(_records);
            ExamCompleted?.Invoke(result);
        }
        else
        {
            _currentIndex++;
            LoadCurrentQuestion();
        }
    }

    /// <summary>播放当前题目相关的单词发音</summary>
    public void PlayCurrentWord()
    {
        if (CurrentQuestion?.Prompt != null)
        {
            // 从 Prompt 中提取单词
            var prompt = CurrentQuestion.Prompt;
            if (CurrentQuestion.QuestionType == ExamQuestionType.ChoosePhonemeForWord)
            {
                var start = prompt.IndexOf('"') + 1;
                var end = prompt.IndexOf('"', start);
                if (start > 0 && end > start)
                {
                    var word = prompt.Substring(start, end - start);
                    PlayWordRequested?.Invoke(word);
                }
            }
        }
    }

    private void LoadCurrentQuestion()
    {
        if (_currentIndex < _questions.Count)
        {
            CurrentQuestion = _questions[_currentIndex];
            SelectedOptionIndex = null;
            HasAnswered = false;
            FeedbackMessage = string.Empty;
            OnPropertyChanged(nameof(CurrentNumber));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(IsLastQuestion));
        }
    }
}
