namespace PUJASM.POS.Utilities
{
    using System;
    using System.Linq;
    using System.Windows;
    using Models;

    public static class UtilityMethods
    {
        public static void CloseForemostWindow()
        {
            var editWindow = Application.Current.Windows[Application.Current.Windows.Count - 1];
            editWindow?.Close();
        }

        public static DateTime GetCurrentDate()
        {
            DateTime currentDate;
            using (var context = new ERPContext())
            {
                var date = context.Dates.SingleOrDefault(e => e.Name.Equals("Current"));
                if (date != null)
                    currentDate = date.DateTime;
                else
                {
                    currentDate = DateTime.Now.Date;
                    context.Dates.Add(new Date { DateTime = DateTime.Now.Date, Name = "Current" });
                }
            }
            return currentDate;
        }
    }
}
