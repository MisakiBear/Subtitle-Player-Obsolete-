using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using TriggerLib;

namespace BearSubPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Arr { get; private set; }
        private SubPlayer _subPlayer;
        private readonly TriggerSource _triggerSource;
        private Brush _currentBrush;

        public MainWindow()
        {
            InitializeComponent();

            Arr = this;

            // MainWindow size arrange
            this.Width = SystemParameters.PrimaryScreenWidth * 2 / 3;
            Top = SystemParameters.PrimaryScreenHeight * 5 / 6;
            Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;

            SubLabel.Width = this.Width;
            var marginleft = this.Width - MenuPanel.Width - 10;
            MenuPanel.Margin = new Thickness(marginleft, 0, 0, 0);

            MainInitialize();

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
                _triggerSource.ResetTrigger(pullImmed: false);
                MenuPanel.Visibility = Visibility.Visible;
            }
        }

        private async void SubLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".srt", // Default file extension
                Filter = "Sub File (*.srt, *ass)|*.srt;*.ass" // Filter files by extension
            };

            var result = dlg.ShowDialog();
            if (!(bool)result) return;

            MainReset(true);
            _subPlayer = await SubPlayer.CreateSubPlayerAsync(dlg.FileName);
        }

        private async void PlayLb_MouseDown(object sender, MouseButtonEventArgs e)
            => await _subPlayer.PlayAsync();

        private void TimeSld_MouseMove(object sender, MouseEventArgs e)
        {
            if (TimeSld.IsMouseCaptureWithin)
                _subPlayer.TimeSldChanged(TimeSld.Value);
        }

        private void BackWardLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Backward();

        private void ForWardLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Forward();

        private void PauseLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Pause();

        private void StopLb_MouseDown(object sender, MouseButtonEventArgs e)
            => _subPlayer.Stop();

        private void SettingLb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!SettingWindow.IsOpened)
            {
                var settingwindow = new SettingWindow();
                settingwindow.Show();
            }
        }

        // Highlight animation
        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            if (label.Foreground == Brushes.White)
            {
                _currentBrush = Brushes.White;
                label.Foreground = Brushes.DimGray;
            }
            else if (label.Foreground == Brushes.Black)
            {
                _currentBrush = Brushes.Black;
                label.Foreground = Brushes.LightGray;
            }
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            label.Foreground = _currentBrush;
        }

        // Arrange
        public void MainBackground(Brush fontbrush, Color backgroundcolor, double opacity)
        {
            Background = new SolidColorBrush(backgroundcolor);

            this.InvokeIfNeeded(() =>
            {
                if (opacity <= 0.002)
                    Background.Opacity = 0.002;    // Almost transparent
                else
                    Background.Opacity = opacity;

                foreach (var child in MenuPanel.Children)  // Set all labels except slider
                {
                    if (child is Label label)
                    {
                        label.Foreground = fontbrush;
                    }
                }
            });
        }

        public void FontEffect(Brush fontbrush, Color shadowcolor, double opacity, double softness, double fontsize)
        {
            SubLabel.Foreground = fontbrush;

            var effect = new DropShadowEffect()
            {
                Color = shadowcolor,
                Direction = 320,
                ShadowDepth = 5,
                Opacity = opacity,
                BlurRadius = softness
            };

            this.InvokeIfNeeded(() =>
            {
                SubLabel.Effect = effect;
                SubLabel.FontSize = fontsize;
            });
        }

        public void MainInitialize()
        {
            var config = new Config();

            if (config.MainCol == 0)  // White
                MainBackground(Brushes.Black, Colors.White, config.MainOp);
            else
                MainBackground(Brushes.White, Colors.Black, config.MainOp);

            if (config.FontCol == 0)  // White
                FontEffect(Brushes.White, Colors.White, config.FontOp, config.FontSn, config.FontSize);
            else
                FontEffect(Brushes.Black, Colors.Black, config.FontOp, config.FontSn, config.FontSize);
        }

        public void SubLbContents(string contents)
            => this.InvokeIfNeeded(() => SubLabel.Content = contents);

        public void SubLbIsEnabled(bool isenabled)
            => this.InvokeIfNeeded(() => SubLabel.IsEnabled = isenabled);

        public void TimeLbContents(string time)
            => this.InvokeIfNeeded(() => TimeLb.Content = time);

        public void TimeSldValue(double value)
        {
            if (!TimeSld.IsMouseCaptureWithin)
                this.InvokeIfNeeded(() => TimeSld.Value = value);
        }

        public void PlayWidgetControl(bool isenabled)
        {
            this.InvokeIfNeeded(() =>
            {
                foreach (var child in MenuPanel.Children)
                {
                    if (child is Label label)
                        label.IsEnabled = isenabled;
                    else if (child is Slider slider)
                        slider.IsEnabled = isenabled;
                }
            });
        }

        public void MainReset(bool ispartial)
        {
            if (!ispartial)
                SubLbContents("Double click here to select a srt file");
            PlayWidgetControl(false);
            TimeSldValue(0);
            TimeLbContents("00:00:00 / 00:00:00");
            SubLbIsEnabled(true);
        }
    }
}
