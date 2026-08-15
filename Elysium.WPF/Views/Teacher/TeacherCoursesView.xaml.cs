using System.Windows;
using System.Windows.Controls;
using Elysium.WPF.Models.Courses;
using Elysium.WPF.Presenters;
using ValidationError = Elysium.WPF.Models.ValidationError;

namespace Elysium.WPF.Views.Teacher;

/// <summary>
/// Interaction logic for TeacherCoursesView
/// </summary>
public partial class TeacherCoursesView : UserControl
{
    private TeacherCoursesPresenter? _presenter;

    /// <summary>
    /// Raised when the user requests to go back to the dashboard
    /// </summary>
    public event EventHandler? BackRequested;

    public TeacherCoursesView()
    {
        InitializeComponent();
        BackButton.Click += BackButton_Click;
        CreateButton.Click += CreateButton_Click;
        CreateAnotherButton.Click += CreateAnotherButton_Click;
    }

    /// <summary>
    /// Wire the shared presenter; safe to call more than once
    /// </summary>
    public void Initialize(TeacherCoursesPresenter presenter)
    {
        if (_presenter is not null)
            return;

        _presenter = presenter;

        _presenter.CourseCreated += Presenter_CourseCreated;
        _presenter.CourseCreateFailed += Presenter_CourseCreateFailed;
        _presenter.ValidationErrorsChanged += Presenter_ValidationErrorsChanged;
    }

    /// <summary>
    /// Reset the form back to its initial state
    /// </summary>
    public void Reset()
    {
        NameInput.Text = string.Empty;
        DescriptionInput.Text = string.Empty;
        NameInput.IsEnabled = true;
        DescriptionInput.IsEnabled = true;
        NameError.Text = string.Empty;
        DescriptionError.Text = string.Empty;
        GeneralError.Text = string.Empty;
        SuccessText.Visibility = Visibility.Collapsed;
        CourseCodeText.Visibility = Visibility.Collapsed;
        CreateButton.Visibility = Visibility.Visible;
        CreateAnotherButton.Visibility = Visibility.Collapsed;
        CreateLoadingPanel.Visibility = Visibility.Collapsed;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    #region Presenter events

    private void Presenter_CourseCreated(object? sender, CreateCourseResponse response)
    {
        CreateLoadingPanel.Visibility = Visibility.Collapsed;
        GeneralError.Text = string.Empty;

        CreateButton.Visibility = Visibility.Collapsed;
        NameInput.IsEnabled = false;
        DescriptionInput.IsEnabled = false;

        SuccessText.Visibility = Visibility.Visible;
        CourseCodeText.Text = $"Course Code: {response.Code}";
        CourseCodeText.Visibility = Visibility.Visible;
        CreateAnotherButton.Visibility = Visibility.Visible;
    }

    private void Presenter_CourseCreateFailed(object? sender, string message)
    {
        CreateLoadingPanel.Visibility = Visibility.Collapsed;
        SuccessText.Visibility = Visibility.Collapsed;
        CourseCodeText.Visibility = Visibility.Collapsed;
        GeneralError.Text = message;
    }

    private void Presenter_ValidationErrorsChanged(object? sender, List<ValidationError> errors)
    {
        NameError.Text = string.Empty;
        DescriptionError.Text = string.Empty;
        GeneralError.Text = string.Empty;

        foreach (var error in errors)
        {
            switch (error.Field.ToLower())
            {
                case "name":
                    NameError.Text = error.Message;
                    break;
                case "description":
                    DescriptionError.Text = error.Message;
                    break;
            }
        }
    }

    #endregion

    #region Actions

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        if (_presenter is null)
            return;

        CreateButton.IsEnabled = false;
        CreateLoadingPanel.Visibility = Visibility.Visible;
        GeneralError.Text = string.Empty;

        try
        {
            await _presenter.HandleCreateCourseAsync(NameInput.Text, DescriptionInput.Text);
        }
        finally
        {
            CreateButton.IsEnabled = true;
            CreateLoadingPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void CreateAnotherButton_Click(object sender, RoutedEventArgs e)
    {
        Reset();
    }

    #endregion
}