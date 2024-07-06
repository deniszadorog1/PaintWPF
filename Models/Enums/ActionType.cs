using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintWPF.Models.Enums
{
    public enum ActionType
    {
        Drawing = 0, 
        Filling,
        Erazing,
        Figuring,
        Selection,
        Text,
        PickingColor,
        ChangingFigureSize,
        Nothing
    }
}
