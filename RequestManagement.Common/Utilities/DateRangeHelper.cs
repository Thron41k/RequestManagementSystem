namespace RequestManagement.Common.Utilities;

public static class DateRangeHelper
{
    /// <summary>
    /// Возвращает диапазон дат для половины месяца,
    /// основываясь на переданной дате.
    /// </summary>
    private static (DateTime FromDate, DateTime ToDate) GetHalfMonthRange(DateTime date)
    {
        if (date.Day <= 15)
        {
            return (
                new DateTime(date.Year, date.Month, 1),
                new DateTime(date.Year, date.Month, 15)
            );
        }

        return (
            new DateTime(date.Year, date.Month, 16),
            new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month))
        );
    }

    /// <summary>
    /// Возвращает диапазон дат для текущего дня.
    /// </summary>
    public static (DateTime FromDate, DateTime ToDate) GetCurrentHalfMonthRange()
        => GetHalfMonthRange(DateTime.Today);
}