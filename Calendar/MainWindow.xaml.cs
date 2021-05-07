using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Calendar.ViewModels;
using System.Windows.Markup;
using Calendar.ViewModels;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NoteViewModel NoteViewModel = new NoteViewModel();
        NotifyIcon notifyIcon = new NotifyIcon();
        CheckListViewModel _checkListViewModel = new CheckListViewModel();

        ResourceDictionary currentTheme = new ResourceDictionary() { Source = new Uri(Properties.Settings.Default.Theme, UriKind.Relative) };
        public MainWindow()
        {
            App.Current.MainWindow.WindowStyle = WindowStyle.None;
            App.Current.Resources.MergedDictionaries.Add(currentTheme);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            InitializeComponent();
            notifyIcon.Icon = new System.Drawing.Icon("favicon.ico");
            notifyIcon.Click += notifyIcon_Click;
            ToolTipIcon icon = new ToolTipIcon();
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(Properties.Resources.ContextMenuClose, null, CloseClicked);
            notifyIcon.ContextMenuStrip.Items.Add(Properties.Resources.ContextMenuSettings, null, OpenSettingsWindow);
            notifyIcon.ContextMenuStrip.Items.Add(Properties.Resources.ContextMenuPinUnpin, null, PinUnpin);
            this.DataContext = new ApplicationViewModel();
        }
        /// <summary>
        private IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y,
        int cx, int cy, uint uFlags);

        public const int HWND_BOTTOM = 0x1;
        public const uint SWP_NOSIZE = 0x1;
        public const uint SWP_NOMOVE = 0x2;
        public const uint SWP_SHOWWINDOW = 0x40;

        private void ShoveToBackground()
        {
            SetWindowPos((int)this.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE |
                SWP_NOSIZE | SWP_SHOWWINDOW);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion
        /// </summary>

        private void ThemeChange(object sender, EventArgs e)
        {
            string SwitchNom;
            string newTheme = "";
            MenuItem menuItem = (MenuItem)sender;
            SwitchNom = menuItem.Tag.ToString();
            switch (SwitchNom)
            {
                case "Blue":
                    newTheme = "Themes/Blue.xaml";
                    break;
                case "Pink":
                    newTheme = "Themes/Pink.xaml";
                    break;
                case "White":
                    newTheme = "Themes/White.xaml";
                    break;
                case "Black":
                    newTheme = "Themes/Black.xaml";
                    break;
                case "USA":
                    newTheme = "Themes/USA.xaml";
                    break;
                case "NHL":
                    newTheme = "Themes/NHL.xaml";
                    break;
                case "NBA":
                    newTheme = "Themes/NBA.xaml";
                    break;
                case "NFL":
                    newTheme = "Themes/NFL.xaml";
                    break;
                case "MLB":
                    newTheme = "Themes/MLB.xaml";
                    break;
            }
            System.Collections.ObjectModel.Collection<ResourceDictionary> res = App.Current.Resources.MergedDictionaries;
            App.Current.Resources.MergedDictionaries.Remove(currentTheme);
            currentTheme = new ResourceDictionary() { Source = new Uri(newTheme, UriKind.Relative) };
            App.Current.Resources.MergedDictionaries.Add(currentTheme);

            Properties.Settings.Default.Theme = currentTheme.Source.ToString();
            Properties.Settings.Default.Save();
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return XmlLanguage.GetLanguage((string)parameter);
        }
        private void OpenSettingsWindow(object sender, EventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }
        
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)e).Button == MouseButtons.Right) return;
            WindowState = (WindowState)FormWindowState.Normal;
            Activate();
        }
        private void CloseClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            notifyIcon.Icon = null;
            System.Windows.Application.Current.Shutdown();
        }
        void CheckUpdates()
        {
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 120;
            AutoUpdater.ReportErrors = false;
            AutoUpdater.Start("https://s3.eu-west-3.amazonaws.com/www.myorganaizer.com/Download/UpdateCheck.xml");
        }
        private void PinUnpin(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.isPinned == true)
            {
                Properties.Settings.Default.isPinned = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.isPinned = true;
                Properties.Settings.Default.Save();
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.isPinned)
            {
                base.OnMouseLeftButtonDown(e);
                this.DragMove();
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Save();
            }
        }
        protected override void OnActivated(EventArgs e)
        {
            if(Properties.Settings.Default.ToBackground)
            {
                base.OnActivated(e);
                ShoveToBackground();
            }
            
        }
    }
}
