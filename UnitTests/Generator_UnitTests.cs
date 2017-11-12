using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeLines;

namespace UnitTests
{
	[TestClass]
	public class Generator_UnitTests
	{
		[TestMethod]
		public void Generator_Generate_Test()
		{
			DateTime monthsBegin = new DateTime(2015, 10, 01);
			//DateTime monthsEnd = new DateTime(2016, 02, 01);
			DateTime monthsEnd = new DateTime(2015, 10, 02);

			var actual = Generator.Generate(
				monthsBegin,
				start => start < monthsEnd ? start.AddMonths(1) : (DateTime?)null,
				start => start < monthsEnd ? new Period(start, start.AddMonths(1)) : null)
				.ToList();

			List<Period> expected = new List<Period>
			{
				Period.Days(2015, 10, 01, 31),
				//Period.Days(2015, 11, 01, 30),
				//Period.Days(2015, 12, 01, 31),
				//Period.Days(2016, 01, 01, 31),
            };

			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void Generator_Shedule_8_Test()
		{
			DateTime startDate = new DateTime(2015, 11, 01);
			DateTime endDate = new DateTime(2015, 11, 10);
			IEnumerable<DateTime> holydays = new DateTime[]
			{
				new DateTime(2015, 06, 12),
				new DateTime(2015, 11, 04),
			};
			IEnumerable<DateTime> preHolydays = new DateTime[]
			{
				new DateTime(2015, 04, 30),
				new DateTime(2015, 05, 08),
				new DateTime(2015, 06, 11),
				new DateTime(2015, 11, 03),
				new DateTime(2015, 12, 31)
			};

			var actual = Generator.Generate(startDate.AddHours(8), endDate, TimeSpan.FromHours(8), TimeSpan.FromDays(1), (start, end) =>
				{
					if (preHolydays.Contains(start.Date))
						return new Period { Begin = start, End = end.AddHours(-1) };
					else if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday || holydays.Contains(start.Date))
						return null;
					else
						return new Period { Begin = start, End = end };
				})
				.ToList();

			List<Period> expected = new List<Period>
			{
				new Period { Begin = new DateTime(2015, 11, 02, 08, 00, 00), End = new DateTime(2015, 11, 02, 16, 00, 00) },
				new Period { Begin = new DateTime(2015, 11, 03, 08, 00, 00), End = new DateTime(2015, 11, 03, 15, 00, 00) },
				new Period { Begin = new DateTime(2015, 11, 05, 08, 00, 00), End = new DateTime(2015, 11, 05, 16, 00, 00) },
				new Period { Begin = new DateTime(2015, 11, 06, 08, 00, 00), End = new DateTime(2015, 11, 06, 16, 00, 00) },
				new Period { Begin = new DateTime(2015, 11, 09, 08, 00, 00), End = new DateTime(2015, 11, 09, 16, 00, 00) },
			};

			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void Generator_Shedule_12_Test()
		{
			DateTime StartDate = new DateTime(2015, 11, 01);
			DateTime EndDate = new DateTime(2015, 11, 11);
			IEnumerable<IPeriod> mask = new Period[]
			{
				new Period { Begin = StartDate, End = EndDate }
			};

			int shiftNo;
			int dayNo;
			List<IPeriod> actual;
			List<IPeriod> expected;

			shiftNo = 4;
			dayNo = 1;
			DateTime startDate = StartDate;
			switch (shiftNo)
			{
				case 2:
					startDate = StartDate.AddDays(1);
					break;
				case 3:
					startDate = StartDate.AddDays(-2);
					break;
				case 4:
					startDate = StartDate.AddDays(-1);
					break;
			}
			actual = 
				TimeLines.TimeLineUtils.And(
					new IEnumerable<IPeriod>[]
					{
						Generator.Generate(
							startDate,
							EndDate,
							TimeSpan.FromHours(12),
							TimeSpan.FromDays(1),
							(start, end) =>
							{
								TimeSpan delta = TimeSpan.FromHours(8) + TimeSpan.FromHours(12 * (dayNo - 1));
								dayNo = dayNo < 4 ? dayNo + 1 : 1;
								return delta.TotalDays >= 1 ? null : new Period { Begin = start + delta, End = end + delta };
							}),
						mask
					},
					(start, end) => new Period[] { new Period { Begin = start, End = end }})
				.ToList();
			//expected = new List<IPeriod>
			//{
			//};
			//CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}
	}
}
