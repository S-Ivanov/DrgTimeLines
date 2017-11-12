using System;

namespace TimeLines
{
	/// <summary>
	/// Интерфейс простого периода
	/// </summary>
	public interface IPeriod 
	{
		/// <summary>
		/// Начало периода - включительно
		/// </summary>
		DateTime Begin { get; set; }

		/// <summary>
		/// Конец периода - исключительно
		/// </summary>
		DateTime End { get; set; }
	}

	/// <summary>
	/// Интерфейс параметризованного периода
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IPeriod<T> : IPeriod
    {
		/// <summary>
		/// Ассоциированные с периодом данные
		/// </summary>
		T Data { get; set; }
    }
}
