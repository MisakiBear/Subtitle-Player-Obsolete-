using System;
using ConfReaderLib;

namespace BearSubPlayer
{
    public class Config
    {
        public double MainOp { get; set; }
        public int MainCol { get; set; }
        public int FontSize { get; set; }
        public int FontCol { get; set; }
        public double FontOp { get; set; }
        public int FontSn { get; set; }

        private static readonly (string, string, string)[] _defaultconfig = new[] {
            ("mainop", "0.5", "value=\"0-1\"") ,
            ("maincol", "0", "value=\"white=0, black=1\""),
            ("fontsize", "32", "value=\"12-46\""),
            ("fontcol", "0", "value=\"white=0, black=1\""),
            ("fontop", "0.5", "value=\"0-1\""),
            ("fontsn", "8", "value=\"5-15\"")
        };

        public Config()
        {
            try
            {
                Load();

                // Check Value
                if (!(0 <= MainOp && MainOp <= 1)) throw new Exception();
                if (!(0 <= MainCol && MainCol <= 1)) throw new Exception();
                if (!(12 <= FontSize && FontSize <= 46)) throw new Exception();
                if (!(0 <= FontCol && FontCol <= 1)) throw new Exception();
                if (!(0 <= FontOp && FontOp <= 1)) throw new Exception();
                if (!(5 <= FontSn && FontSn <= 15)) throw new Exception();
            }
            catch
            {
                SetDefault();
                Load();
            }
        }

        public void SetDefault()
        {
            ConfReader.Create(_defaultconfig, "config.conf");
        }

        private void Load()
        {

            var reader = new ConfReader("config.conf", strict: true);
            MainOp = double.Parse(reader.GetValue("mainop"));
            MainCol = int.Parse(reader.GetValue("maincol"));
            FontSize = int.Parse(reader.GetValue("fontsize"));
            FontCol = int.Parse(reader.GetValue("fontcol"));
            FontOp = double.Parse(reader.GetValue("fontop"));
            FontSn = int.Parse(reader.GetValue("fontsn"));
        }

        public void Save()
        {
            try
            {
                var reader = new ConfReader("config.conf", strict: true);
                reader.ChangeValue("mainop", MainOp.ToString(), false);
                reader.ChangeValue("maincol", MainCol.ToString(), false);
                reader.ChangeValue("fontsize", FontSize.ToString(), false);
                reader.ChangeValue("fontcol", FontCol.ToString(), false);
                reader.ChangeValue("fontop", FontOp.ToString(), false);
                reader.ChangeValue("fontsn", FontSn.ToString(), false);

                reader.SaveConf();
            }
            catch
            {
                SetDefault();
            }
        }
    }
}
