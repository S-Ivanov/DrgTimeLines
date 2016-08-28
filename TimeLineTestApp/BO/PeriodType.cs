namespace TimeLineTestApp
{
	/// <summary>
	/// Типы периодоа отработанного времени
	/// </summary>
	public enum PeriodType
	{
        /// <summary>
        /// Отработанное время, совпадающее с плановыми периодами
        /// </summary>
        Staff,

        /// <summary>
		/// Работа в выходные и нерабочие праздничные дни
		/// </summary>
		Freedays,

		/// <summary>
		/// Сверхурочные
		/// </summary>
		Overtime
	}
}
