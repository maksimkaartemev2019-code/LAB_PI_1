using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LAB_PI_1.Models;

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
        SetStatus("РџСЂРёР»РѕР¶РµРЅРёРµ Р·Р°РїСѓС‰РµРЅРѕ");
    }

    /// <summary>
    /// Adds starter habits for the first application run.
    /// </summary>
    private void SeedDemoData()
    {
        habits.Add(new HabitEntry { Name = "Р’С‹РїРёС‚СЊ РІРѕРґСѓ", Time = "09:00", IsCompleted = true });
        habits.Add(new HabitEntry { Name = "РџСЂРѕС‡РёС‚Р°С‚СЊ 20 СЃС‚СЂР°РЅРёС†", Time = "20:00", IsCompleted = false });
        habits.Add(new HabitEntry { Name = "РџСЂРѕРіСѓР»РєР°", Time = "18:30", IsCompleted = true });
        HabitsDataGrid.ItemsSource = habits;
    }

    /// <summary>
    /// Handles the main save button click.
    /// </summary>
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveProfile();
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
        ProfileSummaryTextBlock.Text = "РџСЂРѕС„РёР»СЊ РЅРµ СЃРѕС…СЂР°РЅРµРЅ";
        SetStatus("РџРѕР»СЏ РѕС‡РёС‰РµРЅС‹");
    }

    /// <summary>
    /// Adds a new habit from the input field to the habit table.
    /// </summary>
    private void AddHabitButton_Click(object sender, RoutedEventArgs e)
    {
        var habitName = NewHabitTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(habitName))
        {
            MessageBox.Show("Р’РІРµРґРёС‚Рµ РЅР°Р·РІР°РЅРёРµ РїСЂРёРІС‹С‡РєРё.", "РџСЂРѕРІРµСЂРєР°", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        habits.Add(new HabitEntry { Name = habitName, Time = DateTime.Now.ToString("HH:mm"), IsCompleted = false });
        NewHabitTextBox.Clear();
        SetStatus($"Р”РѕР±Р°РІР»РµРЅР° РїСЂРёРІС‹С‡РєР°: {habitName}");
    }

    /// <summary>
    /// Updates progress after productivity or satisfaction slider changes.
    /// </summary>
    private void HabitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateProgress();
    }

    /// <summary>
    /// Updates the status bar when a calendar date is selected.
    /// </summary>
    private void HabitCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HabitCalendar.SelectedDate is DateTime selectedDate)
        {
            SetStatus($"Р’С‹Р±СЂР°РЅ РґРµРЅСЊ: {selectedDate:dd.MM.yyyy}");
        }
    }

    /// <summary>
    /// Updates the status bar when another tab is selected.
    /// </summary>
    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainTabControl.SelectedItem is TabItem tabItem)
        {
            SetStatus($"РћС‚РєСЂС‹С‚Р° РІРєР»Р°РґРєР°: {tabItem.Header}");
        }
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
        SetStatus("РџСЂРѕС„РёР»СЊ СЃРѕС…СЂР°РЅРµРЅ РІ РѕР±СЉРµРєС‚ UserProfile");
    }

    /// <summary>
    /// Checks required profile fields and date boundaries.
    /// </summary>
    private bool ValidateProfile()
    {
        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("РРјСЏ Рё С„Р°РјРёР»РёСЏ РЅРµ РґРѕР»Р¶РЅС‹ Р±С‹С‚СЊ РїСѓСЃС‚С‹РјРё.", "РћС€РёР±РєР° РІР°Р»РёРґР°С†РёРё", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (BirthDatePicker.SelectedDate is DateTime birthDate && birthDate.Date > DateTime.Today)
        {
            MessageBox.Show("Р”Р°С‚Р° СЂРѕР¶РґРµРЅРёСЏ РЅРµ РјРѕР¶РµС‚ Р±С‹С‚СЊ РІ Р±СѓРґСѓС‰РµРј.", "РћС€РёР±РєР° РІР°Р»РёРґР°С†РёРё", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            return "РќРёР·РєРёР№";
        }

        return HighActivityRadioButton.IsChecked == true ? "Р’С‹СЃРѕРєРёР№" : "РЎСЂРµРґРЅРёР№";
    }
}