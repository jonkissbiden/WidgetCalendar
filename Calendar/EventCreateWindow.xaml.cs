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
using Notifications.Wpf;

namespace Calendar
{
    /// <summary>
    /// Логика взаимодействия для EventCreateWindow.xaml
    /// </summary>
    public partial class EventCreateWindow : Window
    {
        public Event Event { get; private set; } = new Event() { Date = DateTime.Now, EndDate = DateTime.Now };
        public EventCreateWindow(DateTime date)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Event.Date = date;
            Event.EndDate = Event.Date;
            this.DataContext = Event;
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
            TimeCmbBox.SelectedIndex = 0;
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
            EndTimeCmbBox.SelectedIndex = 0;
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (Event.Title != null)
            {
                this.DialogResult = true;
            }
            else
            {
                App.notificationManager.Show(new NotificationContent
                {
                    Title = "MyOrganizer",
                    Message = Properties.Resources.NewEventNotificationMassage,
                    Type = NotificationType.Warning
                });
            }
        }
        private void TimeCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TimeSpan ts = TimeSpan.Parse(TimeCmbBox.SelectedItem.ToString());
            DateTime dt = Event.Date.Date;
            dt += ts;
            Event.Date = dt;
            Event.EndDate = dt;
            Event.Updated = DateTime.Now;
        }
        private void EndTimeCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TimeSpan ts = TimeSpan.Parse(EndTimeCmbBox.SelectedItem.ToString());
            DateTime dt = Event.EndDate.Date;
            dt += ts;
            Event.EndDate = dt;
            Event.Updated = DateTime.Now;
        }
    }
}
