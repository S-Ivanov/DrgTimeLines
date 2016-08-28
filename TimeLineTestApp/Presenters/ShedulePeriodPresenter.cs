using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TimeLineTestApp
{
	public class ShedulePeriodPresenter : PeriodPresenter
	{
		public ShedulePeriodPresenter(Period period, int nextPeriodIndex, TimeSpan minDuration)
			: base(period, nextPeriodIndex, minDuration)
		{
		}

		protected override IPeriodView CreateView()
		{
			return new ShedulePeriodView();
		}

		protected override IPeriodModel CreateModel()
		{
			return new ShedulePeriodModel();
		}

		protected override TimeLine GetPeriods(PaymentCalc paymentCalculator)
		{
			return paymentCalculator.SheduleChanges;
		}

		protected override void SetModelProperties(Period period)
		{
			(periodModel as IShedulePeriodModel).Shedule = period == null ? (Application.Current.Resources["PaymentCalculator"] as PaymentCalc).Shedules[0] : (period as ShedulePeriod).Data;
		}

		protected override void SetPeriodProperties(Period period)
		{
			(period as ShedulePeriod).Data = (periodModel as IShedulePeriodModel).Shedule;
		}

		protected override Period CreatePeriod()
		{
			return new ShedulePeriod { Start = periodModel.Start, End = periodModel.End, Data = (periodModel as IShedulePeriodModel).Shedule };
		}

	}
}
