using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DDay.iCal;

namespace TimeLines.iCalGenerator
{
	/// <summary>
	/// Генератор временного ряда
	/// </summary>
    public static class Generator
    {
		/// <summary>
		/// Генерация временного ряда
		/// </summary>
		/// <param name="template">шаблое генерации в соответствии с RFC-5545</param>
		/// <param name="start">начало периода генерации</param>
		/// <param name="end">конец периода генерации</param>
		/// <param name="createPeriodFunc">функция генерации периодов</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Generate(string template, DateTime start, DateTime end, Func<DateTime, IEnumerable<IPeriod>> createPeriodFunc)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(template);
            MemoryStream memStream = new MemoryStream(buffer);
            foreach (var oc in iCalendar.LoadFromStream(memStream).First().GetOccurrences<Event>(start, end))
            {
				if (oc.Period.StartTime.Value < start)
					continue;
				if (oc.Period.StartTime.Value > end)
					yield break;

				foreach (IPeriod period in createPeriodFunc(oc.Period.StartTime.Value))
				{
					if (period != null)
						yield return period;
				}
            }
        }
    }
}
