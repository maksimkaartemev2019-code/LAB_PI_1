using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LAB_PI_1.Models;
using Microsoft.Win32;

namespace LAB_PI_1;

/// <summary>
/// Provides code-behind logic for the habit manager WPF window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Stores habits displayed in data grids.
    /// </summary>
    private readonly ObservableCollection<HabitEntry> habits = [];

    /// <summary>
    /// Stores daily statistic rows displayed in list views.
    /// </summary>
    private readonly ObservableCollection<DailyStatistic> statistics = [];

    /// <summary>
    /// Stores the latest saved user profile.
    /// </summary>
    private UserProfile savedProfile = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SeedDemoData();
    }

    /// <summary>
    /// Initializes date-dependent controls after the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        HabitCalendar.SelectedDate = DateTime.Today;
        HabitCalendar.DisplayDate = DateTime.Today;
        UpdateProgress();
        SetStatus("Приложение запущено");
    }

    /// <summary>
    /// Adds starter habits and statistics for the first application run.
    /// </summary>
    private void SeedDemoData()
    {
        habits.Add(new HabitEntry { Name = "Выпить воду", Time = "09:00", IsCompleted = true });
        habits.Add(new HabitEntry { Name = "Прочитать 20 страниц", Time = "20:00", IsCompleted = false });
        habits.Add(new HabitEntry { Name = "Прогулка", Time = "18:30", IsCompleted = true });

        statistics.Add(new DailyStatistic { Day = "Понедельник", Completed = 5, Total = 7 });
        statistics.Add(new DailyStatistic { Day = "Вторник", Completed = 4, Total = 7 });
        statistics.Add(new DailyStatistic { Day = "Среда", Completed = 6, Total = 7 });
        statistics.Add(new DailyStatistic { Day = "Сегодня", Completed = habits.Count(item => item.IsCompleted), Total = habits.Count });

        HabitsDataGrid.ItemsSource = habits;
        StatisticsListView.ItemsSource = statistics;
    }

    /// <summary>
    /// Handles the main save button click.
    /// </summary>
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile();
    }

    /// <summary>
    /// Handles save commands from the menu and toolbar.
    /// </summary>
    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile();
    }

    /// <summary>
    /// Simulates loading previously saved profile data.
    /// </summary>
    private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
    {
        FirstNameTextBox.Text = savedProfile.FirstName;
        LastNameTextBox.Text = savedProfile.LastName;
        PasswordInputBox.Password = savedProfile.Password;
        BirthDatePicker.SelectedDate = savedProfile.BirthDate;
        SetComboBoxValue(EducationComboBox, savedProfile.Education);
        NotificationsCheckBox.IsChecked = savedProfile.ReceiveNotifications;
        PublicStatsCheckBox.IsChecked = savedProfile.ShowPublicStatistics;
        AutoSaveCheckBox.IsChecked = savedProfile.AutoSave;
        WeeklyReportCheckBox.IsChecked = savedProfile.WeeklyReport;
        SelectActivity(savedProfile.ActivityLevel);
        SetStatus("Данные загружены из объекта UserProfile");
    }

    /// <summary>
    /// Closes the application from the menu.
    /// </summary>
    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Copies selected text from the focused text box if possible.
    /// </summary>
    private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (Keyboard.FocusedElement is TextBox textBox)
        {
            textBox.Copy();
            SetStatus("Текст скопирован");
        }
    }

    /// <summary>
    /// Pastes clipboard text into the focused text box if possible.
    /// </summary>
    private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (Keyboard.FocusedElement is TextBox textBox)
        {
            textBox.Paste();
            SetStatus("Текст вставлен");
        }
    }

    /// <summary>
    /// Applies a light visual theme to the window.
    /// </summary>
    private void LightThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        RootPanel.Background = new SolidColorBrush(Color.FromRgb(244, 246, 250));
        Foreground = Brushes.Black;
        SetStatus("Включена светлая тема");
    }

    /// <summary>
    /// Applies a dark visual theme to the window.
    /// </summary>
    private void DarkThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        RootPanel.Background = new SolidColorBrush(Color.FromRgb(38, 45, 56));
        Foreground = Brushes.White;
        SetStatus("Включена темная тема");
    }

    /// <summary>
    /// Clears all profile input fields.
    /// </summary>
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        FirstNameTextBox.Clear();
        LastNameTextBox.Clear();
        PasswordInputBox.Clear();
        BirthDatePicker.SelectedDate = null;
        EducationComboBox.SelectedIndex = -1;
        HobbiesListBox.SelectedItems.Clear();
        NotificationsCheckBox.IsChecked = false;
        PublicStatsCheckBox.IsChecked = false;
        AutoSaveCheckBox.IsChecked = false;
        WeeklyReportCheckBox.IsChecked = false;
        MediumActivityRadioButton.IsChecked = true;
        ProfileSummaryTextBlock.Text = "Профиль не сохранен";
        SetStatus("Поля очищены");
    }

    /// <summary>
    /// Adds a new habit from the input field to the habit table.
    /// </summary>
    private void AddHabitButton_Click(object sender, RoutedEventArgs e)
    {
        var habitName = NewHabitTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(habitName))
        {
            MessageBox.Show("Введите название привычки.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        habits.Add(new HabitEntry { Name = habitName, Time = DateTime.Now.ToString("HH:mm"), IsCompleted = false });
        NewHabitTextBox.Clear();
        RefreshTodayStatistic();
        SetStatus($"Добавлена привычка: {habitName}");
    }

    /// <summary>
    /// Updates progress after productivity or satisfaction slider changes.
    /// </summary>
    private void HabitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateProgress();
    }

    /// <summary>
    /// Decreases both sliders by five points.
    /// </summary>
    private void DecreaseProgressButton_Click(object sender, RoutedEventArgs e)
    {
        ProductivitySlider.Value = Math.Max(ProductivitySlider.Minimum, ProductivitySlider.Value - 5);
        SatisfactionSlider.Value = Math.Max(SatisfactionSlider.Minimum, SatisfactionSlider.Value - 5);
    }

    /// <summary>
    /// Increases both sliders by five points.
    /// </summary>
    private void IncreaseProgressButton_Click(object sender, RoutedEventArgs e)
    {
        ProductivitySlider.Value = Math.Min(ProductivitySlider.Maximum, ProductivitySlider.Value + 5);
        SatisfactionSlider.Value = Math.Min(SatisfactionSlider.Maximum, SatisfactionSlider.Value + 5);
    }

    /// <summary>
    /// Updates the status bar when a calendar date is selected.
    /// </summary>
    private void HabitCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HabitCalendar.SelectedDate is DateTime selectedDate)
        {
            SetStatus($"Выбран день: {selectedDate:dd.MM.yyyy}");
        }
    }

    /// <summary>
    /// Refreshes statistics after a habit checkbox value changes.
    /// </summary>
    private void HabitsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        Dispatcher.BeginInvoke(RefreshTodayStatistic);
    }

    /// <summary>
    /// Updates the status bar when another tab is selected.
    /// </summary>
    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainTabControl.SelectedItem is TabItem tabItem)
        {
            SetStatus($"Открыта вкладка: {tabItem.Header}");
        }
    }

    /// <summary>
    /// Loads an avatar image through a standard file dialog.
    /// </summary>
    private void LoadAvatarButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Выберите аватар",
            Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp|Все файлы|*.*"
        };

        if (dialog.ShowDialog(this) == true)
        {
            AvatarImage.Source = new BitmapImage(new Uri(dialog.FileName));
            AvatarPlaceholderTextBlock.Visibility = Visibility.Collapsed;
            SetStatus("Аватар загружен");
        }
    }

    /// <summary>
    /// Shows or hides the additional settings group.
    /// </summary>
    private void EditModeToggleButton_Changed(object sender, RoutedEventArgs e)
    {
        DisplaySettingsGroupBox.Visibility = EditModeToggleButton.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
        SetStatus(EditModeToggleButton.IsChecked == true ? "Режим редактирования включен" : "Дополнительная панель скрыта");
    }

    /// <summary>
    /// Validates profile fields and saves them into a UserProfile object.
    /// </summary>
    private void SaveProfile()
    {
        if (!ValidateProfile())
        {
            return;
        }

        savedProfile = new UserProfile
        {
            FirstName = FirstNameTextBox.Text.Trim(),
            LastName = LastNameTextBox.Text.Trim(),
            Password = PasswordInputBox.Password,
            BirthDate = BirthDatePicker.SelectedDate,
            Education = (EducationComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
            ReceiveNotifications = NotificationsCheckBox.IsChecked == true,
            ShowPublicStatistics = PublicStatsCheckBox.IsChecked == true,
            AutoSave = AutoSaveCheckBox.IsChecked == true,
            WeeklyReport = WeeklyReportCheckBox.IsChecked == true,
            ActivityLevel = GetSelectedActivity()
        };

        foreach (ListBoxItem item in HobbiesListBox.SelectedItems)
        {
            if (item.Content is string hobby)
            {
                savedProfile.Hobbies.Add(hobby);
            }
        }

        ProfileSummaryTextBlock.Text = $"{savedProfile.FirstName} {savedProfile.LastName}, {savedProfile.ActivityLevel}";
        SetStatus("Профиль сохранен в объект UserProfile");
    }

    /// <summary>
    /// Checks required profile fields and date boundaries.
    /// </summary>
    private bool ValidateProfile()
    {
        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Имя и фамилия не должны быть пустыми.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (BirthDatePicker.SelectedDate is DateTime birthDate && birthDate.Date > DateTime.Today)
        {
            MessageBox.Show("Дата рождения не может быть в будущем.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Updates the progress bar using average slider values.
    /// </summary>
    private void UpdateProgress()
    {
        if (DayProgressBar is null || ProgressValueTextBlock is null)
        {
            return;
        }

        var progress = (ProductivitySlider.Value + SatisfactionSlider.Value) / 2;
        DayProgressBar.Value = progress;
        ProgressValueTextBlock.Text = $"{progress:0}%";
    }

    /// <summary>
    /// Recalculates the row that represents today's progress.
    /// </summary>
    private void RefreshTodayStatistic()
    {
        var today = statistics.FirstOrDefault(item => item.Day == "Сегодня");
        if (today is null)
        {
            return;
        }

        var index = statistics.IndexOf(today);
        statistics[index] = new DailyStatistic
        {
            Day = "Сегодня",
            Completed = habits.Count(item => item.IsCompleted),
            Total = habits.Count
        };
    }

    /// <summary>
    /// Sets the current status bar message.
    /// </summary>
    private void SetStatus(string message)
    {
        StatusTextBlock.Text = message;
    }

    /// <summary>
    /// Returns the selected activity level text.
    /// </summary>
    private string GetSelectedActivity()
    {
        if (LowActivityRadioButton.IsChecked == true)
        {
            return "Низкий";
        }

        return HighActivityRadioButton.IsChecked == true ? "Высокий" : "Средний";
    }

    /// <summary>
    /// Selects an activity radio button by level text.
    /// </summary>
    private void SelectActivity(string activityLevel)
    {
        LowActivityRadioButton.IsChecked = activityLevel == "Низкий";
        MediumActivityRadioButton.IsChecked = activityLevel == "Средний";
        HighActivityRadioButton.IsChecked = activityLevel == "Высокий";
    }

    /// <summary>
    /// Selects a combo box item by displayed value.
    /// </summary>
    private static void SetComboBoxValue(ComboBox comboBox, string value)
    {
        foreach (ComboBoxItem item in comboBox.Items)
        {
            item.IsSelected = item.Content?.ToString() == value;
        }
    }
}
