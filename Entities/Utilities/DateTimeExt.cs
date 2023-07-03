using System.Globalization;

namespace Entities.Utilities;

public static class DateTimeExt
{
    public static string ToShortDate(this DateTime dateTime)
    {
        try
        {
            if (dateTime == null)
                return "";

            var pc = new PersianCalendar();
            var persianDate = $"{pc.GetYear(dateTime)}/{pc.GetMonth(dateTime)}/{pc.GetDayOfMonth(dateTime)}";
            return persianDate;
        }
        catch
        {
            return "";
        }
    }
}