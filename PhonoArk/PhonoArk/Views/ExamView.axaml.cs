using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Avalonia.Layout;
using System;

namespace PhonoArk.Views;

public partial class ExamView : UserControl
{
    private const double TabletBreakpoint = 900;

    public ExamView()
    {
        InitializeComponent();

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            Classes.Add("mobile");
        }
        else
        {
            Classes.Add("desktop");
        }

        AttachedToVisualTree += (_, _) => ApplyPhoneLandscapeLayoutIfNeeded();
        SizeChanged += (_, _) => ApplyPhoneLandscapeLayoutIfNeeded();
    }

    private void ApplyPhoneLandscapeLayoutIfNeeded()
    {
        if (!Classes.Contains("mobile"))
        {
            SetupTitleText.IsVisible = true;
            SetupBorder.VerticalAlignment = VerticalAlignment.Center;
            SetupBorder.HorizontalAlignment = HorizontalAlignment.Center;
            SetupBorder.MaxWidth = 760;
            SetupBorder.MinHeight = 440;
            SetupBorder.Padding = new Thickness(20);
            ExamRootGrid.Margin = new Thickness(20);

            SetupFieldsGrid.ColumnDefinitions = new ColumnDefinitions("*");
            SetupFieldsGrid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto");
            SetupFieldsGrid.RowSpacing = 12;
            SetupFieldsGrid.ColumnSpacing = 12;

            QuestionCountCard.Width = 360;
            ExamScopeCard.Width = 360;
            QuestionCountCard.Margin = new Thickness(0);
            ExamScopeCard.Margin = new Thickness(0);

            Grid.SetColumn(QuestionCountCard, 0);
            Grid.SetRow(QuestionCountCard, 0);

            Grid.SetColumn(ExamScopeCard, 0);
            Grid.SetRow(ExamScopeCard, 1);

            Grid.SetColumn(StartExamButton, 0);
            Grid.SetRow(StartExamButton, 2);
            Grid.SetRowSpan(StartExamButton, 1);
            StartExamButton.Height = double.NaN;
            StartExamButton.MinHeight = 0;
            StartExamButton.Margin = new Thickness(0);
            StartExamButton.HorizontalAlignment = HorizontalAlignment.Center;
            StartExamButton.VerticalAlignment = VerticalAlignment.Center;

            ActiveExamGrid.ColumnDefinitions = new ColumnDefinitions("*");
            ActiveExamGrid.RowDefinitions = new RowDefinitions("Auto,*");
            ActiveExamGrid.ColumnSpacing = 0;
            ActiveExamGrid.RowSpacing = 12;

            LandscapeQuestionInfoCard.IsVisible = false;
            LandscapeEndExamCard.IsVisible = false;
            PortraitQuestionInfoPanel.IsVisible = true;
            PortraitEndExamButton.IsVisible = true;

            Grid.SetRow(ExamLeftScroll, 0);
            Grid.SetColumn(ExamLeftScroll, 0);
            ExamLeftScroll.Margin = new Thickness(0);

            Grid.SetRow(ExamAnswerScroll, 1);
            Grid.SetColumn(ExamAnswerScroll, 0);
            ExamAnswerScroll.Margin = new Thickness(0);
            ExamAnswerScroll.VerticalAlignment = VerticalAlignment.Center;

            ExamAnswerContentPanel.Width = double.NaN;
            ExamAnswerContentPanel.MinWidth = 600;
            ExamAnswerContentPanel.MaxWidth = 600;
            ExamAnswerContentPanel.HorizontalAlignment = HorizontalAlignment.Center;

            FeedbackBorder.Width = 560;
            FeedbackBorder.Margin = new Thickness(0, 6, 0, 0);

            return;
        }

        var isPhoneLandscape = Bounds.Width > Bounds.Height && Bounds.Width < TabletBreakpoint;

        if (isPhoneLandscape)
        {
            if (!Classes.Contains("phone-landscape"))
                Classes.Add("phone-landscape");

            // --- Setup section landscape ---
            SetupTitleText.IsVisible = false;
            SetupBorder.VerticalAlignment = VerticalAlignment.Stretch;
            SetupBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            SetupBorder.MaxWidth = double.PositiveInfinity;
            SetupBorder.MinHeight = Math.Max(360, Bounds.Height * 0.78);
            SetupBorder.Padding = new Thickness(16, 10);
            ExamRootGrid.Margin = new Thickness(12);
            SetupFieldsGrid.ColumnDefinitions = new ColumnDefinitions("1.1*,0.9*");
            SetupFieldsGrid.RowDefinitions = new RowDefinitions("Auto,Auto");
            SetupFieldsGrid.RowSpacing = 12;
            SetupFieldsGrid.ColumnSpacing = 14;
            QuestionCountCard.Width = double.NaN;
            ExamScopeCard.Width = double.NaN;

            QuestionCountCard.Margin = new Thickness(0, 0, 0, 2);
            ExamScopeCard.Margin = new Thickness(0, 2, 0, 0);
            StartExamButton.Margin = new Thickness(6, 0, 0, 0);

            Grid.SetColumn(QuestionCountCard, 0);
            Grid.SetRow(QuestionCountCard, 0);

            Grid.SetColumn(ExamScopeCard, 0);
            Grid.SetRow(ExamScopeCard, 1);

            Grid.SetColumn(StartExamButton, 1);
            Grid.SetRow(StartExamButton, 0);
            Grid.SetRowSpan(StartExamButton, 2);
            StartExamButton.Height = 56;
            StartExamButton.MinHeight = 44;
            StartExamButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            StartExamButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

            // --- Active exam landscape ---
            ActiveExamGrid.Margin = new Thickness(0);
            ActiveExamGrid.ColumnDefinitions = new ColumnDefinitions("0.7*,1.3*");
            ActiveExamGrid.RowDefinitions = new RowDefinitions("*");
            ActiveExamGrid.ColumnSpacing = 12;
            ActiveExamGrid.RowSpacing = 0;

            LandscapeQuestionInfoCard.IsVisible = true;
            LandscapeEndExamCard.IsVisible = true;
            PortraitQuestionInfoPanel.IsVisible = false;
            PortraitEndExamButton.IsVisible = false;

            Grid.SetRow(ExamLeftScroll, 0);
            Grid.SetColumn(ExamLeftScroll, 0);
            ExamLeftScroll.Margin = new Thickness(0, 0, 6, 0);

            Grid.SetRow(ExamAnswerScroll, 0);
            Grid.SetColumn(ExamAnswerScroll, 1);
            ExamAnswerScroll.Margin = new Thickness(6, 0, 0, 0);
            ExamAnswerScroll.VerticalAlignment = VerticalAlignment.Stretch;
            ExamAnswerScroll.HorizontalAlignment = HorizontalAlignment.Stretch;
            ExamAnswerContentPanel.Width = double.NaN;
            ExamAnswerContentPanel.MinWidth = 0;
            ExamAnswerContentPanel.MaxWidth = double.PositiveInfinity;
            ExamAnswerContentPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            FeedbackBorder.Width = double.NaN;
            FeedbackBorder.Margin = new Thickness(20, 6, 20, 0);

            // --- Compact fonts for landscape ---
            QuestionLabelText.FontSize = 15;
            QuestionNumberText.FontSize = 15;
            OfLabelText.FontSize = 12;
            TotalQuestionsText.FontSize = 12;
            CorrectLabelText.FontSize = 14;
            CorrectCountText.FontSize = 14;
            PlayPhonemeBtn.FontSize = 17;
            TargetPhonemeLabelText.FontSize = 13;
            TargetPhonemeSymbolText.FontSize = 32;
            LandscapeTargetPhonemeLabelText.FontSize = 13;
            LandscapeTargetPhonemeSymbolText.FontSize = 34;
            SelectWordText.FontSize = 14;
            FeedbackText.FontSize = 14;
        }
        else
        {
            Classes.Remove("phone-landscape");

            // --- Setup section portrait ---
            SetupTitleText.IsVisible = true;
            SetupBorder.VerticalAlignment = VerticalAlignment.Stretch;
            SetupBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            SetupBorder.MaxWidth = double.PositiveInfinity;
            SetupBorder.MinHeight = Math.Max(520, Bounds.Height * 0.72);
            SetupBorder.Padding = new Thickness(20);
            ExamRootGrid.Margin = new Thickness(20);
            SetupFieldsGrid.ColumnDefinitions = new ColumnDefinitions("*");
            SetupFieldsGrid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto");
            SetupFieldsGrid.RowSpacing = 12;
            QuestionCountCard.Width = double.NaN;
            ExamScopeCard.Width = double.NaN;

            Grid.SetColumn(QuestionCountCard, 0);
            Grid.SetRow(QuestionCountCard, 0);

            Grid.SetColumn(ExamScopeCard, 0);
            Grid.SetRow(ExamScopeCard, 1);

            Grid.SetColumn(StartExamButton, 0);
            Grid.SetRow(StartExamButton, 2);
            Grid.SetRowSpan(StartExamButton, 1);
            StartExamButton.Height = double.NaN;
            StartExamButton.MinHeight = 0;
            StartExamButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            StartExamButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

            // --- Active exam portrait ---
            ActiveExamGrid.Margin = new Thickness(0);
            ActiveExamGrid.ColumnDefinitions = new ColumnDefinitions("*");
            ActiveExamGrid.RowDefinitions = new RowDefinitions("Auto,*");
            ActiveExamGrid.ColumnSpacing = 0;
            ActiveExamGrid.RowSpacing = 12;

            LandscapeQuestionInfoCard.IsVisible = false;
            LandscapeEndExamCard.IsVisible = false;
            PortraitQuestionInfoPanel.IsVisible = true;
            PortraitEndExamButton.IsVisible = true;

            Grid.SetRow(ExamLeftScroll, 0);
            Grid.SetColumn(ExamLeftScroll, 0);
            ExamLeftScroll.Margin = new Thickness(0);

            Grid.SetRow(ExamAnswerScroll, 1);
            Grid.SetColumn(ExamAnswerScroll, 0);
            ExamAnswerScroll.Margin = new Thickness(0);
            ExamAnswerScroll.VerticalAlignment = VerticalAlignment.Stretch;
            ExamAnswerScroll.HorizontalAlignment = HorizontalAlignment.Stretch;
            ExamAnswerContentPanel.Width = double.NaN;
            ExamAnswerContentPanel.MinWidth = 0;
            ExamAnswerContentPanel.MaxWidth = double.PositiveInfinity;
            ExamAnswerContentPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            FeedbackBorder.Width = double.NaN;
            FeedbackBorder.Margin = new Thickness(20, 6, 20, 0);

            QuestionCountCard.Margin = new Thickness(0);
            ExamScopeCard.Margin = new Thickness(0);
            StartExamButton.Margin = new Thickness(0);

            // --- Restore fonts for portrait ---
            QuestionLabelText.FontSize = 18;
            QuestionNumberText.FontSize = 18;
            OfLabelText.FontSize = 14;
            TotalQuestionsText.FontSize = 14;
            CorrectLabelText.FontSize = 16;
            CorrectCountText.FontSize = 16;
            PlayPhonemeBtn.FontSize = 20;
            TargetPhonemeLabelText.FontSize = 14;
            TargetPhonemeSymbolText.FontSize = 44;
            LandscapeTargetPhonemeLabelText.FontSize = 13;
            LandscapeTargetPhonemeSymbolText.FontSize = 34;
            SelectWordText.FontSize = 16;
            FeedbackText.FontSize = 16;
        }
    }
}
