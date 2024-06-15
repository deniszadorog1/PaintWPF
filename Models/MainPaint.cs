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


        public MainPaint()
        {
            ImagesHistory = new List<Image>();
            PalleteMod = new PalleteModel();
        }
    }
}
