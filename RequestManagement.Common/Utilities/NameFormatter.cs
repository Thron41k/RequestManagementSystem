namespace RequestManagement.Common.Utilities
{
    public static class NameFormatter
    {
        /// <summary>
        /// Преобразует полное имя (например, "Иванов Иван Иванович", "Иванов Иван" или "Иванов Иван Угли")
        /// в сокращённую запись (например, "Иванов И.И." или "Иванов И."). 
        /// Отчество необязательно, дополнительные части после имени или отчества игнорируются.
        /// </summary>
        /// <param name="fullName">Полное имя, состоящее из 2 или более частей, разделённых пробелами.</param>
        /// <returns>Сокращённая запись имени (например, "Иванов И.И." или "Иванов И.") или null, если входные данные некорректны.</returns>
        public static string FormatToShortName(string? fullName)
        {
            // Проверка на null или пустую строку
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "";
            }

            // Разделяем имя на части, игнорируя множественные пробелы
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Проверяем, что есть минимум 2 части (фамилия и имя)
            if (parts.Length < 2)
            {
                return "";
            }

            // Извлекаем фамилию и имя
            var surname = parts[0];
            var firstName = parts[1];

            // Проверяем, что фамилия и имя не пустые
            if (string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(firstName))
            {
                return "";
            }

            // Формируем сокращённую запись
            // Если есть отчество (3 или более частей), добавляем его инициал
            if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            {
                var patronymic = parts[2];
                return $"{surname} {firstName[0]}.{patronymic[0]}.";
            }

            // Если отчества нет, возвращаем только инициал имени
            return $"{surname} {firstName[0]}.";
        }
    }
}