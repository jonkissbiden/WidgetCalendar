using Calendar.Models;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Calendar.Models;

namespace Calendar
{
    /// <summary>
    /// Логика взаимодействия для EventEditWindow.xaml
    /// </summary>
    public partial class EventEditWindow : Window
    {
        public Event EventCopy { get; private set; } = new Event();
        public Event UpdatedEvent { get; private set; }
        public EventEditWindow(Event e)
        {
            Owner = Application.Current.MainWindow;
            EventCopy = new Event { Date = e.Date, Description = e.Description, EndDate = e.EndDate, GoogleId = e.GoogleId, Title = e.Title };
            InitializeComponent();
            UpdatedEvent = e;
            this.DataContext = EventCopy;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= 23; i++)
            {
                string[] minutes = { "00", "15", "30", "45" };
                for (int j = 0; j < minutes.Length; j++)
                {
                    if (i.ToString().Length == 1)
                    {
                        TimeCmbBox.Items.Add(String.Format("0{0}:{1}", i, minutes[j]));
                    }
                    else
                    {
                        TimeCmbBox.Items.Add(String.Format("{0}:{1}", i, minutes[j]));
                    }
                }
            }
            TimeCmbBox.Text = EventCopy.Date.ToString("HH:mm");
            for (int i = 0; i <= 23; i++)
            {
                string[] minutes = { "00", "15", "30", "45" };
                for (int j = 0; j < minutes.Length; j++)
                {
                    if (i.ToString().Length == 1)
                    {
                        EndTimeCmbBox.Items.Add(String.Format("0{0}:{1}", i, minutes[j]));
                    }
                    else
                    {
                        EndTimeCmbBox.Items.Add(String.Format("{0}:{1}", i, minutes[j]));
                    }
                }
            }
            EndTimeCmbBox.Text = EventCopy.EndDate.ToString("HH:mm");
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (EventCopy.Title != null)
            {
                UpdatedEvent.Date = EventCopy.Date;
                UpdatedEvent.Description = EventCopy.Description;
                UpdatedEvent.EndDate = EventCopy.EndDate;
                UpdatedEvent.Title = EventCopy.Title;
                UpdatedEvent.Updated = DateTime.Now;
                this.DialogResult = true;
            }
            else
            {
                App.notificationManager.Show(new NotificationContent
                {
                    Title = "MyOrganizer",
                    Message = Properties.Resources.EventEditNotificationMassage,
                    Type = NotificationType.Warning
                });
            }
        }
        private void TimeCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TimeSpan ts = TimeSpan.Parse(TimeCmbBox.SelectedItem.ToString());
            DateTime dt = EventCopy.Date.Date;
            dt += ts;
            EventCopy.Date = dt;
        }

        private void EndTimeCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TimeSpan ts = TimeSpan.Parse(EndTimeCmbBox.SelectedItem.ToString());
            DateTime dt = EventCopy.EndDate.Date;
            dt += ts;
            EventCopy.EndDate = dt;
        }
    }
}
