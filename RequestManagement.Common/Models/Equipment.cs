using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models;

/// <summary>
/// Модель единицы техники (назначения) для заявки
/// </summary>
public class Equipment
{
    /// <summary>
    /// Уникальный идентификатор единицы техники
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название единицы техники
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Государственный номер (может отсутствовать)
    /// </summary>
    public string? StateNumber { get; set; }
}