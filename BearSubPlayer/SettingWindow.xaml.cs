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

            // Events that relate to change the controls should be hooked after the window initialized
            OpacitySld.ValueChanged += OpacitySld_ValueChanged;
            FontSizeSld.ValueChanged += FontSizeSld_ValueChanged;
            FontWhiteRBtn.Checked += Font_Changed;
            FontBlackRBtn.Checked += Font_Changed;
            FontShadowOpacitySld.ValueChanged += Font_Changed;
            FontShadowSoftnessSld.ValueChanged += Font_Changed;
        }

        private void OpacitySld_ValueChanged(object sender, EventArgs e)
        {
            OpacityLb.Content = (int)(OpacitySld.Value * 100);  // Change float to %
            if ((bool)WhiteRBtn.IsChecked)
                ArrHelper.ChangeBackground(Brushes.Black, Colors.White, OpacitySld.Value);
            else
                ArrHelper.ChangeBackground(Brushes.White, Colors.Black, OpacitySld.Value);
        }

        private void WhiteRBtn_Checked(object sender, RoutedEventArgs e)
            => ArrHelper.ChangeBackground(Brushes.Black, Colors.White, OpacitySld.Value);

        private void BlackRBtn_Checked(object sender, RoutedEventArgs e)
            => ArrHelper.ChangeBackground(Brushes.White, Colors.Black, OpacitySld.Value);

        private void FontSizeSld_ValueChanged(object sender, EventArgs e)
        {
            FontSizeLb.Content = (int)FontSizeSld.Value;
            ArrHelper.ChangeFontSize(FontSizeSld.Value);
        }

        private void Font_Changed(object sender, EventArgs e)
        {
            FontShadowOpacityLb.Content = (int)(FontShadowOpacitySld.Value * 100); // Change float to %
            FontShadowSoftnessLb.Content = (int)FontShadowSoftnessSld.Value;

            if ((bool)FontWhiteRBtn.IsChecked)
                ArrHelper.ChangeFontEffect(Brushes.White, Colors.White, FontShadowOpacitySld.Value, FontShadowSoftnessSld.Value);
            else
                ArrHelper.ChangeFontEffect(Brushes.Black, Colors.Black, FontShadowOpacitySld.Value, FontShadowSoftnessSld.Value);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }

        private void SetDefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            new Config().SetDefault();
            ArrHelper.MainInitialize();
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
    }
}

