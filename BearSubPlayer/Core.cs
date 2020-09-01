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
        private List<SubInfo> _subList;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(50);
        private TimeSpan _totalTime = new TimeSpan();
        private TimeSpan _adjustTime = new TimeSpan();
        private TimeSpan _baseTime = new TimeSpan();
        private string _currentContents;

        public Core()
            => _timer.Elapsed += TimeObserver;

        public async Task PlayAsync()
        {
            if (!_stopWatch.IsRunning)
            {
                ArrHandler.Serv.SubLabel(false);  // The file can be loaded only the player isn't playing
                ArrHandler.Serv.PlayWidgets(false);  // Lock the play widget

                var countdown = "";
                for (var i = 3; i >= 1; i--)
                {
                    countdown += $"{i}...";
                    ArrHandler.Serv.SubLabel(countdown);
                    await Task.Delay(1000);
                }

                ArrHandler.Serv.PlayWidgets(true);

                ArrHandler.Serv.SubLabel(_currentContents);

                _stopWatch.Start();
                _timer.Start();
            }
        }

        public void TimeSldChanged(double timesldvalue)
        {
            if (_stopWatch.IsRunning)
                _stopWatch.Restart();
            else
                _stopWatch.Reset();

            var adjustedtotaltime = _totalTime - _adjustTime;
            var basetime = (int)(timesldvalue * adjustedtotaltime.TotalMilliseconds / 100);
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
            ArrHandler.Serv.SubLabel(true);
        }

        public void Stop()
        {
            Reset();
            ArrHandler.Serv.SubLabel(true);
        }

        private void TimeObserver(object sender = null, ElapsedEventArgs e = null)
        {
            var currenttime = _stopWatch.Elapsed + _baseTime;
            var adjustedcurrenttime = currenttime + _adjustTime;
            var adjustedtotaltime = _totalTime - _adjustTime;

            var sub = _subList.FirstOrDefault(x => x.TStart <= adjustedcurrenttime && adjustedcurrenttime <= x.TEnd);
            if (sub != null)
            {
                if (_currentContents != sub.Contents)
                {
                    ArrHandler.Serv.SubLabel(sub.Contents);
                    _currentContents = sub.Contents;
                }
            }
            else
            {
                if (_currentContents != "")
                {
                    ArrHandler.Serv.SubLabel("");
                    _currentContents = "";
                }
            }

            if (currenttime >= adjustedtotaltime)
                Reset();
            else
            {
                ArrHandler.Serv.TimeLb($"{currenttime:hh\\:mm\\:ss} / {adjustedtotaltime:hh\\:mm\\:ss}");
                ArrHandler.Serv.TimeSld(currenttime.TotalMilliseconds / adjustedtotaltime.TotalMilliseconds * 100);
            }
        }

        private void Reset(bool ispartial = false)
        {
            _stopWatch.Reset();
            ArrHandler.Serv.MainReset(ispartial);
            _timer.Stop();
        }

        public async Task LoadFileAsync(string path)
        {
            Reset(ispartial: true);
            try
            {
                _subList = await Task.Run(() => new SubReader(path).ReadSrt());
                _totalTime = _subList[^1].TEnd;    // [^1]  C# 8.0
                var filename = Path.GetFileName(path);

                if (filename.Length > 35)
                    ArrHandler.Serv.SubLabel(filename.Substring(0, 35) + "... is loaded");
                else
                    ArrHandler.Serv.SubLabel(filename + " is loaded");
                ArrHandler.Serv.TimeLb($"00:00:00 / {_totalTime:hh\\:mm\\:ss}");
                ArrHandler.Serv.PlayWidgets(true);
            }
            catch
            {
                ArrHandler.Serv.SubLabel("An invalid srt file, please try again");
            }
        }
    }
}