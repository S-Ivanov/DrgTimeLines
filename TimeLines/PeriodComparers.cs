using System;
using System.Collections;
using System.Collections.Generic;

namespace TimeLines
{
	/// <summary>
	/// Простой компаратор периодов
	/// </summary>
	/// <remarks>
	/// Сначала сравниваются начала периодов. Если не равны - результат. Если равны, результат = сравнение концов периодов.
	/// </remarks>
	public class PeriodComparer : IComparer, IComparer<IPeriod>, IEqualityComparer<IPeriod>
	{
		#region Реализация интерфейса IComparer

		public int Compare(object x, object y)
		{
			return Compare(x as IPeriod, y as IPeriod);
		}

		#endregion Реализация интерфейса IComparer

		#region Реализация интерфейса IComparer<IPeriod>

		public int Compare(IPeriod x, IPeriod y)
		{
			if (x == null)
				throw new ArgumentNullException("x");
			if (y == null)
				throw new ArgumentNullException("y");

			int left = x.Begin.CompareTo(y.Begin);
			if (left != 0)
				return left;
			else
				return x.End.CompareTo(y.End);
		}

		#endregion Реализация интерфейса IComparer<IPeriod>

		#region Реализация интерфейса IEqualityComparer<IPeriod>

		public bool Equals(IPeriod x, IPeriod y)
		{
			return Compare(x, y) == 0;
		}

		public int GetHashCode(IPeriod obj)
		{
			return (obj as IPeriod).Begin.GetHashCode() ^ (obj as IPeriod).End.GetHashCode();
		}

		#endregion Реализация интерфейса IEqualityComparer<IPeriod>
	}

	/// <summary>
	/// Компаратор периодов по их началу
	/// </summary>
	public class PeriodComparerByBegin : IComparer, IComparer<IPeriod>
	{
		public int Compare(object x, object y)
		{
			return Compare(x as IPeriod, y as IPeriod);
		}

		public int Compare(IPeriod x, IPeriod y)
		{
			if (x == null)
				throw new ArgumentNullException("x");
			if (y == null)
				throw new ArgumentNullException("y");

			return x.Begin.CompareTo(y.Begin);
		}
	}
}
