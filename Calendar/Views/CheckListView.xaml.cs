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
    /// Логика взаимодействия для CheckListView.xaml
    /// </summary>
    public partial class CheckListView : Window
    {
        public CheckList CheckList { get; private set; } = new CheckList() { CreationDate = DateTime.Now };
        public List<CheckListItem> CheckListItems { get; private set; } = new List<CheckListItem>();

        public CheckListView(DateTime date)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            CheckList.CreationDate = date;
            this.DataContext = CheckList;
        }
        
        private void Checklist_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = PointToScreen(Mouse.GetPosition(null)).X;
            this.Top = PointToScreen(Mouse.GetPosition(null)).Y;
        }

       private void Accept_Click(object sender, RoutedEventArgs e)
       {
            if (!string.IsNullOrEmpty(ChkLstTextbox.Text))
            {
                this.DialogResult = true;
            }
       }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !string.IsNullOrEmpty(ChkLstTextbox.Text))
            {
                this.DialogResult = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

    }
}
