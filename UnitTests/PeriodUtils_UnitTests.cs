using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeLines;
using System.Linq;

namespace UnitTests
{
	[TestClass]
	public class PeriodUtils_UnitTests
	{
		[TestMethod]
		public void PeriodUtils_Subtraction_Test()
		{
			Period period = new Period(new DateTime(2015, 12, 02), new DateTime(2015, 12, 08));
			IPeriod[] actual = period.Subtraction(
				new IPeriod[] 
				{ 
					new Period(new DateTime(2015, 11, 25), new DateTime(2015, 11, 30)),
					new Period(new DateTime(2015, 12, 01), new DateTime(2015, 12, 03)),
					new Period(new DateTime(2015, 12, 04), new DateTime(2015, 12, 05)),
					new Period(new DateTime(2015, 12, 07), new DateTime(2015, 12, 07)),
					new Period(new DateTime(2015, 12, 09), new DateTime(2015, 12, 10)),
				}
			)
			.ToArray();
			IPeriod[] expected = new IPeriod[] 
			{ 
				new Period(new DateTime(2015, 12, 03), new DateTime(2015, 12, 04)),
				new Period(new DateTime(2015, 12, 05), new DateTime(2015, 12, 08)),
			};
			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}

		[TestMethod]
		public void PeriodUtils_CreateAnalogue_Test()
		{
			Period<int> period = new Period<int>(new DateTime(2015, 12, 1), new DateTime(2015, 12, 8), 1);
			IPeriod actual = period.CreateAnalogue(
				new DateTime(2015, 12, 1), 
				new DateTime(2015, 12, 2),
				//(from, to) => (to as Period<int>).Data = (from as Period<int>).Data
				(from, to) => (to as Period<int>).Data = 2
				//(from, to) => { }
				//null
			);
			Assert.IsInstanceOfType(actual, typeof(Period<int>));
		}

		[TestMethod]
        public void PeriodUtils_Shift_Test()
		{
            Period period;
            TimeSpan delta;
            IPeriod actual;
            Period expected;

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 8));
            delta = TimeSpan.Zero;
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 8));
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));


            period = new Period(DateTime.MinValue, DateTime.MaxValue);
            delta = new TimeSpan(1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(DateTime.MinValue, DateTime.MaxValue);
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(DateTime.MinValue, new DateTime(2015, 12, 8));
            delta = new TimeSpan(1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(DateTime.MinValue, new DateTime(2015, 12, 9));
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(new DateTime(2015, 12, 7), DateTime.MaxValue);
            delta = new TimeSpan(1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(new DateTime(2015, 12, 8), DateTime.MaxValue);
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 8));
            delta = new TimeSpan(1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(new DateTime(2015, 12, 8), new DateTime(2015, 12, 9));
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));


            period = new Period(DateTime.MinValue, DateTime.MaxValue);
            delta = new TimeSpan(-1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(DateTime.MinValue, DateTime.MaxValue);
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(DateTime.MinValue, new DateTime(2015, 12, 8));
            delta = new TimeSpan(-1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(DateTime.MinValue, new DateTime(2015, 12, 7));
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(new DateTime(2015, 12, 7), DateTime.MaxValue);
            delta = new TimeSpan(-1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(new DateTime(2015, 12, 6), DateTime.MaxValue);
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 8));
            delta = new TimeSpan(-1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 7));
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 7));
            delta = new TimeSpan(-1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 6));
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 7));
            delta = new TimeSpan(1, 0, 0, 0);
            actual = period.Shift(delta, (p, start, finish) => new Period(start, finish));
            expected = new Period(new DateTime(2015, 12, 8), new DateTime(2015, 12, 8));
            Assert.IsTrue(PeriodUtils.Equals(expected, actual));
        }

        [TestMethod]
		public void PeriodUtils_Contains_Test()
		{
			Period period;
			DateTime point;
			bool includeStart;
			bool includeEnd;
			bool res;

            period = new Period(new DateTime(2015, 12, 7), DateTime.MaxValue);
            point = new DateTime(2015, 12, 7);
            includeStart = true;
            includeEnd = true;
            res = PeriodUtils.Contains(period, point, includeStart, includeEnd);
            Assert.IsTrue(res);

            period = new Period(new DateTime(2015, 12, 7), DateTime.MaxValue);
            point = new DateTime(2015, 12, 7);
            includeStart = false;
            includeEnd = true;
            res = PeriodUtils.Contains(period, point, includeStart, includeEnd);
            Assert.IsFalse(res);

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 9));
            point = new DateTime(2015, 12, 8);
            includeStart = false;
            includeEnd = false;
            res = PeriodUtils.Contains(period, point, includeStart, includeEnd);
            Assert.IsTrue(res);

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 9));
            point = new DateTime(2015, 12, 9);
            includeStart = false;
            includeEnd = false;
            res = PeriodUtils.Contains(period, point, includeStart, includeEnd);
            Assert.IsFalse(res);

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 9));
            point = new DateTime(2015, 12, 9);
            includeStart = false;
            includeEnd = true;
            res = PeriodUtils.Contains(period, point, includeStart, includeEnd);
            Assert.IsTrue(res);

            period = new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 7));
            point = new DateTime(2015, 12, 7);
            includeStart = true;
            includeEnd = false;
            res = PeriodUtils.Contains(period, point, includeStart, includeEnd);
            Assert.IsTrue(res);
        }

        [TestMethod]
		public void PeriodUtils_GetNextPoint_Test()
		{
			PrivateType privateType = new PrivateType(typeof(TimeLineUtils));

			DateTime? prevPoint;
			DateTime nextPoint;
			Period[] periods;
			bool res;

			periods =
				new Period[] 
					{ 
						new Period(new DateTime(2015, 12, 7), DateTime.MaxValue), 
						new Period(new DateTime(2015, 12, 8), DateTime.MaxValue) 
					};
			prevPoint = null;
			res = PeriodUtils.GetNextPoint(periods, prevPoint, out nextPoint);
			Assert.AreEqual(true, res);
			Assert.AreEqual(new DateTime(2015, 12, 7), nextPoint);

			periods =
				new Period[] 
					{ 
						new Period(new DateTime(2015, 12, 7), DateTime.MaxValue), 
						new Period(new DateTime(2015, 12, 8), DateTime.MaxValue) 
					};
			prevPoint = new DateTime(2015, 12, 7);
			res = PeriodUtils.GetNextPoint(periods, prevPoint, out nextPoint);
			Assert.AreEqual(true, res);
			Assert.AreEqual(new DateTime(2015, 12, 8), nextPoint);

			periods =
				new Period[] 
					{ 
						new Period(new DateTime(2015, 12, 7), new DateTime(2015, 12, 8)), 
						new Period(new DateTime(2015, 12, 9), DateTime.MaxValue) 
					};
			prevPoint = new DateTime(2015, 12, 7);
			res = PeriodUtils.GetNextPoint(periods, prevPoint, out nextPoint);
			Assert.AreEqual(true, res);
			Assert.AreEqual(new DateTime(2015, 12, 8), nextPoint);
		}

	}
}
