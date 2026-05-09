namespace LAB_PI_1.Models;

/// <summary>
/// Represents one habit row in the habit table.
/// </summary>
public sealed class HabitEntry
{
    /// <summary>
    /// Gets or sets the habit title.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the planned habit time.
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the habit is completed.
    /// </summary>
    public bool IsCompleted { get; set; }
}
