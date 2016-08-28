﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLineTool
{
	public interface ITimeLineDataItem
	{
		DateTime? StartTime { get; set; }
		DateTime? EndTime { get; set; }
		// Иванов - 04.02.2016 - закомментировано
		//Boolean TimelineViewExpanded { get; set; }
	}
}
