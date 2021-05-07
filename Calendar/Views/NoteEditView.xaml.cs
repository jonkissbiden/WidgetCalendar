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
    /// Логика взаимодействия для NoteEditView.xaml
    /// </summary>
    public partial class NoteEditView : Window
    {
        public Note NoteCopy { get; private set; } = new Note();
        public Note UpdatedNote { get; private set; }
        public NoteEditView(Note n)
        {
            UpdatedNote = n;
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            NoteCopy.NoteText = n.NoteText;
            NoteCopy.NoteTitle = n.NoteTitle;
            this.DataContext = NoteCopy;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NoteCopy.NoteTitle != null)
            {
                UpdatedNote.NoteText = NoteCopy.NoteText;
                UpdatedNote.NoteTitle = NoteCopy.NoteTitle;
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
