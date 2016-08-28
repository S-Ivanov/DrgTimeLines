using System;

namespace TimeLineTestApp
{
	public interface IPeriodView
	{
		void SetAddMode(bool add);
		object DataContext { get; set; }
		event EventHandler<EventArgs> InsertOrEdited;
		event EventHandler<EventArgs> Deleted;
	}
}
