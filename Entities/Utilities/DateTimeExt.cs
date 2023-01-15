using System.Globalization;

namespace Entities;

public static class DateTimeExt
{
    public static string ToShortDate(this DateTime dateTime)
    {
        try
        {
            if (dateTime == null)
                return "";

            PersianCalendar pc = new PersianCalendar();
            string persianDate = $"{pc.GetYear(dateTime)}/{pc.GetMonth(dateTime)}/{pc.GetDayOfMonth(dateTime)}";
            return persianDate;
        }
        catch
        {
            return "";
        }
    }
}