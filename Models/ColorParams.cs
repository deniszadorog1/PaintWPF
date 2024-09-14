using System;
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
        public ColorParams()
        {
            TColor = new System.Windows.Media.Color();
            Hex = "";
            HSVParam = (-1, -1, -1);
            CordOnSpec = (-1, -1);
        }
        public ColorParams(System.Windows.Media.Color color)
        {
            TColor = color;
            Hex = "";
            HSVParam = (-1, -1, -1);
            CordOnSpec = (-1, -1);
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
            CordOnSpec = (-1, -1);
        }
    }
}
