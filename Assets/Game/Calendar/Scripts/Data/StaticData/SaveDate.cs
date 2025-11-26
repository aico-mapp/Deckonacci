using System;

namespace Calendar.Scripts.Data.StaticData
{
    [Serializable]
    public class SaveDate
    {
        public int Year;
        public int Month;
        public int Day;

        public string GetString() => Day + "." + Month + "." + Year;

        public SaveDate()
        {
            Year = 0;
            Month = 0;
            Day = 0;
        }

        public SaveDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }
        
        public SaveDate(DateTime date)
        {
            Year = date.Year;
            Month = date.Month;
            Day = date.Day;
        }

        public bool Compare(SaveDate date)
        {
            return Year == date.Year && Month == date.Month && Day == date.Day;
        }
        
        public bool CompareMonth(DateTime date)
        {
            return Year == date.Year && Month == date.Month;
        }

        public DateTime GetDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }
}