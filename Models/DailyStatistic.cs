namespace LAB_PI_1.Models;

// Класс описывает статистику за один день.
public sealed class DailyStatistic
{
    // Название дня.
    public string Day { get; set; } = string.Empty;

    // Количество выполненных привычек.
    public int Completed { get; set; }

    // Общее количество привычек.
    public int Total { get; set; }

    // Отформатированный процент выполнения.
    public string Progress => Total == 0 ? "0%" : $"{Completed * 100 / Total}%";
}
