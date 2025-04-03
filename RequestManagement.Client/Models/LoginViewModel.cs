using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Client.Models
{
    /// <summary>
    /// Модель представления для формы входа
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, ErrorMessage = "Логин не может превышать 50 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 50 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}