using System;
using System.Collections.Generic;

namespace TimeLines
{
	public static class Generator
	{
		#region Генерация временных рядов

		/// <summary>
		/// Генерация временного ряда с равномерно расположенными одинаковыми периодами
		/// </summary>
		/// <param name="start">начало временного ряда</param>
		/// <param name="end">конец временного ряда</param>
		/// <param name="duration">длительность периодов</param>
		/// <param name="periodicity">периодичнсоть периодов</param>
		/// <param name="generatePeriodFunc">функция генерации периодов</param>
		/// <param name="startFilter">дополнительный фильтр для отсечения по началу периодов</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Generate(DateTime start, DateTime end, TimeSpan duration, TimeSpan periodicity, Func<DateTime, DateTime, IPeriod> generatePeriodFunc = null, Func<DateTime, bool> startFilter = null)
		{
			// проверка параметров
			if (start > end)
				throw new ArgumentException("start");
			if (duration < TimeSpan.Zero)
				throw new ArgumentException("duration");
			if (periodicity <= TimeSpan.Zero || periodicity < duration)
				throw new ArgumentException("periodicity");
			if (startFilter == null)
				startFilter = dt => true;
			if (generatePeriodFunc == null)
				generatePeriodFunc = (periodStart, periodEnd) => new Period(periodStart, periodEnd);

			while (start < end)
			{
				if (startFilter(start))
				{
					IPeriod period = generatePeriodFunc(start, start + duration);
					if (period != null)
						yield return period;
				}
				start += periodicity;
			}
		}

		/// <summary>
		/// Делегат генерации периодов временного ряда
		/// </summary>
		/// <param name="start"></param>
		/// <param name="nextStart"></param>
		/// <returns></returns>
		public delegate IPeriod GeneratePeriodDelegate(DateTime start, out DateTime nextStart);

		/// <summary>
		/// Генерация временного ряда с использованием делегата генерации периодов
		/// </summary>
		/// <param name="start">начало временного ряда</param>
		/// <param name="generatePeriod">делегат генерации периодов временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Generate(DateTime start, GeneratePeriodDelegate generatePeriod)
		{
			if (generatePeriod == null)
				throw new ArgumentNullException("generatePeriod");

			DateTime nextStart;
			IPeriod period;
			while ((period = generatePeriod(start, out nextStart)) != null)
			{
				yield return period;
				start = nextStart;
			}
		}

		/// <summary>
		/// Генерация временного ряда
		/// </summary>
		/// <param name="start">начало временного ряда = начало первого периода</param>
		/// <param name="nextStartFunc">функция вычисления начала следующего периода; если возврат = null, генерация прекращается</param>
		/// <param name="generatePeriodFunc">функция генерации периодов временного ряда; если возврат = null, генерация прекращается</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Generate(DateTime start, Func<DateTime, DateTime?> nextStartFunc, Func<DateTime, IPeriod> generatePeriodFunc = null)
		{
			DateTime? nextStart = start;
			while (true)
			{
				IPeriod period = generatePeriodFunc(nextStart.Value);
				if (period == null)
					break;
				else
				{
					yield return period;

					nextStart = nextStartFunc(nextStart.Value);
					if (nextStart == null)
						break;
				}
			}
		}

        #endregion Генерация временных рядов

    }
}
