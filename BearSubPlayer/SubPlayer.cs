using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BearSubPlayer
{
    public class SubPlayer
    {
        private readonly List<SubInfo> _subList;
        private readonly SubTimer _subTimer;
        private string _currentContents;

        private SubPlayer(List<SubInfo> sublist, string filename)
        {
            _subList = sublist;
            var totaltime = _subList.Max(info => info.TEnd);
            _subTimer = new SubTimer(totaltime);
            _subTimer.Elapsed += Display;

            ShowLoadedNotice(filename, totaltime);
            MainWindow.Arr.PlayWidgetControl(true);
        }

        private static void ShowLoadedNotice(string filename, TimeSpan totaltime)
        {
            if (filename.Length > 35)
                MainWindow.Arr.SubLbContents(filename.Substring(0, 35) + "... is loaded");
            else
                MainWindow.Arr.SubLbContents(filename + " is loaded");
            MainWindow.Arr.TimeLbContents($"00:00:00 / {totaltime:hh\\:mm\\:ss}");
        }

        public async Task PlayAsync()
        {
            if (_subTimer.IsRunning) return;

            MainWindow.Arr.SubLbIsEnabled(false);  // The file can be loaded only the player isn't playing
            MainWindow.Arr.PlayWidgetControl(false);  // Lock the play widget

            for (var i = 6; i >= 1; i--)
            {
                MainWindow.Arr.SubLbContents(i % 2 == 0 ? Repeat("=", i / 2) : $"- {i / 2 + 1} -");
                await Task.Delay(500);
            }

            MainWindow.Arr.PlayWidgetControl(true);

            MainWindow.Arr.SubLbContents(_currentContents);

            _subTimer.Start();

            static string Repeat(string str, int repeat)
            {
                var newstr = "";
                for (var i = 0; i < repeat; i++)
                {
                    newstr += str;
                    if (i < repeat - 1) newstr += " ";
                }
                return newstr;
            }
        }

        public void TimeSldChanged(double timesldvalue)
        {
            var time = (int)(timesldvalue * _subTimer.TotalTime.TotalMilliseconds / 100);
            _subTimer.MoveTo(new TimeSpan(0, 0, 0, 0, time));
            Display();
        }

        public void Backward()
        {
            _subTimer.AdjustTime(new TimeSpan(0, 0, 0, 0, -50));
            if (!_subTimer.IsRunning)
                Display();
        }

        public void Forward()
        {
            _subTimer.AdjustTime(new TimeSpan(0, 0, 0, 0, 50));
            if (!_subTimer.IsRunning)
                Display();
        }

        public void Pause()
        {
            _subTimer.Pause();
            MainWindow.Arr.SubLbIsEnabled(true);
        }

        public void Stop()
        {
            _subTimer.Stop();
            MainWindow.Arr.MainReset(false);
            MainWindow.Arr.SubLbIsEnabled(true);
        }

        private void Display()
        {
            SubDisplay();
            TimeDisplay();
            if (_subTimer.IsEnded) Stop();
        }

        private void SubDisplay()
        {
            var sub = _subList.FirstOrDefault(x => x.TStart <= _subTimer.CurrentTime && _subTimer.CurrentTime <= x.TEnd);
            var contents = "";
            if (sub != null) contents = sub.Contents;

            if (_currentContents != contents)
            {
                MainWindow.Arr.SubLbContents(contents);
                _currentContents = contents;
            }
        }

        private void TimeDisplay()
        {
            var elapsedtime = _subTimer.ElapsedTime;
            var totaltime = _subTimer.TotalTime;
            MainWindow.Arr.TimeLbContents($"{elapsedtime:hh\\:mm\\:ss} / {totaltime:hh\\:mm\\:ss}");
            MainWindow.Arr.TimeSldValue(elapsedtime.TotalMilliseconds / totaltime.TotalMilliseconds * 100);
        }

        public static async Task<SubPlayer> CreateSubPlayerAsync(string path)
        {
            try
            {
                var subreader = GetSubReader(Path.GetExtension(path));
                var sublist = await subreader.ReadAsync(path);
                var filename = Path.GetFileName(path);

                return new SubPlayer(sublist, filename);
            }
            catch
            {
                MainWindow.Arr.SubLbContents("An invalid subtitle file, please try again");
                return null;
            }
        }

        private static ISubReader GetSubReader(string ext)
        {
            return ext switch
            {
                ".srt" => new SrtReader(),
                ".ass" => new AssReader(),
                _ => throw new Exception("Unknown exception")
            };
        }
    }
}