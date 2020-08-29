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

            OpacitySld.ValueChanged += OpacitySld_ValueChanged;
            WhiteRBtn.Checked += WhiteRBtn_Checked;
            BlackRBtn.Checked += BlackRBtn_Checked;

            FontSizeSld.ValueChanged += FontSizeSld_ValueChanged;
            FontWhiteRBtn.Checked += Font_Changed;
            FontBlackRBtn.Checked += Font_Changed;

            FontShadowOpacitySld.ValueChanged += Font_Changed;
            FontShadowSoftnessSld.ValueChanged += Font_Changed;

            ExitBtn.Click += ExitBtn_Click;
            SetDefaultBtn.Click += SetDefaultBtn_Click;
            CloseBtn.Click += CloseBtn_Click;

            this.Activated += SettingWindow_Activated;
            this.Closing += SettingWindow_Closing;
        }

        private void OpacitySld_ValueChanged(object sender, EventArgs e)
        {
            OpacityLb.Content = (int)OpacitySld.Value;
            if ((bool)WhiteRBtn.IsChecked)
                ArrHelper.ChangeBackground(Brushes.Black, Colors.White, OpacitySld.Value / 100);
            else
                ArrHelper.ChangeBackground(Brushes.White, Colors.Black, OpacitySld.Value / 100);
        }

        private void WhiteRBtn_Checked(object sender, EventArgs e)
            => ArrHelper.ChangeBackground(Brushes.Black, Colors.White, OpacitySld.Value / 100);

        private void BlackRBtn_Checked(object sender, EventArgs e)
            => ArrHelper.ChangeBackground(Brushes.White, Colors.Black, OpacitySld.Value / 100);

        private void FontSizeSld_ValueChanged(object sender, EventArgs e)
        {
            FontSizeLb.Content = (int)FontSizeSld.Value;
            ArrHelper.ChangeFontSize(FontSizeSld.Value);
        }

        private void Font_Changed(object sender, EventArgs e)
        {
            FontShadowOpacityLb.Content = (int)FontShadowOpacitySld.Value;
            FontShadowSoftnessLb.Content = (int)FontShadowSoftnessSld.Value / 10;

            if ((bool)FontWhiteRBtn.IsChecked)
                ArrHelper.ChangeFontEffect(Brushes.White, Colors.White, FontShadowOpacitySld.Value / 100, FontShadowSoftnessSld.Value / 10);
            else
                ArrHelper.ChangeFontEffect(Brushes.Black, Colors.Black, FontShadowOpacitySld.Value / 100, FontShadowSoftnessSld.Value / 10);
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }

        private void SetDefaultBtn_Click(object sender, EventArgs e)
        {
            new Config().SetDefault();
            ArrHelper.MainInitialize();
            Initialize();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
            => this.Close();

        private void SettingWindow_Activated(object sender, EventArgs e)
            => IsOpened = true;

        private void SettingWindow_Closing(object sender, EventArgs e)
        {
            Save();
            IsOpened = false;
        }
    }
}

