using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TimeLines;

namespace TimeLineTestApp
{
	public class PeriodPresenter
	{
		public static PeriodPresenter Create(Type periodType, Period period, int nextPeriodIndex)
		{
			PeriodPresenter presenter = null;
			if (periodType == typeof(ShedulePeriod))
				presenter = new ShedulePeriodPresenter(period, nextPeriodIndex, TimeSpan.FromDays(1));
			else if (periodType == typeof(SalaryPeriod))
				presenter = new SalaryPeriodPresenter(period, nextPeriodIndex, TimeSpan.FromDays(1));
			else if (periodType == typeof(Period))
				presenter = new PeriodPresenter(period, nextPeriodIndex, TimeSpan.FromMinutes(30));
			return presenter;
		}

		public PeriodPresenter(Period period, int nextPeriodIndex, TimeSpan minDuration)
		{
            if (minDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("minDuration");

			this.periodView = CreateView();
			this.periodModel = CreateModel();

			this.period = period;
			this.nextPeriodIndex = nextPeriodIndex;
			this.minDuration = minDuration;

			Prepare();
		}

		public IPeriodView View
		{
			get { return periodView; }
		}

		protected virtual IPeriodView CreateView()
		{
			return new WorkPeriodView();
		}

		protected virtual IPeriodModel CreateModel()
		{
			return new PeriodModel();
		}

		protected virtual TimeLine GetPeriods(PaymentCalc paymentCalculator)
		{
			return paymentCalculator.WorkPeriods;
		}
			
		void Prepare()
		{
			var paymentCalculator = Application.Current.Resources["PaymentCalculator"] as PaymentCalc;
			var periods = GetPeriods(paymentCalculator);

			DateTime startLimit = paymentCalculator.StartDate;
			DateTime endLimit = paymentCalculator.EndDate;

			if (period == null)
			{
				periodView.SetAddMode(true);

				if (nextPeriodIndex < 0)
				{
					var lastPeriod = (periods as IEnumerable<IPeriod>).LastOrDefault();
					if (lastPeriod != null)
						startLimit = lastPeriod.End;
				}
				else
				{
					endLimit = (periods[nextPeriodIndex] as IPeriod).Begin;
					if (nextPeriodIndex > 0)
						startLimit = (periods[nextPeriodIndex - 1] as IPeriod).End;
				}

				periodModel.SetPeriodProperties(startLimit, startLimit, endLimit, endLimit, minDuration);
			}
			else
			{
				periodView.SetAddMode(false);

				int periodIndex = periods.IndexOf(period);
				if (periodIndex > 0)
					startLimit = (periods[periodIndex - 1] as IPeriod).End;
				if (periodIndex < periods.Count - 1)
					endLimit = (periods[periodIndex + 1] as IPeriod).Begin;

				periodModel.SetPeriodProperties(period.Begin, startLimit, period.End, endLimit, minDuration);
			}

			SetModelProperties(period);

			periodView.DataContext = periodModel;
			periodView.InsertOrEdited += periodView_InsertOrEdited;
			periodView.Deleted += periodView_Deleted;

			SetViewProperties();
		}

		protected virtual void SetViewProperties()
		{
		}

		protected virtual void SetModelProperties(Period period)
		{
		}

		void periodView_Deleted(object sender, EventArgs e)
		{
			// конечное действие - удаление периода
			var paymentCalculator = Application.Current.Resources["PaymentCalculator"] as PaymentCalc;
			var periods = GetPeriods(paymentCalculator);

			periods.Remove(period);
		}

		void periodView_InsertOrEdited(object sender, EventArgs e)
		{
			// конечное действие - изменение или добавление периода
			var paymentCalculator = Application.Current.Resources["PaymentCalculator"] as PaymentCalc;
			var periods = GetPeriods(paymentCalculator);

			if (period == null)
				periods.Insert(~periods.IndexOf(periodModel.Start), CreatePeriod());
			else
			{
				// конечное действие - изменение периода
				period.Begin = periodModel.Start;
				period.End = periodModel.End;
				SetPeriodProperties(period);
			}
		}

		protected virtual void SetPeriodProperties(Period period)
		{
		}

		protected virtual Period CreatePeriod()
		{
			return new Period { Begin = periodModel.Start, End = periodModel.End };
		}

		/// <summary>
		/// Период для редактирования/удаления
		/// </summary>
		/// <remarks>
		/// Если == null, выполняется добавление нового периода
		/// Иначе - редактирование или удаление существующего (режим задается пользователем)
		/// </remarks>
		Period period;

		int nextPeriodIndex;

		protected IPeriodView periodView;
		protected IPeriodModel periodModel;
		TimeSpan minDuration;
	}
}
