using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PaintWPF.Models
{
    public class PalleteModel
    {
        public Color?[,] UserColors { get; set; }

        const int userColorWidth = 6;
        const int userColorHeight = 5;


        public PalleteModel()
        {
            InitLength();
        }
        public void InitLength()
        {
            UserColors = new Color?[userColorWidth, userColorHeight];
            for (int i = 0; i < UserColors.GetLength(0); i++)
            {
                for (int j = 0; j < UserColors.GetLength(1); j++)
                {
                    UserColors[i, j] = null;
                }
            }
        }

    }
}
