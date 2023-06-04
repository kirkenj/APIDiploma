namespace Logic.Services
{
    public static class DateTimeProvider
    {
        public static IEnumerable<DateTime> GetDateRangeViaAddMonth(DateTime dateStart, DateTime dateEnd)
        {
            dateStart = new DateTime(dateStart.Year, dateStart.Month, 1);
            dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, 1);
            (dateStart, dateEnd) = dateStart > dateEnd ? (dateEnd, dateStart) : (dateStart, dateEnd);
            var dates = new List<DateTime>();
            var cDate = dateStart;
            while (cDate <= dateEnd)
            {
                dates.Add(cDate);
                cDate = cDate.AddMonths(1);
            }

            return dates;
        }
    }
}
