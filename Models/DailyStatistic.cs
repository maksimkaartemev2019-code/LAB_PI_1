namespace LAB_PI_1.Models;

/// <summary>
/// Describes a day summary shown in the statistics list.
/// </summary>
public sealed class DailyStatistic
{
    /// <summary>
    /// Gets or sets the displayed day name.
    /// </summary>
    public string Day { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the amount of completed habits.
    /// </summary>
    public int Completed { get; set; }

    /// <summary>
    /// Gets or sets the total habit amount.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Gets the formatted progress value.
    /// </summary>
    public string Progress => Total == 0 ? "0%" : $"{Completed * 100 / Total}%";
}
