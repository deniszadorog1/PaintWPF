using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using PaintWPF.Models.Tools;

namespace PaintWPF.Models
{
    public class PalleteModel
    {
        public TaskColor[,] UserColors { get; set; }
        public ColorParams[,] MainColors { get; set; }
        public int TempL { get; set; }
        public (int, int) ChosenCustomColorIndex { get; set; }
        public SolidColorBrush ChosenColor { get; set; }

        const int userColorWidth = 6;
        const int userColorHeight = 5;

        const int mainColorsWidth = 12;
        const int mainColors = 4;
        const int _maxLValue = 100;

        public PalleteModel()
        {
            InitUserColorLength();
            InitMainColors();
            TempL = _maxLValue;
            ChosenCustomColorIndex = (0, 0);
        }

        public void InitUserColorLength()
        {
            UserColors = new TaskColor[userColorHeight, userColorWidth];
            for (int i = 0; i < UserColors.GetLength(0); i++)
            {
                for (int j = 0; j < UserColors.GetLength(1); j++)
                {
                    UserColors[i, j] = null;
                }
            }
        }

        public void MoveUserColorIndex()
        {
            const int _oneCorrel = 1;
            if (ChosenCustomColorIndex.Item2 + _oneCorrel <= userColorWidth - _oneCorrel)
            {
                ChosenCustomColorIndex = (ChosenCustomColorIndex.Item1, ChosenCustomColorIndex.Item2 + _oneCorrel);
            }
            else if (ChosenCustomColorIndex.Item1 + _oneCorrel < userColorHeight - _oneCorrel &&
                ChosenCustomColorIndex.Item2 + _oneCorrel > userColorHeight - _oneCorrel)
            {
                ChosenCustomColorIndex = (ChosenCustomColorIndex.Item1 + _oneCorrel, 0);
            }
            else 
            {
                ChosenCustomColorIndex = (0, 0);
            }
        }

        public void InitMainColors()
        {
            MainColors = new ColorParams[mainColors, mainColorsWidth];
            //First Line
            MainColors[0, 0] = new ColorParams(
            Color.FromRgb(240, 135, 132), "#F08784", (2, 45, 94));
            MainColors[0, 1] = new ColorParams(
            Color.FromRgb(235, 51, 36), "#EB3324", (5, 85, 92));
            MainColors[0, 2] = new ColorParams(
            Color.FromRgb(119, 67, 66), "#774342", (1, 45, 47));
            MainColors[0, 3] = new ColorParams(
            Color.FromRgb(142, 64, 58), "#8E403A", (4, 59, 56));
            MainColors[0, 4] = new ColorParams(
            Color.FromRgb(58, 6, 3), "#3A0603", (3, 95, 23));
            MainColors[0, 5] = new ColorParams(
            Color.FromRgb(159, 252, 253), "#9FFCFD", (181, 37, 99));
            MainColors[0, 6] = new ColorParams(
            Color.FromRgb(115, 251, 253), "#73FBFD", (181, 55, 99));
            MainColors[0, 7] = new ColorParams(
            Color.FromRgb(50, 130, 246), "#3282F6", (216, 80, 96));
            MainColors[0, 8] = new ColorParams(
            Color.FromRgb(0, 35, 245), "#0023F5", (231, 100, 96));
            MainColors[0, 9] = new ColorParams(
            Color.FromRgb(0, 18, 154), "#00129A", (233, 100, 60));
            MainColors[0, 10] = new ColorParams(
            Color.FromRgb(22, 65, 124), "#16417C", (215, 82, 49));
            MainColors[0, 11] = new ColorParams(
            Color.FromRgb(0, 12, 123), "#000C7B", (234, 100, 48));

            //Second Line 
            MainColors[1, 0] = new ColorParams(
            Color.FromRgb(255, 254, 145), "#FFFE91", (59, 43, 100));
            MainColors[1, 1] = new ColorParams(
            Color.FromRgb(255, 253, 85), "#FFFD55", (59, 67, 100));
            MainColors[1, 2] = new ColorParams(
            Color.FromRgb(240, 155, 89), "#F09B59", (26, 63, 94));
            MainColors[1, 3] = new ColorParams(
            Color.FromRgb(240, 134, 80), "#F08650", (20, 67, 94));
            MainColors[1, 4] = new ColorParams(
            Color.FromRgb(120, 67, 21), "#784315", (28, 83, 47));
            MainColors[1, 5] = new ColorParams(
            Color.FromRgb(129, 127, 38), "#817F26", (59, 71, 51));
            MainColors[1, 6] = new ColorParams(
            Color.FromRgb(126, 132, 247), "#7E84F7", (237, 49, 97));
            MainColors[1, 7] = new ColorParams(
            Color.FromRgb(115, 43, 245), "#732BF5", (261, 82, 96));
            MainColors[1, 8] = new ColorParams(
            Color.FromRgb(53, 128, 187), "#3580BB", (206, 72, 73));
            MainColors[1, 9] = new ColorParams(
            Color.FromRgb(0, 2, 61), "#00023D", (238, 100, 24));
            MainColors[1, 10] = new ColorParams(
            Color.FromRgb(88, 19, 94), "#58135E", (295, 80, 37));
            MainColors[1, 11] = new ColorParams(
            Color.FromRgb(58, 8, 62), "#3A083E", (296, 87, 24));

            //Third Line
            MainColors[2, 0] = new ColorParams(
            Color.FromRgb(161, 251, 142), "#A1FB8E", (110, 43, 98));
            MainColors[2, 1] = new ColorParams(
            Color.FromRgb(161, 250, 79), "#A1FA4F", (91, 68, 98));
            MainColors[2, 2] = new ColorParams(
            Color.FromRgb(117, 249, 77), "#75F94D", (106, 69, 98));
            MainColors[2, 3] = new ColorParams(
            Color.FromRgb(117, 250, 97), "#75FA61", (112, 61, 98));
            MainColors[2, 4] = new ColorParams(
            Color.FromRgb(131, 53, 98), "#75FA8D", (117, 250, 141));
            MainColors[2, 5] = new ColorParams(
            Color.FromRgb(129, 128, 73), "#818049", (59, 43, 51));
            MainColors[2, 6] = new ColorParams(
            Color.FromRgb(239, 136, 190), "#EF88BE", (329, 43, 94));
            MainColors[2, 7] = new ColorParams(
            Color.FromRgb(238, 138, 248), "#EE8AF8", (295, 44, 97));
            MainColors[2, 8] = new ColorParams(
            Color.FromRgb(234, 63, 247), "#EA3FF7", (296, 74, 97));
            MainColors[2, 9] = new ColorParams(
            Color.FromRgb(234, 54, 128), "#EA3680", (335, 77, 92));
            MainColors[2, 10] = new ColorParams(
            Color.FromRgb(127, 130, 187), "#7F82BB", (237, 32, 73));
            MainColors[2, 11] = new ColorParams(
            Color.FromRgb(117, 22, 63), "#75163F", (334, 81, 46));

            //Fourth Line
            MainColors[3, 0] = new ColorParams(
            Color.FromRgb(55, 125, 34), "#377D22", (106, 73, 49));
            MainColors[3, 1] = new ColorParams(
            Color.FromRgb(55, 125, 71), "#377E47", (134, 56, 49));
            MainColors[3, 2] = new ColorParams(
            Color.FromRgb(54, 126, 127), "#367E7F", (181, 57, 50));
            MainColors[3, 3] = new ColorParams(
            Color.FromRgb(80, 127, 128), "#507F80", (181, 38, 50));
            MainColors[3, 4] = new ColorParams(
            Color.FromRgb(24, 62, 12), "#183E0C", (106, 81, 24));
            MainColors[3, 5] = new ColorParams(
            Color.FromRgb(23, 63, 63), "#173F3F", (180, 63, 25));
            MainColors[3, 6] = new ColorParams(
            Color.FromRgb(116, 27, 124), "#741B7C", (295, 78, 49));
            MainColors[3, 7] = new ColorParams(
            Color.FromRgb(57, 16, 123), "#39107B", (263, 87, 48));
            MainColors[3, 8] = new ColorParams(
            Color.FromRgb(0, 0, 0), "#000000", (0, 0, 0));
            MainColors[3, 9] = new ColorParams(
            Color.FromRgb(128, 128, 128), "#808080", (0, 0, 50));
            MainColors[3, 10] = new ColorParams(
            Color.FromRgb(192, 192, 192), "#C0C0C0", (0, 0, 75));
            MainColors[3, 11] = new ColorParams(
            Color.FromRgb(255, 255, 255), "#FFFFFF", (0, 0, 100));
        }

        public (int, int) GetMainColorIndexByColor(System.Windows.Media.Color color)
        {
            const int failIndex = -1;
            for (int i = 0; i < MainColors.GetLength(0); i++)
            {
                for (int j = 0; j < MainColors.GetLength(1); j++)
                {
                    if (MainColors[i, j].TColor.Equals(color))
                    {
                        return (i, j);
                    }
                }
            }
            return (failIndex, failIndex);
        }
    }
}
