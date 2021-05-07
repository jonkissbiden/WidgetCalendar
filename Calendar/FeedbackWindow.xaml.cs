using Calendar.Models;
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

namespace Calendar
{
    /// <summary>
    /// Логика взаимодействия для FeedbackForm.xaml
    /// </summary>
    public partial class FeedbackWindow : Window
    {
        public ClientInfo Info { get; private set; } = new ClientInfo();
        public FeedbackWindow()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            this.DataContext = Info;
        }
        private void FeedbackClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
