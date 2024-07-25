using PaintWPF.CustomControls.SubMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PaintWPF.CustomControls.SubMenu;
using PaintWPF.Models.Enums;
namespace PaintWPF.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для LeftClickSelectionMenu.xaml
    /// </summary>
    public partial class LeftClickSelectionMenu : UserControl
    {
        public RightClickSubMenu _subMenu = null;

        private readonly List<SubMenuItems> _turnItems = new List<SubMenuItems>()
        {
            SubMenuItems.TurnIn180
        };
        private readonly List<SubMenuItems> _flipItems = new List<SubMenuItems>()
        {
            SubMenuItems.FlipInVertical,
            SubMenuItems.FlipInHorizontal
        };
        private Canvas _drawingCanvas;
        public LeftClickSelectionMenu(Canvas drawingCanvas)
        {
            _drawingCanvas = drawingCanvas;
            InitializeComponent();
        }
        private void ToTurn_Click(object sender, RoutedEventArgs e)
        {
            _subMenu = new RightClickSubMenu(_flipItems);
            SetLocationForSubMenuRightClick((Button)sender);
        }
        private void Swap_Click(object sender, RoutedEventArgs e)
        {
            _subMenu = new RightClickSubMenu(_turnItems);
            SetLocationForSubMenuRightClick((Button)sender);
        }
        private void SubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            //_subMenu = null;
            //SetLocationForSubMenuRightClick((Button)sender);
        }
        private const int _menuSubMenuDist = 1;
        private void SetLocationForSubMenuRightClick(Button sender)
        {
            _drawingCanvas.Children.Remove(_subMenu);
            RemoveSubMenu();
            if (_subMenu is null) return;
            _drawingCanvas.Children.Add(_subMenu);

            //get this location
            Point rightClickMenuLoc =
                new Point(Canvas.GetLeft(this), Canvas.GetTop(this));

            //but po in canvas
            Point buttonPosition = sender.TranslatePoint(new Point(0, 0), _drawingCanvas);

            //if position left or right
            InitXLocForSubMenu(rightClickMenuLoc);
            InitYLocSubMenu(buttonPosition);
        }
        private void InitXLocForSubMenu(Point rightClickMenuLoc)
        {
            if (rightClickMenuLoc.X + Width + _subMenu.Width <
                _drawingCanvas.Width)
            {
                Canvas.SetLeft(_subMenu,
                    rightClickMenuLoc.X + Width + _menuSubMenuDist);
                return;
            }
            Canvas.SetLeft(_subMenu, rightClickMenuLoc.X - _subMenu.Width - _menuSubMenuDist);
        }
        private void InitYLocSubMenu(Point buttonPosition)
        {
            if(buttonPosition.Y + _subMenu.Height > _drawingCanvas.Height)
            {
                double pos = buttonPosition.Y + _subMenu.Height - _drawingCanvas.Height;
                Canvas.SetTop(_subMenu, buttonPosition.Y - pos);
                return;
            }
            Canvas.SetTop(_subMenu, buttonPosition.Y);
        }

        public void RemoveSubMenu()
        {
            for(int i = 0; i < _drawingCanvas.Children.Count; i++)
            {
                if (_drawingCanvas.Children[i] is RightClickSubMenu)
                {
                    _drawingCanvas.Children.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
