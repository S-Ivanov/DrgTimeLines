using System;
using System.ComponentModel;

namespace TimeLineTestApp
{
	public class PeriodModel : IPeriodModel
    {
		public DateTime Start
		{
			get { return start; }
			set
			{
				if (value > End - minDuration)
					start = End - minDuration;
				else if (value < startLimit)
					start = startLimit;
				else
					start = value;
				OnPropertyChanged("Start");
			}
		}
		DateTime start;

		public DateTime End
		{
			get { return end; }
			set
			{
				if (value < Start + minDuration)
					end = Start + minDuration;
				else if (value > endLimit)
					end = endLimit;
				else
					end = value;
				OnPropertyChanged("End");
			}
		}
		DateTime end;

		public DateTime StartLimit
		{
			get { return startLimit; }
		}
		DateTime startLimit;

		public DateTime EndLimit
		{
			get { return endLimit; }
		}
		DateTime endLimit;

		TimeSpan minDuration;

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void SetPeriodProperties(DateTime start, DateTime startLimit, DateTime end, DateTime endLimit, TimeSpan minDuration)
		{
			this.start = start;
			this.startLimit = startLimit;
			this.end = end;
			this.endLimit = endLimit;
			this.minDuration = minDuration;
		}
	}
}
