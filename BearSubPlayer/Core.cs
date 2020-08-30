using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BearSubPlayer
{
    public class Core
    {
        private readonly List<SubInfo> _subList = new List<SubInfo>();
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(50);
        private readonly MainWindow _mainWin = MainWindow.SelfAccess;
        private TimeSpan _totalTime = new TimeSpan();
        private TimeSpan _adjustTime = new TimeSpan();
        private TimeSpan _baseTime = new TimeSpan();
        private string _currentContents;

        public Core()
            => _timer.Elapsed += TimeObserver;

        public async Task Play()
        {
            if (!_stopWatch.IsRunning)
            {
                PlayWidget(false);  // Lock the play widget

                var countdown = "";
                for (var i = 3; i >= 1; i--)
                {
                    countdown += $"{i}...";
                    _mainWin.Dispatcher.Invoke(() => _mainWin.SubLabel.Content = countdown);
                    await Task.Run(() => Thread.Sleep(1000));
                }

                PlayWidget(true);

                _mainWin.SubLabel.Content = _currentContents;

                _mainWin.SubLabel.IsEnabled = false;  // The file can be loaded only the player isn't playing
                _stopWatch.Start();
                _timer.Start();
            }
        }

        public void TimeSldChanged()
        {
            if (!_mainWin.TimeSld.IsMouseCaptureWithin) return;

            if (_stopWatch.IsRunning)
                _stopWatch.Restart();
            else
                _stopWatch.Reset();

            var adjustedtotaltime = _totalTime - _adjustTime;
            var basetime = (int)(_mainWin.TimeSld.Value * adjustedtotaltime.TotalMilliseconds / 100);
            _baseTime = new TimeSpan(0, 0, 0, 0, basetime);
            TimeObserver();
        }

        public void Backward()
        {
            _adjustTime = _adjustTime.Add(new TimeSpan(0, 0, 0, 0, -50));
            if (!_timer.Enabled)
                TimeObserver();
        }

        public void Forward()
        {
            _adjustTime = _adjustTime.Add(new TimeSpan(0, 0, 0, 0, 50));
            if (!_timer.Enabled)
                TimeObserver();
        }

        public void Pause()
        {
            _timer.Stop();
            _stopWatch.Stop();
            _mainWin.SubLabel.IsEnabled = true;
        }

        public void Stop()
        {
            Reset();
            _mainWin.SubLabel.IsEnabled = true;
        }

        private void TimeObserver(object sender = null, ElapsedEventArgs e = null)
        {
            var currenttime = _stopWatch.Elapsed + _baseTime;
            var adjustedtime = currenttime + _adjustTime;
            var adjustedtotaltime = _totalTime - _adjustTime;

            var sub = _subList.FirstOrDefault(x => x.TStart <= adjustedtime && adjustedtime <= x.TEnd);
            if (sub != null)
            {
                if (_currentContents != sub.Contents)
                {
                    _mainWin.Dispatcher.Invoke(() => _mainWin.SubLabel.Content = sub.Contents);
                    _currentContents = sub.Contents;
                }
            }
            else
            {
                if (_currentContents != "")
                {
                    _mainWin.Dispatcher.Invoke(() => _mainWin.SubLabel.Content = "");
                    _currentContents = "";
                }
            }

            _mainWin.Dispatcher.Invoke(() =>
            {
                _mainWin.TimeLb.Content = $"{currenttime:hh\\:mm\\:ss} / {adjustedtotaltime:hh\\:mm\\:ss}";
                if (!_mainWin.TimeSld.IsMouseCaptureWithin)
                    _mainWin.TimeSld.Value = currenttime.TotalMilliseconds / adjustedtotaltime.TotalMilliseconds * 100;
            });

            if (currenttime >= adjustedtotaltime)
                Reset();
        }

        private void Reset()
        {
            _stopWatch.Reset();
            _mainWin.Dispatcher.Invoke(() =>
            {
                _mainWin.SubLabel.Content = "Double click there to select a srt file";
                _mainWin.TimeSld.Value = 0;
                _mainWin.TimeLb.Content = "00:00:00 / 00:00:00";
                PlayWidget(false);
                _mainWin.SubLabel.IsEnabled = true;
            });
            _timer.Stop();
        }

        public async Task LoadFileAsync(string path)
        {
            Reset();
            var readfile = await ReadSrtAsync(path);
            if (readfile)
            {
                _mainWin.Dispatcher.Invoke(() =>
                {
                    _mainWin.SubLabel.Content = Path.GetFileName(path) + " is loaded";
                    _mainWin.TimeLb.Content = $"00:00:00 / {_totalTime:hh\\:mm\\:ss}";
                    PlayWidget(true);
                });
            }
            else
                _mainWin.Dispatcher.Invoke(() => _mainWin.SubLabel.Content = "An invalid srt file, please try again");
        }

        private async Task<bool> ReadSrtAsync(string path)
        {
            try
            {
                using var reader = new StreamReader(path);
                var tempstr = "";
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == "" && tempstr != "")   // The end of one subtitle
                    {
                        var subinfo = new SubInfo();
                        var splited = tempstr.Split("\n");
                        var time = splited[1].Split(" --> ");    // Second line is time info
                        subinfo.TStart = TimeSpan.Parse(time[0].Replace(',', '.'));
                        subinfo.TEnd = TimeSpan.Parse(time[1].Replace(',', '.'));

                        for (var i = 2; i < splited.Length; i++)  // Third line to second last line is subtitle info
                        {
                            var contents = Regex.Replace(splited[i], @"<.*?>", "");
                            contents = Regex.Replace(contents, @"{.*?}", "");
                            subinfo.Contents += contents;
                            if (i < splited.Length - 2)
                                subinfo.Contents += "\n";
                        }

                        _subList.Add(subinfo);
                        tempstr = "";
                    }
                    else
                    {
                        tempstr += line + "\n";
                    }
                }
                _totalTime = _subList[^1].TEnd;    // [^1]  C# 8.0
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void PlayWidget(bool isenabled)
        {
            _mainWin.PlayLb.IsEnabled = isenabled;
            _mainWin.BackwardLb.IsEnabled = isenabled;
            _mainWin.ForwardLb.IsEnabled = isenabled;
            _mainWin.PauseLb.IsEnabled = isenabled;
            _mainWin.StopLb.IsEnabled = isenabled;
            _mainWin.TimeSld.IsEnabled = isenabled;
        }
    }
}
