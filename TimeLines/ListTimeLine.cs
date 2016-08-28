using System;
using System.Collections;
using System.Collections.Generic;

namespace TimeLines
{
	/// <summary>
	/// Простой временной ряд - список периодов
	/// </summary>
	public class ListTimeLine : IList<IPeriod>, IList
	{
        /// <summary>
        /// Конструктор
        /// </summary>
		public ListTimeLine()
		{
		}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="periods">исходный временной ряд</param>
		public ListTimeLine(IEnumerable<IPeriod> periods)
		{
            if (!this.periods.Check())
                throw new ArgumentException("periods");
            
            this.periods.AddRange(periods);
		}

		#region Реализация интерфейса IList<IPeriod>

		public int IndexOf(IPeriod period)
		{
			return periods.IndexOf(period);
		}

		public void Insert(int index, IPeriod period)
		{
            if (period == null)
                throw new ArgumentNullException("period");
            if (period == null)
                throw new ArgumentNullException("period");
            if (!(0 <= index && index <= periods.Count))
                throw new ArgumentOutOfRangeException("index");

			// проверить новый период относительно соседей
            CheckPeriod(index == 0 ? null : periods[index - 1], period, index == periods.Count ? null : periods[index]);
            periods.Insert(index, period);
		}

		public void RemoveAt(int index)
		{
			DoRemoveAt(index);
		}

		public IPeriod this[int index]
		{
			get { return periods[index]; }
			set { SetValue(value, index); }
		}

        /// <summary>
        /// Добавление периода
        /// </summary>
        /// <param name="period"></param>
        /// <exception cref="ArgumentNullException">period == null</exception>
        /// <exception cref="ArgumentException">некорректный период period</exception>
        /// <exception cref="ArgumentOutOfRangeException">период period пересекается с одним или несколькими существующими периодами</exception>
		public void Add(IPeriod period)
		{
            if (period == null)
                throw new ArgumentNullException("period");
            if (!period.Check())
                throw new ArgumentException("period");

			int index = IndexForInsert(period.Start, period.End);
            if (index < 0)
                throw new ArgumentOutOfRangeException("period");
            else
                periods.Insert(index, period);
		}

		public void Clear()
		{
			DoClear();
		}

		public bool Contains(IPeriod period)
		{
			return periods.Contains(period);
		}

		public void CopyTo(IPeriod[] periods, int arrayIndex)
		{
            this.periods.CopyTo(periods, arrayIndex);
		}

		public int Count
		{
			get { return periods.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IPeriod period)
		{
			return periods.Remove(period);
		}

		public IEnumerator<IPeriod> GetEnumerator()
		{
			return periods.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return periods.GetEnumerator();
		}

		#endregion Реализация интерфейса IList<IPeriod>

		#region Реализация интерфейса IList

		int IList.Add(object period)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			IPeriod p = period as IPeriod;
			if (p == null)
				throw new ArgumentException("period");

			Add(p);
			return IndexOf(p);
		}

		void IList.Clear()
		{
			DoClear();
		}

		bool IList.Contains(object period)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			IPeriod p = period as IPeriod;
			if (p == null)
				throw new ArgumentException("period");

			return Contains(p);
		}

		int IList.IndexOf(object period)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			IPeriod p = period as IPeriod;
			if (p == null)
				throw new ArgumentException("period");

			return IndexOf(p);
		}

		void IList.Insert(int index, object period)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			IPeriod p = period as IPeriod;
			if (p == null)
				throw new ArgumentException("period");

			Insert(index, p);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		void IList.Remove(object period)
		{
			if (period == null)
				throw new ArgumentNullException("period");
			IPeriod p = period as IPeriod;
			if (p == null)
				throw new ArgumentException("period");

			Remove(p);
		}

		void IList.RemoveAt(int index)
		{
			DoRemoveAt(index);
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				IPeriod p = value as IPeriod;
				if (p == null)
					throw new ArgumentException("value");

				SetValue(p, index);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			int i = index;
			foreach (var period in periods)
			{
				array.SetValue(period, i);
				i++;
			}
		}

		int ICollection.Count
		{
			get { return periods.Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return null; }
		}

		#endregion Реализация интерфейса IList

        /// <summary>
        /// Добавление периода в конец временного ряда
        /// </summary>
        /// <param name="period"></param>
		public void Append(IPeriod period)
        {
            if (period == null)
                throw new ArgumentNullException("period");
            if (!period.Check())
                throw new ArgumentException("period");

            DateTime minStart = periods.Count == 0 ? DateTime.MinValue : periods[periods.Count - 1].End;
            if (!(minStart <= period.Start))
                throw new ArgumentException("period");

            periods.Add(period);
        }

        /// <summary>
        /// Поиск периода, содержащего указанную дату
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>аналогично List.BinarySearch</returns>
        public int BinarySearch(DateTime dt)
        {
			return periods.BinarySearch(new PointPeriod(dt), new PeriodBinarySearchComparer());
        }

		public void MovePeriod(IPeriod period, DateTime newStart, DateTime newEnd)
		{
			if (period == null)
				throw new ArgumentNullException("period");

			int indexOfPeriod = IndexOf(period);
			if (indexOfPeriod < 0)
				throw new ArgumentOutOfRangeException("period");

			MovePeriod(indexOfPeriod, newStart, newEnd);
		}

		public void MovePeriod(int indexOfPeriod, DateTime newStart, DateTime newEnd)
		{
			if (!(0 <= indexOfPeriod && indexOfPeriod < periods.Count))
				throw new ArgumentOutOfRangeException("indexOfPeriod");

			IPeriod period = periods[indexOfPeriod];

			int newIndex = IndexForInsert(newStart, newEnd);
			if (newIndex < 0)
				throw new ArgumentOutOfRangeException("newBegin");
			else
			{
				periods.RemoveAt(indexOfPeriod);
				period.Start = newStart;
				period.End = newEnd;
				periods.Insert(newIndex, period);
			}
		}

		#region Для BinarySearch

		class PointPeriod : IPeriod
		{
			public PointPeriod(DateTime dt)
			{
				this.dt = dt;
			}

			public DateTime Start
			{
				get { return dt; }
				set { dt = value; }
			}

			public DateTime End
			{
				get { return dt; }
				set { dt = value; }
			}

			DateTime dt;
		}

		class PeriodBinarySearchComparer : IComparer, IComparer<IPeriod>
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
				if (!(y is PointPeriod))
					throw new ArgumentException("y");

				DateTime dt = y.Start;
				if (x.Start <= dt && dt < x.End)
				{
					return 0;
				}
				else if (x.End <= dt)
					return -1;
				else // if (dt < x.Begin)
					return 1;
			}
		}

		#endregion Для BinarySearch

        /// <summary>
        /// Проверка корректности периода относительно соседей
        /// </summary>
        /// <param name="previousPeriod">сосед слева</param>
        /// <param name="period">проверяемый период</param>
        /// <param name="nextPeriod">сосед справа</param>
        /// <exception cref="ArgumentNullException">period == null</exception>
        /// <exception cref="ArgumentOutOfRangeException">period перекрывается с предыдущим или следующим периодом</exception>
		void CheckPeriod(IPeriod previousPeriod, IPeriod period, IPeriod nextPeriod)
		{
            if (period == null)
                throw new ArgumentNullException("period");

            bool ok = (previousPeriod == null ? DateTime.MinValue : previousPeriod.End) <= period.Start &&
                period.End <= (nextPeriod == null ? DateTime.MaxValue : nextPeriod.Start);

            if (!ok)
                throw new ArgumentOutOfRangeException("period");
		}

		int IndexForInsert(DateTime start, DateTime end)
		{
			int periodByBegin = BinarySearch(start);
			int periodByEnd = BinarySearch(end);
			return (periodByBegin >= 0 || periodByBegin != periodByEnd) ? -1 :  ~periodByBegin;
		}

		void DoClear()
		{
			periods.Clear();
		}

		void DoRemoveAt(int index)
		{
			periods.RemoveAt(index);
		}

		void SetValue(IPeriod period, int index)
		{
			// проверить новый период относительно соседей
			CheckPeriod(index == 0 ? null : periods[index - 1], period, index == periods.Count - 1 ? null : periods[index + 1]);
			periods[index] = period;
		}

		List<IPeriod> periods = new List<IPeriod>();
	}
}
