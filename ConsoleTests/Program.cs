using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Сравнительные тесты быстродействия TimePeriod и Time Lines");

			int count = 100000;
			Console.WriteLine("Количество операций: {0}", count);
			Console.WriteLine();

			//count = 100000;
			//Console.WriteLine("Вычисление разрывов между периодами: {0} операций", count);
			Console.WriteLine("Вычисление разрывов между периодами:");
			TimePeriodTests.Test1(count);
			TimeLineTests.Test1(count);
			Console.WriteLine();

			//count = 100000;
			//Console.WriteLine("Логическое сложение (OR): {0} операций", count);
			Console.WriteLine("Логическое сложение (OR):");
			TimePeriodTests.Test2(count);
			TimeLineTests.Test2(count);
			Console.WriteLine();

			//count = 100000;
			//Console.WriteLine("Логическое умножение (AND): {0} операций", count);
			Console.WriteLine("Логическое умножение (AND):");
			TimePeriodTests.Test3(count);
			TimeLineTests.Test3(count);
			Console.WriteLine();

			//count = 100000;
			//Console.WriteLine("Вычитание: {0} операций", count);
			Console.WriteLine("Вычитание:");
			TimePeriodTests.Test4(count);
			TimeLineTests.Test4(count);
			Console.WriteLine();


			Console.Write("Press Enter to exit...");
			Console.ReadLine();
		}

		public static void Test(bool isTimeLines, int count, Action testAction)
		{
			Console.Write((isTimeLines ? "Time Lines" : "TimePeriod") + ":");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < count; i++)
			{
				testAction();
			}

			stopwatch.Stop();
			Console.WriteLine(" {0} ms", stopwatch.ElapsedMilliseconds);
		}
	}
}
