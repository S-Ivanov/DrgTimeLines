using System;
using System.Windows;
using System.Windows.Controls;

namespace TimeLineTestApp
{
	/// <summary>
	/// Interaction logic for WorkPeriodView.xaml
	/// </summary>
	public partial class WorkPeriodView : Window, IPeriodView
	{
		public WorkPeriodView()
		{
			InitializeComponent();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			if (Validation.GetErrors(this.StartTextBox).Count != 0 || Validation.GetErrors(this.EndTextBox).Count != 0)
			{
				MessageBox.Show("Устраните ошибки ввода данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				if (InsertOrEdited != null)
					InsertOrEdited(this, EventArgs.Empty);

				this.DialogResult = true;
				Close();
			}
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (Deleted != null)
				Deleted(this, EventArgs.Empty);

			this.DialogResult = true;
			Close();
		}

		public void SetAddMode(bool add)
		{
			if (add)
			{
				this.Title = "Добавление периода";
				DeleteButton.Visibility = System.Windows.Visibility.Hidden;
			}
			else
			{
				this.Title = "Изменение/удаление периода";
				DeleteButton.Visibility = System.Windows.Visibility.Visible;
			}
		}

		public event EventHandler<EventArgs> InsertOrEdited;


		public event EventHandler<EventArgs> Deleted;
	}
}
