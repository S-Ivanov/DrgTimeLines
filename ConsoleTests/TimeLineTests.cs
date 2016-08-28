using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TimeLines;

namespace ConsoleTests
{
	/// <summary>
	/// Тесты библиотеки временных рядов
	/// </summary>
	static class TimeLineTests
	{
		/// <summary>
		/// Из временного ряда, состоящего из 1 периода, вычитается 4 ряда по 8 периодов
		/// </summary>
		/// <param name="count">количество операций</param>
		public static void Test1(int count)
		{
			List<IPeriod> sourceTimeLine = 
				new List<IPeriod>
				{
					new Period(new DateTime(2011, 03, 30), new DateTime(2011, 03, 31)),
					new Period(new DateTime(2011, 03, 31), new DateTime(2011, 04, 01)),
					new Period(new DateTime(2011, 04, 01), new DateTime(2011, 04, 12)),
					new Period(new DateTime(2011, 04, 12), new DateTime(2011, 04, 18)),
					new Period(new DateTime(2011, 04, 18), new DateTime(2011, 04, 19)),
					new Period(new DateTime(2011, 04, 29), new DateTime(2011, 04, 30)),
					new Period(new DateTime(2011, 05, 01), new DateTime(2011, 05, 12)),
					new Period(new DateTime(2011, 05, 12), new DateTime(2011, 05, 18)),
				};

			IEnumerable<IPeriod> result = sourceTimeLine;
			Program.Test(true, count, () => result = result.Not().ToList());
		}

		public static void Test2(int count)
		{
			IEnumerable<IPeriod>[] timeLines = new IEnumerable<IPeriod>[]
			{
				new IPeriod[]
				{
					new Period(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10)),
					new Period(new DateTime(2011, 3, 15), new DateTime(2011, 3, 18)),
					new Period(new DateTime(2011, 3, 20), new DateTime(2011, 3, 24))
				},
				new IPeriod[]
				{
					new Period(new DateTime(2011, 3, 04), new DateTime(2011, 3, 08)),
					new Period(new DateTime(2011, 3, 18), new DateTime(2011, 3, 22)),
					new Period(new DateTime(2011, 3, 26), new DateTime(2011, 3, 30))
				},
			};

			IEnumerable<IPeriod> timeLine;

			Program.Test(true, count, () => timeLine = TimeLineUtils.Or(timeLines, (begin, end, pp) => new Period(begin, end)));
		}

		public static void Test3(int count)
		{
			IEnumerable<IPeriod>[] timeLines = new IEnumerable<IPeriod>[]
			{
				new IPeriod[]
				{
					new Period(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10)),
					new Period(new DateTime(2011, 3, 15), new DateTime(2011, 3, 18)),
					new Period(new DateTime(2011, 3, 20), new DateTime(2011, 3, 24))
				},
				new IPeriod[]
				{
					new Period(new DateTime(2011, 3, 04), new DateTime(2011, 3, 08)),
					new Period(new DateTime(2011, 3, 18), new DateTime(2011, 3, 22)),
					new Period(new DateTime(2011, 3, 26), new DateTime(2011, 3, 30))
				},
			};

			IEnumerable<IPeriod> timeLine;

			Program.Test(true, count, () => timeLine = TimeLineUtils.And(timeLines, (begin, end, pp) => new Period(begin, end)));
		}

		public static void Test4(int count)
		{
			DateTime moment = new DateTime(2012, 1, 29);
			IEnumerable<IPeriod> sourcePeriods = new IPeriod[]
			{
				new Period(moment.AddHours(2), moment.AddDays(1))
			};
			IEnumerable<IPeriod> subtractingPeriods = new IPeriod[]
			{
				new Period(moment.AddHours(6), moment.AddHours(10)),
				new Period(moment.AddHours(12), moment.AddHours(16))
			};
			IEnumerable<IPeriod> subtractedPeriods;

			Program.Test(true, count, () => subtractedPeriods = sourcePeriods.Subtract(new IEnumerable<IPeriod>[] { subtractingPeriods }));
		}
	}
}
