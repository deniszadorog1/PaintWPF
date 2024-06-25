using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using PaintWPF.Models.Enums;

namespace PaintWPF.Models
{
    public  class MainPaint
    {
        public List<Image> ImagesHistory { get; set; }
        public PalleteModel PalleteMod { get; set; }
        public SolidColorBrush ColorToPaint { get; set; } 
        public SolidColorBrush FirstColor { get; set; }
        public SolidColorBrush SecondColor { get; set; }
        public BrushType TempBrushType { get; set; }

        public List<Canvas> CanvasStates = new List<Canvas>();

        public MainPaint()
        {
            ColorToPaint = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            ImagesHistory = new List<Image>();
            PalleteMod = new PalleteModel();
            FirstColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            SecondColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            CanvasStates = new List<Canvas>();
            TempBrushType = BrushType.UsualBrush; 
        }
        
    }
}
