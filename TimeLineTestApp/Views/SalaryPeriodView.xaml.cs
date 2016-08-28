using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TimeLineTestApp
{
	/// <summary>
	/// Interaction logic for SalaryPeriodView.xaml
	/// </summary>
	public partial class SalaryPeriodView : Window, ISalaryPeriodView
	{
		public SalaryPeriodView()
		{
			InitializeComponent();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			if (Validation.GetErrors(this.StartTextBox).Count != 0 || Validation.GetErrors(this.EndTextBox).Count != 0 || Validation.GetErrors(this.SalaryTextBox).Count != 0)
			{
				MessageBox.Show("Устраните ошибки ввода данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				// проверить корректность Salary
				if (SalaryChanged != null)
				{
					CancelEventArgs args = new CancelEventArgs();
					SalaryChanged(this, args);
					if (args.Cancel)
					{
						MessageBox.Show("Оклад должен быть больше 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
				}

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

		public event EventHandler<CancelEventArgs> SalaryChanged;
	}
}
