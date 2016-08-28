using System;
using System.ComponentModel;

namespace TimeLineTestApp
{
	public interface ISalaryPeriodView : IPeriodView
	{
		event EventHandler<CancelEventArgs> SalaryChanged;
	}
}
