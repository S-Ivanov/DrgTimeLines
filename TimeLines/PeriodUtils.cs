using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace TimeLines
{
	/// <summary>
	/// Методы для работы с периодами
	/// </summary>
	public static class PeriodUtils
	{
		public static IEnumerable<IPeriod> ToTimeLine(this IPeriod period)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			if (!period.Check())
				throw new ArgumentException("period");

			return new IPeriod[] { period };
		}

		/// <summary>
		/// Длительность периода
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		public static TimeSpan Duration(this IPeriod period)
		{
			if (period == null)
				throw new ArgumentNullException("period");

			return period.End - period.Start;
		}

		/// <summary>
		/// Вычитание нескольких периодов
		/// </summary>
		/// <param name="period">исходный период</param>
		/// <param name="periods">вычитаемые периоды - должен быть правильный временной ряд, т.е. периоды упорядочены по возрастанию их начала и не пересекаются между собой</param>
        /// <param name="copyPeriodFunc">фугкция копирования периода</param>
        /// <returns>периоды-результат = правильный временной ряд</returns>
		public static IEnumerable<IPeriod> Subtraction(this IPeriod period, IEnumerable<IPeriod> periods, Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc = null)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			if (periods == null)
				throw new ArgumentNullException("periods");
            
            if (copyPeriodFunc == null)
                copyPeriodFunc = (source, newStart, newEnd) => source.CreateAnalogue(newStart, newEnd);

			DateTime start = period.Start;
			foreach (var p in periods)
			{
				if (p.End <= period.Start) continue;
				if (p.Start >= period.End) break;

				if (p.End > period.Start && p.Start < period.End && p.Start > start)
                    yield return copyPeriodFunc(period, start, p.Start);
				start = p.End;
			}

			if (start == period.Start)
				yield return period;
			else if (start < period.End)
                yield return copyPeriodFunc(period, start, period.End);
		}

		/// <summary>
		/// Создание периода, аналогичного исходному
		/// </summary>
		/// <param name="period">исходный период</param>
		/// <param name="start">начало нового периода</param>
		/// <param name="end">конец нового периода</param>
		/// <param name="copyDataFunc">функция копирования данных из исходного периода в периоды-результат</param>
		/// <returns>период с заданными границами и данными, аналогичными исходному периоду</returns>
		public static IPeriod CreateAnalogue(this IPeriod period, DateTime start, DateTime end, Action<IPeriod, IPeriod> copyDataFunc = null)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			if (start > end)
				throw new ArgumentOutOfRangeException("start");

			if (start == end)
				return null;
			else
			{
				IPeriod newPeriod;
				if (period is ICloneable)
					// создание копии исходного периода с помощью инетрфейса ICloneable
					newPeriod = (period as ICloneable).Clone() as IPeriod;
				else
				{
					// создание копии исходного периода с помощью рефлексии
					Type periodType = period.GetType();
					MethodInfo copyMethod = periodType.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
					newPeriod = copyMethod.Invoke(period, null) as IPeriod;
				}

				// установка границ нового периода
				newPeriod.Start = start;
				newPeriod.End = end;

				// копирование данных исходного периода
				if (copyDataFunc != null)
					copyDataFunc(period, newPeriod);
				
				return newPeriod;
			}
		}

		/// <summary>
		/// Смещение периода
		/// </summary>
		/// <param name="period">исходный период</param>
		/// <param name="delta">смещение</param>
		/// <param name="copyPeriodFunc">фугкция копирования периода, если не задана, возвращается исходный период с измененными границами</param>
		/// <returns></returns>
        public static IPeriod Shift(this IPeriod period, TimeSpan delta, Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc = null)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			if (copyPeriodFunc == null)
				copyPeriodFunc = (p, start, end) => p.CreateAnalogue(start, end);

            if (delta == TimeSpan.Zero || (period.Start == DateTime.MinValue && period.End == DateTime.MaxValue))
                return copyPeriodFunc(period, period.Start, period.End);

			DateTime newPeriodStart = period.Start;
            DateTime newPeriodEnd = period.End;
            if (delta > TimeSpan.Zero)
            {
                if (period.Start > DateTime.MaxValue - delta)
                    return null;
                else
                {
					if (newPeriodStart > DateTime.MinValue)
						newPeriodStart += delta;
                    if (period.End > DateTime.MaxValue - delta)
                        newPeriodEnd = DateTime.MaxValue;
                    else
                        newPeriodEnd += delta;
                }
            }
            else // if (delta < TimeSpan.Zero)
            {
                if (period.End < DateTime.MinValue - delta)
                    return null;
                else
                {
                    if (period.Start < DateTime.MinValue - delta)
						newPeriodStart = DateTime.MinValue;
                    else
						newPeriodStart += delta;
                    if (newPeriodEnd < DateTime.MaxValue)
                        newPeriodEnd += delta;
                }
            }
			return newPeriodStart == newPeriodEnd ? null : copyPeriodFunc(period, newPeriodStart, newPeriodEnd);
        }

        /// <summary>
        /// Проверка корректности границ периода
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static bool Check(this IPeriod period)
        {
            if (period == null)
                throw new ArgumentNullException("period");
            return Check(period.Start, period.End);
        }

        /// <summary>
		/// Проверка корректности границ периода
		/// </summary>
		/// <param name="start">начало периода</param>
		/// <param name="end">конец периода</param>
		/// <returns></returns>
		public static bool Check(DateTime start, DateTime end)
		{
			return start < end;
		}

		/// <summary>
		/// Проверка эквивалентности периодов
		/// </summary>
		/// <param name="period"></param>
		/// <param name="other"></param>
		/// <param name="equalsDataFunc">функция проверки эквивалентности ассоциированных с периодом данных</param>
		/// <returns></returns>
		public static bool Equals(this IPeriod period, IPeriod other, Func<IPeriod, IPeriod, bool> equalsDataFunc = null)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			if (other == null)
				throw new ArgumentNullException("other");

			if (period.Start == other.Start && period.End == other.End)
				return equalsDataFunc == null ? true : equalsDataFunc(period, other);
			else
				return false;
		}

		/// <summary>
		/// Проверка пересечения одного периода с другим
		/// </summary>
		/// <param name="period"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool Intersected(this IPeriod period, IPeriod other)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			if (other == null)
				throw new ArgumentNullException("other");

			return period.Intersected(other.Start, other.End);
		}

		/// <summary>
		/// Проверка пересечения периода с датами, эквивалентными границам другого периода
		/// </summary>
		/// <param name="period"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static bool Intersected(this IPeriod period, DateTime start, DateTime end)
		{
			if (period == null)
				throw new ArgumentNullException("period");

			return period.Contains(start, true, false) || period.Contains(end, false, false);
		}

		/// <summary>
		/// Проверка попадания точки в период
		/// </summary>
		/// <param name="period"></param>
		/// <param name="point"></param>
		/// <param name="includeBegin"></param>
		/// <param name="includeEnd"></param>
		/// <returns></returns>
		public static bool Contains(this IPeriod period, DateTime point, bool includeBegin = true, bool includeEnd = true)
		{
			if (period == null)
				throw new ArgumentNullException("period");

			return (includeBegin ? point >= period.Start : point > period.Start) && (includeEnd ? point <= period.End : point < period.End);
		}

		/// <summary>
		/// Получить следующую точку пересечения периодов
		/// </summary>
		/// <param name="periods">проверяемые периода</param>
		/// <param name="prevPoint">точка пересечения, относительно которой выполняется проверка; если null, то ищется самая левая точка пересечения</param>
		/// <param name="nextPoint">результат</param>
		/// <returns>true, если следующая точка пересечения найдена</returns>
		public static bool GetNextPoint(IEnumerable<IPeriod> periods, DateTime? prevPoint, out DateTime nextPoint)
		{
			nextPoint = DateTime.MaxValue;
			bool periodsEmpty = true;
			foreach (var period in periods)
			{
				if (period == null)
					continue;
				else
				{
					periodsEmpty = false;
					DateTime dt =
						prevPoint == null ?
						period.Start :
						prevPoint.Value < period.Start ? period.Start : period.End;
					if (dt < nextPoint)
						nextPoint = dt;
				}
			}
			return !periodsEmpty;
		}
	}
}
