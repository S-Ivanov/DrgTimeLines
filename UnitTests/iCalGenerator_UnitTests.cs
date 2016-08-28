using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TimeLines;
using TimeLines.iCalGenerator;

namespace UnitTests
{
    [TestClass]
    public class iCalGenerator_UnitTests
    {
		[TestMethod]
        public void Generator_Generate_Test()
		{
            List<IPeriod> actual;
            List<Period> expected;

			// описание iCal определяет следующие правила генерации событий:
			// - первое событие - 02.11.2015 в 0:00
			// - частота генерации событий - ежедневно
			// - генерировать события до 10.11.2015 включительно
			// - не генерировать события 04.11.2015
			// - генерировать события только для дней недели: пн, вт, ср, чт, пт
            string template = 
@"BEGIN:VCALENDAR
VERSION:2.0
X-WR-CALNAME:Test 01
PRODID:-//Apple Computer\, Inc//iCal 2.0//EN
X-WR-RELCALID:8F2E90BA-42BA-49C4-8D14-4D4AD8C03A6A
X-WR-TIMEZONE:US/Pacific
CALSCALE:GREGORIAN
METHOD:PUBLISH
BEGIN:VEVENT
DTSTART;VALUE=DATE:20151102
EXDATE;VALUE=DATE:20151104
UID:4A2526B1-DCBB-451E-BB96-878895CA4946
DTSTAMP:20041015T171054Z
RRULE:FREQ=DAILY;UNTIL=20151110T000000Z;BYDAY=MO,TU,WE,TH,FR
END:VEVENT
END:VCALENDAR";

			// генерация временного ряда на основе описания iCal
            actual = TimeLines.iCalGenerator.Generator.Generate(
                template,
				new DateTime(2015, 11, 01),
				new DateTime(2015, 11, 11),
                (start) => 
					{
						// события генератора приходят в 0:00 каждого дня в соответствии с правилами генерации в описании iCal
						// генерируем перида (рабочего времени) 8:00-12:00 и 13:00-17:00
						// для 03.11.2015: 8:00-12:00 и 13:00-16:00 - сокращенный рабочий день
						return new Period[]
						{
							new Period(start.AddHours(8), TimeSpan.FromHours(4)),
							new Period(start.AddHours(13), TimeSpan.FromHours(start.Day == 3 ? 3 : 4)),
						};
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
    }
}
