using System;
using System.ComponentModel;

namespace TimeLineTestApp
{
	public interface IPeriodModel : INotifyPropertyChanged
	{
		DateTime Start { get; set; }
		DateTime StartLimit { get; }
		DateTime End { get; set; }
		DateTime EndLimit { get; }
		void SetPeriodProperties(DateTime start, DateTime startLimit, DateTime end, DateTime endLimit, TimeSpan minDuration);
	}
}
