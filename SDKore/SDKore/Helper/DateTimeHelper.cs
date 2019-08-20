using System;

namespace SDKore.Helper
{
    public class DateTimeHelper
    {
        public DateTime UltimoDiaDoUltimoTrimestre()
        {
            return PrimeiroDiaDoTrimestre().AddDays(-1);
        }
        
        public DateTime PrimeiroDiaDoTrimestre()
        {
            DateTime date = DateTime.Today;

            int quarterNumber = (date.Month - 1) / 3 + 1;
            DateTime firstDayOfQuarter = new DateTime(date.Year, (quarterNumber - 1) * 3 + 1, 1);

            return firstDayOfQuarter;
        }

        public int TrimestreAtual()
        {
            DateTime date = DateTime.Today;

            int quarterNumber = (date.Month - 1) / 3 + 1;

            return quarterNumber;
        }
    }
}