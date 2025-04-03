using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Client.Models
{
    /// <summary>
    /// Модель представления для заявки
    /// </summary>
    public class RequestViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Номер заявки обязателен")]
        [StringLength(50, ErrorMessage = "Номер заявки не может превышать 50 символов")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Дата создания обязательна")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Срок исполнения обязателен")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [StringLength(500, ErrorMessage = "Комментарий не может превышать 500 символов")]
        public string? Comment { get; set; }

        [StringLength(500, ErrorMessage = "Комментарий по исполнению не может превышать 500 символов")]
        public string? ExecutionComment { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        public RequestStatus Status { get; set; }

        [Required(ErrorMessage = "Назначение (единица техники) обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "ID техники должен быть положительным числом")]
        public int EquipmentId { get; set; }

        public List<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();
    }

    /// <summary>
    /// Модель представления для наименования в заявке
    /// </summary>
    public class ItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название наименования обязательно")]
        [StringLength(100, ErrorMessage = "Название не может превышать 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Артикул обязателен")]
        [StringLength(50, ErrorMessage = "Артикул не может превышать 50 символов")]
        public string Article { get; set; }

        [Required(ErrorMessage = "Количество обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть положительным числом")]
        public int Quantity { get; set; }

        [StringLength(200, ErrorMessage = "Примечание не может превышать 200 символов")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Статус наименования обязателен")]
        public ItemStatus Status { get; set; }
    }
}