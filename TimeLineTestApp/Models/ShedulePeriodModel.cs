using System.Windows;

namespace TimeLineTestApp
{
	public class ShedulePeriodModel : PeriodModel, IShedulePeriodModel
	{
		public Shedule Shedule
		{
			get { return shedule; }
			set
			{
				shedule = value;
				OnPropertyChanged("Shedule");
			}
		}
		Shedule shedule;

		public Shedule[] Shedules
		{
			get { return (Application.Current.Resources["PaymentCalculator"] as PaymentCalc).Shedules; }
		}
	}
}
