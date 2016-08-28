using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeLines
{
	/// <summary>
	/// Временной ряд
	/// </summary>
	public class TimeLine : IEnumerable<IPeriod>, IDisposable
	{
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="periods">периоды временного ряда</param>
		/// <param name="checkTimeLine">требуется ли проверка входного временного ряда</param>
		public TimeLine(IEnumerable<IPeriod> periods, bool checkTimeLine = false)
		{
			if (periods == null)
				throw new ArgumentNullException("periods");
			else if (checkTimeLine && !TimeLineUtils.Check(periods))
				throw new ArgumentException("periods");

			this.Periods = periods.ToList();
		}

		/// <summary>
		/// Периоды временного ряда
		/// </summary>
		public IEnumerable<IPeriod> Periods { get; private set; }

		#region Operators

        /// <summary>
        /// Операция ИЛИ
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator |(TimeLine first, TimeLine second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            return Or(first.Periods, second.Periods);
        }

        /// <summary>
        /// Операция ИЛИ
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator |(TimeLine first, IEnumerable<IPeriod> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            return Or(first.Periods, second);
        }

        /// <summary>
        /// Операция ИЛИ
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator |(IEnumerable<IPeriod> first, TimeLine second)
        {
            if (second == null)
                throw new ArgumentNullException("second");

            return Or(first, second.Periods);
        }

        /// <summary>
        /// Операция ИЛИ - функциональная форма
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
		public static TimeLine Or(IEnumerable<IPeriod> first, IEnumerable<IPeriod> second)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (second == null)
				throw new ArgumentNullException("second");

			return new TimeLine(TimeLineUtils.Or(new IEnumerable<IPeriod>[] { first, second }));
		}

		/// <summary>
		/// Операция И
		/// </summary>
		/// <param name="first">первый временной ряд</param>
		/// <param name="second">второй временной ряд</param>
		/// <returns></returns>
		public static TimeLine operator &(TimeLine first, TimeLine second)
		{
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            return And(first.Periods, second.Periods);
		}

        /// <summary>
        /// Операция И
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator &(TimeLine first, IEnumerable<IPeriod> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            return And(first.Periods, second);
        }

        /// <summary>
        /// Операция И
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator &(IEnumerable<IPeriod> first, TimeLine second)
        {
            if (second == null)
                throw new ArgumentNullException("second");

            return And(first, second.Periods);
        }

        /// <summary>
        /// Операция И - функциональная форма
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
		public static TimeLine And(IEnumerable<IPeriod> first, IEnumerable<IPeriod> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            return new TimeLine(TimeLineUtils.And(new IEnumerable<IPeriod>[] { first, second }));
        }

        /// <summary>
        /// Операция сложения
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator +(TimeLine first, TimeLine second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            return Addition(first.Periods, second.Periods);
        }

        /// <summary>
        /// Операция сложения
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator +(TimeLine first, IEnumerable<IPeriod> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            return Addition(first.Periods, second);
        }

        /// <summary>
        /// Операция сложения
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator +(IEnumerable<IPeriod> first, TimeLine second)
        {
            if (second == null)
                throw new ArgumentNullException("second");

            return Addition(first, second.Periods);
        }

        /// <summary>
		/// Операция сложения - функциональная форма
		/// </summary>
		/// <param name="first">первый временной ряд</param>
		/// <param name="second">второй временной ряд</param>
		/// <returns></returns>
		public static TimeLine Addition(IEnumerable<IPeriod> first, IEnumerable<IPeriod> second)
		{
            //if (first == null)
            //    throw new ArgumentNullException("first");
            //if (second == null)
            //    throw new ArgumentNullException("second");

            //return new TimeLine(
            //    TimeLineUtils.Or(
            //        new IEnumerable<IPeriod>[] { first, second },
            //        (start, finish, buffer) =>
            //        {
            //            if (buffer[0] != null && buffer[1] == null)
            //                return buffer[0];
            //            else if (buffer[0] == null && buffer[1] != null)
            //                return buffer[1];
            //            else
            //                return new Period<IPeriod[]>(start, finish, new IPeriod[] { buffer[0], buffer[1] });
            //        }
            //    )
            //    .ToTimeLine()
            //);

            return new TimeLine(TimeLineUtils.Join(first, second));
        }

        /// <summary>
        /// Операция вычитания
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator -(TimeLine first, TimeLine second)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            return Subtraction(first.Periods, second.Periods);
        }

        /// <summary>
        /// Операция вычитания
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator -(TimeLine first, IEnumerable<IPeriod> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            return Subtraction(first.Periods, second);
        }

        /// <summary>
        /// Операция вычитания
        /// </summary>
        /// <param name="first">первый временной ряд</param>
        /// <param name="second">второй временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator -(IEnumerable<IPeriod> first, TimeLine second)
        {
            if (second == null)
                throw new ArgumentNullException("second");

            return Subtraction(first, second.Periods);
        }

        /// <summary>
		/// Операция вычитания - функциональная форма
		/// </summary>
		/// <param name="first">первый временной ряд</param>
		/// <param name="second">второй временной ряд</param>
		/// <returns></returns>
		public static TimeLine Subtraction(IEnumerable<IPeriod> first, IEnumerable<IPeriod> second)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (second == null)
				throw new ArgumentNullException("second");

			return new TimeLine(first.Subtract(new IEnumerable<IPeriod>[] { second }));
		}

        /// <summary>
        /// Операция отрицания
        /// </summary>
        /// <param name="t">временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator !(TimeLine t)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            return UnaryNegation(t.Periods);
        }

        /// <summary>
        /// Операция отрицания
        /// </summary>
        /// <param name="t">временной ряд</param>
        /// <returns></returns>
        public static TimeLine operator -(TimeLine t)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            return UnaryNegation(t.Periods);
        }

        /// <summary>
		/// Операция отрицания - функциональная форма
		/// </summary>
		/// <param name="t">временной ряд</param>
		/// <returns></returns>
		public static TimeLine UnaryNegation(IEnumerable<IPeriod> t)
		{
			if (t == null)
				throw new ArgumentNullException("t");

			return new TimeLine(t.Not());
		}

        /// <summary>
        /// Операция умножения = привязка к периодам ассоциированных данных
        /// </summary>
        /// <param name="t">временной ряд</param>
        /// <param name="data">данные</param>
        /// <returns></returns>
        public static TimeLine operator *(TimeLine t, object data)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            return Multiply(t.Periods, data);
        }

        /// <summary>
		/// Операция умножения = привязка к периодам ассоциированных данных - функциональная форма
		/// </summary>
		/// <param name="t">временной ряд</param>
		/// <param name="data">данные</param>
		/// <returns></returns>
		public static TimeLine Multiply(IEnumerable<IPeriod> t, object data)
        {
			// При создании новых периодов если были данные, ассоциированные с исходными периодами, теряются
            return new TimeLine(TimeLineUtils.SetData(t, data));
        }

		#endregion Operators

		#region Реализация интерфейса IEnumerable<IPeriod>

		public IEnumerator<IPeriod> GetEnumerator()
		{
			return Periods.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Periods.GetEnumerator();
		}

		#endregion Реализация интерфейса IEnumerable<IPeriod>

        #region Реализация интерфейса IDisposable

        public void Dispose()
        {
            // dummy
        }

        #endregion Реализация интерфейса IDisposable
    }
}
