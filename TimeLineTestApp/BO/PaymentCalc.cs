using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using TimeLines;

namespace TimeLineTestApp
{
	public class PaymentCalc : INotifyPropertyChanged
	{
		public PaymentCalc()
			: this(ConfigurationManager.AppSettings["PaymentData"])
		{
		}

		public PaymentCalc(string dataFileName)
			: this(PaymentData.Load(dataFileName))
		{
		}

		public PaymentCalc(PaymentData paymentData)
		{
			this.paymentData = paymentData;

			if (paymentData.Year != 2015)
				throw new ArgumentOutOfRangeException("paymentData.Year");

			shedules = new Shedule[5];
			shedules[0] = LoadShedule_8();
			for (int i = 1; i <= 4; i++)
			{
				shedules[i] = GenerateShedule_12(i); ;
			}

			Calculate();
		}

		#region Вспомогательные свойства - для отображения временных рядов

		/// <summary>
		/// Ограничения расчетного месяца
		/// </summary>
		/// <remarks>для нормального отображения пустых строк набора временных рядов</remarks>
		public TimeLineTestApp.TimeLine MonthLimits
		{
			get
			{
				return new TimeLineTestApp.TimeLine
                {
                    new TimeLineTestApp.Period { Start = StartDate, End = StartDate.AddSeconds(1) },
                    new TimeLineTestApp.Period { Start = EndDate.AddSeconds(-1), End = EndDate },
                };
			}
		}

		/// <summary>
		/// Все дни месяца - для заголовка временных рядов
		/// </summary>
		public TimeLineTestApp.TimeLine AllDays
		{
			get
			{
				return new TimeLineTestApp.TimeLine(
					TimeLines.Generator.Generate(StartDate, StartDate.AddMonths(1), TimeSpan.FromDays(1), TimeSpan.FromDays(1),
						(start, end) => new CalendarPeriod { Start = start, End = end, Data = start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday }));
			}
		}

		/// <summary>
		/// Начало расчетного периода
		/// </summary>
		public DateTime StartDate
		{
			get { return new DateTime(paymentData.Year, paymentData.Month, 1); }
		}

		/// <summary>
		/// Конец расчетного периода
		/// </summary>
		public DateTime EndDate
		{
			get { return StartDate.AddMonths(1); }
		}

		#endregion Вспомогательные свойства - для отображения временных рядов

		#region Общие исходные данные

		/// <summary>
		/// Праздники
		/// </summary>
		public TimeLineTestApp.TimeLine Holydays
		{
			get
			{
				if (holydays == null)
					holydays = LoadHolydays();
				return holydays;
			}
		}
		TimeLineTestApp.TimeLine holydays = null;

		/// <summary>
		/// Графики работы
		/// </summary>
		public Shedule[] Shedules
		{
			get { return shedules; }
		}
		Shedule[] shedules = null;

		#endregion Общие исходные данные

		#region Индивидуальные исходные данные

		/// <summary>
		/// Изменения графиков работы
		/// </summary>
		public TimeLineTestApp.TimeLine SheduleChanges
		{
			get
			{
				if (sheduleChanges == null)
				{
					sheduleChanges = LoadPeriods(
						paymentData.Shedules,
						(start, end, x) => new ShedulePeriod
						{
							Start = start,
							End = end,
							Data = shedules[(x as PaymentDataShedule).code]
						},
						x => 0 <= (x as PaymentDataShedule).code && (x as PaymentDataShedule).code < shedules.Length);
					sheduleChanges.CollectionChanged += sheduleChanges_CollectionChanged;
				}
				return sheduleChanges;
			}
		}
		TimeLineTestApp.TimeLine sheduleChanges = null;

		/// <summary>
		/// Изменения оклада
		/// </summary>
		public TimeLineTestApp.TimeLine Salaries
		{
			get
			{
				if (salaries == null)
				{
					salaries = LoadPeriods(
						paymentData.Salaries,
						(start, end, x) => new SalaryPeriod
						{
							Start = start,
							End = end,
							Data = (x as PaymentDataSalary).value
						});
					salaries.CollectionChanged += salaries_CollectionChanged;
				}
				return salaries;
			}
		}
		TimeLineTestApp.TimeLine salaries = null;

		/// <summary>
		/// Фактически отработанное время
		/// </summary>
		public TimeLineTestApp.TimeLine WorkPeriods
		{
			get
			{
				if (workPeriods == null)
				{
					workPeriods = LoadPeriods(
						paymentData.Work,
						(start, end, x) => new Period
						{
							Start = start,
							End = end
						});
					workPeriods.CollectionChanged += workPeriods_CollectionChanged;
				}
				return workPeriods;
			}
		}
		TimeLineTestApp.TimeLine workPeriods = null;

		#endregion Индивидуальные исходные данные

        #region Вспомогательные ряды

        /// <summary>
        /// Нормальные периоды
        /// </summary>
        public TimeLines.TimeLine NightHours
        {
            get 
            {
                if (nightHours == null)
                {
                    nightHours = new TimeLines.TimeLine(TimeLines.Generator.Generate(
                        StartDate.AddDays(-1).AddHours(22),
                        EndDate,
                        TimeSpan.FromHours(8),
                        TimeSpan.FromDays(1)));
                }
                return nightHours; 
            }
        }
        TimeLines.TimeLine nightHours = null;

        #endregion Вспомогательные ряды

        #region Результаты расчета

        /// <summary>
        /// Нормальные периоды
        /// </summary>
        public TimeLineTestApp.TimeLine NormalPeriods
        {
            get { return normalPeriods; }
			set
			{
				normalPeriods = value;
				OnPropertyChanged("NormalPeriods");
			}
        }
        TimeLineTestApp.TimeLine normalPeriods;

        /// <summary>
        /// Ночные периоды
        /// </summary>
        public TimeLineTestApp.TimeLine NightPeriods
        {
            get { return nightPeriods; }
			set
			{
				nightPeriods = value;
				OnPropertyChanged("NightPeriods");
			}
		}
        TimeLineTestApp.TimeLine nightPeriods;

        /// <summary>
        /// Работа в выходные дни
        /// </summary>
        public TimeLineTestApp.TimeLine FreedaysPeriods
        {
            get { return freedaysPeriods; }
			set
			{
				freedaysPeriods = value;
				OnPropertyChanged("FreedaysPeriods");
			}
		}
        TimeLineTestApp.TimeLine freedaysPeriods;

        /// <summary>
        /// Сверхурочная работа
        /// </summary>
        public TimeLineTestApp.TimeLine OvertimePeriods
        {
            get { return overtimePeriods; }
			set
			{
				overtimePeriods = value;
				OnPropertyChanged("OvertimePeriods");
			}
		}
        TimeLineTestApp.TimeLine overtimePeriods;

        public decimal NormalPayment
        {
            get { return normalPayment; }
            set
            {
                if (normalPayment != value)
                {
                    normalPayment = value;
                    OnPropertyChanged("NormalPayment");
                    OnPropertyChanged("AllPayment");
                }
            }
        }
        decimal normalPayment;

        public decimal NightPayment
        {
            get { return nightPayment; }
            set
            {
                if (nightPayment != value)
                {
                    nightPayment = value;
                    OnPropertyChanged("NightPayment");
                    OnPropertyChanged("AllPayment");
                }
            }
        }
        decimal nightPayment;

        public decimal FreedaysPayment
        {
            get { return freedaysPayment; }
            set
            {
                if (freedaysPayment != value)
                {
                    freedaysPayment = value;
                    OnPropertyChanged("FreedaysPayment");
                    OnPropertyChanged("AllPayment");
                }
            }
        }
        decimal freedaysPayment;

        public decimal OvertimePayment
        {
            get { return overtimePayment; }
            set
            {
                if (overtimePayment != value)
                {
                    overtimePayment = value;
                    OnPropertyChanged("OvertimePayment");
                    OnPropertyChanged("AllPayment");
                }
            }
        }
        decimal overtimePayment;

        public decimal AllPayment
        {
            get { return normalPayment + nightPayment + freedaysPayment + overtimePayment; }
        }

        #endregion Результаты расчета

		#region Обработчики событий изменения исходных данных

		void sheduleChanges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
            Calculate();
		}

		void salaries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
            Calculate();
		}

		void workPeriods_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
            Calculate();
		}

		#endregion Обработчики событий изменения исходных данных

        /// <summary>
        /// Основной расчет зарплаты
        /// </summary>
		public void Calculate()
		{
			// построить плановое время
            var planTime = SheduleChanges.Transform(p => TimeLineUtils.And(p.ToTimeLine(), (p as ShedulePeriod).Data));

            // маркировка периодов фактически отработанного времени
            var markPeriods = MarkPeriods_2(WorkPeriods, planTime);

            // подготовка данных для расчета - нормальные периоды
			NormalPeriods = PrepareResultPeriods(PaymentFactors.Normal, markPeriods.Where(p => (p as TimeLines.Period<PeriodType>).Data == PeriodType.Staff));
            // расчет начислений - нормальные периоды
            NormalPayment = CalculatePayment_2(NormalPeriods);

            // подготовка данных для расчета - доплата за работу в ночное время
            NightPeriods = PrepareResultPeriods(PaymentFactors.Night, GetNightPeriods_2(WorkPeriods, planTime, NightHours));
            // расчет начислений - доплата за работу в ночное время
            NightPayment = CalculatePayment_2(NightPeriods);

            // подготовка данных для расчета - сверхурочные
			OvertimePeriods = PrepareResultPeriods(PaymentFactors.Overtime, markPeriods.Where(p => (p as TimeLines.Period<PeriodType>).Data == PeriodType.Overtime));
            // расчет начислений - сверхурочные
            OvertimePayment = CalculatePayment_2(OvertimePeriods);

            // подготовка данных для расчета - работа в выходные/праздничные дни
			FreedaysPeriods = PrepareResultPeriods(PaymentFactors.Freedays, markPeriods.Where(p => (p as TimeLines.Period<PeriodType>).Data == PeriodType.Freedays));
            // расчет начислений - работа в выходные/праздничные дни
            FreedaysPayment = CalculatePayment_2(FreedaysPeriods);
        }

		/// <summary>
		/// Подготовка данных для расчета 
		/// </summary>
		/// <param name="factor">повышающий коэффициент</param>
		/// <param name="periods">периоды</param>
		/// <returns>[PaymentInfoPeriod]</returns>
        TimeLineTestApp.TimeLine PrepareResultPeriods(decimal factor, IEnumerable<IPeriod> periods)
        {
            return new TimeLineTestApp.TimeLine(
                TimeLines.TimeLineUtils.And(
                    new IEnumerable<IPeriod>[] { Salaries, SheduleChanges, periods },
                    (start, end, pp) => new PaymentInfoPeriod
                    {
                        Start = start,
                        End = end,
                        Data = new PaymentInfo 
                        { 
                            Factor = factor, 
                            Salary = (pp[0] as SalaryPeriod).Data,
                            NormHours = (pp[1] as ShedulePeriod).Data.SummaryDuration.TotalHours
                        }
                    }));
        }

		/// <summary>
		/// Маркировка периодов реально отработанного времени в соответствии с плановым временем работы
		/// </summary>
		/// <param name="realTime">реально отработанное время</param>
		/// <param name="planTime">плановое время работы</param>
		/// <returns>[Period<PeriodType>]</returns>
		TimeLines.TimeLine MarkPeriods(TimeLines.TimeLine realTime, TimeLines.TimeLine planTime)
		{
			using (var workDays = new TimeLines.TimeLine(planTime.Periods.ToDays()))
			{
                return
                    (realTime & planTime) * PeriodType.Staff +
                    (realTime & !workDays) * PeriodType.Freedays +
                    (realTime & workDays & !planTime) * PeriodType.Overtime;
			}
		}

        /// <summary>
        /// Маркировка периодов реально отработанного времени в соответствии с плановым временем работы
        /// </summary>
        /// <param name="realTime">реально отработанное время</param>
        /// <param name="planTime">плановое время работы</param>
        /// <returns>[Period<PeriodType>]</returns>
        IEnumerable<IPeriod> MarkPeriods_2(IEnumerable<IPeriod> realTime, IEnumerable<IPeriod> planTime)
        {
            var workDays = planTime.ToDays();
            return new TimeLines.TimeLine(TimeLineUtils.Join(
                TimeLineUtils.And(realTime, planTime).SetData(PeriodType.Staff),
                TimeLineUtils.And(realTime, workDays.Not()).SetData(PeriodType.Freedays),
                TimeLineUtils.And(realTime, workDays, planTime.Not()).SetData(PeriodType.Overtime)));
        }

        /// <summary>
		/// Получить ночные периоды
		/// </summary>
		/// <param name="markedPeriods">временной ряд [Period<PeriodType>] маркированных периодов</param>
		/// <param name="nightTime">ночное время</param>
		/// <returns></returns>
		TimeLines.TimeLine GetNightPeriods_1(TimeLines.TimeLine markedPeriods, TimeLines.TimeLine nightTime)
		{
			//return
			//	nightTime &
			//	from p in markedPeriods.Periods.Cast<IPeriod<PeriodType>>()
			//	where p.Data == PeriodType.Staff
			//	select p;

			return
				nightTime &
				from p in markedPeriods.Periods
				where ((dynamic)p).Data == PeriodType.Staff
				select p;
		}

		/// <summary>
		/// Получить ночные периоды
		/// </summary>
		/// <param name="realTime">реально отработанное время</param>
		/// <param name="planTime">плановое время работы</param>
		/// <param name="nightTime">ночное время</param>
		/// <returns></returns>
        IEnumerable<IPeriod> GetNightPeriods_2(IEnumerable<IPeriod> realTime, IEnumerable<IPeriod> planTime, IEnumerable<IPeriod> nightTime)
		{
			//return realTime & planTime & nightTime;
            return TimeLineUtils.And(realTime, planTime, nightTime);
		}

		/// <summary>
		/// Расчет начислений - суммирование начислений по всем периодам
		/// </summary>
		/// <param name="resultTimeLine">[PaymentInfoPeriod]</param>
		/// <returns></returns>
		decimal CalculatePayment_1(IEnumerable<IPeriod> resultTimeLine)
		{
			return resultTimeLine.Sum(p => (p as PaymentInfoPeriod).Payment);
		}

		/// <summary>
		/// Расчет начислений - суммирование длительности однотипных периодов, затем вычисление по их сумме
		/// </summary>
		/// <param name="resultTimeLine">[PaymentInfoPeriod]</param>
		/// <returns></returns>
        decimal CalculatePayment_2(IEnumerable<IPeriod> resultTimeLine)
        {
			return
				(
					from PaymentInfoPeriod period in resultTimeLine
					let data = period.Data
					group period by new
					{
						Salary = data.Salary,
						NormHours = data.NormHours,
						Factor = data.Factor
					} into g
					select new
					{
						Salary = g.Key.Salary,
						NormHours = g.Key.NormHours,
						Factor = g.Key.Factor,
						Duration = g.Sum(p => p.Duration().TotalHours)
					}
				).Sum(p => p.Factor * p.Salary * (decimal)(p.Duration / p.NormHours));
        }

		private TimeLineTestApp.TimeLine LoadHolydays()
		{
			var xx =
				from holyday in paymentData.Holydays.Holyday
				orderby holyday.begin
				select new HolydayPeriod
				{
					Start = holyday.begin,
					End = holyday.endSpecified ? holyday.end.AddDays(1) : holyday.begin.AddDays(1),
					TransferFrom = holyday.transferFromSpecified ? holyday.transferFrom : (DateTime?)null,
					Description = holyday.description
				};

			if (TimeLineUtils.Check(xx))
			{
				return new TimeLineTestApp.TimeLine(xx.Where(x => x.Start.Year == paymentData.Year && x.Start.Month == paymentData.Month));
			}
			else
				throw new ArgumentException("Holydays");
		}

		private TimeLineTestApp.TimeLine LoadPeriods(
			IEnumerable<PaymentDataPeriod> periods,
			Func<DateTime, DateTime, PaymentDataPeriod, Period> createPeriod,
			Func<PaymentDataPeriod, bool> filterPeriods = null)
		{
			if (filterPeriods == null)
				filterPeriods = p => true;

			return new TimeLineTestApp.TimeLine(
				from p in
				(
					from x in periods
					where filterPeriods(x)
					let start = x.beginSpecified ? (x.beginTimeSpecified ? x.begin.AddTicks(x.beginTime.Ticks) : x.begin) : StartDate
					orderby start
					select createPeriod(
						start,
						x.endSpecified ? (x.endTimeSpecified ? x.end.AddTicks(x.endTime.Ticks) : x.end) : EndDate, 
						x)
				)
				where p != null
				select p, 
				readOnly: false);
		}

		private Shedule GenerateShedule_12(int shiftNo)
		{

			DateTime startDate = StartDate;
			switch (shiftNo)
			{
				case 2:
					startDate = StartDate.AddDays(1);
					break;
				case 3:
					startDate = StartDate.AddDays(-2);
					break;
				case 4:
					startDate = StartDate.AddDays(-1);
					break;
			}

			int dayNo = 1;
			return new Shedule(shiftNo,
				TimeLines.TimeLineUtils.And(
					new IEnumerable<IPeriod>[]
					{
						Generator.Generate(
							startDate,
							EndDate,
							TimeSpan.FromHours(12),
							TimeSpan.FromDays(1),
							(start, end) =>
							{
								TimeSpan delta = TimeSpan.FromHours(8) + TimeSpan.FromHours(12 * (dayNo - 1));
								dayNo = dayNo < 4 ? dayNo + 1 : 1;
								return delta.TotalDays >= 1 ? null : new Period { Start = start + delta, End = end + delta };
							}),
						new Period[]
						{
							new Period { Start = StartDate, End = EndDate }
						}
					},
					(start, end) => new Period[] { new Period { Start = start, End = end } }));
		}

		private Shedule LoadShedule_8()
		{
			return new Shedule(0,
				GetShedule_8(
					StartDate, 
					EndDate,
					(Holydays as IEnumerable<IPeriod>).Select(d => d.Start.Date), 
					paymentData.PreHolydays.Select(d => d.value)));
		}

		public static IEnumerable<IPeriod> GetShedule_8(DateTime startDate, DateTime endDate, IEnumerable<DateTime> holydays, IEnumerable<DateTime> preHolydays)
		{
			return Generator.Generate(
				startDate.AddHours(8), 
				endDate, 
				TimeSpan.FromHours(8), 
				TimeSpan.FromDays(1), 
				(start, end) =>
				{
					if (preHolydays.Contains(start.Date))
						return new Period { Start = start, End = end.AddHours(-1) };
					else if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday || holydays.Contains(start.Date))
						return null;
					else
						return new Period { Start = start, End = end };
				});
		}

		PaymentData paymentData = null;

		void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
