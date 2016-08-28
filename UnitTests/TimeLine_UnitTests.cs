using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeLines;

namespace UnitTests
{
	[TestClass]
	public class TimeLine_UnitTests
	{
        [TestMethod]
        public void TimeLine_Multiply_Test()
        {
            TimeLine t1 = new TimeLine(
                new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
					new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 7)), 
				}
            );
            TimeLine t2 = t1 * 1;
            Assert.IsInstanceOfType(t2.Periods.First(), typeof(Period<int>));
        }

        [TestMethod]
		public void TimeLine_Addition_Test()
		{
			TimeLine t1;
			TimeLine t2;
			TimeLine actual;
			TimeLine expected;

			t1 = new TimeLine(
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
					new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 7)), 
				}
			);
			t2 = new TimeLine(
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 2), new DateTime(2015, 12, 3)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
				}
			);
			expected = new TimeLine(
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
					new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 7)), 
				}
			);
            //actual = t1 + t2;
            actual = new TimeLine((t1 + t2).Periods.Merge());
			CollectionAssert.AreEqual(expected.Periods.ToList(), actual.Periods.ToList(), new PeriodComparer());
		}

		[TestMethod]
		public void TimeLine_Subtraction_Test()
		{
			TimeLine t1;
			TimeLine t2;
			TimeLine actual;
			TimeLine expected;

			t1 = new TimeLine(
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 7)), 
				}
			);
			t2 = new TimeLine(
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 2)), 
					new Period(new DateTime(2015, 12, 5), new DateTime(2015, 12, 6)), 
				}
			);
			expected = new TimeLine(
				new List<IPeriod>
				{
					new Period(new DateTime(2015, 12, 2), new DateTime(2015, 12, 3)), 
					new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
					new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 7)), 
				}
			);
			actual = t1 - t2;
			CollectionAssert.AreEqual(expected.Periods.ToList(), actual.Periods.ToList(), new PeriodComparer());
		}
	}
}
