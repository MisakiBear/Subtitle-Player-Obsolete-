using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using TriggerLib;

namespace BearSubPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core _core;
        private TriggerSource _triggerSource;
        private Brush _currentBrush;

        public MainWindow()
        {
            InitializeComponent();

            // MainWindow
            this.Width = SystemParameters.PrimaryScreenWidth * 2 / 3;
            Top = SystemParameters.PrimaryScreenHeight * 5 / 6;
            Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;

            SubLabel.Width = this.Width;
            var marginleft = this.Width - MenuPanel.Width - 10;
            MenuPanel.Margin = new Thickness(marginleft, 0, 0, 0);

            // Arrange
            MainInitialize();
            ArrHandler.Serv.BackgroundChangeReq += MainBackground;
            ArrHandler.Serv.FontEffectChangeReq += FontEffect;
            ArrHandler.Serv.MainInitializeReq += MainInitialize;

            ArrHandler.Serv.SubLbContentsChangeReq += SubLbContents;
            ArrHandler.Serv.SubLbIsEnableChangeReq += SubLbIsEnabled;
            ArrHandler.Serv.TimeLbChangeReq += TimeLbContents;
            ArrHandler.Serv.TimeSldChangeReq += TimeSldValue;
            ArrHandler.Serv.PlayWidgetsChangeReq += PlayWidgetControl;
            ArrHandler.Serv.MainResetReq += MainReset;

            // Init TriggerSource
            _triggerSource = new TriggerSource(3000, () =>
            {
                this.InvokeIfNeeded(() =>
                {
                    MenuPanel.Visibility = Visibility.Hidden;
                });
            }, pullImmed: false);
        }

        private void Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Main_MouseLeave(object sender, MouseEventArgs e)
        {
            _triggerSource.Pull();
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (_triggerSource.Trigger.IsPulled)
            {
                _triggerSource.CreateNewTrigger(pullImmed: false);
                MenuPanel.Visibility = Visibility.Visible;
            }
        }

        private async void SubLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".srt", // Default file extension
                Filter = "SRT File (.srt)|*.srt" // Filter files by extension
            };

            var result = dlg.ShowDialog();
            if ((bool)result)
            {
                _core = new Core();
                await _core.LoadFileAsync(dlg.FileName);
            }
        }

        private async void PlayLb_MouseDown(object sender, MouseButtonEventArgs e)
            => await _core.PlayAsync();

        private void TimeSld_MouseMove(object sender, MouseEventArgs e)
        {
            if (TimeSld.IsMouseCaptureWithin)
                _core.TimeSldChanged(TimeSld.Value);
        }

        private void BackWardLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _core.Backward();

        private void ForWardLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _core.Forward();

        private void PauseLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _core.Pause();

        private void StopLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _core.Stop();

        private void SettingLb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!SettingWindow.IsOpened)
            {
                var settingwindow = new SettingWindow();
                settingwindow.Show();
            }
        }

        // Arrange
        private void Label_MouseMove(object sender, MouseEventArgs e)
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

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            label.Foreground = _currentBrush;
        }
    }
}
