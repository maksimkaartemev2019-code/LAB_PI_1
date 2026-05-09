using System;
using System.Collections.Generic;

namespace LAB_PI_1.Models;

/// <summary>
/// Stores personal data entered on the profile tab.
/// </summary>
public sealed class UserProfile
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the demo password value.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected birth date.
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Gets or sets the selected education level.
    /// </summary>
    public string Education { get; set; } = string.Empty;

    /// <summary>
    /// Gets the selected hobby names.
    /// </summary>
    public List<string> Hobbies { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether notifications are enabled.
    /// </summary>
    public bool ReceiveNotifications { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether public statistics are enabled.
    /// </summary>
    public bool ShowPublicStatistics { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether autosave is enabled.
    /// </summary>
    public bool AutoSave { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether weekly reports are enabled.
    /// </summary>
    public bool WeeklyReport { get; set; }

    /// <summary>
    /// Gets or sets the selected activity level.
    /// </summary>
    public string ActivityLevel { get; set; } = "Средний";
}
