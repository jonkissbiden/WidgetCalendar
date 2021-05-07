using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IWshRuntimeLibrary;
using System.Xaml;
using System.Windows.Markup;

namespace Calendar
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        internal static void Start()
        {
            WshShell wsh = new WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\MyOrganizer.lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.Arguments = "";
            shortcut.TargetPath = Directory.GetCurrentDirectory() + "\\MyOrganizer.exe";
            shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
            shortcut.IconLocation = Directory.GetCurrentDirectory() + "\\Calendar.ico";
            shortcut.Save();
        }
        internal static void Delete()
        {
           System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\MyOrganizer.lnk");
        }



        private void AutoRun_Checked(object sender, RoutedEventArgs e)
        {
            Start();
            Properties.Settings.Default.AutoStart = true;
            Properties.Settings.Default.Save();
        }

        private void AutoRun_Unchecked(object sender, RoutedEventArgs e)
        {
            Delete();
            Properties.Settings.Default.AutoStart = false;
            Properties.Settings.Default.Save();
        }
        private void ShoveToBackground_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ToBackground = true;
            Properties.Settings.Default.Save();
        }

        private void ShoveToBackground_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ToBackground = false;
            Properties.Settings.Default.Save();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string newLanguege = "";
            int newIndex = 0;
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem cbi = (ComboBoxItem)comboBox.SelectedItem;
            string selectedItem = cbi.Content.ToString();
            switch (selectedItem)
            {
                case "English":
                    newLanguege = "en-US";
                    newIndex = 0;
                    break;
                case "Deutsche":
                    newLanguege = "de-DE";
                    newIndex = 1;
                    break;
                case "Français":
                    newLanguege = "fr-FR";
                    newIndex = 2;
                    break;
                case "Italiano":
                    newLanguege = "it-IT";
                    newIndex = 3;
                    break;
                case "España":
                    newLanguege = "es-ES";
                    newIndex = 4;
                    break;
            }
            Properties.Settings.Default.SelectesIndex = newIndex;
            Properties.Settings.Default.Language = newLanguege;
            Properties.Settings.Default.Save();
        }
    }
}
