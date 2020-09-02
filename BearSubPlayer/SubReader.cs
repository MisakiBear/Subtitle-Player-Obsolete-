using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace BearSubPlayer
{
    public class SubReader
    {
        private readonly string _path;

        public SubReader(string path)
            => this._path = path;

        public List<SubInfo> ReadSrt()
        {
            using var reader = new StreamReader(_path);
            var sublist = new List<SubInfo>();
            var templines = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == "" && templines.Count != 0)   // The end of one subtitle
                {
                    var subinfo = new SubInfo();
                    var time = templines[1].Split(" --> ");    // Second line is time info
                    subinfo.TStart = TimeSpan.Parse(time[0].Replace(',', '.'));
                    subinfo.TEnd = TimeSpan.Parse(time[1].Replace(',', '.'));

                    for (var i = 2; i < templines.Count; i++)  // Third line to the last line is subtitle info
                    {
                        subinfo.Contents += Regex.Replace(templines[i], @"<.*?>|{.*?}", "");
                        if (i < templines.Count - 1)
                            subinfo.Contents += "\n";
                    }

                    sublist.Add(subinfo);
                    templines.Clear();
                }
                else
                {
                    templines.Add(line);
                }
            }
            return sublist;
        }
    }
}
