using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeLines;

namespace UnitTests
{
	[TestClass]
	public class TimeLineUtils_UnitTests
	{
		//[TestMethod]
		//public void TimeLineUtils_Compare_Test()
		//{
		//	IPeriod[] first =
		//		new IPeriod[]
		//		{
		//			new Period(new DateTime(2015, 10, 09), new DateTime(2015, 10, 10)), 
		//			new Period(new DateTime(2015, 10, 11), new DateTime(2015, 10, 12)), 
		//			//new Period(new DateTime(2015, 10, 13), new DateTime(2016, 10, 14)), 
		//			new Period(new DateTime(2015, 10, 15), new DateTime(2015, 10, 16)),
 
		//			new Period(new DateTime(2015, 10, 15), new DateTime(2015, 10, 16)), 
		//			new Period(new DateTime(2015, 10, 15), new DateTime(2015, 10, 16)), 
		//		};

		//	IPeriod[] second =
		//		new IPeriod[]
		//		{
		//			new Period(new DateTime(2015, 10, 09), new DateTime(2015, 10, 10)), 
		//			new Period(new DateTime(2015, 10, 11), new DateTime(2015, 10, 12)), 
		//			new Period(new DateTime(2015, 10, 13), new DateTime(2015, 10, 14)), 
		//		};

		//	var result = TimeLineUtils.Compare(first, second);

		//	Assert.AreNotEqual(0, result.Length);
		//}

        [TestMethod]
        public void TimeLineUtils_IndexOf_Test()
        {
            IPeriod[] periods;
            int index;

            periods = new IPeriod[] { };
            index = periods.IndexOf(new DateTime(2015, 11, 03));
            Assert.AreEqual(-1, index);

            periods = new IPeriod[] 
            { 
                new Period(new DateTime(2015, 11, 02), new DateTime(2015, 11, 04)),
                new Period(new DateTime(2015, 11, 06), new DateTime(2015, 11, 08)),
            };
            index = periods.IndexOf(new DateTime(2015, 11, 01));
            Assert.AreEqual(-1, index);
            index = periods.IndexOf(new DateTime(2015, 11, 03));
            Assert.AreEqual(0, index);
            index = periods.IndexOf(new DateTime(2015, 11, 05));
            Assert.AreEqual(-2, index);
            index = periods.IndexOf(new DateTime(2015, 11, 07));
            Assert.AreEqual(1, index);
            index = periods.IndexOf(new DateTime(2015, 11, 09));
            Assert.AreEqual(-3, index);
        }

        [TestMethod]
		public void TimeLineUtils_ToMonths_Test()
{
			IPeriod[] periods =
				new IPeriod[]
				{
					new Period(new DateTime(2015, 10, 09, 08, 0, 0), new DateTime(2015, 10, 09, 12, 0, 0)), 
					new Period(new DateTime(2015, 10, 09, 13, 0, 0), new DateTime(2015, 10, 09, 17, 0, 0)), 
            
					//new Period(new DateTime(2015, 11, 10, 08, 0, 0), new DateTime(2015, 11, 10, 12, 0, 0)), 
					//new Period(new DateTime(2015, 11, 10, 13, 0, 0), new DateTime(2015, 11, 10, 17, 0, 0)), 

					new Period(new DateTime(2015, 12, 11, 20, 0, 0), new DateTime(2016, 01, 12, 08, 0, 0)), 
                };

			var actual = periods.ToMonths().ToList();

			List<Period> expected = new List<Period>
			{
				Period.Days(2015, 10, 01, 31),
				//Period.Days(2015, 11, 01, 30),
				Period.Days(2015, 12, 01, 31),
				Period.Days(2016, 01, 01, 31),
            };

			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void TimeLineUtils_ToDays_Test()
		{
			IPeriod[] periods =
				new IPeriod[]
				{
					new Period(new DateTime(2015, 11, 09, 08, 0, 0), new DateTime(2015, 11, 09, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 09, 13, 0, 0), new DateTime(2015, 11, 09, 17, 0, 0)), 
            
					new Period(new DateTime(2015, 11, 10, 08, 0, 0), new DateTime(2015, 11, 10, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 10, 13, 0, 0), new DateTime(2015, 11, 10, 17, 0, 0)), 

					new Period(new DateTime(2015, 11, 11, 20, 0, 0), new DateTime(2015, 11, 12, 08, 0, 0)), 
				
				    Period.Days(2015, 11, 20, 2),
                };
			
			var actual = periods.ToDays();
			
			List<Period> expected = new List<Period>
			{
				Period.Days(2015, 11, 09, 1),
				Period.Days(2015, 11, 10, 1),
				Period.Days(2015, 11, 11, 1),
				Period.Days(2015, 11, 12, 1),
				Period.Days(2015, 11, 20, 1),
				Period.Days(2015, 11, 21, 1),
            };

			CollectionAssert.AreEqual(expected, actual.ToList(), new PeriodComparer());
		}
		
		[TestMethod]
        public void TimeLineUtils_Subtract_Test()
        {
            List<IPeriod> t1 = new List<IPeriod>
			{
				//new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				Period.Days(2015, 12, 1, 2),
				//new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 7)), 
				Period.Days(2015, 12, 4, 3),
			};
            List<IPeriod> t2 = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
				new Period(new DateTime(2015, 12, 5), new DateTime(2015, 12, 6)), 
			};
            List<IPeriod> expected = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 2), new DateTime(2015, 12, 3)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
				new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 7)), 
			};
            List<IPeriod> actual = t1.Subtract(new List<IEnumerable<IPeriod>> { t2 }).ToList();
            CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
        }
        
        [TestMethod]
		public void TimeLineUtils_Join_Test()
		{
			List<IPeriod> timeLine1;
			List<IPeriod> timeLine2;
			List<IPeriod> actual;
			List<IPeriod> expected;

			timeLine1 = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
			};
			timeLine2 = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 3), new DateTime(2015, 12, 4)), 
				new Period(new DateTime(2015, 12, 5), new DateTime(2015, 12, 6)), 
				new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 8)), 
			};
			expected = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
				new Period(new DateTime(2015, 12, 3), new DateTime(2015, 12, 4)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
				new Period(new DateTime(2015, 12, 5), new DateTime(2015, 12, 6)), 
				new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 8)), 
			};
			actual = timeLine1.Join(timeLine2).ToList();
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void TimeLineUtils_Split_Test()
		{
			List<IPeriod> timeLine;
			List<IPeriod> actual;
			List<IPeriod> expected;

			timeLine = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 7)), 
				new Period(new DateTime(2015, 12, 8), new DateTime(2015, 12, 9)), 
			};
			expected = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
				new Period(new DateTime(2015, 12, 5), new DateTime(2015, 12, 7)), 
				new Period(new DateTime(2015, 12, 8), new DateTime(2015, 12, 9)), 
			};
			actual = timeLine.Split(TimeSpan.FromDays(4)).ToList();
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void TimeLineUtils_Merge_Test()
		{
			List<IPeriod> timeLine;
			List<IPeriod> actual;
			List<IPeriod> expected;

			timeLine = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 6)), 
				new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 7)), 
			};
			expected = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 7)), 
			};
			actual = timeLine.Merge().ToList();
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void TimeLineUtils_Or_Test()
		{
			List<List<IPeriod>> timeLines;
			List<Period<IPeriod[]>> actual;
			List<Period<IPeriod[]>> expected;

			timeLines = new List<List<IPeriod>>
			{
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 6)), 

				},
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 2), new DateTime(2015, 12, 5)), 
				},
			};
			expected = new List<Period<IPeriod[]>>
			{
				new Period<IPeriod[]>(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 2), new DateTime(2015, 12, 3), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 3), new DateTime(2015, 12, 4), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 5), new DateTime(2015, 12, 6), null),
			};
			actual = TimeLineUtils.Or(timeLines).ToList();
			// проверка периодов результата
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
			// проверка данных периодов результата:
			// запись 0
			Assert.AreEqual(timeLines[0][0], actual[0].Data[0]);
			Assert.IsNull(actual[0].Data[1]);
			// запись 1
			Assert.AreEqual(timeLines[0][0], actual[1].Data[0]);
			Assert.AreEqual(timeLines[1][0], actual[1].Data[1]);
			// запись 2
			Assert.IsNull(actual[2].Data[0]);
			Assert.AreEqual(timeLines[1][0], actual[2].Data[1]);
			// запись 3
			Assert.AreEqual(timeLines[0][1], actual[3].Data[0]);
			Assert.AreEqual(timeLines[1][0], actual[3].Data[1]);
			// запись 4
			Assert.AreEqual(timeLines[0][1], actual[4].Data[0]);
			Assert.IsNull(actual[4].Data[1]);

			timeLines = new List<List<IPeriod>>
			{
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 6)), 

				},
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 3), new DateTime(2015, 12, 5)), 
				},
			};
			expected = new List<Period<IPeriod[]>>
			{
				new Period<IPeriod[]>(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 3), new DateTime(2015, 12, 4), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 5), new DateTime(2015, 12, 6), null),
			};
			actual = TimeLineUtils.Or(timeLines).ToList();
			// проверка периодов результата
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void TimeLineUtils_And_Test()
		{
			List<List<IPeriod>> timeLines;
			List<Period<IPeriod[]>> actual;
			List<Period<IPeriod[]>> expected;

			timeLines = new List<List<IPeriod>>
			{
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 6)), 

				},
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 2), new DateTime(2015, 12, 5)), 
				},
			};
			expected = new List<Period<IPeriod[]>>
			{
				new Period<IPeriod[]>(new DateTime(2015, 12, 2), new DateTime(2015, 12, 3), null),
				new Period<IPeriod[]>(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5), null),
			};
			actual = TimeLineUtils.And(timeLines).ToList();
			// проверка периодов результата
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());

			timeLines = new List<List<IPeriod>>
			{
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 6)), 

				},
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 3), new DateTime(2015, 12, 5)), 
				},
			};
			expected = new List<Period<IPeriod[]>>
			{
				new Period<IPeriod[]>(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5), null),
			};
			actual = TimeLineUtils.And(timeLines).ToList();
			// проверка периодов результата
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void TimeLineUtils_Not_Test()
		{
			List<IPeriod> timeLine;
			List<IPeriod> actual;
			List<Period> expected;

			timeLine = new List<IPeriod>
			{
				new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 6)), 
			};

			expected = new List<Period>
			{
				new Period(DateTime.MinValue, new DateTime(2015, 12, 1)), 
				new Period(new DateTime(2015, 12, 3), new DateTime(2015, 12, 4)), 
				new Period(new DateTime(2015, 12, 6), DateTime.MaxValue), 
			};

			actual = TimeLineUtils.Not(timeLine).ToList();
			// проверка периодов результата
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

    	[TestMethod]
		public void TimeLineUtils_Generate_Test()
        {
            List<IPeriod> actual;
            List<Period> expected;

            // генерировать периоды 8-часового графика работы c 01 по 10.11.2015   
			actual = TimeLines.Generator.Generate(
                new DateTime(2015, 11, 1),
                delegate(DateTime start, out DateTime nextStart) 
                    {
                        if (start.Day > 10)
                        {
                            nextStart = start;
                            return null;
                        }

                        while (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday || start.Day == 4)
                        {
                            start = start.AddDays(1);
                        }

                        if (start.Hour == 0)
                            start = start.AddHours(8);

                        nextStart = start.Hour == 8 ? start.AddHours(5) : start.Date.AddDays(1);

                        return new Period(start, start.AddHours(start.Day == 3 && start.Hour == 13 ? 3 : 4));
                    }
                ).ToList();
            expected = new List<Period>
			{
				new Period(new DateTime(2015, 11, 02, 08, 0, 0), new DateTime(2015, 11, 02, 12, 0, 0)), 
				new Period(new DateTime(2015, 11, 02, 13, 0, 0), new DateTime(2015, 11, 02, 17, 0, 0)), 

            	new Period(new DateTime(2015, 11, 03, 08, 0, 0), new DateTime(2015, 11, 03, 12, 0, 0)), 
				new Period(new DateTime(2015, 11, 03, 13, 0, 0), new DateTime(2015, 11, 03, 16, 0, 0)), 

				new Period(new DateTime(2015, 11, 05, 08, 0, 0), new DateTime(2015, 11, 05, 12, 0, 0)), 
				new Period(new DateTime(2015, 11, 05, 13, 0, 0), new DateTime(2015, 11, 05, 17, 0, 0)), 
            
				new Period(new DateTime(2015, 11, 06, 08, 0, 0), new DateTime(2015, 11, 06, 12, 0, 0)), 
				new Period(new DateTime(2015, 11, 06, 13, 0, 0), new DateTime(2015, 11, 06, 17, 0, 0)), 
            
				new Period(new DateTime(2015, 11, 09, 08, 0, 0), new DateTime(2015, 11, 09, 12, 0, 0)), 
				new Period(new DateTime(2015, 11, 09, 13, 0, 0), new DateTime(2015, 11, 09, 17, 0, 0)), 
            
				new Period(new DateTime(2015, 11, 10, 08, 0, 0), new DateTime(2015, 11, 10, 12, 0, 0)), 
				new Period(new DateTime(2015, 11, 10, 13, 0, 0), new DateTime(2015, 11, 10, 17, 0, 0)), 
            };
            CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
        }

		[TestMethod]
		public void TimeLineUtils_Generate2_Test()
		{
			IPeriod[] month11 = new IPeriod[]
			{
				new Period(new DateTime(2015, 11, 1), new DateTime(2015, 12, 1)), 
			};
			double duration;

			// генерировать периоды 12-часового графика работы c 01.11.2015 по 01.12.2015
			DateTime monthFinish = new DateTime(2015, 12, 1);

			// 1-я смена – с 20:00 31.10.2015 
            duration = TimeLineUtils.And(new IEnumerable<IPeriod>[]
				{
					month11,
					TimeLines.Generator.Generate(new DateTime(2015, 10, 31, 20, 0, 0), monthFinish, TimeSpan.FromHours(12), TimeSpan.FromHours(36))
				}
			).Sum(period => (period.End - period.Start).TotalHours);
			Assert.AreEqual(240, duration);

			// 2-я смена – с 8:00 01.11.2015  
            duration = TimeLineUtils.And(new IEnumerable<IPeriod>[]
				{
					month11,
					TimeLines.Generator.Generate(new DateTime(2015, 11, 01, 08, 0, 0), monthFinish, TimeSpan.FromHours(12), TimeSpan.FromHours(36))
				}
			).Sum(period => (period.End - period.Start).TotalHours);
			Assert.AreEqual(240, duration);

			// 3-я смена – с 20:00 01.11.2015 
            duration = TimeLineUtils.And(new IEnumerable<IPeriod>[]
				{
					month11,
					TimeLines.Generator.Generate(new DateTime(2015, 11, 01, 20, 0, 0), monthFinish, TimeSpan.FromHours(12), TimeSpan.FromHours(36))
				}
			).Sum(period => (period.End - period.Start).TotalHours);
			Assert.AreEqual(240, duration);
		}

		[TestMethod]
		public void TimeLineUtils_Transform_Test()
		{
			List<IPeriod> actual;
			List<Period> expected;
			DateTime lastStart = DateTime.MaxValue;

			// генерировать периоды 8-часового графика работы c 01 по 10.11.2015   
			actual = 
			(
				new IPeriod[]
				{
					new Period(new DateTime(2015, 11, 02, 08, 0, 0), new DateTime(2015, 11, 02, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 02, 13, 0, 0), new DateTime(2015, 11, 02, 17, 0, 0)), 

            		new Period(new DateTime(2015, 11, 03, 08, 0, 0), new DateTime(2015, 11, 03, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 03, 13, 0, 0), new DateTime(2015, 11, 03, 16, 0, 0)), 

					new Period(new DateTime(2015, 11, 05, 08, 0, 0), new DateTime(2015, 11, 05, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 05, 13, 0, 0), new DateTime(2015, 11, 05, 17, 0, 0)), 
            
					new Period(new DateTime(2015, 11, 06, 08, 0, 0), new DateTime(2015, 11, 06, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 06, 13, 0, 0), new DateTime(2015, 11, 06, 17, 0, 0)), 
            
					new Period(new DateTime(2015, 11, 09, 08, 0, 0), new DateTime(2015, 11, 09, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 09, 13, 0, 0), new DateTime(2015, 11, 09, 17, 0, 0)), 
            
					new Period(new DateTime(2015, 11, 10, 08, 0, 0), new DateTime(2015, 11, 10, 12, 0, 0)), 
					new Period(new DateTime(2015, 11, 10, 13, 0, 0), new DateTime(2015, 11, 10, 17, 0, 0)), 
				}
			)
			.Transform(
				period => 
					{
						if (period.Start.Date == lastStart)
							return null;
						else
						{
							lastStart = period.Start.Date;
							return new IPeriod[] { new Period(lastStart, TimeSpan.FromDays(1)) };
						}
					}
			)
			.ToList();
			expected = new List<Period>
			{
				new Period(new DateTime(2015, 11, 02), new DateTime(2015, 11, 03)), 
				new Period(new DateTime(2015, 11, 03), new DateTime(2015, 11, 04)), 
            	new Period(new DateTime(2015, 11, 05), new DateTime(2015, 11, 06)), 
            	new Period(new DateTime(2015, 11, 06), new DateTime(2015, 11, 07)), 
				new Period(new DateTime(2015, 11, 09), new DateTime(2015, 11, 10)), 
				new Period(new DateTime(2015, 11, 10), new DateTime(2015, 11, 11)), 
            };
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}
    }
}
