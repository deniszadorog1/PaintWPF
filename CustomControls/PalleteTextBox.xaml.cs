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

namespace PaintWPF.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для PalleteTextBox.xaml
    /// </summary>
    public partial class PalleteTextBox : UserControl
    {
        public PalleteTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void InfoTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckForZeroPreviewTextInput((TextBox)sender, e);
        }
        private void CheckForZeroPreviewTextInput(TextBox box, TextCompositionEventArgs e)
        {
            const string zeroParam = "0";
            if (box.Text == zeroParam)
            {
                box.Text = string.Empty;
            }
            string newText = box.Text + e.Text;
            if (!IsTextAllowed(newText))
            {
                e.Handled = true;
            }
        }
        private bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        private void InfoTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearBut.Visibility = Visibility.Visible;
        }

        private void InfoTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //ClearBut.Visibility = Visibility.Hidden;
        }

        private void ClearBut_Click(object sender, RoutedEventArgs e)
        {
            InfoTextBox.Text = string.Empty;
            ClearBut.Visibility = Visibility.Hidden;
        }
    }
}
