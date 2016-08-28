using System;

namespace TimeLines
{
	/// <summary>
	/// Реализация простого периода
	/// </summary>
	public class Period : IPeriod, ICloneable
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		public Period()
		{
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="checkPeriod"></param>
		public Period(DateTime start, DateTime end, bool checkPeriod = false)
		{
			// проверить корректность границ периода
			if (checkPeriod && !PeriodUtils.Check(start, end))
				throw new ArgumentException("start");

			Start = start;
			End = end;
		}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        /// <param name="checkPeriod"></param>
        public Period(DateTime start, TimeSpan duration, bool checkPeriod = false)
            : this(start, start + duration, checkPeriod)
        {
        }

        /// <summary>
		/// Создание периода целых дней
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <param name="dayCount"></param>
		/// <returns></returns>
		public static Period Days(int year, int month, int day, int dayCount)
		{
			return new Period(new DateTime(year, month, day), TimeSpan.FromDays(dayCount));
		}

		/// <summary>
		/// Создание периода целых часов
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <param name="hour"></param>
		/// <param name="hourCount"></param>
		/// <returns></returns>
		public static Period Hours(int year, int month, int day, int hour, int hourCount)
		{
			return new Period(new DateTime(year, month, day, hour, 0, 0), TimeSpan.FromHours(hourCount));
		}

		#region Реализация интерфейса IPeriod

		/// <summary>
		/// Начало периода - включительно
		/// </summary>
		public DateTime Start { get; set; }

		/// <summary>
		/// Конец периода - исключительно
		/// </summary>
		public DateTime End { get; set; }

		#endregion Реализация интерфейса IPeriod

		#region Реализация интерфейса IClonable

		public virtual object Clone()
		{
			return new Period(Start, End);
		}

		#endregion Реализация интерфейса IClonable
	}

	/// <summary>
	/// Реализация параметризованного периода
	/// </summary>
	/// <typeparam name="T">тип ассоциированных с периодом данных</typeparam>
	/// <remarks>
	/// В классе не используется конструктор с опциональным параметром вида:
	///		public Period(DateTime begin, DateTime end, T data, bool checkPeriod = false)
	/// Причина - конструктор вызывается в методе TimeLine.Multiply через механизм рефлексии, который требует точного совпадения количества и типов параметров.
	/// Если задать значение по умолчанию параметра checkPeriod в методе TimeLine.Multiply, то связь с классом TimeLine необоснованно усилится.
	/// </remarks>
	public class Period<T> : Period, IPeriod<T>
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		public Period()
		{
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="start">начало периода - включительно</param>
		/// <param name="end">конец периода - исключительно</param>
		/// <param name="data">ассоциированные с периодом данные</param>
		public Period(DateTime start, DateTime end, T data)
			: this(start, end, data, false)
		{
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="start">начало периода - включительно</param>
		/// <param name="end">конец периода - исключительно</param>
		/// <param name="data">ассоциированные с периодом данные</param>
		/// <param name="checkPeriod"></param>
		public Period(DateTime start, DateTime end, T data, bool checkPeriod)
			: base(start, end, checkPeriod)
		{
			Data = data;
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="start">начало периода - включительно</param>
		/// <param name="duration">длительность периода</param>
		/// <param name="data">ассоциированные с периодом данные</param>
		/// <param name="checkPeriod"></param>
		public Period(DateTime start, TimeSpan duration, T data, bool checkPeriod = false)
			: this(start, start + duration, data, checkPeriod)
		{
		}

		#region Реализация интерфейса IClonable

		public override object Clone()
		{
			return new Period<T>(Start, End, Data);
		}

		#endregion Реализация интерфейса IClonable

		#region Реализация интерфейса IPeriod<T>

		/// <summary>
		/// Ассоциированные с периодом данные
		/// </summary>
		public T Data { get; set; }

		#endregion Реализация интерфейса IPeriod<T>
	}
}
