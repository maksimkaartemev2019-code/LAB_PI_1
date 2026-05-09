using System;
using System.Collections.Generic;

namespace LAB_PI_1.Models;

// Класс хранит личные данные пользователя.
public sealed class UserProfile
{
    // Имя пользователя.
    public string FirstName { get; set; } = string.Empty;

    // Фамилия пользователя.
    public string LastName { get; set; } = string.Empty;

    // Демонстрационное значение пароля.
    public string Password { get; set; } = string.Empty;

    // Дата рождения пользователя.
    public DateTime? BirthDate { get; set; }

    // Выбранный уровень образования.
    public string Education { get; set; } = string.Empty;

    // Список выбранных хобби.
    public List<string> Hobbies { get; } = [];

    // Признак включенных уведомлений.
    public bool ReceiveNotifications { get; set; }

    // Признак публичного отображения статистики.
    public bool ShowPublicStatistics { get; set; }

    // Признак включенного автосохранения.
    public bool AutoSave { get; set; }

    // Признак включенного еженедельного отчета.
    public bool WeeklyReport { get; set; }

    // Выбранный уровень активности.
    public string ActivityLevel { get; set; } = "Средний";
}
