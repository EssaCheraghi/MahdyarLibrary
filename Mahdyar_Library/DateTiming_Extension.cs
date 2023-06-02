using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Mahdyar_Library
{
   public static class DateTiming_Extension
    {
        /// <summary>
        /// DateDiff in SQL style. 
        /// Datepart implemented: 
        ///     "year" (abbr. "yy", "yyyy"), 
        ///     "quarter" (abbr. "qq", "q"), 
        ///     "month" (abbr. "mm", "m"), 
        ///     "day" (abbr. "dd", "d"), 
        ///     "week" (abbr. "wk", "ww"), 
        ///     "hour" (abbr. "hh"), 
        ///     "minute" (abbr. "mi", "n"), 
        ///     "second" (abbr. "ss", "s"), 
        ///     "millisecond" (abbr. "ms").
        /// </summary>
        /// <param name="DatePart"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static Int64 Ext_DateDiff(this DateTime StartDate, String DatePart, DateTime EndDate)
        {
            Int64 DateDiffVal = 0;
            System.Globalization.Calendar cal = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar;
            TimeSpan ts = new TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (DatePart.ToLower().Trim())
            {
                #region year
                case "year":
                case "yy":
                case "yyyy":
                    DateDiffVal = (Int64)(cal.GetYear(EndDate) - cal.GetYear(StartDate));
                    break;
                #endregion

                #region quarter
                case "quarter":
                case "qq":
                case "q":
                    DateDiffVal = (Int64)((((cal.GetYear(EndDate)
                                        - cal.GetYear(StartDate)) * 4)
                                        + ((cal.GetMonth(EndDate) - 1) / 3))
                                        - ((cal.GetMonth(StartDate) - 1) / 3));
                    break;
                #endregion

                #region month
                case "month":
                case "mm":
                case "m":
                    DateDiffVal = (Int64)(((cal.GetYear(EndDate)
                                        - cal.GetYear(StartDate)) * 12
                                        + cal.GetMonth(EndDate))
                                        - cal.GetMonth(StartDate));
                    break;
                #endregion

                #region day
                case "day":
                case "d":
                case "dd":
                    DateDiffVal = (Int64)ts.TotalDays;
                    break;
                #endregion

                #region week
                case "week":
                case "wk":
                case "ww":
                    DateDiffVal = (Int64)(ts.TotalDays / 7);
                    break;
                #endregion

                #region hour
                case "hour":
                case "hh":
                    DateDiffVal = (Int64)ts.TotalHours;
                    break;
                #endregion

                #region minute
                case "minute":
                case "mi":
                case "n":
                    DateDiffVal = (Int64)ts.TotalMinutes;
                    break;
                #endregion

                #region second
                case "second":
                case "ss":
                case "s":
                    DateDiffVal = (Int64)ts.TotalSeconds;
                    break;
                #endregion

                #region millisecond
                case "millisecond":
                case "ms":
                    DateDiffVal = (Int64)ts.TotalMilliseconds;
                    break;
                #endregion

                default:
                    throw new Exception(String.Format("DatePart \"{0}\" is unknown", DatePart));
            }
            return DateDiffVal;
        }

        public static DateTime Ext_ToPersianDateTime(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return DateTime.Parse(string.Format("{0}/{1}/{2} {3}:{4}:{5}", pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date), pc.GetHour(date), pc.GetMinute(date), pc.GetSecond(date)));
        }
        public static string Ext_ToPersianDateTimeForFilename(this DateTime date)
        {
            return Ext_ToPersianDateTime(date).ToString().Replace('/', '.').Replace(':', '.');
        }
        public static DateTime Ext_ToGregorian(this DateTime obj)
        {
            DateTime dt = new DateTime(obj.Year, obj.Month, obj.Day, obj.Hour, obj.Minute, obj.Second, new PersianCalendar());
            return dt;
        }

        /// <summary>
        /// Converts To Persian Statement
        /// </summary>
        /// <param name="Dt"></param>
        /// <returns></returns>
        public static string Ext_ToStringSentence(this DateTime Dt)
        {
            TimeSpan ts = DateTime.Now - Dt;
            int TotalMinutes = (int)ts.TotalMinutes;
            int TotalHours = (int)ts.TotalHours;
            int TotalDays = (int)ts.TotalDays;

            if (TotalMinutes < 7) return "دقایقی پیش";
            if (TotalMinutes < 12) return "ده دقیقه پیش";
            if (TotalMinutes < 17) return "ربع ساعت پیش";
            if (TotalMinutes < 35 && TotalMinutes > 25) return "نیم ساعت پیش";
            if (TotalHours == 0) return TotalMinutes + "دقیقه پیش ";
            if (TotalHours < 24) return TotalHours + "ساعت پیش ";
            if (TotalDays == 1) return TotalHours + "دیروز";
            if (TotalDays < 7) return TotalHours + "دیروز";

            return TotalDays + "روز پیش ";
        }


    }


    public class Cls_DateTimingString
    {
        //public static bool IsValidDate(string DateString)
        //{

        //}
    }


}
