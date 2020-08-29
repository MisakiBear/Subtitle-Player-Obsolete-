using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BearSubPlayer
{
    public class Config
    {
        public int MainOp { get; set; }
        public int MainCol { get; set; }
        public int FontSize { get; set; }
        public int FontCol { get; set; }
        public int FontOp { get; set; }
        public int FontSn { get; set; }

        public Config()
        {
            try
            {
                Load();

                // Check Value
                if (!(0 <= MainOp && MainOp <= 100)) throw new Exception();
                if (!(0 <= MainCol && MainCol <= 1)) throw new Exception();
                if (!(12 <= FontSize && FontSize <= 46)) throw new Exception();
                if (!(0 <= FontCol && FontCol <= 1)) throw new Exception();
                if (!(0 <= FontOp && FontOp <= 100)) throw new Exception();
                if (!(50 <= FontSn && FontSn <= 150)) throw new Exception();
            }
            catch
            {
                SetDefault();
                Load();
            }
        }

        public void SetDefault()
        {
            using var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "config.xml"));
            writer.Write(_defaultconfig);
        }

        private void Load()
        {
            var xdoc = XDocument.Load(Path.Combine(Environment.CurrentDirectory, "config.xml"));
            var pairs = xdoc.Root.Elements()
                .Select(x => new
                {
                    Key = x.Name.LocalName,
                    Value = x.Value,
                });
            var dict = pairs.ToDictionary(x => x.Key, x => x.Value);

            MainOp = int.Parse(dict["mainop"]);
            MainCol = int.Parse(dict["maincol"]);
            FontSize = int.Parse(dict["fontsize"]);
            FontCol = int.Parse(dict["fontcol"]);
            FontOp = int.Parse(dict["fontop"]);
            FontSn = int.Parse(dict["fontsn"]);
        }

        public void Save()
        {
            try
            {
                var xdoc = XDocument.Load(Path.Combine(Environment.CurrentDirectory, "config.xml"));
                var pairs = xdoc.Root;

                pairs.Element("mainop").Value = MainOp.ToString();
                pairs.Element("maincol").Value = MainCol.ToString();
                pairs.Element("fontsize").Value = FontSize.ToString();
                pairs.Element("fontcol").Value = FontCol.ToString();
                pairs.Element("fontop").Value = FontOp.ToString();
                pairs.Element("fontsn").Value = FontSn.ToString();

                xdoc.Save(Path.Combine(Environment.CurrentDirectory, "config.xml"));
            }
            catch
            {
                SetDefault();
            }
        }

        private static readonly string _defaultconfig =
@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<config>
  <mainop value=""0-100"">50</mainop>
  <maincol value=""white=0, black=1"">0</maincol>
  <fontsize  value=""12-46"">32</fontsize>
  <fontcol value=""white=0, black=1"">0</fontcol>
  <fontop value=""0-100"">50</fontop>
  <fontsn value=""50-150"">80</fontsn>
</config>";
    }
}
