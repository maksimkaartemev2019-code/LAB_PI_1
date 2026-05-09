using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LAB_PI_1.Models;
using Microsoft.Win32;

namespace LAB_PI_1;

// Класс содержит логику главного окна менеджера привычек.
public partial class MainWindow : Window
{
    // Коллекция привычек для таблиц.
    private readonly ObservableCollection<HabitEntry> habits = [];

    // Коллекция строк статистики по дням.
    private readonly ObservableCollection<DailyStatistic> statistics = [];

    // Последний сохраненный профиль пользователя.
    private UserProfile savedProfile = new();

    // Создает главное окно и заполняет демонстрационные данные.
    public MainWindow()
    {
        InitializeComponent();
        SeedDemoData();
    }

    // Настраивает элементы, зависящие от даты, после загрузки окна.
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        HabitCalendar.SelectedDate = DateTime.Today;
        HabitCalendar.DisplayDate = DateTime.Today;
        ApplyDisplaySettings(false);
        UpdateProgress();
        SetStatus("Приложение запущено");
    }

    // Добавляет стартовые привычки и статистику.
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

    // Обрабатывает нажатие основной кнопки сохранения.
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile();
    }

    // Обрабатывает команды сохранения из меню и панели инструментов.
    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile();
    }

    // Загружает данные из сохраненного объекта профиля обратно в поля.
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
        ApplyDisplaySettings(false);
        SetStatus("Данные загружены из объекта UserProfile");
    }

    // Закрывает приложение через меню.
    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // Копирует выделенный текст из активного текстового поля.
    private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (Keyboard.FocusedElement is TextBox textBox)
        {
            textBox.Copy();
            SetStatus("Текст скопирован");
        }
    }

    // Вставляет текст из буфера обмена в активное текстовое поле.
    private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (Keyboard.FocusedElement is TextBox textBox)
        {
            textBox.Paste();
            SetStatus("Текст вставлен");
        }
    }

    // Включает светлую тему окна.
    private void LightThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        RootPanel.Background = new SolidColorBrush(Color.FromRgb(244, 246, 250));
        Foreground = Brushes.Black;
        SetStatus("Включена светлая тема");
    }

    // Включает темную тему окна.
    private void DarkThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        RootPanel.Background = new SolidColorBrush(Color.FromRgb(38, 45, 56));
        Foreground = Brushes.White;
        SetStatus("Включена темная тема");
    }

    // Очищает все поля личных данных.
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
        ApplyDisplaySettings(false);
        SetStatus("Поля очищены");
    }

    // Добавляет новую привычку из поля ввода.
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

    // Удаляет выбранную привычку из таблицы.
    private void DeleteHabitButton_Click(object sender, RoutedEventArgs e)
    {
        if (HabitsDataGrid.SelectedItem is not HabitEntry selectedHabit)
        {
            MessageBox.Show("Выберите привычку в таблице для удаления.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        habits.Remove(selectedHabit);
        RefreshTodayStatistic();
        SetStatus($"Удалена привычка: {selectedHabit.Name}");
    }

    // Обновляет прогресс при изменении слайдеров.
    private void HabitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateProgress();
    }

    // Уменьшает оба слайдера на пять пунктов.
    private void DecreaseProgressButton_Click(object sender, RoutedEventArgs e)
    {
        ProductivitySlider.Value = Math.Max(ProductivitySlider.Minimum, ProductivitySlider.Value - 5);
        SatisfactionSlider.Value = Math.Max(SatisfactionSlider.Minimum, SatisfactionSlider.Value - 5);
    }

    // Увеличивает оба слайдера на пять пунктов.
    private void IncreaseProgressButton_Click(object sender, RoutedEventArgs e)
    {
        ProductivitySlider.Value = Math.Min(ProductivitySlider.Maximum, ProductivitySlider.Value + 5);
        SatisfactionSlider.Value = Math.Min(SatisfactionSlider.Maximum, SatisfactionSlider.Value + 5);
    }

    // Обновляет строку состояния при выборе даты в календаре.
    private void HabitCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HabitCalendar.SelectedDate is DateTime selectedDate)
        {
            SetStatus($"Выбран день: {selectedDate:dd.MM.yyyy}");
        }
    }

    // Пересчитывает статистику после редактирования таблицы привычек.
    private void HabitsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        Dispatcher.BeginInvoke(RefreshTodayStatistic);
    }

    // Обновляет строку состояния при переключении вкладок.
    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainTabControl.SelectedItem is TabItem tabItem)
        {
            SetStatus($"Открыта вкладка: {tabItem.Header}");
        }
    }

    // Загружает аватар через стандартное окно выбора файла.
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

    // Показывает или скрывает блок настроек отображения.
    private void EditModeToggleButton_Changed(object sender, RoutedEventArgs e)
    {
        if (DisplaySettingsGroupBox is null || EditModeToggleButton is null)
        {
            return;
        }

        DisplaySettingsGroupBox.Visibility = EditModeToggleButton.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
        SetStatus(EditModeToggleButton.IsChecked == true ? "Режим редактирования включен" : "Режим редактирования выключен");
    }

    // Применяет настройки отображения при изменении флажков.
    private void DisplaySetting_Changed(object sender, RoutedEventArgs e)
    {
        ApplyDisplaySettings(true);
    }

    // Проверяет и сохраняет данные профиля в объект UserProfile.
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

    // Проверяет обязательные поля профиля.
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

    // Обновляет ProgressBar по среднему значению двух слайдеров.
    private void UpdateProgress()
    {
        if (ProductivitySlider is null || SatisfactionSlider is null || DayProgressBar is null || ProgressValueTextBlock is null)
        {
            return;
        }

        var progress = (ProductivitySlider.Value + SatisfactionSlider.Value) / 2;
        DayProgressBar.Value = progress;
        ProgressValueTextBlock.Text = $"{progress:0}%";
    }

    // Пересчитывает строку статистики за сегодня.
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

    // Применяет флажки из блока настроек отображения.
    private void ApplyDisplaySettings(bool showMessage)
    {
        if (QuickStatsListView is null || QuickStatsTitleTextBlock is null)
        {
            return;
        }

        var showStats = PublicStatsCheckBox.IsChecked == true;
        QuickStatsListView.Visibility = showStats ? Visibility.Visible : Visibility.Collapsed;
        QuickStatsTitleTextBlock.Visibility = showStats ? Visibility.Visible : Visibility.Collapsed;

        if (showMessage)
        {
            var notifications = NotificationsCheckBox.IsChecked == true ? "уведомления включены" : "уведомления выключены";
            var report = WeeklyReportCheckBox.IsChecked == true ? ", еженедельный отчет включен" : string.Empty;
            SetStatus($"Настройки применены: {notifications}{report}");

            if (AutoSaveCheckBox.IsChecked == true && CanAutoSaveProfile())
            {
                SaveProfile();
            }
        }
    }

    // Проверяет, можно ли автосохранять профиль без показа ошибок.
    private bool CanAutoSaveProfile()
    {
        return !string.IsNullOrWhiteSpace(FirstNameTextBox.Text)
            && !string.IsNullOrWhiteSpace(LastNameTextBox.Text)
            && (BirthDatePicker.SelectedDate is null || BirthDatePicker.SelectedDate.Value.Date <= DateTime.Today);
    }

    // Устанавливает текст в строке состояния.
    private void SetStatus(string message)
    {
        if (StatusTextBlock is null)
        {
            return;
        }

        StatusTextBlock.Text = message;
    }

    // Возвращает выбранный уровень активности.
    private string GetSelectedActivity()
    {
        if (LowActivityRadioButton.IsChecked == true)
        {
            return "Низкий";
        }

        return HighActivityRadioButton.IsChecked == true ? "Высокий" : "Средний";
    }

    // Выбирает радио-кнопку по уровню активности.
    private void SelectActivity(string activityLevel)
    {
        LowActivityRadioButton.IsChecked = activityLevel == "Низкий";
        MediumActivityRadioButton.IsChecked = activityLevel == "Средний";
        HighActivityRadioButton.IsChecked = activityLevel == "Высокий";
    }

    // Выбирает элемент ComboBox по текстовому значению.
    private static void SetComboBoxValue(ComboBox comboBox, string value)
    {
        foreach (ComboBoxItem item in comboBox.Items)
        {
            item.IsSelected = item.Content?.ToString() == value;
        }
    }
}
