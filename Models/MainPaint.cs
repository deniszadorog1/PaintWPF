using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PaintWPF.Models
{
    public  class MainPaint
    {
        public List<Image> ImagesHistory { get; set; }
        public PalleteModel PalleteMod { get; set; }
        public ColorParams FirstColor { get; set; }
        public ColorParams SecondColor { get; set; }

        public MainPaint()
        {
            ImagesHistory = new List<Image>();
            PalleteMod = new PalleteModel();
            FirstColor = new ColorParams(System.Windows.Media.Color.FromRgb(0,0,0), "#000000", (0, 0, 0));
            SecondColor = new ColorParams(System.Windows.Media.Color.FromRgb(255, 255, 255), "#FFFFFF", (0, 0, 100));
        }
    }
}
