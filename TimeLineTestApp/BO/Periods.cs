using System;
using System.ComponentModel;
using TimeLines;
using TimeLineTool;

namespace TimeLineTestApp
{
	/// <summary>
	/// Простой период
	/// Используется:
	/// - графики работы
	/// - фактически отработанное время
	/// </summary>
	public class Period : IPeriod, ITimeLineDataItem, INotifyPropertyChanged
	{
		public Period()
		{
		}

		public Period(IPeriod period)
		{
			start = period.Start;
			end = period.End;
		}

		public DateTime? StartTime
		{
			get
			{
				return Start;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("StartTime");
				else
					Start = value.Value;
			}
		}

		public DateTime? EndTime
		{
			get
			{
				return End;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("EndTime");
				else
					End = value.Value;
			}
		}

		public DateTime Start 
		{
			get { return start; }
			set
			{
				if (start != value)
				{
					start = value;
					OnPropertyChanged("Start");
				}
			}
		}
		DateTime start;

		public DateTime End 
		{
			get { return end; }
			set
			{
				if (end != value)
				{
					end = value;
					OnPropertyChanged("End");
				}
			}
		}
		DateTime end;

        public TimeSpan Duration
        {
            get { return End - Start; }
        }

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class Period<T> : Period, IPeriod<T>, ITimeLineDataItem
	{
		public T Data 
		{
			get { return data; }
			set
			{
				data = value;
				OnPropertyChanged("Data");
			}
		}
		T data;
	}


	public class CalendarPeriod : Period<bool>
	{
	}

	/// <summary>
	/// Период для отображения праздников
	/// </summary>
	public class HolydayPeriod : Period
	{
		public string Description { get; set; }

		public DateTime? TransferFrom { get; set; }
	}

	/// <summary>
	/// Период для отображения изменений оклада
	/// </summary>
	public class SalaryPeriod : Period<decimal>
	{
	}

	/// <summary>
	/// Период для отображения изменений графиков работы
	/// </summary>
	public class ShedulePeriod : Period<Shedule>
	{
	}

    /// <summary>
    /// Период для отображения результатов расчета
    /// </summary>
    public class PaymentInfoPeriod : Period<PaymentInfo>
    {
        /// <summary>
        /// Начислено за период
        /// </summary>
        public decimal Payment
        {
            get { return Data.Factor * Data.Salary * (decimal)(this.Duration.TotalHours / Data.NormHours); }
        }
    }
}
