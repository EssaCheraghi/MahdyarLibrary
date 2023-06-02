using Microsoft.Win32;
using System.Windows;
using Mahdyar_Library;
using System.Drawing.Imaging;

namespace Mahdyar_Library_Tester
{
    /// <summary>
    /// Interaction logic for Win_ImageResizer.xaml
    /// </summary>
    public partial class Win_ImageResizer : Window
    {
        public Win_ImageResizer()
        {
            InitializeComponent();
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = true;
            op.ShowDialog();
            var folder = System.IO.Directory.GetParent(op.FileName).FullName;
            int counter = 1;

            foreach(var file in op.FileNames)
            {
                System.Drawing.Image.FromFile(file).Ext_ScaleImage(800, 2000, true, true)
                    .Ext_Format(ImageFormat.Jpeg).Save(folder + @"\"+counter++ + ".jpg");
            }
        }
    }
}
