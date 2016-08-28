using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Itenso.TimePeriod;

namespace ConsoleTests
{
	/// <summary>
	/// Тесты библиотеки Time Period Library for .NET
	/// </summary>
	/// <see cref="http://www.codeproject.com/Articles/168662/Time-Period-Library-for-NET"/>
	static class TimePeriodTests
	{
		/// <summary>
		/// Содержание теста:
		/// Из одного периода вычитается 32 (4х8) неуникальных периода.
		/// Результат - коллекция периодов.
		/// 
		/// Решение: TimePeriod.Desktop
		/// Проект: TimePeriodTests.Desktop
		/// Файл: PerformanceTest.cs
		/// Класс: PerformanceTest
		/// Метод: GapCalculator32
		/// </summary>
		/// <param name="count">количество операций</param>
		public static void Test1(int count)
		{
			TimeRange limits = new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 6, 1));
			TimeGapCalculator<TimeRange> gapCalculator = new TimeGapCalculator<TimeRange>();

			TimePeriodCollection excludePeriods = new TimePeriodCollection();
			for (int i = 0; i < 4; i++)
			{
				excludePeriods.Add(new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0)));
				excludePeriods.Add(new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0)));
				excludePeriods.Add(new TimeRange(new DateTime(2011, 4, 01, 00, 00, 0), new DateTime(2011, 4, 12, 00, 00, 0)));
				excludePeriods.Add(new TimeRange(new DateTime(2011, 4, 12, 00, 00, 0), new DateTime(2011, 4, 18, 00, 00, 0)));
				excludePeriods.Add(new TimeRange(new DateTime(2011, 4, 29, 00, 00, 0), new DateTime(2011, 4, 30, 00, 00, 0)));
				excludePeriods.Add(new TimeRange(new DateTime(2011, 4, 29, 00, 00, 0), new DateTime(2011, 4, 30, 00, 00, 0)));
				excludePeriods.Add(new TimeRange(new DateTime(2011, 5, 01, 00, 00, 0), new DateTime(2011, 5, 12, 00, 00, 0)));
				excludePeriods.Add(new TimeRange(new DateTime(2011, 5, 12, 00, 00, 0), new DateTime(2011, 5, 18, 00, 00, 0)));
			}

			Program.Test(false, count, () => gapCalculator.GetGaps(excludePeriods, limits));
		}

		/// <summary>
		/// TimePeriodCombiner
		/// </summary>
		/// <param name="count"></param>
		public static void Test2(int count)
		{
			TimePeriodCollection periods = new TimePeriodCollection();
			periods.Add(new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 04), new DateTime(2011, 3, 08)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 18)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 18), new DateTime(2011, 3, 22)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 24)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 26), new DateTime(2011, 3, 30)));

			TimePeriodCombiner<TimeRange> periodCombiner = new TimePeriodCombiner<TimeRange>();
			ITimePeriodCollection combinedPeriods;

			Program.Test(false, count, () => combinedPeriods = periodCombiner.CombinePeriods(periods));
		}

		public static void Test3(int count)
		{
			TimePeriodCollection periods = new TimePeriodCollection();
			periods.Add(new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 04), new DateTime(2011, 3, 08)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 18)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 18), new DateTime(2011, 3, 22)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 24)));
			periods.Add(new TimeRange(new DateTime(2011, 3, 26), new DateTime(2011, 3, 30)));

			TimePeriodIntersector<TimeRange> periodIntersector = new TimePeriodIntersector<TimeRange>();
			ITimePeriodCollection intersectedPeriods;

			Program.Test(false, count, () => intersectedPeriods = periodIntersector.IntersectPeriods(periods));
		}

		public static void Test4(int count)
		{
			DateTime moment = new DateTime(2012, 1, 29);
			TimePeriodCollection sourcePeriods = new TimePeriodCollection
			{
				new TimeRange( moment.AddHours( 2 ), moment.AddDays( 1 ) )
			};
			TimePeriodCollection subtractingPeriods = new TimePeriodCollection
			{
				new TimeRange( moment.AddHours( 6 ), moment.AddHours( 10 ) ),
				new TimeRange( moment.AddHours( 12 ), moment.AddHours( 16 ) )
			};
			TimePeriodSubtractor<TimeRange> subtractor = new TimePeriodSubtractor<TimeRange>();
			ITimePeriodCollection subtractedPeriods;

			Program.Test(false, count, () => subtractedPeriods = subtractor.SubtractPeriods(sourcePeriods, subtractingPeriods));
		}
	}
}
