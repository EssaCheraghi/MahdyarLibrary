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
using System.Windows.Shapes;
using Mahdyar_Library;

namespace Mahdyar_Library_Tester
{
    /// <summary>
    /// Interaction logic for Win_DigitConvertor.xaml
    /// </summary>
    public partial class Win_DigitConvertor : Window
    {
        public Win_DigitConvertor()
        {
            InitializeComponent();
        }

        private void Btn_ToEnglishDigits_Click(object sender, RoutedEventArgs e)
        {
            Rtxt_Text.Ext_SetText(Rtxt_Text.Ext_GetText().Ext_ToEnglishNumber());
        }
    }
}
