namespace TimeLineTestApp
{
    public class PaymentInfo
    {
        /// <summary>
        /// Оклад
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// Коэффициент оплаты:
        /// для стандартных периодов = 1,
        /// если есть доплаты, то больше 1
        /// </summary>
        public decimal Factor { get; set; }

        /// <summary>
        /// Норма времени
        /// </summary>
        public double NormHours { get; set; }
    }
}
