using System;

namespace TimeLineTestApp
{
	public class SalaryPeriodPresenter : PeriodPresenter
	{
		public SalaryPeriodPresenter(Period period, int nextPeriodIndex, TimeSpan minDuration)
			: base(period, nextPeriodIndex, minDuration)
		{
		}

		protected override IPeriodView CreateView()
		{
			return new SalaryPeriodView();
		}

		protected override IPeriodModel CreateModel()
		{
			return new SalaryPeriodModel();
		}

		protected override TimeLine GetPeriods(PaymentCalc paymentCalculator)
		{
			return paymentCalculator.Salaries;
		}

		protected override void SetModelProperties(Period period)
		{
			(periodModel as ISalaryPeriodModel).Salary = period == null ? 10000 : (period as SalaryPeriod).Data;
		}

		protected override void SetViewProperties()
		{
			(periodView as ISalaryPeriodView).SalaryChanged += SalaryPeriodPresenter_SalaryChanged;
		}

		void SalaryPeriodPresenter_SalaryChanged(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = (periodModel as ISalaryPeriodModel).Salary <= 0;
		}

		protected override void SetPeriodProperties(Period period)
		{
			(period as SalaryPeriod).Data = (periodModel as ISalaryPeriodModel).Salary;
		}

		protected override Period CreatePeriod()
		{
			return new SalaryPeriod { Start = periodModel.Start, End = periodModel.End, Data = (periodModel as ISalaryPeriodModel).Salary };
		}

	}
}
