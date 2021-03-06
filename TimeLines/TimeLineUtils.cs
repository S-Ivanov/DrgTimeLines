﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeLines
{
	/// <summary>
	/// Методы для работы с временными рядами
	/// </summary>
	public static class TimeLineUtils
	{
        /// <summary>
        /// Привязка к периодам ассоциированных данных
        /// </summary>
        /// <param name="timeLine">временной ряд</param>
        /// <param name="dataType">тип данных</param>
        /// <param name="data">данные</param>
        /// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
        /// <returns></returns>
        public static IEnumerable<IPeriod> SetData(this IEnumerable<IPeriod> timeLine, Type dataType, object data, bool checkTimeLine = false)
        {
            if (timeLine == null)
                throw new ArgumentNullException("timeLine");
            if (checkTimeLine && !Check(timeLine))
                throw new ArgumentException("checkTimeLine");
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            Type periodType = (typeof(Period<>)).MakeGenericType(new Type[] { dataType });
            // При создании новых периодов если были данные, ассоциированные с исходными периодами, теряются
            return timeLine.Select(
                period =>
                {
                    object[] args = new object[] { period.Begin, period.End, data };
                    return Activator.CreateInstance(periodType, args) as IPeriod;
                }
            );
        }

        /// <summary>
        /// Привязка к периодам ассоциированных данных
        /// </summary>
        /// <param name="timeLine">временной ряд</param>
        /// <param name="data">данные</param>
        /// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
        /// <returns></returns>
        public static IEnumerable<IPeriod> SetData(this IEnumerable<IPeriod> timeLine, object data, bool checkTimeLine = false)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return SetData(timeLine, data.GetType(), data, checkTimeLine);
        }

        /// <summary>
        /// Поиск точки на временном ряду
        /// </summary>
        /// <param name="periods">временной ряд</param>
        /// <param name="dt"></param>
        /// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
        /// <returns>аналогично List.BinarySearch</returns>
        public static int IndexOf(this IEnumerable<IPeriod> timeLine, DateTime dt, bool checkTimeLine = false)
        {
            if (timeLine == null)
                throw new ArgumentNullException("timeLine");
            else if (checkTimeLine && !Check(timeLine))
                throw new ArgumentException("checkTimeLine");
            
            int index = 0;
            foreach (var period in timeLine)
            {
                if (dt < period.Begin)
                    return ~index;
                else if (period.Contains(dt, true, false))
                    return index;
                index++;
            }

            return ~index;
        }

		/// <summary>
		/// Представление набора периодов в виде временного ряда
		/// </summary>
		/// <param name="periods"></param>
        /// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
        /// <returns></returns>
        public static IEnumerable<IPeriod> ToTimeLine(this IEnumerable<IPeriod> periods, bool checkTimeLine = false)
		{
			var timeLine =
				from p in periods
				where p != null
				orderby p.Begin
				select p;

            if (!checkTimeLine || Check(timeLine))
				return timeLine;
			else
				throw new ArgumentException("periods");
		}

		/// <summary>
		/// Трансформация временного ряда
		/// </summary>
		/// <param name="timeLine">исходный временной ряд</param>
		/// <param name="transformPeriodFunc">функция копирования периода</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		/// <remarks>
		/// Каждый период исходного временного ряда преобразуется в 0..N новых периодов 
		/// </remarks>
		public static IEnumerable<IPeriod> Transform(this IEnumerable<IPeriod> timeLine, Func<IPeriod, IEnumerable<IPeriod>> transformPeriodFunc, bool checkTimeLine = false)
		{
			if (timeLine == null)
				throw new ArgumentNullException("timeLine");
			else if (checkTimeLine && !Check(timeLine))
				throw new ArgumentException("checkTimeLine");

			if (transformPeriodFunc == null)
				throw new ArgumentNullException("transformPeriodFunc");

			return TransformInternal(timeLine, transformPeriodFunc);
		}

		static IEnumerable<IPeriod> TransformInternal(this IEnumerable<IPeriod> timeLine, Func<IPeriod, IEnumerable<IPeriod>> transformPeriodFunc)
		{
			foreach (var period in timeLine)
			{
				var newPeriods = transformPeriodFunc(period);
				if (newPeriods != null)
				{
					foreach (var newPeriod in newPeriods)
					{
						if (newPeriod != null)
							yield return newPeriod;
					}
				}
			}
		}

		/// <summary>
		/// Смещение периодов временного ряда
		/// </summary>
		/// <param name="timeLine">временной ряд</param>
		/// <param name="delta">смещение</param>
		/// <param name="copyPeriodFunc">функция копирования периода</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
        /// <remarks>если функция копирования периода = null, выпоняется смещение периодов исходного ряда и новый ряд не форируется</remarks>
		public static IEnumerable<IPeriod> Shift(this IEnumerable<IPeriod> timeLine, TimeSpan delta, Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc = null, bool checkTimeLine = false)
		{
			if (timeLine == null)
				throw new ArgumentNullException("timeLine");
			else if (checkTimeLine && !Check(timeLine))
				throw new ArgumentException("checkTimeLine");

			return ShiftInternal(timeLine, delta, copyPeriodFunc);
		}

		static IEnumerable<IPeriod> ShiftInternal(IEnumerable<IPeriod> timeLine, TimeSpan delta, Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc)
		{
            foreach (var period in timeLine)
			{
                IPeriod newPeriod = period.Shift(delta, copyPeriodFunc);
                if (newPeriod != null)
                    yield return newPeriod;
			}
		}

		/// <summary>
		/// Слияние сочлененных периодов при условии равенства их данных
		/// </summary>
		/// <param name="timeLine">временной ряд</param>
		/// <param name="equalsDataFunc">функция проверки равенства ассоциированных данных</param>
		/// <param name="copyPeriodFunc">функция созданния нового периода</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Merge(this IEnumerable<IPeriod> timeLine, Func<IPeriod, IPeriod, bool> equalsDataFunc = null, Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc = null, bool checkTimeLine = false)
		{
			if (checkTimeLine)
			{
				if (!Check(timeLine))
					throw new ArgumentException("checkTimeLine");
			}
			else if (timeLine == null)
				throw new ArgumentNullException("timeLine");

			if (equalsDataFunc == null)
				equalsDataFunc = (p1, p2) => true;
			if (copyPeriodFunc == null)
				copyPeriodFunc = (p, start, end) => p.CreateAnalogue(start, end);

			return MergeInternal(timeLine, equalsDataFunc, copyPeriodFunc);
		}

		static IEnumerable<IPeriod> MergeInternal(IEnumerable<IPeriod> timeLine, Func<IPeriod, IPeriod, bool> equalsDataFunc, Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc)
		{
			DateTime newPeriodBegin = DateTime.MinValue;
			IPeriod previousPeriod = null;
			foreach (var period in timeLine)
			{
				if (previousPeriod == null)
				{
					previousPeriod = period;
					newPeriodBegin = period.Begin;
				}
				else
				{
					if (period.Begin == previousPeriod.End && equalsDataFunc(previousPeriod, period))
						previousPeriod = period;
					else
					{
						yield return copyPeriodFunc(previousPeriod, newPeriodBegin, previousPeriod.End);
						newPeriodBegin = period.Begin;
						previousPeriod = period;
					}
				}
			}

			if (previousPeriod != null)
				yield return copyPeriodFunc(previousPeriod, newPeriodBegin, previousPeriod.End);
		}

		/// <summary>
		/// Разбить временной ряд по длительности (с начала его первого периода)
		/// </summary>
		/// <param name="timeLine">исходный временной ряд</param>
		/// <param name="duration">контрольная длительность</param>
		/// <param name="copyPeriodFunc">функция создания периодов ряда-результата</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Split(this IEnumerable<IPeriod> timeLine, TimeSpan duration, Func<IPeriod, DateTime, DateTime, bool, IPeriod> copyPeriodFunc = null, bool checkTimeLine = false)
		{
			if (timeLine == null)
				throw new ArgumentNullException("timeLine");
			else if (checkTimeLine && !Check(timeLine))
				throw new ArgumentException("checkTimeLine");

			if (duration <= TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("duration");
			if (copyPeriodFunc == null)
				copyPeriodFunc = (p, begin, end, aboveDuration) => p.CreateAnalogue(begin, end);

			return SplitInternal(timeLine, duration, copyPeriodFunc);
		}

		static IEnumerable<IPeriod> SplitInternal(IEnumerable<IPeriod> timeLine, TimeSpan duration, Func<IPeriod, DateTime, DateTime, bool, IPeriod> copyPeriodFunc)
		{
			TimeSpan realDuration = TimeSpan.Zero;
			bool aboveDurationCurrent = false;
			foreach (var period in timeLine)
			{
				realDuration += period.End - period.Begin;
				if (aboveDurationCurrent || realDuration <= duration)
					yield return copyPeriodFunc(period, period.Begin, period.End, aboveDurationCurrent);
				else
				{
					TimeSpan periodDuration = realDuration - duration;
					yield return copyPeriodFunc(period, period.Begin, period.Begin + periodDuration, false);
					yield return copyPeriodFunc(period, period.Begin + periodDuration, period.End, true);
					aboveDurationCurrent = true;
				}
			}
		}

		/// <summary>
		/// Проверка корректности временного ряда
		/// </summary>
		/// <param name="timeLine"></param>
		/// <returns></returns>
		public static bool Check(this IEnumerable<IPeriod> timeLine)
		{
			if (timeLine == null)
				throw new ArgumentNullException("timeLine");

			DateTime? prevEnd = null;
			foreach (var period in timeLine)
			{
                if (period == null || !period.Check() || (prevEnd != null && period.Begin < prevEnd.Value))
					return false;
					
				prevEnd = period.End;
			}

			return true;
		}

        /// <summary>
        /// Объединение двух временных рядов
		/// </summary>
		/// <param name="first">первый временной ряд</param>
		/// <param name="second">второй временной ряд</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Join(this IEnumerable<IPeriod> first, IEnumerable<IPeriod> second, bool checkTimeLine = false)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			else if (checkTimeLine && !Check(first))
				throw new ArgumentException("checkTimeLine-first");

			if (second == null)
				throw new ArgumentNullException("second");
			else if (checkTimeLine && !Check(second))
				throw new ArgumentException("checkTimeLine-second");

			return Join(new IEnumerable<IPeriod>[] { first, second });
		}

        /// <summary>
        /// Объединение нескольких временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Join(params IEnumerable<IPeriod>[] timeLines)
        {
            return Join(timeLines, false);
        }

		/// <summary>
        /// Объединение нескольких временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Join(IEnumerable<IEnumerable<IPeriod>> timeLines, bool checkTimeLine = false)
		{
			if (timeLines == null)
				throw new ArgumentNullException("timeLines");
			if (checkTimeLine)
			{
				foreach (var timeLine in timeLines)
				{
					if (!Check(timeLine))
						throw new ArgumentException("checkTimeLine");
				}
			}

			IEnumerable<IPeriod> result = null;
			foreach (var timeLine in timeLines)
			{
				if (timeLine != null)
					result = result == null ? timeLine : Enumerable.Concat(result, timeLine);
			}

			if (result == null)
				return null;
			else
			{
				result = result.OrderBy(period => period.Begin);
				if (Check(result))
					return result;
				else
					throw new ArgumentException("timeLines");
			}
		}

		/// <summary>
		/// Логическое отрицание = инверсия временного ряда
		/// </summary>
		/// <param name="timeLine">исходный временной ряд</param>
		/// <param name="periodCreateFunc">функция генерации периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Not(this IEnumerable<IPeriod> timeLine, Func<DateTime, DateTime, IPeriod> periodCreateFunc = null, bool checkTimeLine = false)
		{
			if (timeLine == null)
				throw new ArgumentNullException("timeLine");
			else if (checkTimeLine && !Check(timeLine))
				throw new ArgumentException("checkTimeLine");

			if (periodCreateFunc == null)
				periodCreateFunc = (periodBegin, periodEnd) => new Period(periodBegin, periodEnd);

			return NotInternal(timeLine, periodCreateFunc, checkTimeLine, DateTime.MinValue, DateTime.MaxValue);
		}

		static IEnumerable<IPeriod> NotInternal(IEnumerable<IPeriod> timeLine, Func<DateTime, DateTime, IPeriod> periodCreateFunc, bool checkTimeLine, DateTime begin, DateTime end)
		{
			foreach (var period in timeLine)
			{
				end = period.Begin;
				if (begin < end)
					yield return periodCreateFunc(begin, end);
				begin = period.End;
			}
			if (begin < DateTime.MaxValue)
				yield return periodCreateFunc(begin, DateTime.MaxValue);
		}

        /// <summary>
        /// Логическое сложение временных рядов
        /// </summary>
        /// <param name="timeLines">исходные временные ряды</param>
        /// <returns></returns>
        public static IEnumerable<Period<IPeriod[]>> Or(params IEnumerable<IPeriod>[] timeLines)
        {
            return Or(timeLines, false);
        } 

		/// <summary>
		/// Логическое сложение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<Period<IPeriod[]>> Or(IEnumerable<IEnumerable<IPeriod>> timeLines, bool checkTimeLine = false)
		{
			return Or(timeLines, (begin, end, periodData) => new IPeriod[] { new Period<IPeriod[]>(begin, end, periodData) }, checkTimeLine).Cast<Period<IPeriod[]>>();
		} 

		/// <summary>
		/// Логическое сложение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="periodsCreateFunc">функция генерации периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Or(IEnumerable<IEnumerable<IPeriod>> timeLines, Func<DateTime, DateTime, IEnumerable<IPeriod>> periodsCreateFunc, bool checkTimeLine = false)
		{
			if (periodsCreateFunc == null)
				throw new ArgumentNullException("periodCreateFunc");

			return Or(timeLines, null, (begin, end, periodData) => periodsCreateFunc(begin, end), checkTimeLine);
		}

		/// <summary>
		/// Логическое сложение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="periodCreateFunc">функция генерации периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Or(IEnumerable<IEnumerable<IPeriod>> timeLines, Func<DateTime, DateTime, IPeriod[], IPeriod> periodCreateFunc, bool checkTimeLine = false)
		{
			if (periodCreateFunc == null)
				throw new ArgumentNullException("periodCreateFunc");

			return Or(
				timeLines,
				(begin, end, buffer, target) => CopyBufferPeriods(begin, end, buffer, target),
				(begin, end, periods) => new IPeriod[] { periodCreateFunc(begin, end, periods) },
				checkTimeLine
			);
		}

		/// <summary>
		/// Логическое сложение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="periodsCreateFunc">функция генерации периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Or(IEnumerable<IEnumerable<IPeriod>> timeLines, Func<DateTime, DateTime, IPeriod[], IEnumerable<IPeriod>> periodsCreateFunc, bool checkTimeLine = false)
		{
			if (periodsCreateFunc == null)
				throw new ArgumentNullException("periodCreateFunc");

			return Or(
				timeLines,
				(begin, end, buffer, target) => CopyBufferPeriods(begin, end, buffer, target),
				periodsCreateFunc,
				checkTimeLine
			);
		}

		/// <summary>
		/// Логическое сложение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="copyPeriodsAction">функция копирования периодов</param>
		/// <param name="periodsCreateFunc">функция генерации периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
        public static IEnumerable<IPeriod> Or(
			IEnumerable<IEnumerable<IPeriod>> timeLines, 
			Action<DateTime, DateTime, IPeriod[], IPeriod[]> copyPeriodsAction, 
			Func<DateTime, DateTime, IPeriod[], IEnumerable<IPeriod>> periodsCreateFunc,
			bool checkTimeLine = false)
		{
			return Combine(
				timeLines, 
				periodsCreateFunc, 
				(begin, end, buffer) => buffer.Any(period => period != null && period.Intersected(begin, end)), 
				copyPeriodsAction,
				checkTimeLine);
		}

        /// <summary>
        /// Логическое умножение временных рядов
        /// </summary>
        /// <param name="timeLines">исходные временные ряды</param>
        /// <returns></returns>
        public static IEnumerable<Period<IPeriod[]>> And(params IEnumerable<IPeriod>[] timeLines)
        {
            return And(timeLines, false);
        }

		/// <summary>
		/// Логическое умножение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<Period<IPeriod[]>> And(IEnumerable<IEnumerable<IPeriod>> timeLines, bool checkTimeLine = false)
		{
            Func<DateTime, DateTime, IPeriod[], IPeriod> periodsCreateFunc = (begin, end, periods) =>
            {
                var momentoPeriod = periods.FirstOrDefault(p => p.Begin == p.End);
                if (momentoPeriod != null)
                    begin = end = momentoPeriod.Begin;
                return new Period<IPeriod[]>(begin, end, periods);
            };
            return And(timeLines, periodsCreateFunc, checkTimeLine).Cast<Period<IPeriod[]>>();
        }

        /// <summary>
        /// Логическое умножение временных рядов
        /// </summary>
        /// <param name="timeLines">исходные временные ряды</param>
        /// <param name="periodsCreateFunc">функция генерации периодов</param>
        /// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
        /// <returns></returns>
        public static IEnumerable<IPeriod> And(IEnumerable<IEnumerable<IPeriod>> timeLines, Func<DateTime, DateTime, IEnumerable<IPeriod>> periodsCreateFunc, bool checkTimeLine = false)
		{
			if (periodsCreateFunc == null)
				throw new ArgumentNullException("periodCreateFunc");

			return And(timeLines, null, (begin, end, periodData) => periodsCreateFunc(begin, end), checkTimeLine);
		}

		/// <summary>
		/// Логическое умножение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="periodCreateFunc">функция генерации периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> And(IEnumerable<IEnumerable<IPeriod>> timeLines, Func<DateTime, DateTime, IPeriod[], IPeriod> periodCreateFunc, bool checkTimeLine = false)
		{
			if (periodCreateFunc == null)
				throw new ArgumentNullException("periodCreateFunc");

			return And(
				timeLines,
				(begin, end, buffer, target) => CopyBufferPeriods(begin, end, buffer, target),
				(begin, end, periods) => new IPeriod[] { periodCreateFunc(begin, end, periods) },
				checkTimeLine
			);
		}

		/// <summary>
		/// Копирование периодов
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <param name="source"></param>
		/// <param name="target"></param>
		static void CopyBufferPeriods(DateTime begin, DateTime end, IPeriod[] source, IPeriod[] target)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] != null && source[i].Intersected(begin, end))
				{
					target[i] = source[i];
				}
			}
		}

		/// <summary>
		/// Логическое умножение временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="copyPeriodsAction">функция копирования периодов</param>
		/// <param name="periodsCreateFunc">функция генерации периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
        public static IEnumerable<IPeriod> And(
			IEnumerable<IEnumerable<IPeriod>> timeLines, 
			Action<DateTime, DateTime, IPeriod[], IPeriod[]> copyPeriodsAction,
			Func<DateTime, DateTime, IPeriod[], IEnumerable<IPeriod>> periodsCreateFunc, 
			bool checkTimeLine = false)
		{
			return Combine(
				timeLines, 
				periodsCreateFunc,
				(begin, end, buffer) => buffer.All(period => period != null && period.Intersected(begin, end)), 
				copyPeriodsAction,
				checkTimeLine
			);
		}

        /// <summary>
        /// Вычитание из одного ряда другого ряда
        /// </summary>
        /// <param name="timeLine">исходный ряд</param>
        /// <param name="timeLines">вычитаемые ряды</param>
        /// <param name="copyPeriodFunc">функция копироания периода</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
        public static IEnumerable<IPeriod> Subtract(
			this IEnumerable<IPeriod> first, 
			IEnumerable<IPeriod> second, 
			Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc = null,
			bool checkTimeLine = false)
        {
            return Subtract(first, new IEnumerable<IPeriod>[] { second }, copyPeriodFunc, checkTimeLine);
        }

        /// <summary>
        /// Вычитание из указанного ряда одного или нескольких ременных рядов
        /// </summary>
        /// <param name="timeLine">исходный ряд</param>
        /// <param name="timeLines">вычитаемые ряды</param>
        /// <param name="copyPeriodFunc">функция копироания периода</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
        public static IEnumerable<IPeriod> Subtract(
			this IEnumerable<IPeriod> timeLine, 
			IEnumerable<IEnumerable<IPeriod>> timeLines, 
			Func<IPeriod, DateTime, DateTime, IPeriod> copyPeriodFunc = null,
			bool checkTimeLine = false)
        {
			if (timeLine == null)
				throw new ArgumentNullException("timeLine");
			else if (checkTimeLine && !Check(timeLine))
				throw new ArgumentException("checkTimeLine");

			if (timeLines == null)
				throw new ArgumentNullException("timeLines");
			if (checkTimeLine)
			{
				foreach (var t in timeLines)
				{
					if (!Check(t))
						throw new ArgumentException("checkTimeLine");
				}
			}

            if (copyPeriodFunc == null)
                copyPeriodFunc = (source, newStart, newEnd) => source.CreateAnalogue(newStart, newEnd);

            List<IEnumerable<IPeriod>> allTimeLines = new List<IEnumerable<IPeriod>> { timeLine };
            allTimeLines.AddRange(timeLines);
            if (allTimeLines.Count < 2)
                throw new ArgumentException("timeLines");

            return Combine(
                    allTimeLines,
                    periodCreateFunc: (begin, end, buffer) =>
                    {
                        if (buffer.Skip(1).Any(o => o != null && o.Intersected(begin, end)))
                            return (IPeriod)null;
                        else
                            //return buffer[0].Begin == buffer[0].End ? copyPeriodFunc(buffer[0], buffer[0].Begin, buffer[0].Begin) : copyPeriodFunc(buffer[0], begin, end);
                            return copyPeriodFunc(buffer[0], begin, end);
                    },
                    checkBufferFunc: (begin, end, buffer) =>
                        buffer[0] != null && buffer[0].Intersected(begin, end) // && buffer.Skip(1).Any(o => o != null))
                );
        }

		/// <summary>
		/// Пользовательская обработка периодов временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="periodCreateFunc">функция генерации периодов</param>
		/// <param name="checkBufferFunc">функция проверки буфера периодов</param>
		/// <param name="copyPeriodsAction">функция копирования периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Combine(
			IEnumerable<IEnumerable<IPeriod>> timeLines,
			Func<DateTime, DateTime, IPeriod[], IPeriod> periodCreateFunc = null,
			Func<DateTime, DateTime, IPeriod[], bool> checkBufferFunc = null,
			Action<DateTime, DateTime, IPeriod[], IPeriod[]> copyPeriodsAction = null,
			bool checkTimeLine = false)
		{
			if (periodCreateFunc == null)
				periodCreateFunc = (begin, end, pp) => new Period(begin, end);

			return Combine(
				timeLines,
				(begin, end, pp) => new IPeriod[] { periodCreateFunc(begin, end, pp) },
				checkBufferFunc,
				copyPeriodsAction,
				checkTimeLine
			);
		}

		/// <summary>
        /// Пользовательская обработка периодов временных рядов
		/// </summary>
		/// <param name="timeLines">исходные временные ряды</param>
		/// <param name="periodsCreateFunc">функция генерации периодов</param>
		/// <param name="checkBufferFunc">функция проверки буфера периодов</param>
		/// <param name="copyPeriodsAction">функция копирования периодов</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> Combine(
			IEnumerable<IEnumerable<IPeriod>> timeLines,
			Func<DateTime, DateTime, IPeriod[], IEnumerable<IPeriod>> periodsCreateFunc,
			Func<DateTime, DateTime, IPeriod[], bool> checkBufferFunc = null,
			Action<DateTime, DateTime, IPeriod[], IPeriod[]> copyPeriodsAction = null,
			bool checkTimeLine = false)
		{
			if (timeLines == null)
				throw new ArgumentNullException("timeLines");
			if (checkTimeLine)
			{
				foreach (var t in timeLines)
				{
					if (!Check(t))
						throw new ArgumentException("checkTimeLine");
				}
			}

			if (checkBufferFunc == null)
				checkBufferFunc = (begin, end, pp) => true;
			if (copyPeriodsAction == null)
				copyPeriodsAction = (begin, end, buffer, target) => CopyBufferPeriods(begin, end, buffer, target);
			if (periodsCreateFunc == null)
				periodsCreateFunc = (begin, end, pp) => new IPeriod[] { new Period(begin, end) };

			return CombineInternal(timeLines, periodsCreateFunc, checkBufferFunc, copyPeriodsAction);
		}

		static IEnumerable<IPeriod> CombineInternal(
			IEnumerable<IEnumerable<IPeriod>> timeLines, 
			Func<DateTime, DateTime, IPeriod[], IEnumerable<IPeriod>> periodsCreateFunc, 
			Func<DateTime, DateTime, IPeriod[], bool> checkBufferFunc, 
			Action<DateTime, DateTime, IPeriod[], IPeriod[]> copyPeriodsAction)
		{
			IEnumerator<IPeriod>[] enumerators = timeLines.Select(t => t == null ? null : t.GetEnumerator()).ToArray();

			IPeriod[] buffer = new IPeriod[enumerators.Length];

			// чтение периодов
			for (int i = 0; i < enumerators.Length; i++)
			{
				buffer[i] = (enumerators[i] == null || !enumerators[i].MoveNext()) ? null : enumerators[i].Current;
			}

			// определение самой левой точки = наименьший старт
			DateTime minBegin;
			if (!PeriodUtils.GetNextPoint(buffer, null, out minBegin))
				yield break;

			// взять следующую наименьшую точку
			DateTime prevPoint = minBegin;
			DateTime nextPoint;
			while (PeriodUtils.GetNextPoint(buffer, prevPoint, out nextPoint))
			{
				// проверить содержимое буфера, сместить промежуток prevPoint - nextPoint, если нет подходящих для него периодов
				if (prevPoint == nextPoint)
					break;
				else if (buffer.All(p => p == null || !p.Intersected(prevPoint, nextPoint)))
				{
					prevPoint = nextPoint;
					continue;
				}

				if (checkBufferFunc(prevPoint, nextPoint, buffer))
				{
					IPeriod[] periodData = new IPeriod[buffer.Length];
					copyPeriodsAction(prevPoint, nextPoint, buffer, periodData);
					var newPeriods = periodsCreateFunc(prevPoint, nextPoint, periodData);
					if (newPeriods != null)
					{
						foreach (var newPeriod in newPeriods)
						{
							if (newPeriod != null)
								yield return newPeriod;
						}
					}
				}
					
				// чтение периодов
				for (int i = 0; i < enumerators.Length; i++)
				{
					if (buffer[i] == null || buffer[i].End == nextPoint)
					{
						buffer[i] = (enumerators[i] == null || !enumerators[i].MoveNext()) ? null : enumerators[i].Current;
					}
				}

				if (prevPoint == nextPoint)
					break;
				else
					prevPoint = nextPoint;
			}
		}

		/// <summary>
		/// Проецирование периодов временного ряда в дни
		/// </summary>
		/// <param name="timeLine"></param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> ToDays(this IEnumerable<IPeriod> timeLine, bool checkTimeLine = false)
		{
            if (timeLine == null)
                throw new ArgumentNullException("timeLine");
            else if (checkTimeLine && !Check(timeLine))
                throw new ArgumentException("checkTimeLine");

            return ToDaysInternal(timeLine, checkTimeLine);
        }

		static IEnumerable<IPeriod> ToDaysInternal(IEnumerable<IPeriod> timeLine, bool checkTimeLine)
        {
            DateTime periodBeginDate = DateTime.MaxValue;
            foreach (var period in timeLine)
            {
                if (periodBeginDate == DateTime.MaxValue || periodBeginDate <= period.Begin.Date)
                    periodBeginDate = period.Begin.Date;
                else if (periodBeginDate < period.End)
                    periodBeginDate = periodBeginDate.AddDays(1);
                else //if (periodBeginDate >= period.End)
                    continue;

                DateTime periodEndDate = period.End.Date;
                if (period.End > periodEndDate)
                    periodEndDate = periodEndDate.AddDays(1);

                while (periodBeginDate < periodEndDate)
                {
                    yield return new Period(periodBeginDate, TimeSpan.FromDays(1));
                    periodBeginDate = periodBeginDate.AddDays(1);
                }
            }
		}

		/// <summary>
		/// Проецирование периодов временного ряда в месяцы
		/// </summary>
		/// <param name="timeLine"></param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		/// <returns></returns>
		public static IEnumerable<IPeriod> ToMonths(this IEnumerable<IPeriod> timeLine, bool checkTimeLine = false)
		{
			if (timeLine == null)
				throw new ArgumentNullException("timeLine");
			else if (checkTimeLine && !Check(timeLine))
				throw new ArgumentException("checkTimeLine");

			var firstPeriod = timeLine.FirstOrDefault();
			if (firstPeriod == null)
				return null;
			else
			{
				var lastPeriod = timeLine.LastOrDefault();
				DateTime monthsStart = new DateTime(firstPeriod.Begin.Year, firstPeriod.Begin.Month, 1);
				var months = Generator.Generate(
					monthsStart,
                    begin => begin < lastPeriod.End ? begin.AddMonths(1) : (DateTime?)null,
                    begin => new Period(begin, begin.AddMonths(1)));
				var xx = Combine(
					new IEnumerable<IPeriod>[] { months, timeLine },
					(begin, end, pp) => pp[1] == null ? null : new Period(new DateTime(begin.Year, begin.Month, 1), new DateTime(begin.Year, begin.Month, 1).AddMonths(1)));
				return xx.Distinct(new PeriodComparer());
			}
		}
	}
}
