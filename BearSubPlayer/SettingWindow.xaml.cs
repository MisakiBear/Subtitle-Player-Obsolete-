using System;
using System.Windows;
using System.Windows.Media;

namespace BearSubPlayer
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public static bool IsOpened = false;

        public SettingWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            var config = new Config();

            OpacitySld.Value = config.MainOp;
            OpacityLb.Content = (int)(config.MainOp * 100);

            if (config.MainCol == 0)  // White
                WhiteRBtn.IsChecked = true;
            else
                BlackRBtn.IsChecked = true;

            FontSizeSld.Value = config.FontSize;
            FontSizeLb.Content = config.FontSize;

            if (config.FontCol == 0)  // White
                FontWhiteRBtn.IsChecked = true;
            else
                FontBlackRBtn.IsChecked = true;

            FontShadowOpacitySld.Value = config.FontOp;
            FontShadowOpacityLb.Content = (int)(config.FontOp * 100);
            FontShadowSoftnessSld.Value = config.FontSn;
            FontShadowSoftnessLb.Content = config.FontSn;
        }

        private void Main_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
            => Main_Changed();

        private void Main_Changed(object sender, RoutedEventArgs e)
            => Main_Changed();

        private void Main_Changed()
        {
            if (this.IsInitialized)
            {
                OpacityLb.Content = (int)(OpacitySld.Value * 100);  // Change float to %
                if ((bool)WhiteRBtn.IsChecked)
                    ArrHandler.Serv.MainBackground(Brushes.Black, Colors.White, OpacitySld.Value);
                else
                    ArrHandler.Serv.MainBackground(Brushes.White, Colors.Black, OpacitySld.Value);
            }
        }

        private void Font_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
            => Font_Changed();

        private void Font_Changed(object sender, RoutedEventArgs e)
            => Font_Changed();

        private void Font_Changed()
        {
            if (this.IsInitialized)
            {
                FontSizeLb.Content = (int)FontSizeSld.Value;
                FontShadowOpacityLb.Content = (int)(FontShadowOpacitySld.Value * 100); // Change float to %
                FontShadowSoftnessLb.Content = (int)FontShadowSoftnessSld.Value;

                if ((bool)FontWhiteRBtn.IsChecked)
                    ArrHandler.Serv.FontEffect(Brushes.White, Colors.White, FontShadowOpacitySld.Value,
                        FontShadowSoftnessSld.Value, FontSizeSld.Value);
                else
                    ArrHandler.Serv.FontEffect(Brushes.Black, Colors.Black, FontShadowOpacitySld.Value,
                        FontShadowSoftnessSld.Value, FontSizeSld.Value);
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void SetDefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            new Config().SetDefault();
            ArrHandler.Serv.MainInitialize();
            Initialize();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
            => this.Close();

        private void SettingWindow_Activated(object sender, EventArgs e)
            => IsOpened = true;

        private void SettingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
            IsOpened = false;
        }

        private void Save()
        {
            var config = new Config
            {
                MainOp = Math.Round(OpacitySld.Value * 100) / 100,  // Round the number
                FontSize = (int)FontSizeSld.Value,
                FontOp = Math.Round(FontShadowOpacitySld.Value * 100) / 100,
                FontSn = (int)FontShadowSoftnessSld.Value,
            };

            if ((bool)WhiteRBtn.IsChecked)  // White
                config.MainCol = 0;
            else
                config.MainCol = 1;

            if ((bool)FontWhiteRBtn.IsChecked)  // White
                config.FontCol = 0;
            else
                config.FontCol = 1;

            config.Save();
        }
    }
}
