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

namespace Calendar.Views
{
    /// <summary>
    /// Логика взаимодействия для NoteView.xaml
    /// </summary>
    public partial class NoteView : Window
    {
        public Note Note { get; private set; } = new Note() { CreationDate = DateTime.Now };
        public NoteView(DateTime date)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Note.CreationDate = date;
            this.DataContext = Note;
        }
      

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (Note.NoteTitle != null)
            {
                this.DialogResult = true;
            }
            else
            {
                App.notificationManager.Show(new NotificationContent
                {
                    Title = "MyOrganizer",
                    Message = Properties.Resources.NoteFileMassage,
                    Type = NotificationType.Warning
                });
            }
        }
    }
}
