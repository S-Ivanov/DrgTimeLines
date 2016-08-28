using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TimeLines;

namespace UnitTests
{
    [TestClass]
    public class ListTimeLine_UnitTests
    {
        [TestMethod]
        public void ListTimeLine_BinarySearch_Test()
        {
            ListTimeLine timeLine = new ListTimeLine(
                new IPeriod[] { 
				    new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				    new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 8)), 
                }
            );

            int index;
            DateTime dt;

			dt = new DateTime(2015, 12, 4);
			index = timeLine.BinarySearch(dt);
			Assert.AreEqual(-2, index);
		
			dt = new DateTime(2015, 12, 7);
			index = timeLine.BinarySearch(dt);
			Assert.AreEqual(1, index);
		}

		[TestMethod]
		public void ListTimeLine_Add_Test()
		{
			ListTimeLine actual = new ListTimeLine(
				new IPeriod[] { 
				    new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				    new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 8)), 
                }
			);

			actual.Add(new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)));
			actual.Add(new Period(new DateTime(2015, 12, 9), new DateTime(2015, 12, 10)));

			ListTimeLine expected = new ListTimeLine(
				new IPeriod[] { 
				    new Period(new DateTime(2015, 12, 1), new DateTime(2015, 12, 3)), 
				    new Period(new DateTime(2015, 12, 4), new DateTime(2015, 12, 5)), 
				    new Period(new DateTime(2015, 12, 6), new DateTime(2015, 12, 8)), 
				    new Period(new DateTime(2015, 12, 9), new DateTime(2015, 12, 10)), 
                }
			);

			CollectionAssert.AreEqual(expected, actual, new PeriodComparer());
		}
	}
}
