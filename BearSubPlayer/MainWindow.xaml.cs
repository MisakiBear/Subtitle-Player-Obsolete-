using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace BearSubPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow SelfAccess { get; private set; }
        private Core _core;
        private bool _isActive;
        private Brush _currentBrush;

        public MainWindow()
        {
            InitializeComponent();

            SelfAccess = this;

            // MainWindow
            this.Width = SystemParameters.PrimaryScreenWidth * 2 / 3;
            Top = SystemParameters.PrimaryScreenHeight * 5 / 6;
            Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;

            // Toolbox
            SubLabel.Width = this.Width;
            var marginleft = this.Width - MenuPanel.Width - 10;
            MenuPanel.Margin = new Thickness(marginleft, 0, 0, 0);

            // Arrange
            ArrHelper.MainInitialize();

            this.MouseLeave += Main_MouseLeave;
            this.MouseMove += Main_MouseMove;

            PlayLb.MouseMove += Label_MouseMove;
            PlayLb.MouseLeave += Label_MouseLeave;
            BackwardLb.MouseMove += Label_MouseMove;
            BackwardLb.MouseLeave += Label_MouseLeave;
            ForwardLb.MouseMove += Label_MouseMove;
            ForwardLb.MouseLeave += Label_MouseLeave;
            PauseLb.MouseMove += Label_MouseMove;
            PauseLb.MouseLeave += Label_MouseLeave;
            StopLb.MouseMove += Label_MouseMove;
            StopLb.MouseLeave += Label_MouseLeave;
            SettingLb.MouseMove += Label_MouseMove;
            SettingLb.MouseLeave += Label_MouseLeave;

            // Action
            this.MouseDown += Main_MouseDown;
            SubLabel.MouseDoubleClick += SubLabel_MouseDoubleClick;
            TimeSld.MouseMove += TimeSld_MouseMove;
            PlayLb.MouseDown += PlayLb_MouseDown;
            BackwardLb.MouseDown += BackWardLb_MouseDown;
            ForwardLb.MouseDown += ForWardLb_MouseDown;
            PauseLb.MouseDown += PauseLb_MouseDown;
            StopLb.MouseDown += StopLb_MouseDown;
            SettingLb.MouseDown += SettingLb_MouseDown;
        }

        private void Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private async void Main_MouseLeave(object sender, EventArgs e)
        {
            _isActive = false;
            await Task.Run(() => Thread.Sleep(3000));
            if (!_isActive)
                MenuPanel.Visibility = Visibility.Hidden;
        }

        private void Main_MouseMove(object sender, EventArgs e)
        {
            _isActive = true;
            MenuPanel.Visibility = Visibility.Visible;
        }

        private async void SubLabel_MouseDoubleClick(object sender, EventArgs e)
        {
            _core = new Core();

            var dlg = new OpenFileDialog
            {
                DefaultExt = ".srt", // Default file extension
                Filter = "SRT File (.srt)|*.srt" // Filter files by extension
            };

            var result = dlg.ShowDialog();
            if ((bool)result)
                await _core.LoadFileAsync(dlg.FileName);
        }

        private async void PlayLb_MouseDown(object sender, EventArgs e)
            => await _core.Play();

        private void TimeSld_MouseMove(object sender, EventArgs e)
            => _core.TimeSldChanged();

        private void BackWardLb_MouseDown(object sender, EventArgs e)
            => _core.Backward();

        private void ForWardLb_MouseDown(object sender, EventArgs e)
            => _core.Forward();

        private void PauseLb_MouseDown(object sender, EventArgs e)
            => _core.Pause();

        private void StopLb_MouseDown(object sender, EventArgs e)
            => _core.Stop();

        private void SettingLb_MouseDown(object sender, EventArgs e)
        {
            if (!SettingWindow.IsOpened)
            {
                var settingwindow = new SettingWindow();
                settingwindow.Show();
            }
        }

        // Arrange
        private void Label_MouseMove(object sender, EventArgs e)
        {
            var label = (Label)sender;
            if (label.Foreground == Brushes.White)
                _currentBrush = Brushes.White;
            if (label.Foreground == Brushes.Black)
                _currentBrush = Brushes.Black;

            if (_currentBrush == Brushes.White)
                label.Foreground = Brushes.DimGray;
            else
                label.Foreground = Brushes.LightGray;
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            var label = (Label)sender;
            label.Foreground = _currentBrush;
        }
    }
}
