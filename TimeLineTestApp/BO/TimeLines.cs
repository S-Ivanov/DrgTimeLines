using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TimeLines;
using TimeLineTool;

namespace TimeLineTestApp
{
	public class TimeLine : ObservableCollection<ITimeLineDataItem>, IEnumerable<IPeriod>
	{
		#region Конструкторы

		public TimeLine(bool readOnly = true)
			: base()
		{
			ReadOnly = readOnly;
		}

		public TimeLine(IEnumerable<IPeriod> periods, Func<IPeriod, ITimeLineDataItem> timeLineItemCreate = null, bool readOnly = true)
		{
			if (periods == null)
				throw new ArgumentNullException("periods");

			this.timeLineItemCreate = 
                timeLineItemCreate ??
                (p => new TimeLineTestApp.Period { Begin = p.Begin, End = p.End });

			CopyFrom(periods);
			ReadOnly = readOnly;
		}

		#endregion Конструкторы

		#region Доработка для генерации событий изменения элементов списка
		// см. http://www.codeproject.com/Articles/660423/ObservableCollection-notification-on-member-change

		protected override void InsertItem(int index, ITimeLineDataItem item)
		{
			base.InsertItem(index, item);
			if (item is INotifyPropertyChanged)
				(item as INotifyPropertyChanged).PropertyChanged += Item_PropertyChanged;

			Item_PropertyChanged(this, new PropertyChangedEventArgs("Items"));
		}

		private void CopyFrom(IEnumerable<IPeriod> collection)
		{
			var items = Items;
			if (collection != null && items != null)
			{
				using (IEnumerator<IPeriod> enumerator = collection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						var current = enumerator.Current;
						ITimeLineDataItem item = current as ITimeLineDataItem;
						if (item == null)
						{
							if (timeLineItemCreate != null)
								item = timeLineItemCreate(current);
						}
						if (item == null)
							throw new ArgumentException("collection");

						items.Add(item);
						if (item is INotifyPropertyChanged)
							(item as INotifyPropertyChanged).PropertyChanged += Item_PropertyChanged;
					}
				}
			}
		}

		protected override void RemoveItem(int index)
		{
			var item = Items[index] as INotifyPropertyChanged;
			if (item != null)
				item.PropertyChanged -= Item_PropertyChanged;
			base.RemoveItem(index);
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			var removedItem = this[oldIndex] as INotifyPropertyChanged;
			if (removedItem == null)
				base.MoveItem(oldIndex, newIndex);
			else
			{
				base.RemoveItem(oldIndex);
				base.InsertItem(newIndex, removedItem as ITimeLineDataItem);
			}
		}

		protected override void ClearItems()
		{
			foreach (var item in Items)
			{
				if (item is INotifyPropertyChanged)
					(item as INotifyPropertyChanged).PropertyChanged -= Item_PropertyChanged;
			}
			base.ClearItems();
		}

		protected override void SetItem(int index, ITimeLineDataItem item)
		{
			INotifyPropertyChanged oldItem = Items[index] as INotifyPropertyChanged;
			if (oldItem != null)
				oldItem.PropertyChanged -= Item_PropertyChanged;
			if (item is INotifyPropertyChanged)
				(item as INotifyPropertyChanged).PropertyChanged += Item_PropertyChanged;
			base.SetItem(index, item);
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (ItemPropertyChanged != null) 
			{ 
				ItemPropertyChanged(sender, e);
			}
			this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
		}

		public event PropertyChangedEventHandler ItemPropertyChanged;

		#endregion Доработка для генерации событий изменения элементов списка

		public bool ReadOnly { get; private set; }

		#region Реализация интерфейса IEnumerable<IPeriod>

		IEnumerator<IPeriod> IEnumerable<IPeriod>.GetEnumerator()
		{
			return Items.Cast<IPeriod>().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion Реализация интерфейса IEnumerable<IPeriod>

		Func<IPeriod, ITimeLineDataItem> timeLineItemCreate;
	}

	public class Shedule : TimeLine
	{
		public Shedule(int code, IEnumerable<IPeriod> periods, bool readOnly = true)
			: base(periods, null, readOnly)
		{
			Code = code;
		}

		public int Code { get; private set; }

		public string Name
		{
			get
			{
				switch (Code)
				{
					case 0: return "40-часовая неделя";
					case 1: return "Смена №1";
					case 2: return "Смена №2";
					case 3: return "Смена №3";
					case 4: return "Смена №4";
					default: return string.Empty;
				}
			}
		}

        public TimeSpan SummaryDuration
        {
            get
            {
                if (summaryDuration == TimeSpan.Zero)
                {
                    summaryDuration = TimeSpan.FromMilliseconds(Items.Sum(p => (p.EndTime.Value - p.StartTime.Value).TotalMilliseconds));
                }
                return summaryDuration;
            }
        }
        TimeSpan summaryDuration = TimeSpan.Zero;
	}
}
