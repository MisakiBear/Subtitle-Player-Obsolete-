using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace BearSubPlayer
{
    public static class ArrHelper
    {
        private static readonly MainWindow _mainWin = MainWindow.SelfAccess;

        public static void ChangeBackground(Brush fontbrush, Color backgroundcolor, double opacity)
        {
            _mainWin.Background = new SolidColorBrush(backgroundcolor);

            if (opacity == 0)
                _mainWin.Background.Opacity = 0.002;    // Almost transparent
            else
                _mainWin.Background.Opacity = opacity;

            _mainWin.TimeLb.Foreground = fontbrush;
            _mainWin.TimeLb.Foreground = fontbrush;
            _mainWin.PlayLb.Foreground = fontbrush;
            _mainWin.BackwardLb.Foreground = fontbrush;
            _mainWin.ForwardLb.Foreground = fontbrush;
            _mainWin.PauseLb.Foreground = fontbrush;
            _mainWin.StopLb.Foreground = fontbrush;
            _mainWin.SettingLb.Foreground = fontbrush;
        }

        public static void ChangeFontEffect(Brush fontbrush, Color shadowcolor, double opacity, double softness)
        {
            _mainWin.SubLabel.Foreground = fontbrush;

            var effect = new DropShadowEffect()
            {
                Color = shadowcolor,
                Direction = 320,
                ShadowDepth = 5,
                Opacity = opacity,
                BlurRadius = softness
            };
            _mainWin.SubLabel.Effect = effect;
        }

        public static void ChangeFontSize(double fontsize)
            => _mainWin.SubLabel.FontSize = fontsize;

        public static void MainInitialize()
        {
            var config = new Config();

            if (config.MainCol == 0)  // White
                ChangeBackground(Brushes.Black, Colors.White, config.MainOp);
            else
                ChangeBackground(Brushes.White, Colors.Black, config.MainOp);

            ChangeFontSize(config.FontSize);

            if (config.FontCol == 0)  // White
                ChangeFontEffect(Brushes.White, Colors.White, config.FontOp, config.FontSn);
            else
                ChangeFontEffect(Brushes.Black, Colors.Black, config.FontOp, config.FontSn);
        }
    }

    public partial class SettingWindow : Window
    {
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
