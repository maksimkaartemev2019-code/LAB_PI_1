namespace LAB_PI_1.Models;

// Класс описывает одну привычку в таблице.
public sealed class HabitEntry
{
    // Название привычки.
    public string Name { get; set; } = string.Empty;

    // Запланированное время выполнения.
    public string Time { get; set; } = string.Empty;

    // Признак выполнения привычки.
    public bool IsCompleted { get; set; }
}
