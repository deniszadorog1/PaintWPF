using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintWPF.Models
{
    public class ColorParams
    {
        public System.Windows.Media.Color TColor { get; set; }
        public string Hex { get; set; }
        public (int H, int S, int V) HSVParam { get; set; }
        public (int, int) CordOnSpec { get; set; }

        private const int stratValue = -1;
        public ColorParams()
        {
            TColor = new System.Windows.Media.Color();
            Hex = "";
            HSVParam = (stratValue, stratValue, stratValue);
            CordOnSpec = (stratValue, stratValue);
        }

        public ColorParams(System.Windows.Media.Color color)
        {
            TColor = color;
            Hex = "";
            HSVParam = (stratValue, stratValue, stratValue);
            CordOnSpec = (stratValue, stratValue);
        }

        public ColorParams(System.Windows.Media.Color color, string hex,
            (int, int, int) HSVParam, (int, int) cordOnSpec)
        {
            TColor = color;
            Hex = hex;
            this.HSVParam = HSVParam;
            CordOnSpec = cordOnSpec;
        }

        public ColorParams(System.Windows.Media.Color color, string hex,
            (int, int, int) HSVParam)
        {
            TColor = color;
            Hex = hex;
            this.HSVParam = HSVParam;
            CordOnSpec = (stratValue, stratValue);
        }
    }
}
