using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimeLineTool;

namespace TimeLineTestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

            // настройка временных рядов
            var paymentCalculator = Application.Current.Resources["PaymentCalculator"] as PaymentCalc;
			foreach (TimeLineControl timeLineControl in TimeLines.Items)
			{
                // настройка контролов
                timeLineControl.Height = int.Parse(ConfigurationManager.AppSettings["TimeLineControl.Height"]);
				timeLineControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                timeLineControl.MinimumUnitWidth = 20;
                timeLineControl.DrawTimeGrid = true;
                timeLineControl.MinWidth = 50;
                timeLineControl.SynchedWithSiblings = true;
                timeLineControl.SnapToGrid = true;
                timeLineControl.StartDate = paymentCalculator.StartDate;
                timeLineControl.EndDate = paymentCalculator.EndDate;

                // подключение обработчиков событий
				if (!timeLineControl.ReadOnly)
				{
					timeLineControl.MouseLeftButtonDown += TimeLine_Click;
					timeLineControl.PeriodClick += Period_Click;
				}
			}
        }

		void Period_Click(object sender, RoutedEventArgs e)
		{
			var period = (sender as TimeLineItemControl).Content as Period;
			PeriodPresenter presenter = PeriodPresenter.Create(period.GetType(), period, -1);
			if (presenter.View is Window)
			{
				(presenter.View as Window).Owner = this;
				(presenter.View as Window).ShowDialog();
			}
		}

		void TimeLine_Click(object sender, MouseButtonEventArgs e)
		{
			double x = e.GetPosition(sender as IInputElement).X;
			int nextIndex = -1;
			for (int i = 1; i < (sender as TimeLineControl).Children.Count; i++)
			{
				TimeLineItemControl child = (sender as TimeLineControl).Children[i] as TimeLineItemControl;
				double left = child.CalculateLeftPosition();
				if (x < left)
				{
					nextIndex = i - 1;
					break;
				}
			}

			Type periodType = null;
			switch ((sender as TimeLineControl).Name)
			{
				case "TimeLineShedules":
					periodType = typeof(ShedulePeriod);
					break;
				case "TimeLineSalaries":
					periodType = typeof(SalaryPeriod);
					break;
				case "TimeLineWorkPeriods":
					periodType = typeof(Period);
					break;
			}
			PeriodPresenter presenter = PeriodPresenter.Create(periodType, null, nextIndex);
			if (presenter.View is Window)
			{
				(presenter.View as Window).Owner = this;
				(presenter.View as Window).ShowDialog();
			}
		}

		public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
			TreeViewItem treeViewItem = e.Source as TreeViewItem;
			if (treeViewItem != null)
			{
				DoExpand(treeViewItem.Items);
			}
		}

		private void DoExpand(ItemCollection treeViewItems)
		{
			foreach (TreeViewItem treeViewItem in treeViewItems)
			{
				TimeLineControl timeLineControl = treeViewItem.Tag as TimeLineControl;
				if (timeLineControl != null)
					timeLineControl.Visibility = Visibility.Visible;
				if (treeViewItem.IsExpanded && treeViewItem.HasItems)
					DoExpand(treeViewItem.Items);
			}
		}

		private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
		{
			TreeViewItem treeViewItem = e.Source as TreeViewItem;
			if (treeViewItem != null)
			{
				DoCollapse(treeViewItem.Items);
			}
		}

		private void DoCollapse(ItemCollection treeViewItems)
		{
			foreach (TreeViewItem treeViewItem in treeViewItems)
			{
				TimeLineControl timeLineControl = treeViewItem.Tag as TimeLineControl;
				if (timeLineControl != null)
					timeLineControl.Visibility = Visibility.Collapsed;
				if (treeViewItem.HasItems)
					DoCollapse(treeViewItem.Items);
			}
		}

		//private void Slider_Scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		//{
		//	foreach (TimeLineControl x in TimeLines.Items)
		//	{
		//		//x.UnitSize = Slider_Scale.Value;
		//	}

		//	//TimeLine2.ApplyTemplate();
		//	//TimeLine3.ApplyTemplate();

		//	//TimeLine2.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
		//	//TimeLine3.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
		//}
	}
}
