Используется адаптированная библиотека http://www.codeproject.com/Articles/240411/WPF-Timeline-Control-Part-I

Установка: C:\Проекты\TimeLines\CodeProjectTimelineDemo

ToDo:

+1. Сделать TimeLineControlBase.Readonly

+2. Добавить конец оси - TimeLineControlBase.EndDate
   см. TimeLineControlBase.DrawBackGround
   TimeLineControlBase.ReDrawChildren: 
     mover.TimeLineStartTime = start

+3. Ограничить перемещение и изменение длительности периодов справа концом оси.
   см. TimeLineControlBase.CreateTimeLineItemControl

4. Не показывать периоды, которые не вмещаются на оси. Которые вмещаются частично, показывать частично и как-то маркировать

5. Слева - дерево рядов:
+   Общие исходные
+     Праздники
+     Графики работы
+      ...
+   Индивидуально
+     Оклад
+	 Графики работы
+	 Фактическое время
+   Результаты:
+     Нормальное время
+	 Ночное время
+	 Сверхурочно
+	 Выходные/праздники

   Кастомизация TreeView:
   http://www.codeproject.com/Articles/124644/Basic-Understanding-of-Tree-View-in-WPF

+6. Заголовок временных рядов при отображении (дни)

7. Синхронизация вертикальных линий временных рядов при масштабировании
   см. TimeLineControlBase.DrawBackGround - возможно, сделать публичным и вызывать извне

8. Проверить для чего TimelineViewExpanded

9. Сделать минимальную длину периода

10. Редактирование данных - датагрид ?
    Mastering WPF DataGrid in a Day: Hour 8 CRUD Using DataGrid - http://www.c-sharpcorner.com/UploadFile/mahesh/mastering-wpf-datagrid-in-a-day-hour-8-crud-using-datagrid/

+11. Глобальные ресурсы приложения:
    https://msdn.microsoft.com/en-us/library/system.windows.application.resources(v=vs.110).aspx

12. Сравнение 2-х рядов, на выходе массив изменений: 
    [0] - периоды 1-го ряда, которые нужно изменить
    [1] - периоды 2-го ряда, которыми нужно изменить [0]
	[2] - периоды 1-го ряда, которые нужно удалить
	[3] - периоды 2-го ряда, которые нужно добавить

13. Обрезка ряда - Shrink (Left, Right, All)

14. Ограничения при изменении периодов ряда при редактировании

15. Коллекция с событиями изменения элементов: http://www.codeproject.com/Articles/660423/ObservableCollection-notification-on-member-change

+16. Перенсчет координат клика в TimeLineControl

