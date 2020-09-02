using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace BearSubPlayer
{
    public class ArrHandler
    {
        public static ArrHandler Serv { get; }
        public event Action<BackgroundEventArgs> BackgroundChangeReq;
        public event Action<FontEffectEventArgs> FontEffectChangeReq;
        public event Action<EventArgs> MainInitializeReq;
        public event Action<string> SubLbContentsChangeReq;
        public event Action<bool> SubLbIsEnableChangeReq;
        public event Action<string> TimeLbChangeReq;
        public event Action<double> TimeSldChangeReq;
        public event Action<bool> PlayWidgetsChangeReq;
        public event Action<bool> MainResetReq;

        static ArrHandler()
        {
            Serv = new ArrHandler();
        }

        private ArrHandler() { }

        public void MainBackground(Brush fontbrush, Color backgroundcolor, double opacity)
            => BackgroundChangeReq?.Invoke(new BackgroundEventArgs(fontbrush, backgroundcolor, opacity));

        public void FontEffect(Brush fontbrush, Color shadowcolor, double opacity, double softness, double fontsize)
            => FontEffectChangeReq?.Invoke(new FontEffectEventArgs(fontbrush, shadowcolor, opacity, softness, fontsize));

        public void MainInitialize()
            => MainInitializeReq?.Invoke(new EventArgs());

        public void SubLabel(string contents)
            => SubLbContentsChangeReq?.Invoke(contents);

        public void SubLabel(bool isenable)
            => SubLbIsEnableChangeReq?.Invoke(isenable);

        public void TimeLb(string time)
            => TimeLbChangeReq?.Invoke(time);

        public void TimeSld(double value)
            => TimeSldChangeReq?.Invoke(value);

        public void PlayWidgets(bool isenable)
            => PlayWidgetsChangeReq?.Invoke(isenable);

        public void MainReset(bool ispartial)
            => MainResetReq?.Invoke(ispartial);
    }

    public class BackgroundEventArgs
    {
        public Brush FontBrush { get; private set; }
        public Color BackgroundColor { get; private set; }
        public double Opacity { get; private set; }

        public BackgroundEventArgs(Brush fontbrush, Color backgroundcolor, double opacity)
        {
            this.FontBrush = fontbrush;
            this.BackgroundColor = backgroundcolor;
            this.Opacity = opacity;
        }
    }

    public class FontEffectEventArgs
    {
        public Brush FontBrush { get; private set; }
        public Color ShadowColor { get; private set; }
        public double Opacity { get; private set; }
        public double Softness { get; private set; }
        public double FontSize { get; private set; }

        public FontEffectEventArgs(Brush fontbrush, Color shadowcolor, double opacity, double softness, double fontsize)
        {
            this.FontBrush = fontbrush;
            this.ShadowColor = shadowcolor;
            this.Opacity = opacity;
            this.Softness = softness;
            this.FontSize = fontsize;
        }
    }

    public partial class MainWindow : Window
    {
        private void MainBackground(BackgroundEventArgs e)
        {
            Background = new SolidColorBrush(e.BackgroundColor);

            this.InvokeIfNeeded(() =>
            {
                if (e.Opacity <= 0.002)
                    Background.Opacity = 0.002;    // Almost transparent
                else
                    Background.Opacity = e.Opacity;

                TimeLb.Foreground = e.FontBrush;
                PlayLb.Foreground = e.FontBrush;
                BackwardLb.Foreground = e.FontBrush;
                ForwardLb.Foreground = e.FontBrush;
                PauseLb.Foreground = e.FontBrush;
                StopLb.Foreground = e.FontBrush;
                SettingLb.Foreground = e.FontBrush;
            });
        }

        private void FontEffect(FontEffectEventArgs e)
        {
            SubLabel.Foreground = e.FontBrush;

            var effect = new DropShadowEffect()
            {
                Color = e.ShadowColor,
                Direction = 320,
                ShadowDepth = 5,
                Opacity = e.Opacity,
                BlurRadius = e.Softness
            };

            this.InvokeIfNeeded(() =>
            {
                SubLabel.Effect = effect;
                SubLabel.FontSize = e.FontSize;
            });
        }

        private void MainInitialize(EventArgs e = null)
        {
            var config = new Config();

            if (config.MainCol == 0)  // White
                MainBackground(new BackgroundEventArgs(Brushes.Black, Colors.White, config.MainOp));
            else
                MainBackground(new BackgroundEventArgs(Brushes.White, Colors.Black, config.MainOp));

            if (config.FontCol == 0)  // White
                FontEffect(new FontEffectEventArgs(Brushes.White, Colors.White, config.FontOp, config.FontSn, config.FontSize));
            else
                FontEffect(new FontEffectEventArgs(Brushes.Black, Colors.Black, config.FontOp, config.FontSn, config.FontSize));
        }

        private void SubLbContents(string contents)
            => this.InvokeIfNeeded(() => SubLabel.Content = contents);

        private void SubLbIsEnabled(bool isenabled)
            => this.InvokeIfNeeded(() => SubLabel.IsEnabled = isenabled);

        private void TimeLbContents(string time)
            => this.InvokeIfNeeded(() => TimeLb.Content = time);

        private void TimeSldValue(double value)
        {
            if (!TimeSld.IsMouseCaptureWithin)
                this.InvokeIfNeeded(() => TimeSld.Value = value);
        }

        private void PlayWidgetControl(bool isenabled)
        {
            this.InvokeIfNeeded(() =>
            {
                PlayLb.IsEnabled = isenabled;
                BackwardLb.IsEnabled = isenabled;
                ForwardLb.IsEnabled = isenabled;
                PauseLb.IsEnabled = isenabled;
                StopLb.IsEnabled = isenabled;
                TimeSld.IsEnabled = isenabled;
            });
        }

        private void MainReset(bool ispartial)
        {
            if (!ispartial)
                SubLbContents("Double click there to select a srt file");
            PlayWidgetControl(false);
            TimeSldValue(0);
            TimeLbContents("00:00:00 / 00:00:00");
            SubLbIsEnabled(true);
        }
    }
}