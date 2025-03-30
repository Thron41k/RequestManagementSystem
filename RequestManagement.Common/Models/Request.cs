using RequestManagement.Common.Models.Enums;
using System;
using System.Collections.Generic;

namespace RequestManagement.Common.Models;

/// <summary>
/// Модель заявки
/// </summary>
public class Request
{
    /// <summary>
    /// Уникальный идентификатор заявки
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Номер заявки
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// Дата создания заявки
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Срок исполнения заявки
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Комментарий к заявке
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Комментарий по исполнению заявки
    /// </summary>
    public string? ExecutionComment { get; set; }

    /// <summary>
    /// Статус заявки
    /// </summary>
    public RequestStatus Status { get; set; }

    /// <summary>
    /// Список наименований в заявке
    /// </summary>
    public List<Item> Items { get; set; } = new List<Item>();

    /// <summary>
    /// Идентификатор назначения (единицы техники)
    /// </summary>
    public int EquipmentId { get; set; }

    /// <summary>
    /// Ссылка на назначение (единицу техники)
    /// </summary>
    public Equipment Equipment { get; set; }
}