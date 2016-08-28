namespace TimeLineTestApp
{
	public class SalaryPeriodModel : PeriodModel, ISalaryPeriodModel
	{
		public decimal Salary
		{
			get { return salary; }
			set
			{
				salary = value;
				OnPropertyChanged("Salary");
			}
		}
		decimal salary;
	}
}
